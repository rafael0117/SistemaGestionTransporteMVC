using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SistemaGestionTransporteMVC.Models
{
    public class Rol
    {
        public int IdRol { get; set; }
        public string Descripcion { get; set; }
    }
}
