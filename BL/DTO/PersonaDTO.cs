using BL.ValidationCustom;
using System.ComponentModel.DataAnnotations;

namespace BL.DTO
{
    public class PersonaDTO
    {
        public long? Id { get; set; } = 0;

        [Required(ErrorMessage = "El nombre es requerido")]
        [RegularExpression(@"^(?!\s)(?![\s\S]*\s$)([a-zA-ZáéíóúüñÁÉÍÓÚÑÜ]\s{0,1})+$", ErrorMessage = "El nombre sólo debe contener letras y/o un espacio entre palabras")]
        public string Nombre { get; set; } = null!;

        [Required(ErrorMessage = "El apellido es requerido")]
        [RegularExpression(@"^(?!\s)(?![\s\S]*\s$)([a-zA-ZáéíóúüñÁÉÍÓÚÑÜ]\s{0,1})+$", ErrorMessage = "El apellido sólo debe contener letras y/o un espacio entre palabras")]
        public string Apellido { get; set; } = null!;

        [StringLength(10, MinimumLength = 10, ErrorMessage = "El teléfono debe tener 10 dígitos")]
        [RegularExpression("(^[0-9]+$)", ErrorMessage = "El teléfono solo debe contener números")]
        [IsPhoneExist(nameof(Id), "El teléfono está en uso, ingrese otro")]
        public string? Telefono { get; set; }

        [Required(ErrorMessage = "La fecha de nacimiento es requerido")]
        [IsAdult("Debe ser mayor de edad")]
        [IsAvailableDate("La fecha no es válida")]
        public DateTime? FechaNacimiento { get; set; }
        public string? Foto { get; set; }
    }
}
