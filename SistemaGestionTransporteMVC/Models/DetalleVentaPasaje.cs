namespace SistemaGestionTransporteMVC.Models
{
	public class DetalleVentaPasaje
	{
		public int IdDetalle { get; set; }
		public int Cantidad { get; set; }
		public double Precio { get; set; }
		public double Total { get; set; }
		public int IdVenta { get; set; }
		public int IdViaje { get; set; }
	}
}
