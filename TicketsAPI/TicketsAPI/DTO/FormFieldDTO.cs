using System.ComponentModel.DataAnnotations;

namespace TicketsAPI.DTO
{
    public class FormFieldDTO
    {
        public long IdFormField { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public int Index { get; set; }
        public bool IsOptional { get; set; }

        public long FieldTypeId { get; set; }
        public long FormGroupId { get; set; }

    }
}
