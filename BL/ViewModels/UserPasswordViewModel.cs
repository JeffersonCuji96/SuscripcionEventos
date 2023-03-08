using BL.ValidationCustom;
using System.ComponentModel.DataAnnotations;

namespace BL.ViewModels
{
    public class UserPasswordViewModel
    {
        [Required(ErrorMessage = "El código del usuario es requerido")]
        public long? Id { get; set; }

        [Required(ErrorMessage = "La clave es requerida")]
        [MinLength(8, ErrorMessage = "Debe tener mínimo 8 caracteres")]
        [MaxLength(15, ErrorMessage = "Debe tener máximo 15 caracteres")]
        [IsPassword]
        public string Clave { get; set; } = null!;

        [Required(ErrorMessage = "La clave actual es requerida")]
        public string ClaveActual { get; set; } = null!;
    }
}
