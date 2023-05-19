using System.ComponentModel.DataAnnotations;

namespace BL.ValidationCustom
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    sealed public class IsValidDigitsAttribute : ValidationAttribute
    {
        public IsValidDigitsAttribute(string message)
        {
            ErrorMessage = message;
        }

        /// <summary>
        /// Método que valida el número de digitos de un teléfono
        /// </summary>
        /// <remarks>
        /// La cantidad que se verifica es que sea de 10 para un número de ecuador
        /// </remarks>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool IsValid(object? value)
        {
            string? num = Convert.ToString(value);
            if (num == "")
                return true;
            if (num.Length != 10)
                return false;
            return true;
        }
    }
}
