using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaGestionTransporteMVC.Models
{
    public class Destino
    {
        public int IdDestino{ get; set; }
        public string nombre { get; set; }
        public string imagen { get; set; }

        // Esta propiedad es para recibir el archivo de la imagen
        [NotMapped]  // No se mapeará en la base de datos
        public IFormFile ImagenUrl { get; set; }
    }


}
