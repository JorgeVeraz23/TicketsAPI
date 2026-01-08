using System.ComponentModel.DataAnnotations;

namespace TicketsAPI.DTO
{
    public class ParaleloDTO
    {
        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }
    }

    public class ParaleloResponseDto
    {
        public long Id { get; set; }
        public string Nombre { get; set; }
    }


}
