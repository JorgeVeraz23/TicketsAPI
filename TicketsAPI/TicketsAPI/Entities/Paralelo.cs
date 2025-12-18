using System.ComponentModel.DataAnnotations;
using TicketsAPI.Utils;

namespace TicketsAPI.Entities
{
    public class Paralelo : CrudEntities
    {
        [Key]
        public long Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }
        public virtual ICollection<MateriaParalelo> MateriaParalelos { get; set; }
    }
}
