using System;
using System.Collections.Generic;

namespace BL.Models;

public partial class Suscripcion
{
    public long Id { get; set; }

    public long IdUsuario { get; set; }

    public long IdEvento { get; set; }

    public int IdEstado { get; set; }

    public Estado Estado { get; set; } = null!;

    public Evento Evento { get; set; } = null!;

    public Usuario Usuario { get; set; } = null!;
}
