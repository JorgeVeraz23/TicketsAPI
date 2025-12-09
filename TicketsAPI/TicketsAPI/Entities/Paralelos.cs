using System.ComponentModel.DataAnnotations;

namespace TicketsAPI.Entities
{
    public class Paralelos : CrudEntities
    {
        [Key]
        public long IdParalelo { get; set; }
        public string Nombre { get; set; }
        public virtual ICollection<Matricula>? Matricula { get; set; }
    }
}
