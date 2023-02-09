using BL.ValidationCustom;
using System.ComponentModel.DataAnnotations;

namespace BL.DTO
{
    public class UsuarioDTO
    {
        public long? Id { get; set; }

        [EmailAddress(ErrorMessage = "El formato del email es inválido")]
        [Required(ErrorMessage = "El email es requerido")]
        [RegularExpression("([a-zA-Z0-9ñÑ.@]+$)", ErrorMessage = "Se permiten letras (a-z), números (0-9) y puntos (.)")]
        [IsEmailExist("El email no está disponible")]
        public string Email { get; set; } = null!;
        
        [Required(ErrorMessage = "La clave es requerida")]
        [MinLength(8, ErrorMessage = "Debe tener mínimo 8 caracteres")]
        [MaxLength(15,ErrorMessage = "Debe tener máximo 15 caracteres")]
        [IsPassword]
        public string Clave { get; set; } = null!;

        [IsAvailablePhoto(1000000)]
        public string? ImageBase64 { get; set; }
        public int IdUsuario { get; set; }
        public PersonaDTO Persona { get; set; } = null!;
    }
}
