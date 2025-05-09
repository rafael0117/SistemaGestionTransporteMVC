namespace SistemaGestionTransporteMVC.Models
{
	public class DetalleVentaPasaje
	{
        public int IdDetalle { get; set; }
        public int IdVenta { get; set; }
        public int IdViaje { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
        public decimal Total { get; set; }
    }
}
