using System.ComponentModel.DataAnnotations;

namespace BL.DTO
{
    public class AccesoDTO
    {

        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La clave es requerida")]
        public string Clave { get; set; } = string.Empty;
    }
}
