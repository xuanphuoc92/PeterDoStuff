using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using PeterDoStuff.Attributes;
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

        private static IEnumerable<PropertyInfo> GetPropertyInfosByGenericType(this DbContext context, Type genericType)
            => context
                .GetType()
                .GetProperties()
                .Where(p =>
                    p.PropertyType.IsGenericType &&
                    p.PropertyType.GetGenericTypeDefinition() == genericType
                );

        /// <summary>
        /// Get the property infos of DbSet properties of a DbContext.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        internal static IEnumerable<PropertyInfo> GetDbSetPropertyInfos(this DbContext context)
            => context.GetPropertyInfosByGenericType(typeof(DbSet<>));
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
            StringBuilder sql = new StringBuilder();

            var dbSets = context.GetDbSetPropertyInfos();
            foreach (var dbSet in dbSets)
            {
                sql.AppendLine($"DROP TABLE IF EXISTS [{dbSet.Name}];");

                if (dbSet.GetCustomAttribute<WithDeletedBinAttribute>() != null)
                    sql.AppendLine($"DROP TABLE IF EXISTS [{dbSet.Name}_Deleted];");
            }

            return sql.ToString();
        }

        /// <summary>
        /// Get the SQL script to create all tables of the DbContext. 
        /// Columns will based on Column attribute first, or (if Column attribute not defined) based on Migrator mapping setting (call DescribeMapping() for more info).
        /// </summary>
        /// <returns></returns>
        public string GetCreateSql()
        {
            StringBuilder sql = new StringBuilder();

            var dbSets = context.GetDbSetPropertyInfos();
            foreach (var dbSet in dbSets)
            {
                var entityType = dbSet.PropertyType.GetGenericArguments()[0];
                string tableName = dbSet.Name;

                string mainTable = CraftCreateSql(entityType, tableName);
                sql.AppendLine(mainTable);

                if (dbSet.GetCustomAttribute<WithDeletedBinAttribute>() != null)
                {
                    string deletedBinTable = CraftCreateSql(entityType, $"{tableName}_Deleted", 
                        includePrimaryKey: false);
                    sql.AppendLine(deletedBinTable);
                }
            }

            return sql.ToString();
        }

        private string CraftCreateSql(Type entityType, string tableName, bool includePrimaryKey = true)
        {
            var subSql = new StringBuilder();
            var columnInfos = entityType
                .GetProperties()
                .Where(pi => IsColumn(pi));

            var columns = columnInfos
                .Select(info => GetSqlColumn(info));
            var constraints = new List<string>();
            
            if (includePrimaryKey)
                constraints.Add($"    CONSTRAINT PK_{tableName}_Id PRIMARY KEY (Id)");

            subSql.AppendLine($"CREATE TABLE [{tableName}] (");
            subSql.AppendLine(columns.Union(constraints).Join(",\n"));
            subSql.Append($");");
            return subSql.ToString();
        }

        private static int[] Size(params int[] sizes)
        {
            return sizes;
        }

        // Based on: https://learn.microsoft.com/en-us/dotnet/framework/data/adonet/sql-server-data-type-mappings
        private static readonly Dictionary<Type, (string DefaultSqlType, int[] DefaultSize)> _presetMapping
            = new Dictionary<Type, (string DefaultSqlType, int[] DefaultSize)>()
        {
            { typeof(string), ("nvarchar", Size(-1) ) },
            { typeof(Guid), ("uniqueidentifier", Size() ) },
            { typeof(byte[]), ("varbinary", Size(-1) ) },
            { typeof(decimal), ("decimal", Size() ) },
            { typeof(int), ("int", Size() ) },
            { typeof(float), ("real", Size() ) },
            { typeof(double), ("float", Size() ) },
            { typeof(DateTime), ("datetime2", Size() ) },
            { typeof(DateOnly), ("date", Size() ) },
            { typeof(Int64), ("bigint", Size() ) },
            { typeof(bool), ("bit", Size() ) },
        };

        private Dictionary<Type, (string DefaultSqlType, int[] DefaultSize)> _mapping
            { get; set; } = _presetMapping;

        public string DescribeMapping()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var kv in _mapping)
            {
                string size = sqlSize(kv.Value.DefaultSize);
                size = size == "" ? "" : $"({size})";

                sb.AppendLine($"{kv.Key.FullName} => {kv.Value.DefaultSqlType}{size}");
            }
            sb.AppendLine("Enum => int");
            return sb.ToString();
        }

        private static string sqlSize(int[] size)
        {
            if (size.Length == 1 && size[0] == -1)
                return "max";

            return size.Any()
                ? size.Select(c => c.ToString()).Join(",")
                : "";
        }

        private bool IsColumn(PropertyInfo pi)
        {
            if (pi.GetCustomAttribute<NotMappedAttribute>() != null)
                return false;

            Type typeToCheck = GetInnerTypeIfNullable(pi.PropertyType);
            return typeToCheck.IsEnum || _mapping.ContainsKey(typeToCheck);
        }

        private string GetSqlColumn(PropertyInfo pi)
        {
            var name = pi.Name;
            
            string defaultColumnType = GetDefaultColumnType(pi);
            string customColumnType = pi.GetCustomAttribute<ColumnAttribute>()?.TypeName ?? "";

            string columnType = customColumnType != "" ? customColumnType : defaultColumnType;

            return $"    [{name}] {columnType}";
        }

        private string GetDefaultColumnType(PropertyInfo pi)
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

        private string GetDefaultSize(PropertyInfo pi)
        {
            Type typeToCheck = GetInnerTypeIfNullable(pi.PropertyType);

            int[] size = _mapping.ContainsKey(typeToCheck)
                ? _mapping[typeToCheck].DefaultSize
                : Size(); // Enum

            return sqlSize(size);
        }

        private string GetDefaultType(Type propertyType)
        {
            Type typeToCheck = GetInnerTypeIfNullable(propertyType);

            return _mapping.ContainsKey(typeToCheck)
                ? _mapping[typeToCheck].DefaultSqlType
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

            int[] size = Size();

            if (typeToCheck == typeof(string) || typeToCheck == typeof(byte[])) 
            {
                var attribute = pi.GetCustomAttribute<MaxLengthAttribute>();
                if (attribute != null)
                    size = Size(attribute.Length);
            }

            if (typeToCheck == typeof(decimal))
            {
                var attribute = pi.GetCustomAttribute<DecimalPrecisionScaleAttribute>();
                if (attribute != null)
                    size = Size(attribute.Precision,attribute.Scale);
            }

            return sqlSize(size);
        }
    }
}
