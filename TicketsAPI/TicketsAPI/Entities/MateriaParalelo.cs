using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TicketsAPI.Utils;

namespace TicketsAPI.Entities
{
    public class MateriaParalelo : CrudEntities
    {
        [Key]
        public long Id { get; set; }
        [ForeignKey("Materia")]
        public long MateriaId { get; set; }
        [ForeignKey("AnioLectivo")]
        public long AnioLectivoId { get; set; }
        [ForeignKey("Paralelo")]
        public long ParaleloId { get; set;  }
        [Required]
        public int Cupos { get; set; }

        public virtual Materia Materia { get; set; }
        public virtual AnioLectivo AnioLectivo { get;set; }
        public virtual Paralelo Paralelo { get; set; }
    }
}
