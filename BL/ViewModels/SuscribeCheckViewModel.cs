using System.ComponentModel.DataAnnotations;

namespace BL.ViewModels
{
    public class SuscribeCheckViewModel
    {
        [Required(ErrorMessage = "El código del usuario es requerido")]
        public long? IdUsuario { get; set; }

        [Required(ErrorMessage = "El código del evento es requerido")]
        public long? IdEvento { get; set; }
    }
}
