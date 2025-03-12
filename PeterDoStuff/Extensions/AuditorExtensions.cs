using Microsoft.EntityFrameworkCore;
using PeterDoStuff.Attributes;
using PeterDoStuff.Database;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace PeterDoStuff.Extensions
{
    public class AuditEntity
    {
        public Guid Id { get; set; }

        [MaxLength(50)]
        public string AuditTable { get; set; }
        
        [MaxLength(20)]
        public string Action { get; set; }

        public DateTime? Time { get; set; }

        //public Guid? UserId { get; set; }

        //[MaxLength(50)]
        //public string UserDescription { get; set; }
    }

    public interface IAuditableContext
    {
        DbSet<AuditEntity> Audit { get; set; }
    }

    public static class AuditorExtensions
    {
        public static Auditor GetAuditor(this DbContext context)
            => new Auditor(context);
    }

    public class Auditor
    {
        private readonly DbContext _context;
        public Auditor(DbContext context) 
        {
            _context = context;
        }

        public SqlCommand GetAuditSql()
        {
            var auditTime = DateTime.Now;
            
            var entries = _context.ChangeTracker.Entries()
                            .Where(entry =>
                                entry.State == EntityState.Added ||
                                entry.State == EntityState.Modified ||
                                entry.State == EntityState.Deleted
                            );

            var tableGroups = entries.GroupBy(e => e.Metadata.GetTableName());
            var dbSetInfos = _context
                .GetDbSetPropertyInfos()
                .ToDictionary(pi => pi.Name);

            SqlCommand sql = SqlCommand.New();

            var auditEntities = new List<AuditEntity>();

            foreach (var group in tableGroups)
            {
                if (dbSetInfos[group.Key].GetCustomAttribute<AuditableAttribute>() == null)
                    continue;

                var auditTable = $"{group.Key}_Audit";

                var list = group.ToList();
                var columns = list.First().Entity
                    .GetType()
                    .GetProperties()
                    .Where(pi => Migrator.IsMappedToColumn(pi))
                    .Select(pi => pi.Name)
                    .ToList();

                sql.AppendLine($"INSERT INTO [{auditTable}] ({columns.Select(c => $"[{c}]").Join(", ")}, AuditId) VALUES");

                for (int i = 0; i < list.Count(); i++)
                {
                    var entry = list[i];

                    var action = entry.State switch
                    {
                        EntityState.Added => "INSERT",
                        EntityState.Modified => "UPDATE",
                        _ => "DELETE"
                    };

                    var entity = entry.Entity;
                    var oldPropertyValues = entry.OriginalValues.Properties.ToDictionary(
                        p => p.Name,
                        p => entry.OriginalValues[p]);

                    sql.Append("(");

                    foreach (var col in columns)
                        sql.Append("{0}, ", 
                            // It is possible to corrupt deleted data by changing it before delete.
                            // Therefore, in the case of delete, use the oldPropertyValues dictionary instead.
                            action == "DELETE" ? oldPropertyValues[col] : entity.GetPropertyValue(col));
                    
                    var auditId = Guid.NewGuid();
                    sql.Append("{0})", auditId);

                    if (i == list.Count() - 1)
                        sql.AppendLine(";");
                    else
                        sql.AppendLine(",");

                    auditEntities.Add(new AuditEntity() { Id = auditId, Action = action, AuditTable = auditTable, Time = auditTime });
                }
            }

            // Insert the audit entities
            sql.AppendLine($"INSERT INTO [Audit] ([Id], [Action], [AuditTable], [Time]) VALUES");
            for (int i = 0; i < auditEntities.Count(); i++)
            {
                var auditEntity = auditEntities[i];
                sql.AppendLine("({0}, {1}, {2}, {3})", 
                    auditEntity.Id, auditEntity.Action, auditEntity.AuditTable, auditEntity.Time);
                if (i == auditEntities.Count() - 1)
                    sql.AppendLine(";");
                else 
                    sql.AppendLine(",");
            }

            return sql;
        }
    }
}
