using BL.ValidationCustom;
using System.ComponentModel.DataAnnotations;

namespace BL.ViewModels
{
    public class UserEmailViewModel
    {
        [IsAvailableId]
        public long? Id { get; set; }

        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "El formato del email es inválido")]
        [RegularExpression("([a-zA-Z0-9ñÑ.@]+$)", ErrorMessage = "Se permiten letras (a-z), números (0-9) y puntos (.)")]
        public string Email { get; set; } = null!;
    }
}
