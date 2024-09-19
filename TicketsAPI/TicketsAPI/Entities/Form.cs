using System.ComponentModel.DataAnnotations;

namespace TicketsAPI.Entities
{
    public class Form : CrudEntities
    {
        [Key]
        public long IdForm { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        // Relación con FormGroup y FilledForm
        public ICollection<FormGroup> FormGroups { get; set; } = new List<FormGroup>();
        public ICollection<FilledForm> FilledForms { get; set; } = new List<FilledForm>();
    }
}
