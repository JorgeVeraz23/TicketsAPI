using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketsAPI.Entities
{
    public class Estudiantes : CrudEntities
    {
        [Key]
        public long IdEstudiantes { get; set; }
        public string Nombre { get; set; }
        [ForeignKey("Representante")]
        public long RepresentanteId { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string Identificacion { get; set; }
        public string Celular { get; set; }
        public int Edad { get; set; }
        public virtual Representante? Representante { get; set; }
        public virtual ICollection<Matricula>? Matriculas { get; set; }
    }
}
