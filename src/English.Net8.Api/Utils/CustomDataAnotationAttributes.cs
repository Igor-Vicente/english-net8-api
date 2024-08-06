using System.ComponentModel.DataAnnotations;

namespace English.Net8.Api.Utils
{
    public class EnsureArraySizeAttribute : ValidationAttribute
    {
        private readonly int _minElements;
        private readonly int _maxElements;

        public EnsureArraySizeAttribute(int minElements, int maxElements)
        {
            _minElements = minElements;
            _maxElements = maxElements;
        }

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value is Array array)
            {
                if (array.Length >= _minElements && array.Length <= _maxElements)
                {
                    return ValidationResult.Success!;
                }
                else if (array.Length < _minElements)
                {
                    var errorMessage = string.IsNullOrEmpty(ErrorMessage) ?
                        $"The array must contain at least {_minElements} elements." :
                        ErrorMessage;
                    return new ValidationResult(errorMessage);
                }
                else if (array.Length > _maxElements)
                {
                    var errorMessage = string.IsNullOrEmpty(ErrorMessage) ?
                        $"The array must contain no more than {_maxElements} elements." :
                        ErrorMessage;
                    return new ValidationResult(errorMessage);
                }
            }

            return new ValidationResult("Invalid array.");
        }
    }

    public class MaxLengthForEachAttribute : ValidationAttribute
    {
        private readonly int _maxLength;

        public MaxLengthForEachAttribute(int maxLength)
        {
            _maxLength = maxLength;
        }

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            var array = value as string[];

            if (array == null)
                return ValidationResult.Success!;

            foreach (var str in array)
            {
                if (str.Length > _maxLength)
                {
                    return new ValidationResult($"Each string in the array must be at most {_maxLength} characters long.");
                }
            }

            return ValidationResult.Success!;
        }
    }
}
