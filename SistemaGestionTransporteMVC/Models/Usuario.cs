using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaGestionTransporteMVC.Models
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Username { get; set; }
        public string Clave { get; set; }
        public string Correo { get; set; }
        public string Direccion { get; set; }
        public int IdRol { get; set; }


    }
}
