using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TicketsAPI.Utils;

namespace TicketsAPI.Entities
{
    public class Documento : CrudEntities
    {
        [Key]
        public long Id { get; set; }
        public string Nombre { get; set; }
        public string Estado { get; set; }
        public byte[] Archivo { get; set; }
        [ForeignKey("Estudiante")]
        public long EstudianteId { get; set; }
        public virtual Estudiante Estudiante { get; set; }
    }
}
