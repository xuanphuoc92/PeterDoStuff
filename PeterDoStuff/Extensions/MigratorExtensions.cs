using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Reflection;
using System.ComponentModel.DataAnnotations;

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
                    .Select(p => GetSqlColumn(p));

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
            { typeof(decimal), ("decimal", "19,4" ) },
            { typeof(int), ("int", "" ) },
            { typeof(float), ("float", "" ) },
            { typeof(double), ("float", "" ) },
            { typeof(DateTime), ("datetime", "" ) },
            { typeof(DateOnly), ("date", "" ) },
        };

        private static bool AmongColumnTypes(Type propertyType)
        {
            return _columnTypes.Keys.Contains(propertyType);
        }
        private static string GetSqlColumn(PropertyInfo pi)
        {
            var name = pi.Name;
            
            string defaultType = _columnTypes[pi.PropertyType].DefaultSqlType;
            string customType = GetCustomType(pi);

            var type = customType != "" ? customType : defaultType;
            
            string defaultSize = _columnTypes[pi.PropertyType].DefaultSize;
            string customSize = GetCustomSize(pi);
            
            string finalSize = customSize != "" ? customSize : defaultSize;
            
            var size = finalSize.IsNullOrEmpty() ? "" : $"({finalSize})";
            return $"    [{name}] {type}{size}";
        }

        private static string GetCustomType(PropertyInfo pi)
        {
            if (pi.PropertyType == typeof(DateTime))
            {
                var attribute = pi.GetCustomAttribute<DateOnlyAttribute>();
                if (attribute != null)
                    return "date";
            }

            return "";
        }

        private static string GetCustomSize(PropertyInfo pi)
        {
            if (pi.PropertyType == typeof(string) || pi.PropertyType == typeof(byte[])) 
            {
                var attribute = pi.GetCustomAttribute<MaxLengthAttribute>();
                if (attribute != null)
                    return attribute.Length.ToString();
            }

            if (pi.PropertyType == typeof(decimal))
            {
                var attribute = pi.GetCustomAttribute<DecimalPrecisionScaleAttribute>();
                if (attribute != null)
                    return $"{attribute.Precision},{attribute.Scale}";
            }

            return "";
        }
    }

    public class DecimalPrecisionScaleAttribute : ValidationAttribute
    {
        public readonly int Precision;
        public readonly int Scale;

        public DecimalPrecisionScaleAttribute(int precision, int scale)
        {
            Precision = precision;
            Scale = scale;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || !(value is decimal))
            {
                return ValidationResult.Success; // Let RequiredAttribute handle null values
            }

            decimal decimalValue = (decimal)value;

            // Check precision
            string stringValue = decimalValue.ToString();
            int integerDigits = stringValue.Contains(".") ? stringValue.Split('.')[0].Length : stringValue.Length;

            if (integerDigits > Precision - Scale)
            {
                return new ValidationResult($"The field {validationContext.DisplayName} must have a precision of {Precision} or fewer digits in total.");
            }

            // Check scale
            if (decimalValue % 1 != 0 && stringValue.Split('.')[1].Length > Scale)
            {
                return new ValidationResult($"The field {validationContext.DisplayName} must have a scale of {Scale} or fewer digits after the decimal point.");
            }

            return ValidationResult.Success;
        }
    }

    public class DateOnlyAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null && value is DateTime)
            {
                var date = (DateTime)value;

                if (date.Hour == 0 && date.Minute == 0 && date.Second == 0)
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult("The value must be a date only (hours, minutes, seconds should all be 0).");
                }
            }

            // If the value is null or not a DateTime instance, then consider it as valid.
            return ValidationResult.Success;
        }
    }
}
