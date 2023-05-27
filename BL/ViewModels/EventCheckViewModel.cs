namespace BL.ViewModels
{
    public class EventCheckViewModel
    {
        public long IdUsuario { get; set; }
        public DateTime FechaInicio { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public TimeSpan? HoraFin { get; set; }
    }
}