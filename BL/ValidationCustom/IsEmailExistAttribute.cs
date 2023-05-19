using BL.Models;
using BL.Helpers;
using System.ComponentModel.DataAnnotations;

namespace BL.ValidationCustom
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    sealed public class IsEmailExistAttribute : ValidationAttribute
    {
        public IsEmailExistAttribute(string message)
        {
            ErrorMessage = message;
        }

        /// <summary>
        /// Método que valida la existencia de un email
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool IsValid(object? value)
        {
            string? email = value?.ToString();
            if (!string.IsNullOrEmpty(email))
            {
                using var db = new DbSuscripcionEventosContext();
                string eEmail = Crypto.GetSHA256(email);
                var emailExist = db.Usuarios.Any(x => x.Email == eEmail);
                if (emailExist)
                    return false;
            }
            return true;
        }
    }
}
