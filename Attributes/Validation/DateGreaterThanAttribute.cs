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
        var startDateProperty = validationContext.ObjectType.GetProperty(_property);
        if (startDateProperty == null)
        {
            throw new InvalidOperationException($"Unknown property: {_property}");
        }
        
        if (startDateProperty.PropertyType != typeof(DateTime))
        {
            throw new InvalidOperationException($"Property {_property} is not of type DateTime.");
        }
    
        var startDateValue = (DateTime) startDateProperty.GetValue(validationContext.ObjectInstance);
        var endDateValue = (DateTime) value;
    
        if (endDateValue <= startDateValue)
        {
            return new ValidationResult(ErrorMessage ?? $"{_property} must be greater than {startDateProperty.Name}.");
        }
    
        return ValidationResult.Success;
    }
}