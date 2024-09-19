using System.ComponentModel.DataAnnotations;
using TicketsAPI.Entities;

namespace TicketsAPI.DTO
{
    public class FormDTO
    {

        public long IdForm { get; set; }


        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

    }
}
