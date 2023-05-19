using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace BL.ValidationCustom
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    sealed public class IsPasswordAttribute : ValidationAttribute
    {
        /// <summary>
        /// Método que valida una contraseña
        /// </summary>
        /// <remarks>
        /// Se verifica que la clave que ingresa el usuario debe tener mínimo un número,
        /// una letra, mayúscula y minúscula, un caracter especial y no contener espacios,
        /// </remarks>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool IsValid(object? value)
        {
            string? pass = value?.ToString();
            if (!string.IsNullOrEmpty(pass))
            {
                if (!Regex.Match(pass, @"\d").Success)
                {
                    ErrorMessage = "Debe contener mínimo un número";
                    return false;
                }
                if (pass.Contains(' '))
                {
                    ErrorMessage = "No debe contener espacios";
                    return false;
                }
                else if (!Regex.Match(pass, @"[A-Z]").Success)
                {
                    ErrorMessage = "Debe contener mínimo un letra mayúscula";
                    return false;
                }
                else if (!Regex.Match(pass, @"[a-z]").Success)
                {
                    ErrorMessage = "Debe contener mínimo un letra minúscula";
                    return false;
                }
                else if (!Regex.Match(pass, @"[.*@%#]").Success)
                {
                    ErrorMessage = "Debe contener mínimo un caracter especial: *@.%#";
                    return false;
                }
            }
            return true;
        }
    }
}
