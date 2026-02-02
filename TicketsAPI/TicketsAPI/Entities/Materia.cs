using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TicketsAPI.Utils;

namespace TicketsAPI.Entities
{
    public class Materia : CrudEntities
    {
        [Key]
        public long Id { get; set; }
        [Required, MaxLength(150)]
        public string Nombre { get; set; }
        [ForeignKey("Grado")]   
        public long GradoId { get; set; }
        public virtual Grado Grado { get; set; }    
    }
}
