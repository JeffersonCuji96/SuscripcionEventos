using System;
using System.Collections.Generic;

namespace BL.Models;

public partial class Categoria
{
    public int Id { get; set; }

    public string Descripcion { get; set; } = null!;

    public virtual ICollection<Evento> Eventos { get; } = new List<Evento>();
}
