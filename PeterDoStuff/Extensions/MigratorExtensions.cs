using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using PeterDoStuff.Attributes;
using System.Runtime.CompilerServices;
using System.ComponentModel.DataAnnotations.Schema;

namespace PeterDoStuff.Extensions
{
    public static class MigratorExtensions
    {
        /// <summary>
        /// Get the new Migrator (new Migrator(DbContext)) for the DbContext.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static Migrator GetMigrator(this DbContext context)
            => new(context);

        /// <summary>
        /// Get the property infos of DbSet properties of a DbContext.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        internal static IEnumerable<PropertyInfo> GetDbSetPropertyInfos(this DbContext context)
            => context
                .GetType()
                .GetProperties()
                .Where(p =>
                    p.PropertyType.IsGenericType &&
                    p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>));
    }

    /// <summary>
    /// This class provides the SQL to drop or create tables that defined as DbSet inside the DbContext.
    /// </summary>
    public class Migrator
    {
        private readonly DbContext context;
        public Migrator(DbContext context) 
        {
            this.context = context;
        }

        /// <summary>
        /// Get the SQL script to drop all tables of the DbContext.
        /// </summary>
        /// <returns></returns>
        public string GetDropSql()
        {
            var dbSets = context.GetDbSetPropertyInfos();

            StringBuilder sql = new StringBuilder();
            foreach (var dbSet in dbSets)
            {
                sql.AppendLine($"DROP TABLE IF EXISTS [{dbSet.Name}];");                
            }
            return sql.ToString();
        }

        /// <summary>
        /// Get the SQL script to create all tables of the DbContext.
        /// </summary>
        /// <returns></returns>
        public string GetCreateSql()
        {
            var dbSets = context.GetDbSetPropertyInfos();

            StringBuilder sql = new StringBuilder();
            foreach (var dbSet in dbSets)
            {
                var entityType = dbSet.PropertyType.GetGenericArguments()[0];
                var columnInfos = entityType
                    .GetProperties()
                    .Where(pi => IsColumn(pi));
                
                var columns = columnInfos
                    .Select(info => GetSqlColumn(info));
                var constraints = new List<string>() { $"    CONSTRAINT PK_{dbSet.Name}_Id PRIMARY KEY (Id)" };

                sql.AppendLine($"CREATE TABLE [{dbSet.Name}] (");
                sql.AppendLine(columns.Union(constraints).Join(",\n"));
                sql.AppendLine($");");
            }
            return sql.ToString();
        }

        // Based on: https://learn.microsoft.com/en-us/dotnet/framework/data/adonet/sql-server-data-type-mappings
        private static readonly Dictionary<Type, (string DefaultSqlType, string DefaultSize)> _columnTypes
            = new Dictionary<Type, (string DefaultSqlType, string DefaultSize)>()
        {
            { typeof(string), ("nvarchar", "max" ) },
            { typeof(Guid), ("uniqueidentifier", "" ) },
            { typeof(byte[]), ("varbinary", "max" ) },
            { typeof(decimal), ("decimal", "" ) },
            { typeof(int), ("int", "" ) },
            { typeof(float), ("real", "" ) },
            { typeof(double), ("float", "" ) },
            { typeof(DateTime), ("datetime2", "" ) },
            { typeof(DateOnly), ("date", "" ) },
        };

        private static bool IsColumn(PropertyInfo pi)
        {
            if (pi.GetCustomAttribute<NotMappedAttribute>() != null)
                return false;

            Type typeToCheck = GetInnerTypeIfNullable(pi.PropertyType);
            return typeToCheck.IsEnum || _columnTypes.ContainsKey(typeToCheck);
        }

        private static string GetSqlColumn(PropertyInfo pi)
        {
            var name = pi.Name;
            
            string defaultColumnType = GetDefaultColumnType(pi);
            string customColumnType = pi.GetCustomAttribute<ColumnAttribute>()?.TypeName ?? "";

            string columnType = customColumnType != "" ? customColumnType : defaultColumnType;

            return $"    [{name}] {columnType}";
        }

        private static string GetDefaultColumnType(PropertyInfo pi)
        {
            string defaultType = GetDefaultType(pi.PropertyType);
            string customType = GetCustomType(pi);
            var type = customType != "" ? customType : defaultType;

            string defaultSize = GetDefaultSize(pi);
            string customSize = GetCustomSize(pi);
            string finalSize = customSize != "" ? customSize : defaultSize;
            var size = finalSize.IsNullOrEmpty() ? "" : $"({finalSize})";

            var columnType = $"{type}{size}";
            return columnType;
        }

        private static string GetDefaultSize(PropertyInfo pi)
        {
            Type typeToCheck = GetInnerTypeIfNullable(pi.PropertyType);

            return _columnTypes.ContainsKey(typeToCheck)
                ? _columnTypes[typeToCheck].DefaultSize
                : ""; // Enum
        }

        private static string GetDefaultType(Type propertyType)
        {
            Type typeToCheck = GetInnerTypeIfNullable(propertyType);

            return _columnTypes.ContainsKey(typeToCheck)
                ? _columnTypes[typeToCheck].DefaultSqlType
                : "int"; // Enum
        }

        private static Type GetInnerTypeIfNullable(Type propertyType)
        {
            var isNullable = 
                propertyType.IsGenericType && 
                propertyType.GetGenericTypeDefinition() == typeof(Nullable<>);

            Type typeToCheck = isNullable
                ? propertyType.GetGenericArguments()[0]
                : propertyType;

            return typeToCheck;
        }

        private static string GetCustomType(PropertyInfo pi)
        {
            Type typeToCheck = GetInnerTypeIfNullable(pi.PropertyType);

            if (typeToCheck == typeof(DateTime))
            {
                var attribute = pi.GetCustomAttribute<DateOnlyAttribute>();
                if (attribute != null)
                    return "date";
            }

            return "";
        }

        private static string GetCustomSize(PropertyInfo pi)
        {
            Type typeToCheck = GetInnerTypeIfNullable(pi.PropertyType);

            if (typeToCheck == typeof(string) || typeToCheck == typeof(byte[])) 
            {
                var attribute = pi.GetCustomAttribute<MaxLengthAttribute>();
                if (attribute != null)
                    return attribute.Length.ToString();
            }

            if (typeToCheck == typeof(decimal))
            {
                var attribute = pi.GetCustomAttribute<DecimalPrecisionScaleAttribute>();
                if (attribute != null)
                    return $"{attribute.Precision},{attribute.Scale}";
            }

            return "";
        }
    }
}
