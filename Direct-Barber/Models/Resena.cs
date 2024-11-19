namespace Direct_Barber.Models
{
    public class Resena
    {
        public int Id { get; set; } // Id de la reseña
        public string Contenido { get; set; } // El comentario
        public int Calificacion { get; set; } // Calificación numérica
        public DateTime FechaPublicacion { get; set; } // Fecha de la reseña

        // Llaves foráneas
        public int Id_Cliente { get; set; } // Clave foránea al cliente
        public Usuario Cliente { get; set; } // Relación con cliente

        public int Id_Barbero { get; set; } // Clave foránea al barbero
        public Usuario Barbero { get; set; } // Relación con barbero
    }

}