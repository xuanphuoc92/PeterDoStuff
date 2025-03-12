using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeterDoStuff.Attributes
{
    public class DateOnlyAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
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

            // If the value is null, then consider it as valid.
            return ValidationResult.Success;
        }
    }
}
