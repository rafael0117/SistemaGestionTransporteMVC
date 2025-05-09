namespace SistemaGestionTransporteMVC.Models
{
	public class VentaPasaje
	{
        public int IdVenta { get; set; }
        public string Estado { get; set; }
        public DateTime FechaVenta { get; set; }
        public decimal Total { get; set; }
        public int IdUsuario { get; set; }
        public string Numero { get; set; }
        public List<DetalleVentaPasaje> Detalles { get; set; } = new List<DetalleVentaPasaje>();

    }
}
