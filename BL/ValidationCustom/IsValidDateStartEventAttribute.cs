using BL.Helpers;
using System.ComponentModel.DataAnnotations;

namespace BL.ValidationCustom
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]

    sealed public class IsValidDateStartEventAttribute : ValidationAttribute
    {
        public IsValidDateStartEventAttribute(string message)
        {
            ErrorMessage = message;
        }

        /// <summary>
        /// Método para validar la fecha inicial de un evento 
        /// </summary>
        /// <remarks>
        /// Se verifica que la fecha inicial no sea menor a la fecha actual
        /// </remarks>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool IsValid(object? value)
        {
            if (value != null)
            {
                var dateValue = Convert.ToDateTime(value);
                var currentDate = DateHelper.GetCurrentDate().Date;
                if (dateValue < currentDate)
                    return false;
            }
            return true;
        }
    }
}
