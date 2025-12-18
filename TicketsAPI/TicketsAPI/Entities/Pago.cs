using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TicketsAPI.Utils;

namespace TicketsAPI.Entities
{
    public class Pago : CrudEntities
    {
        [Key]
        public long Id { get; set; }
        [Required]
        public decimal Monto { get; set; }
        [Required]
        public bool Estado { get; set; }
        public DateTime FechaPago { get; set; }
        [ForeignKey("Estudiante")]
        public long EstudianteId { get; set; }
        public virtual Estudiante Estudiante { get; set; }
    }
}
