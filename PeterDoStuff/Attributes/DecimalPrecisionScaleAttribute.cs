using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeterDoStuff.Attributes
{
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
}
