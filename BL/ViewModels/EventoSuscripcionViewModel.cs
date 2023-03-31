namespace BL.ViewModels
{
    public class EventoSuscripcionViewModel
    {
        public long Id { get; set; }
        public string Titulo { get; set; } = null!;
        public DateTime FechaInicio { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public TimeSpan? HoraFin { get; set; }
        public string Ubicacion { get; set; } = null!;
        public string? InformacionAdicional { get; set; }
        public string? Foto { get; set; }
        public string Categoria { get; set; } = null!;
        public PersonaViewModel Organizador { get; set; } = null!;
        public long Suscriptores { get; set; }
    }
}
