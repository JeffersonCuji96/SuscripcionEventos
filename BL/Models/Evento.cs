using System;
using System.Collections.Generic;

namespace BL.Models;

public partial class Evento
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

    public int IdCategoria { get; set; }

    public long IdUsuario { get; set; }

    public int IdEstado { get; set; }

    public Categoria Categoria { get; set; } = null!;

    public Estado Estado { get; set; } = null!;

    public Usuario Usuario { get; set; } = null!;

    public virtual ICollection<Suscripcion> Suscripciones { get; } = new List<Suscripcion>();
}
