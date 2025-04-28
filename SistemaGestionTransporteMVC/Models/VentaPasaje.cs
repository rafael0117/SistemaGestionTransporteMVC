namespace SistemaGestionTransporteMVC.Models
{
	public class VentaPasaje
	{
		public int IdVenta { get; set; }
		public string Estado { get; set; } = "pendiente";
		public DateTime FechaVenta { get; set; }
		public double Total { get; set; }
		public long IdUsuario { get; set; }
		public string Numero { get; set; }
	}
}
