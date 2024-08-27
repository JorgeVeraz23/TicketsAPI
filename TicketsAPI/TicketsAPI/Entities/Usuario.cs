using System.ComponentModel.DataAnnotations;
using TicketsAPI.DTO;

namespace TicketsAPI.Entities
{
    public class Usuario
    {
        [Key]
        public long IdUsuario { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }    
        public EnumRolUsuarioDTO Rol { get; set; }

        public virtual ICollection<Solicitud>? Solicituds { get; set; }
    }
}
