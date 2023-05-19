using BL.Helpers;
using BL.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace BL.ValidationCustom
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    sealed public class IsAvailableIdAttribute : ValidationAttribute
    {
        /// <summary>
        /// Método que obtiene el id del usuario para establecerlo a la propiedad id
        /// </summary>
        /// <remarks>
        /// En caso de que el id del usuario no tenga un valor, Se obtiene el id por medio de la 
        /// propiedad email usando un procedimiento
        /// </remarks>
        /// <param name="value"></param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                var instance = validationContext.ObjectInstance;
                string? email = instance.GetType().GetProperty("Email")?.GetValue(instance, null)?.ToString();
                if (!string.IsNullOrEmpty(email))
                {
                    string eEmail = Crypto.GetSHA256(email.ToString());
                    using var db = new DbSuscripcionEventosContext();
                    SqlParameter[] parameters = {
                    new SqlParameter{ ParameterName = "@email", SqlDbType=SqlDbType.VarChar,Size=100, Value = eEmail },
                    new SqlParameter{ ParameterName = "@id",SqlDbType=SqlDbType.BigInt, Direction = ParameterDirection.Output }
                };
                    db.Database.ExecuteSqlRaw("exec SPGetIdUserByEmail @email, @id OUTPUT", parameters);
                    if (!string.IsNullOrEmpty(parameters[1].Value.ToString()))
                        instance.GetType().GetProperty("Id")?.SetValue(instance, Convert.ToInt64(parameters[1].Value));
                }
            }
            return ValidationResult.Success;
        }
    }
}
