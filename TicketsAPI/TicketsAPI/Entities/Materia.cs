using System.ComponentModel.DataAnnotations;

namespace TicketsAPI.Entities
{
    public class Materia : CrudEntities
    {
        [Key]
        public long IdMateria { get; set; }
        public string Nombre { get; set; }
        public virtual ICollection<Matricula>? Matricula { get; set; }
    }
}
