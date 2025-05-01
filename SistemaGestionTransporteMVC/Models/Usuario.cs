using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaGestionTransporteMVC.Models
{
    public class Usuario
    {
        public long IdUser { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Username { get; set; }
        public string Clave { get; set; }
        public string Correo { get; set; }
        public string Direccion { get; set; }
        public virtual Rol Rol { get; set; }

        // Constructor con parámetros
        public Usuario(string nombres, string apellidos, string username, string clave, Rol rol)
        {
            Nombres = nombres;
            Apellidos = apellidos;
            Username = username;
            Clave = clave;
            Rol = rol;
        }
    }
}
