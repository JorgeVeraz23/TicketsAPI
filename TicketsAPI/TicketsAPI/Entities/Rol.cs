using System.ComponentModel.DataAnnotations;

namespace TicketsAPI.Entities
{
    public class Rol
    {
        [Key]
        public string Id { get; set; }
        public string Nombre { get; set; }
        
    }
}
