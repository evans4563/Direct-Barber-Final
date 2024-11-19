namespace Direct_Barber.Models
{
    public class ResenaViewModel
    {
        public string Contenido { get; set; }
        public int Calificacion { get; set; }
        public DateTime FechaPublicacion { get; set; }
        public string NombreCliente { get; set; } // Nombre del cliente que dejó la reseña
        public string FotoCliente { get; set; } // Foto del cliente que dejó la reseña
    }
}
