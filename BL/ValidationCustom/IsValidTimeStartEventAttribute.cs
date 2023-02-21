using BL.Helpers;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace BL.ValidationCustom
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]

    sealed public class IsValidTimeStartEventAttribute : ValidationAttribute
    {
        public string PropertyFInicio { get; set; }
        public IsValidTimeStartEventAttribute(string propertyFInicio)
        {
            PropertyFInicio = propertyFInicio;
        }
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var instance = validationContext.ObjectInstance;
            var proprtyFInicio = instance.GetType().GetProperty(PropertyFInicio)?.GetValue(instance, null);
            if(proprtyFInicio != null && value != null)
            {
                var fechaInicio = Convert.ToDateTime(proprtyFInicio);
                var currentDate = DateHelper.GetCurrentDate();
                
                var strHora = Convert.ToString(value);
                var horaInicio= TimeSpan.Parse(strHora);

                if (fechaInicio == currentDate.Date)
                {
                    var horaMinima = TimeSpan.Parse(currentDate.AddMinutes(30).ToString("HH:mm tt"));
                    var horaActual = TimeSpan.Parse(currentDate.ToString("HH:mm tt"));
                    var midnight = new TimeSpan(0, 0, 0);

                    if (horaInicio == midnight)
                    {
                        ErrorMessage = "No se puede establecer una hora de inicio en el pasado";
                        return new ValidationResult(ErrorMessage);
                    }
                    if (horaInicio == horaActual)
                    {
                        ErrorMessage = "La hora de inicio no puede ser igual a la hora actual";
                        return new ValidationResult(ErrorMessage);
                    }
                    if (horaInicio < horaActual)
                    {
                        ErrorMessage = "La hora de inicio no puede ser menor a la hora actual";
                        return new ValidationResult(ErrorMessage);
                    }
                    if (horaInicio < horaMinima)
                    {
                        ErrorMessage = "La hora de inicio debe tener mínimo 30 minutos de anticipación";
                        return new ValidationResult(ErrorMessage);
                    }
                }
            }
            return ValidationResult.Success;
        }
    }
}
