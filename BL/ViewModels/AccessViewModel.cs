using System;

namespace BL.ViewModels
{
    public class AccessViewModel
    {
        public long IdUsuario { get; set; }
        public string JwtToken { get; set; }
        public int DaysExpireToken { get; set; }
        public string FullName { get; set; }

        public AccessViewModel()
        {
            IdUsuario = 0;
            JwtToken = "No procesado";
            DaysExpireToken = 0;
            FullName = string.Empty;
        }
    }
}
