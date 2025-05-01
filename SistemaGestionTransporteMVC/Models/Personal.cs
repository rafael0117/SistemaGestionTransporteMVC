using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SistemaGestionTransporteMVC.Models
{
    public class Personal
    {
        public int IdPersonal { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Dni { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }

        public string Direccion { get; set; }

        // Relación muchos a uno con la tabla Rol
        public virtual Rol Rol { get; set; }
    }
}
