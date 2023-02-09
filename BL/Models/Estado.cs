using System;
using System.Collections.Generic;

namespace BL.Models;

public partial class Estado
{
    public int Id { get; set; }

    public string Descripcion { get; set; } = null!;

    public virtual ICollection<Evento> Eventos { get; } = new List<Evento>();

    public virtual ICollection<Suscripcion> Suscripciones { get; } = new List<Suscripcion>();
    public virtual ICollection<Usuario> Usuarios { get; } = new List<Usuario>();
}
