using System.ComponentModel.DataAnnotations;

namespace BL.DTO
{
    public class SuscripcionDTO
    {
        public long? Id { get; set; }

        [Required(ErrorMessage = "El código del usuario es requerido")]
        public long? IdUsuario { get; set; }

        [Required(ErrorMessage = "El código del evento es requerido")]
        public long? IdEvento { get; set; }

        [Required(ErrorMessage = "El código del estado es requerido")]
        public int? IdEstado { get; set; }
        public EventoDTO? Evento { get; set; }

    }
}
