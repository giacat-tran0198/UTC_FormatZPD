using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace FormatZPD.Validations
{
    public class MatchInArrayStringAttribute : ValidationAttribute
    {
        private readonly string[] _stringArray;

        public MatchInArrayStringAttribute(string[] stringArray)
        {
            _stringArray = stringArray;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string type = value as string;
            if (type == null)
                return ValidationResult.Success;
            if (_stringArray.All(str => !String.Equals(str, type, StringComparison.CurrentCultureIgnoreCase)))
                return new ValidationResult(
                    "The " + validationContext.DisplayName + " must be in following strings: " +
                    string.Join(", ", _stringArray));
            return ValidationResult.Success;
        }
    }
}