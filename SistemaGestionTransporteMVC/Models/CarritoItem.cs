namespace SistemaGestionTransporteMVC.Models
{
    public class CarritoItem
    {
        public int IdViaje { get; set; }           // ID del viaje
        public int IdDestino { get; set; }         // ID del destino asociado al viaje
        public string nombreDestino { get; set; }  // Nombre del destino (desde la API de destino)
        public decimal Precio { get; set; }        // Precio del viaje
        public int Cantidad { get; set; }
        public decimal Subtotal => Precio * Cantidad;
    }
}
