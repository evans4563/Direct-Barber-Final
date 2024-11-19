namespace Direct_Barber.Models
{
    public class SolicitudIndexViewModel
    {
        public List<Solicitud> SolicitudesSinBarbero { get; set; }
        public List<Solicitud> SolicitudesDelBarbero { get; set; }
        public List<Solicitud> SolicitudesCliente { get; set; }
        public List<Solicitud> SolicitudesConBarbero { get; set; }
        public List<Solicitud> SolicitudesCompletadas { get; set; } // Nueva propiedad

    }

}