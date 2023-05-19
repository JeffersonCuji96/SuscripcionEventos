using BL.Helpers;
using BL.Models;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.RegularExpressions;

namespace BL.ValidationCustom
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    sealed public class IsAdultAttribute : ValidationAttribute
    {
        public IsAdultAttribute(string message)
        {
            ErrorMessage = message;
        }

        /// <summary>
        /// Método para validar que el usuario sea mayor de edad
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool IsValid(object? value)
        {
            if (value != null)
            {
                int age = DateHelper.GetAgeBirthDate(Convert.ToDateTime(value));
                return age >= 18;
            }
            return true;
        }
    }
}
