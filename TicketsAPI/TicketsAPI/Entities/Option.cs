using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketsAPI.Entities
{
    public class Option : CrudEntities
    {
        [Key]
        public long IdOption { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [ForeignKey("FormField")]
        public long idFormField { get; set; }
        public FormField FormField { get; set; }
    }
}
