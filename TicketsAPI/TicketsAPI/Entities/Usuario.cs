using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TicketsAPI.DTO;

namespace TicketsAPI.Entities
{
    public class Usuario : CrudEntities
    {
        [Key]
        public long IdUsuario { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        [ForeignKey("Rols")]
        public long IdRol { get; set; }
        public virtual Rol Rols { get; set; }

        public virtual ICollection<Solicitud>? Solicituds { get; set; }
    }
}
