using BL.ValidationCustom;
using System.ComponentModel.DataAnnotations;

namespace BL.ViewModels
{
    public class FilePhotoViewModel
    {
        [Required(ErrorMessage = "El código del usuario es requerido")]
        public long? Id { get; set; }

        [Required(ErrorMessage = "La imagen es requerida")]
        [IsAvailablePhoto(1000000)]
        public string Photo { get; set; } = null!;
    }
}
