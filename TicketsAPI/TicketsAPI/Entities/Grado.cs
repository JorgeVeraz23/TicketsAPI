using System.ComponentModel.DataAnnotations;
using TicketsAPI.Utils;

namespace TicketsAPI.Entities
{
    public class Grado : CrudEntities
    {
        [Key]
        public long Id { get; set; }
        [Required, MaxLength(100)]
        public string Nombre { get; set; }
        public int? Nivel { get; set; }
        public virtual ICollection<Materia>? Materias { get; set; }
        


    }
}
