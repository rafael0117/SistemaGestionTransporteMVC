namespace SistemaGestionTransporteMVC.Models
{
    public class Viaje
    {
        public int IdViaje { get; set; }
        public int IdBus { get; set; }
        public int IdDestino { get; set; }
        public DateTime FechaSalida { get; set; }
        public DateTime FechaLlegada { get; set; }
        public string Incidencias { get; set; }
        public double Precio { get; set; }

        // Propiedad de navegación
        public Destino Destino { get; set; }
    }

}
