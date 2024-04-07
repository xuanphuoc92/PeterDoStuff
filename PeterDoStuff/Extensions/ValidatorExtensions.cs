using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeterDoStuff.Extensions
{
    public static class ValidatorExtensions
    {
        public static bool IsValid<T>(this T @this)
            => IsValid(@this, out _);
        
        public static bool IsValid<T>(this T @this, out List<ValidationResult> results)
        {
            results = new List<ValidationResult>();
            var context = new ValidationContext(@this);
            return Validator.TryValidateObject(@this, context, results, true);
        }
    }
}
