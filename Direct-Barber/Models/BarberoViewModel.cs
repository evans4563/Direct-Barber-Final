namespace Direct_Barber.Models
{
    public class BarberoViewModel
    {
        public int BarberoId { get; set; }
        public string NombreBarbero { get; set; }
        public string ApellidoBarbero { get; set; }  // Nuevo campo
        public string Foto { get; set; }
        public string Descripcion { get; set; }
        public decimal PromedioCalificacion { get; set; }
        public int TotalResenas { get; set; }
        public List<Resena> Resenas { get; set; }
        public string Direccion { get; set; }  // Nuevo campo
        public string Telefono { get; set; }   // Nuevo campo
        public int ClienteActualId { get; set; }
    }

}
