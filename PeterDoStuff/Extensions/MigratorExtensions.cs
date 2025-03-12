using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using PeterDoStuff.Attributes;
using System.ComponentModel.DataAnnotations.Schema;
using Dapper;
using PeterDoStuff.Database;

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

                if (dbSet.GetCustomAttribute<AuditableAttribute>() != null)
                    sql.AppendLine($"DROP TABLE IF EXISTS [{dbSet.Name}_Audit];");
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
                string subSql = CreateTableSql(dbSet);
                sql.Append(subSql);
            }

            return sql.ToString();
        }

        private string CreateTableSql(PropertyInfo dbSet)
        {
            var sql = new StringBuilder();

            var entityType = dbSet.PropertyType.GetGenericArguments()[0];
            string tableName = dbSet.Name;

            string mainTable = CraftCreateSql(entityType, tableName,
                $"CONSTRAINT PK_{tableName}_Id PRIMARY KEY (Id)");
            sql.AppendLine(mainTable);

            if (dbSet.GetCustomAttribute<AuditableAttribute>() != null)
            {
                string auditTable = CraftCreateSql(entityType, $"{tableName}_Audit",
                    "[AuditId] uniqueidentifier",
                    $"INDEX IDX_{tableName}_Audit_Id ([Id])",
                    $"INDEX IDX_{tableName}_Audit_AuditId ([AuditId])");
                sql.AppendLine(auditTable);
            }

            return sql.ToString();
        }

        private string CraftCreateSql(Type entityType, string tableName, 
            params string[] additionalDefinitions)
        {
            var subSql = new StringBuilder();
            var columnInfos = entityType
                .GetProperties()
                .Where(pi => IsMappedToColumn(pi));

            var columns = columnInfos
                .Select(info => GetSqlColumn(info));
            
            var additionalSql = new List<string>();
            foreach (var defintion in additionalDefinitions)
                additionalSql.Add($"    {defintion}");

            subSql.AppendLine($"CREATE TABLE [{tableName}] (");
            subSql.AppendLine(columns.Union(additionalSql).Join(",\n"));
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
            { typeof(decimal), ("decimal", Size(18,0) ) },
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

        /// <summary>
        /// Check if a property inside an entity class is to be mapped to a column.
        /// </summary>
        /// <param name="pi"></param>
        /// <returns></returns>
        public static bool IsMappedToColumn(PropertyInfo pi)
        {
            if (pi.GetCustomAttribute<NotMappedAttribute>() != null)
                return false;

            Type typeToCheck = GetInnerTypeIfNullable(pi.PropertyType);
            return typeToCheck.IsEnum || _presetMapping.ContainsKey(typeToCheck);
        }

        private string GetSqlColumn(PropertyInfo pi)
        {
            var name = pi.Name;

            string columnType = GetColumnDefinition(pi);

            return $"    [{name}] {columnType}";
        }

        private string GetColumnDefinition(PropertyInfo pi)
        {
            string defaultColumnDefinition = GetDefaultColumnDefinition(pi);
            string customColumnDefinition = pi.GetCustomAttribute<ColumnAttribute>()?.TypeName ?? "";

            string columnDefinition = customColumnDefinition != "" ? customColumnDefinition : defaultColumnDefinition;
            return columnDefinition;
        }

        private string GetDefaultColumnDefinition(PropertyInfo pi)
        {
            string defaultType = GetDefaultType(pi.PropertyType);
            string customType = GetCustomType(pi);
            var type = customType != "" ? customType : defaultType;

            string defaultSize = GetDefaultSize(pi);
            string customSize = GetCustomSize(pi);
            string finalSize = customSize != "" ? customSize : defaultSize;
            var size = finalSize.IsNullOrEmpty() ? "" : $"({finalSize})";

            var columnDefinition = $"{type}{size}";
            return columnDefinition;
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
                {
                    var isBigNVarChar = typeToCheck == typeof(string) && attribute.Length > 4000;
                    var isBigVarBinary = typeToCheck == typeof(byte[]) && attribute.Length > 8000;

                    if (isBigNVarChar || isBigVarBinary) 
                        size = Size(-1);
                    else
                        size = Size(attribute.Length);
                }
            }

            if (typeToCheck == typeof(decimal))
            {
                var attribute = pi.GetCustomAttribute<DecimalPrecisionScaleAttribute>();
                if (attribute != null)
                    size = Size(attribute.Precision,attribute.Scale);
            }

            return sqlSize(size);
        }

        private class TableProfile
        {
            public string Table { get; set; }
            public string Column { get; set; }
            public string ColumnType { get; set; }
            public int Max { get; set; }
            public int Precision { get; set; }
            public int Scale { get; set; }
            public int PrimaryKeyOrder { get; set; }
        }

        private const string SQL_SERVER_PROFILE =
@"SELECT 
    t.name AS [Table],
    c.name AS [Column],
    ty.name AS [ColumnType],
    c.max_length as [Max],
    c.[precision] as [Precision],
    c.[scale] as [Scale],
    CASE
        WHEN pk_col.key_ordinal IS NOT NULL THEN pk_col.key_ordinal
        ELSE NULL
    END AS [PrimaryKeyOrder]
FROM 
    sys.tables t
INNER JOIN 
    sys.columns c ON t.object_id = c.object_id
INNER JOIN 
    sys.types ty ON c.user_type_id = ty.user_type_id
LEFT JOIN 
    sys.identity_columns ic ON c.object_id = ic.object_id AND c.column_id = ic.column_id
LEFT JOIN 
    sys.indexes pk ON t.object_id = pk.object_id AND pk.is_primary_key = 1
LEFT JOIN 
    sys.index_columns pk_col ON pk.object_id = pk_col.object_id AND pk.index_id = pk_col.index_id AND c.column_id = pk_col.column_id
WHERE 
    t.is_ms_shipped = 0";

        public string GetUpdateSql()
        {
            var dbSets = context.GetDbSetPropertyInfos();

            var tables = dbSets.Select(pi => pi.Name);

            var sqlCommand = SqlCommand.New().AppendLine(SQL_SERVER_PROFILE);
            sqlCommand.AppendLine("AND t.name in {0}", tables);

            var profileGroups = context.Database.GetDbConnection()
                .Query<TableProfile>(sqlCommand)
                .GroupBy(r => r.Table);

            StringBuilder sql = new StringBuilder();
            foreach (var dbSet in dbSets)
            {
                var table = dbSet.Name;

                // No Table exist
                if (profileGroups.Any(g => g.Key == table) == false)
                {
                    string subSql = CreateTableSql(dbSet);
                    sql.Append(subSql);
                    continue;
                }

                // Table exist
                var oldColumns = profileGroups.Single(g => g.Key == table).ToList();
                var newColumns = dbSet.PropertyType.GetGenericArguments()[0]
                    .GetProperties()
                    .Where(pi => IsMappedToColumn(pi));

                var toAdd = newColumns.Where(pi => oldColumns.Any(oc => oc.Column == pi.Name) == false);
                var toAlter = newColumns.Where(pi => oldColumns.Any(oc => oc.Column == pi.Name) == true);

                if (toAdd.Any())
                {
                    sql.AppendLine($"ALTER TABLE [{table}] ADD");
                    sql.AppendLine(toAdd.Select(c => GetSqlColumn(c)).Join("," + Environment.NewLine) + ";");
                }

                foreach (var newAlterColumn in toAlter)
                {
                    var oldColumn = oldColumns.Single(oc => oc.Column == newAlterColumn.Name);
                    var oldColumnType = oldColumn.ColumnType;
                    var oldColumnSize = oldColumnType switch
                    {
                        "nvarchar" => Size(oldColumn.Max == -1 ? -1 : oldColumn.Max / 2),
                        "decimal" => Size(oldColumn.Precision, oldColumn.Scale),
                        _ => Size()
                    };
                    var oldColumnSizeString = oldColumnSize.Any()
                        ? "(" + sqlSize(oldColumnSize) + ")"
                        : "";
                    var oldColumnDefinition = $"{oldColumnType}{oldColumnSizeString}";

                    var newColumnDefintion = GetColumnDefinition(newAlterColumn);
                    var bracketIndex = newColumnDefintion.IndexOf('(');
                    var newColumnType = bracketIndex == -1
                        ? newColumnDefintion
                        : newColumnDefintion.Substring(0, bracketIndex);
                    var newColumnSize = bracketIndex == -1
                        ? Size()
                        : GetSizeFromDefintion(newColumnDefintion);

                    string alterCommand = $"ALTER TABLE [{table}] ALTER COLUMN [{newAlterColumn.Name}] {newColumnDefintion}; -- From: [{newAlterColumn.Name}] {oldColumnDefinition}";

                    bool sameType = oldColumnType == newColumnType;

                    if (sameType == true &&
                        IsSame(oldColumnSize, newColumnSize) == false)
                    {
                        if (IsIncreasing(oldColumnSize, newColumnSize) == true) 
                            sql.AppendLine($"{alterCommand}");
                        else
                            sql.AppendLine($"-- WARNING - Shrinking column: {alterCommand}");
                    }
                    else if (sameType == false)
                        sql.AppendLine($"-- WARNING - Altering column: {alterCommand}");
                }
            }

            return sql.ToString();
        }

        private bool IsSame(int[] oldColumnSize, int[] newColumnSize)
        {
            for (int i = 0; i < oldColumnSize.Length; i++)
            {
                if (oldColumnSize[i] != newColumnSize[i])
                    return false;
            }
            return true;
        }

        private bool IsIncreasing(int[] oldColumnSize, int[] newColumnSize)
        {
            for (int i = 0; i< oldColumnSize.Length; i++) 
            {
                // Max vs non-max
                if (oldColumnSize[i] == -1 && newColumnSize[i] > -1)
                    return false;
                
                if (oldColumnSize[i] > newColumnSize[i] && newColumnSize[i] > -1)
                    return false;
            }
            return true;
        }

        private int[] GetSizeFromDefintion(string defintion)
        {
            var bracketStart = defintion.IndexOf('(');
            var bracketEnd = defintion.IndexOf(')');
            
            var sizeStrings = defintion
                .Substring(bracketStart + 1, bracketEnd - bracketStart - 1)
                .Split(',');

            var sizes = new List<int>();
            foreach (var sizeString in sizeStrings)
            {
                if (sizeString == "max")
                    sizes.Add(-1);
                else if (int.TryParse(sizeString, out int size))
                    sizes.Add(size);
                else
                    throw new Exception($"Invalid Size in the SQL definition: {defintion}");
            }

            return sizes.ToArray();
        }
    }
}
