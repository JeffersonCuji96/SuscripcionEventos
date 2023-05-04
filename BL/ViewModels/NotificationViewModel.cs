namespace BL.ViewModels
{
    public class NotificationViewModel
    {
        public string IdEvento { get; set; } = null!;
        public long IdUsuarioSuscrito { get; set; }
        public string TituloEvento { get; set; } = null!;
        public string InicioEvento { get; set; } = null!;
        public int Estado { get; set; }
    }
    public class MessageViewModel
    {
        public string Grupo { get; set; } = null!;
        public string Evento { get; set; } = null!;
        public string Inicio { get; set; } = null!;
    }
}
