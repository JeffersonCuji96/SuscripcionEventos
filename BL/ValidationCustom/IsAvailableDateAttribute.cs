using BL.Helpers;
using System.ComponentModel.DataAnnotations;

namespace BL.ValidationCustom
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]

    sealed public class IsAvailableDateAttribute: ValidationAttribute
    {
        public IsAvailableDateAttribute(string message)
        {
            ErrorMessage = message;
        }

        /// <summary>
        /// Método para validar si fecha ingresada es mayor o igual a la fecha actual del servidor
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool IsValid(object? value)
        {
            if (value != null)
            {
                var dateValue = Convert.ToDateTime(value);
                var currentDate = DateHelper.GetCurrentDate().Date;
                if (dateValue >= currentDate)
                    return false;
            }
            return true;
        }
    }
}
