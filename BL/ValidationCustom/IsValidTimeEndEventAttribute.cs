using BL.Helpers;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace BL.ValidationCustom
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]

    sealed public class IsValidTimeEndEventAttribute : ValidationAttribute
    {
        public string PropertyHInicio { get; set; }
        public string PropertyFFin { get; set; }
        public string PropertyFInicio { get; set; }
        public IsValidTimeEndEventAttribute(string propertyhInicio,string propertyFFin, string propertyFInicio)
        {
            PropertyHInicio = propertyhInicio;
            PropertyFFin = propertyFFin;
            PropertyFInicio = propertyFInicio;
        }
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var instance = validationContext.ObjectInstance;
            var proprtyhInicio = instance.GetType().GetProperty(PropertyHInicio)?.GetValue(instance, null);
            var proprtyfFin = instance.GetType().GetProperty(PropertyFFin)?.GetValue(instance, null);
            var proprtyfInicio = instance.GetType().GetProperty(PropertyFInicio)?.GetValue(instance, null);

            if (proprtyhInicio != null && value != null)
            {
                string strHInicio = Convert.ToString(proprtyhInicio);
                string strHFin = Convert.ToString(value);
                var horaInicio = TimeSpan.Parse(strHInicio);
                var horaFin = TimeSpan.Parse(strHFin);
                var horaMinima = horaInicio.Add(new TimeSpan(0, 30, 0));
                var fechaInicio = Convert.ToDateTime(proprtyfInicio);

                if (proprtyfFin == null)
                {
                    ErrorMessage = "La hora de fin requiere una fecha de fin";
                    return new ValidationResult(ErrorMessage);
                }
                var fechaFin = Convert.ToDateTime(proprtyfFin);
                if (fechaFin == fechaInicio)
                {
                    if (horaFin == horaInicio)
                    {
                        ErrorMessage = "La hora de fin no puede ser igual a la hora de inicio";
                        return new ValidationResult(ErrorMessage);
                    }
                    if (horaFin < horaInicio)
                    {
                        ErrorMessage = "La hora de fin no puede ser menor a la hora de inicio";
                        return new ValidationResult(ErrorMessage);
                    }
                    if (horaFin < horaMinima)
                    {
                        ErrorMessage = "La hora de fin debe durar mínimo 30 minutos partiendo de la hora inicial";
                        return new ValidationResult(ErrorMessage);
                    }
                }
            }
            return ValidationResult.Success;
        }
    }
}
