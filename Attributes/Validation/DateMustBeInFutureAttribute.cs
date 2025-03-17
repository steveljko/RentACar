using System.ComponentModel.DataAnnotations;

namespace RentACar.Attributes.Validation;

public class DateMustBeInFutureAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value is DateTime dateTime)
        {
            if (dateTime.Date < DateTime.Today)
            {
                return new ValidationResult(ErrorMessage ?? "The date must be today or in the future.");
            }
        }
        return ValidationResult.Success;
    }
}