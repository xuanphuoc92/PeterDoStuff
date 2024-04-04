using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace PeterDoStuff.Extensions
{
    public static class EfCoreExtensions
    {
        public static string GetMigrateSql<TDbContext>(this TDbContext context)
            where TDbContext : DbContext
        {
            var dbSets = context
                .GetType()
                .GetProperties()
                .Where(p =>
                    p.PropertyType.IsGenericType &&
                    p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>));

            StringBuilder sql = new StringBuilder();
            foreach (var dbSet in dbSets) 
            {
                var entityType = dbSet.PropertyType.GetGenericArguments()[0];
                var columns = entityType
                    .GetProperties()
                    .Where(p => AmongColumnTypes(p.PropertyType))
                    .Select(p => GetSqlColumn(p));
                
                sql.AppendLine($"CREATE TABLE [{dbSet.Name}] (");
                sql.AppendLine(columns.Join(",\n"));
                sql.AppendLine($");");
            }
            return sql.ToString();
        }

        private static readonly Dictionary<Type, (string SqlType, string DefaultSize)> _columnTypes = new Dictionary<Type, (string SqlType, string DefaultSize)>()
        {
            { typeof(string), ("nvarchar", "100" ) },
            { typeof(Guid), ("uniqueidentifier", "" ) },
            { typeof(byte[]), ("varbinary", "32" ) },
        };
        private static bool AmongColumnTypes(Type propertyType)
        {
            return _columnTypes.Keys.Contains(propertyType);
        }
        private static string GetSqlColumn(PropertyInfo pi)
        {
            var name = pi.Name;
            var type = _columnTypes[pi.PropertyType].SqlType;
            var defaultSize = _columnTypes[pi.PropertyType].DefaultSize;
            var size = defaultSize.IsNullOrEmpty() ? "" : $"({defaultSize})";
            return $"    [{name}] {type}{size}";
        }
    }
}
