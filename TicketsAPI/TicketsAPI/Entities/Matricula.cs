using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketsAPI.Entities
{
    public class Matricula : CrudEntities
    {
        [Key]
        public long IdMatricula { get; set; }
        public string Codigo { get; set; }
        [ForeignKey("Estudiantes")]
        public long EstudianteId { get; set; }
        [ForeignKey("Curso")]
        public long CursoId { get; set; }
        [ForeignKey("Paralelos")]
        public long ParaleloId { get; set; }
        [ForeignKey("Profesor")]
        public long IdProfesor { get; set; }
        public virtual Estudiantes Estudiantes { get; set; }
        public virtual Curso Curso { get; set; }
        public virtual Paralelos Paralelos { get; set; }
        public virtual Profesor Profesor { get; set; }
    }
}
