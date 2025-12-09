using System.ComponentModel.DataAnnotations;

namespace TicketsAPI.Entities
{
    public class Curso : CrudEntities
    {
        [Key]
        public long IdCurso { get; set;}
        public string Nombre { get; set;}
        public int Cupos { get; set; }    
        public virtual ICollection<Matricula>? Matriculas { get; set; }
    }
}
