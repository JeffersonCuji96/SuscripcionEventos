using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Models;

[Table("Categoria")]
public partial class Categoria
{
    public int Id { get; set; }

    public string Descripcion { get; set; } = null!;

    public virtual ICollection<Evento> Eventos { get; } = new List<Evento>();
}
