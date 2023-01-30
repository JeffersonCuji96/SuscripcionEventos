using BL.Models;
using System.ComponentModel.DataAnnotations;

namespace BL.ValidationCustom
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    sealed public class IsPhoneExistAttribute : ValidationAttribute
    {
        public string PropertyName { get; set; }
        public IsPhoneExistAttribute(string propertyName, string message)
        {
            PropertyName = propertyName;
            ErrorMessage = message;
        }
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var instance = validationContext.ObjectInstance;
            var proprtyvalue = instance.GetType().GetProperty(PropertyName)?.GetValue(instance, null);
            long id = proprtyvalue == null ? 0 : Convert.ToInt64(proprtyvalue);
            string? currTelefono = value?.ToString();

            if (!string.IsNullOrEmpty(currTelefono))
            {
                using var db = new DbSuscripcionEventosContext();
                bool phoneExist = db.Personas.Any(x => x.Telefono == currTelefono);
                if (id == 0)
                {
                    if (phoneExist)
                        return new ValidationResult(ErrorMessage);
                    return ValidationResult.Success;
                }
                var prevTelefono = db.Personas.First(x => x.Id == id).Telefono;
                if (prevTelefono != currTelefono && phoneExist)
                    return new ValidationResult(ErrorMessage);
            }
            return ValidationResult.Success;
        }
    }
}
