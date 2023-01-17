using System;
using System.Collections.Generic;

namespace BL.Models;

public partial class Persona
{
    public long Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Apellido { get; set; } = null!;

    public string? Telefono { get; set; }

    public DateTime FechaNacimiento { get; set; }

    public string? Foto { get; set; }

    public Usuario? Usuario { get; set; }
}
