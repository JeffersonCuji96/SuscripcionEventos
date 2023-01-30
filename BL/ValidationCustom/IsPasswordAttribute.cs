using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace BL.ValidationCustom
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    sealed public class IsPasswordAttribute : ValidationAttribute
    {
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
