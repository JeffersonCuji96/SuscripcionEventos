using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.ViewModels
{
    public class TokenValidViewModel
    {
        [Required(ErrorMessage = "El token es requerido")]
        [RegularExpression("([a-zA-Z0-9]+$)", ErrorMessage = "Formato del token inválido")]
        public string Token { get; set; } = null!;
    }
}
