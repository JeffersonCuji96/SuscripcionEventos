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
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var instance = validationContext.ObjectInstance;
            string? email = instance.GetType().GetProperty("Email")?.GetValue(instance, null)?.ToString();
            string eEmail = String.Empty;
            if (!string.IsNullOrEmpty(email))
            {
                eEmail = Crypto.GetSHA256(email.ToString());
                using var db = new DbSuscripcionEventosContext();
                SqlParameter[] parameters = {
                    new SqlParameter{ ParameterName = "@email", SqlDbType=SqlDbType.VarChar,Size=100, Value = eEmail },
                    new SqlParameter{ ParameterName = "@id",SqlDbType=SqlDbType.BigInt, Direction = ParameterDirection.Output }
                };
                db.Database.ExecuteSqlRaw("exec SPGetIdUserByEmail @email, @id OUTPUT", parameters);
                if(!string.IsNullOrEmpty(parameters[1].Value.ToString()))
                    instance.GetType().GetProperty("Id")?.SetValue(instance, Convert.ToInt64(parameters[1].Value));
            }
            return ValidationResult.Success;
        }
    }
}
