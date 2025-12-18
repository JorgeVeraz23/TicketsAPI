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
        [ForeignKey("MateriaParalelo")]
        public long MateriaParaleloId { get; set; }
        [Required]
        [MaxLength(100)]
        public string EstadoMatricula { get; set; }
        [Required]
        public DateTime FechaMatricula { get; set; }
        public DateTime FechaConfirmacion { get; set; }
        [Required]
        [MaxLength(50)]
        public string PagoEstado { get; set; }
        public DateTime? FechaPago {  get; set; }



        public virtual Estudiante  Estudiante { get; set; }
        public virtual MateriaParalelo MateriaParalelo { get; set; }
        
    }
}
