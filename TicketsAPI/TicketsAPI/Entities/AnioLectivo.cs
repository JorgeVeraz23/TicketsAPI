using System.ComponentModel.DataAnnotations;
using TicketsAPI.Utils;

namespace TicketsAPI.Entities
{
    public class AnioLectivo : CrudEntities
    {
        [Key]
        public long Id { get; set; }
        [Required]
        [MaxLength(10)]
        public string Periodo { get; set; }
        public  virtual ICollection<MateriaParalelo>? MateriaParalelos { get; set; }
    }
}
