using Dapper;
using Microsoft.EntityFrameworkCore;
using PeterDoStuff.Attributes;
using PeterDoStuff.Database;
using System.Reflection;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace PeterDoStuff.Extensions
{
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
                    .Where(pi => Migrator.IsColumn(pi))
                    .Select(pi => pi.Name)
                    .ToList();

                sql.AppendLine($"INSERT INTO [{auditTable}] ({columns.Select(c => $"[{c}]").Join(", ")}, AuditAction, AuditTime) VALUES");

                for (int i = 0; i < list.Count(); i++)
                {
                    var entry = list[i];
                    sql.Append("(");

                    var entity = entry.Entity;
                    foreach (var col in columns)
                        sql.Append("{0}, ", entity.GetPropertyValue(col));

                    var action = entry.State switch
                    {
                        EntityState.Added => "INSERT",
                        EntityState.Modified => "UPDATE",
                        _ => "DELETE"
                    };

                    sql.Append("{0}, getDate())", action);

                    if (i == list.Count() - 1)
                        sql.AppendLine(";");
                    else
                        sql.AppendLine(",");
                }
            }

            object param = new DynamicParameters(sql.Parameters);
            _context.Database.GetDbConnection().Execute(sql.Sql, param);
        }
    }
}
