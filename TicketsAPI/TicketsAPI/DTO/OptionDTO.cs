using System.ComponentModel.DataAnnotations;

namespace TicketsAPI.DTO
{
    public class OptionDTO
    {
        public long IdOption { get; set; }

        public string Name { get; set; } 
        public long idFormField { get; set; }
    }
}
