using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Direct_Barber.Models;

public partial class Usuario
{
    public int Id { get; set; }

    public string? Nombre { get; set; }

    public string? Apellido { get; set; }

    public string Correo { get; set; } = null!;

    public string Contrasena { get; set; } = null!;

    public string? Direccion { get; set; }

    public string? Telefono { get; set; }

    public DateTime? FecRegistro { get; set; }

    public DateOnly? FecNacimiento { get; set; }

    public decimal? Calificacion { get; set; }

    public string? Foto { get; set; }
    public string? Descripcion { get; set; }

    [NotMapped]
    public IFormFile ImagenFile { get; set; }

    public string Documento { get; set; }

    public int Id_Rol { get; set; } // Relación con Rol
    public Rol Rol { get; set; } // Propiedad de navegación

    public virtual ICollection<Solicitud> SolicitudesComoBarbero { get; set; } = new List<Solicitud>();

    public virtual ICollection<Solicitud> SolicitudesComoCliente { get; set; } = new List<Solicitud>();

    public ICollection<Resena> ResenasComoCliente { get; set; }
    public ICollection<Resena> ResenasComoBarbero { get; set; }
}