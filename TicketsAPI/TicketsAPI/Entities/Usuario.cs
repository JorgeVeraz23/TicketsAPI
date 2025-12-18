using System.ComponentModel.DataAnnotations;

namespace TicketsAPI.Entities
{
    public class Usuario
    {
        [Key]
        public string Id { get; set; }
        public string Correo { get; set; }
        public string Contrasenia { get; set; }


       
    }
}
