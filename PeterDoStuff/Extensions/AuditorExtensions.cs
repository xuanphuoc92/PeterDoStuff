using Dapper;
using Microsoft.EntityFrameworkCore;
using PeterDoStuff.Attributes;
using PeterDoStuff.Database;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace PeterDoStuff.Extensions
{
    public class AuditEntity
    {
        public Guid Id { get; set; }

        [MaxLength(50)]
        public string AuditTable { get; set; }
        
        [MaxLength(20)]
        public string Action { get; set; }

        [ConcurrencyCheck]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? CreatedAt { get; set; }

        public Guid? UserId { get; set; }

        [MaxLength(50)]
        public string UserDescription { get; set; }
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

        public void AuditChanges()
        {
            SqlCommand command = AuditLogInsertSql();
            _context.Database.GetDbConnection().Execute(command);
        }

        private SqlCommand AuditLogInsertSql()
        {
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

                sql.AppendLine($"INSERT INTO [{auditTable}] ({columns.Select(c => $"[{c}]").Join(", ")}, AuditAction, AuditTime) VALUES");

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

                    sql.Append("{0}, getDate())", action);

                    if (i == list.Count() - 1)
                        sql.AppendLine(";");
                    else
                        sql.AppendLine(",");
                }
            }

            return sql;
        }
    }
}
