using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using PeterDoStuff.Attributes;

namespace PeterDoStuff.Extensions
{
    public static class MigratorExtensions
    {
        public static Migrator GetMigrator(this DbContext context)
            => new(context);

        internal static IEnumerable<PropertyInfo> GetDbSetInfos(this DbContext context)
            => context
                .GetType()
                .GetProperties()
                .Where(p =>
                    p.PropertyType.IsGenericType &&
                    p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>));
    }

    public class Migrator
    {
        private readonly DbContext context;
        public Migrator(DbContext context) 
        {
            this.context = context;
        }

        public string Sql()
        {
            var dbSets = context.GetDbSetInfos();

            StringBuilder sql = new StringBuilder();
            foreach (var dbSet in dbSets)
            {
                var entityType = dbSet.PropertyType.GetGenericArguments()[0];
                var columns = entityType
                    .GetProperties()
                    .Where(p => AmongColumnTypes(p.PropertyType))
                    .Select(p => GetSqlTerm(p));

                sql.AppendLine($"CREATE TABLE [{dbSet.Name}] (");
                sql.AppendLine(columns.Join(",\n"));
                sql.AppendLine($");");
            }
            return sql.ToString();
        }

        private static readonly Dictionary<Type, (string DefaultSqlType, string DefaultSize)> _columnTypes
            = new Dictionary<Type, (string DefaultSqlType, string DefaultSize)>()
        {
            { typeof(string), ("nvarchar", "max" ) },
            { typeof(Guid), ("uniqueidentifier", "" ) },
            { typeof(byte[]), ("varbinary", "max" ) },
            { typeof(decimal), ("decimal", "" ) },
            { typeof(int), ("int", "" ) },
            { typeof(float), ("float", "" ) },
            { typeof(double), ("float", "" ) },
            { typeof(DateTime), ("datetime", "" ) },
            { typeof(DateOnly), ("date", "" ) },
        };

        private static bool AmongColumnTypes(Type propertyType)
        {
            Type typeToCheck = GetInnerTypeIfNullable(propertyType);
            return _columnTypes.ContainsKey(typeToCheck) || typeToCheck.IsEnum;
        }
        private static string GetSqlTerm(PropertyInfo pi)
        {
            var name = pi.Name;

            string defaultType = GetDefaultType(pi.PropertyType);
            string customType = GetCustomType(pi);
            var type = customType != "" ? customType : defaultType;
            
            string defaultSize = GetDefaultSize(pi, type);
            string customSize = GetCustomSize(pi);
            string finalSize = customSize != "" ? customSize : defaultSize;
            var size = finalSize.IsNullOrEmpty() ? "" : $"({finalSize})";

            return $"    [{name}] {type}{size}";
        }

        private static string GetDefaultSize(PropertyInfo pi, string type)
        {
            Type typeToCheck = GetInnerTypeIfNullable(pi.PropertyType);

            return _columnTypes.ContainsKey(typeToCheck)
                ? _columnTypes[typeToCheck].DefaultSize
                : (type == "nvarchar" ? GetEnumSize(pi) : ""); // Enum
        }

        private static string GetDefaultType(Type propertyType)
        {
            Type typeToCheck = GetInnerTypeIfNullable(propertyType);

            return _columnTypes.ContainsKey(typeToCheck)
                ? _columnTypes[typeToCheck].DefaultSqlType
                : "nvarchar"; // Enum
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

        private static string GetEnumSize(PropertyInfo pi)
        {
            var enumValues = pi.PropertyType.GetEnumValues();
            int maxValue = -1;
            foreach (var enumValue in enumValues ) 
            {
                if (enumValue.ToString().Length > maxValue)
                    maxValue = enumValue.ToString().Length;
            }
            return maxValue.ToString();
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

            if (typeToCheck.IsEnum)
            {
                var attribute = pi.GetCustomAttribute<NumberEnumAttribute>();
                if (attribute != null)
                    return "int";
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
