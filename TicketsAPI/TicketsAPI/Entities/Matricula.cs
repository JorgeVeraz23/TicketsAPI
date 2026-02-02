using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TicketsAPI.Utils;

namespace TicketsAPI.Entities
{
    public class Matricula : CrudEntities
    {
        [Key]
        public long Id { get; set; }
        [ForeignKey("Estudiante")]
        public long EstudianteId { get; set; }

        [Required, MaxLength(100)]
        public string EstadoMatricula { get; set; }
        [Required]
        public DateTime FechaMatricula { get; set; }
        [ForeignKey("GradoParalelo")]
        public long GradoParaleloId { get; set; }
        public DateTime? FechaConfirmacion { get; set; }

        public DateTime? FechaPago {  get; set; }
        public virtual GradoParalelo GradoParalelo { get; set; }


        public virtual Estudiante  Estudiante { get; set; }

        
    }
}
