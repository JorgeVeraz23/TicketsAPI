using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TicketsAPI.Utils;

namespace TicketsAPI.Entities
{
    public class GradoParalelo : CrudEntities
    {
        [Key]
        public long Id { get; set; }
        [ForeignKey("Grado")]
        public long GradoId { get; set; }
        [ForeignKey("AnioLectivo")]
        public long AnioLectivoId { get; set; }
        [ForeignKey("Paralelo")]
        public long ParaleloId { get; set;  }
        [ForeignKey("Profesor")]
        public long ProfesorId { get; set; }
        [Required]
        public int Cupos { get; set; }

        public virtual Grado Grado { get; set; }
        public virtual AnioLectivo AnioLectivo { get;set; }
        public virtual Paralelo Paralelo { get; set; }
        public virtual Profesor Profesor { get; set; }

    }


}
