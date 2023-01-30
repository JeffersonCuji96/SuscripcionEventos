using BL.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Helpers
{
    internal class DateHelper
    {
        public static int GetAgeBirthDate(DateTime birthDate)
        {
            var fechaActual = DateTime.Today;
            int edad = fechaActual.Year - birthDate.Year;
            if (fechaActual < birthDate.AddYears(edad))
                edad--;
            return edad;
        }
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
