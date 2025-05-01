using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SistemaGestionTransporteMVC.Models
{
    public class RevisionBus
    {
        public int IdRevision { get; set; }
        public virtual Bus Bus { get; set; }
        public DateTime FechaRevision { get; set; }
        public string TipoRevision { get; set; }
        public string Resultado { get; set; }
        public string Observaciones { get; set; }
    }
}
