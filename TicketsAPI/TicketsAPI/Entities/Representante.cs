using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;

namespace TicketsAPI.Entities
{
    public class Representante : CrudEntities
    {
        [Key]
        public long IdRepresentante { get; set; }
        public string Nombre { get; set; }
        public int Edad { get; set; }
        public string Identificacion { get; set; }
        public string Celular { get; set; }

        public DateTime FechaNacimiento { get; set; }
        public virtual ICollection<Estudiantes>? Estudiantes { get; set; }
    }
}
