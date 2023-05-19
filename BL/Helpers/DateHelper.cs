using BL.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace BL.Helpers
{
    public class DateHelper
    {
        /// <summary>
        /// Método para calcular la edad del usuario a partir de la fecha de nacimiento
        /// </summary>
        /// <param name="birthDate"></param>
        /// <returns></returns>
        public static int GetAgeBirthDate(DateTime birthDate)
        {
            var fechaActual = DateTime.Today;
            int edad = fechaActual.Year - birthDate.Year;
            if (fechaActual < birthDate.AddYears(edad))
                edad--;
            return edad;
        }

        /// <summary>
        /// Método para obtener la fecha actual del servidor usando un procedimiento almacenado
        /// </summary>
        /// <returns></returns>
        public static DateTime GetCurrentDate()
        {
            using var db = new DbSuscripcionEventosContext();
            var parameter = new SqlParameter
            {
                ParameterName = "@currentDate",
                SqlDbType = SqlDbType.DateTime,
                Direction = ParameterDirection.Output
            };
            db.Database.ExecuteSqlRaw("exec SPGetCurrentDate @currentDate OUTPUT", parameter);
            return Convert.ToDateTime(parameter.Value);
        }
    }
}
