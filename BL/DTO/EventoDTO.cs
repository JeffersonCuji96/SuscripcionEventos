using BL.Models;
using BL.ValidationCustom;
using System.ComponentModel.DataAnnotations;

namespace BL.DTO
{
    public class EventoDTO
    {
        public long? Id { get; set; }
        
        [Required(ErrorMessage = "El título es requerido")]
        [MaxLength(100, ErrorMessage = "Título máximo de 100 caracteres")]
        public string Titulo { get; set; } = null!;

        [Required(ErrorMessage = "La fecha de inicio es requerida")]
        [IsValidDateStartEvent("La fecha de inicio no pueder ser menor a la fecha actual")]
        public DateTime? FechaInicio { get; set; }
        
        [Required(ErrorMessage = "La hora de inicio es requerida")]
        [IsValidTimeStartEvent(nameof(FechaInicio))]
        public TimeSpan? HoraInicio { get; set; }

        [IsValidDateEndEvent(nameof(FechaInicio),nameof(HoraFin))]
        public DateTime? FechaFin { get; set; }

        [IsValidTimeEndEvent(nameof(HoraInicio),nameof(FechaFin),nameof(FechaInicio))]
        public TimeSpan? HoraFin { get; set; }

        [Required(ErrorMessage = "La dirección es requerida")]
        [MaxLength(500, ErrorMessage = "Dirección máxima de 500 caracteres")]
        public string Ubicacion { get; set; } = null!;

        [MaxLength(500, ErrorMessage = "Información adicional máxima de 500 caracteres")]
        public string? InformacionAdicional { get; set; }

        [IsAvailablePhoto(3000000)]
        public string? ImageBase64 { get; set; }

        [Required(ErrorMessage = "La categoría es requerida")]
        public int? IdCategoria { get; set; }
        public CategoriaDTO? Categoria { get; set; }

        [Required(ErrorMessage = "El usuario es requerido")]
        public long? IdUsuario { get; set; }
        public int IdEstado { get; set; } = 1;
    }
}
