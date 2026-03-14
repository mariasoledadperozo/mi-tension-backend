// Author: María Soledad Perozo
using System.ComponentModel.DataAnnotations;

namespace mi_tension_backend.Attributes
{
    /// <summary>
    /// Atributo de validación personalizado que asegura que una fecha no sea posterior a la actual.
    /// </summary>
    public class NoFutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null) return ValidationResult.Success;

            DateTime dateToValidate;

            if (value is DateOnly dateOnly)
            {
                dateToValidate = dateOnly.ToDateTime(TimeOnly.MinValue);
            }
            else if (value is DateTime dateTime)
            {
                dateToValidate = dateTime;
            }
            else
            {
                return new ValidationResult("Formato de fecha no válido.");
            }

            if (dateToValidate > DateTime.Now)
            {
                return new ValidationResult(ErrorMessage ?? "La fecha no puede ser futura.");
            }

            return ValidationResult.Success;
        }
    }
}
