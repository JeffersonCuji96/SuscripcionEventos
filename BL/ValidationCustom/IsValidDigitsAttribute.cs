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
