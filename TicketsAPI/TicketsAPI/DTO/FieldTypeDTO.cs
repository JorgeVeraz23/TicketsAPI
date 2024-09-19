using System.ComponentModel.DataAnnotations;

namespace TicketsAPI.DTO
{
    public class FieldTypeDTO
    {
        public long IdFieldType { get; set; }

        public string Name { get; set; } = string.Empty;
    }
}
