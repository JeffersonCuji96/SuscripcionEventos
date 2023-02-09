using BL.ValidationCustom;
using System.ComponentModel.DataAnnotations;

namespace BL.ViewModels
{
    public class TokenPasswordViewModel
    {
        [Required(ErrorMessage = "El token es requerido")]
        [RegularExpression("([a-zA-Z0-9]+$)", ErrorMessage = "Formato del token inválido")]
        public string Token { get; set; } = null!;

        [Required(ErrorMessage = "La clave es requerida")]
        [MinLength(8, ErrorMessage = "Debe tener mínimo 8 caracteres")]
        [MaxLength(15, ErrorMessage = "Debe tener máximo 15 caracteres")]
        [IsPassword]
        public string Clave { get; set; } = null!;
    }
}
