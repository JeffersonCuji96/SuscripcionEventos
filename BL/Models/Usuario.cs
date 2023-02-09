using System;
using System.Collections.Generic;

namespace BL.Models;

public partial class Usuario
{
    public long Id { get; set; }

    public string Email { get; set; } = null!;

    public string Clave { get; set; } = null!;

    public string? Token { get; set; }

    public DateTime? FechaTokenExpiracion { get; set; }

    public int IdEstado { get; set; }
    public Estado Estado { get; set; } = null!;
    public Persona Persona { get; set; } = null!;
    public virtual ICollection<Evento> Eventos { get; } = new List<Evento>();
    public virtual ICollection<Suscripcion> Suscripciones { get; } = new List<Suscripcion>();
}
