using BL.Helpers;
using System.ComponentModel.DataAnnotations;

namespace BL.ValidationCustom
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]

    sealed public class IsValidDateEndEventAttribute : ValidationAttribute
    {
        public string PropertyFInicio { get; set; }
        public string PropertyHFin { get; set; }
        public IsValidDateEndEventAttribute(string propertyfInicio,string propertyhFin)
        {
            PropertyFInicio = propertyfInicio;
            PropertyHFin = propertyhFin;
        }

        /// <summary>
        /// Método para validar la fecha final de un evento
        /// </summary>
        /// <remarks>
        /// Se compara que la fecha final que se ingresa no sea menor a la fecha actual, 
        /// menor a la fecha de inicio, y que tenga una hora de fin ya que es requerido
        /// junto a la fecha final
        /// </remarks>
        /// <param name="value"></param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var instance = validationContext.ObjectInstance;
            var proprtyfInicio = instance.GetType().GetProperty(PropertyFInicio)?.GetValue(instance, null);
            var proprtyHFin = instance.GetType().GetProperty(PropertyHFin)?.GetValue(instance, null);

            if (proprtyfInicio != null && value != null)
            {
                var fechaInicio = Convert.ToDateTime(proprtyfInicio);
                var strHoraFin = Convert.ToString(proprtyHFin);
                var fechaFin = Convert.ToDateTime(value);
                var currentDate = DateHelper.GetCurrentDate().Date;
                if (fechaFin < currentDate)
                {
                    ErrorMessage = "La fecha de fin no pueder ser menor a la fecha actual";
                    return new ValidationResult(ErrorMessage);
                }
                if (fechaFin < fechaInicio)
                {
                    ErrorMessage = "La fecha de fin no pueder ser menor a la fecha de inicio";
                    return new ValidationResult(ErrorMessage);
                }
                if (string.IsNullOrEmpty(strHoraFin))
                {
                    ErrorMessage = "La fecha de fin requiere una hora de fin";
                    return new ValidationResult(ErrorMessage);
                }
            }
            return ValidationResult.Success;
        }
    }
}
