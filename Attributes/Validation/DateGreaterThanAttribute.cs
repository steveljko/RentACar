using System.ComponentModel.DataAnnotations;

namespace RentACar.Attributes.Validation;

public class DateGreaterThanAttribute : ValidationAttribute
{
    private readonly string _property;
    
    public DateGreaterThanAttribute(string property)
    {
        _property = property;
    }
    
    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        var currentValue = (DateTime?) value;
        
        var comparisonValue = (DateTime?) validationContext.ObjectType
            .GetProperty(_property)?
            .GetValue(validationContext.ObjectInstance);
        
        if (currentValue.HasValue && comparisonValue.HasValue)
        {
            if (currentValue <= comparisonValue)
            {
                return new ValidationResult(ErrorMessage);
            }
        }
        
        return ValidationResult.Success;
    }
}