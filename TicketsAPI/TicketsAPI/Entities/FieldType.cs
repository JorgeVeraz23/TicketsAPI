using System.ComponentModel.DataAnnotations;

namespace TicketsAPI.Entities
{
    public class FieldType : CrudEntities
    {
        [Key]
        public long IdFieldType { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        // Relación con FormField
        public ICollection<FormField>? FormFields { get; set; } 
    }
}
