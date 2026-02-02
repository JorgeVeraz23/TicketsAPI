using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TicketsAPI.Utils;

namespace TicketsAPI.Entities
{
    public class Calificacion : CrudEntities
    {
        [Key]
        public long Id { get; set; }

        [ForeignKey("Estudiante")]
        public long EstudianteId { get; set; }
        public virtual Estudiante Estudiante { get; set; } 

        [ForeignKey("Profesor")]
        public long ProfesorId { get; set; }
        public virtual Profesor Profesor { get; set; } 

        [ForeignKey("Materia")]
        public long MateriaId { get; set; }
        public virtual Materia Materia { get; set; }
        [ForeignKey("PeriodoEvaluativo")]
        public long PeriodoEvaluativoId { get; set; }
        public virtual PeriodoEvaluativo PeriodoEvaluativo { get; set; } 


        [Required, Range(0, 10)]
        public decimal Nota { get; set; }

        [MaxLength(300)]
        public string? Observacion { get; set; }
    }
}
