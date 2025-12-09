using System.ComponentModel.DataAnnotations;

namespace TicketsAPI.Entities
{
    public class Rol : CrudEntities
    {
        [Key]
        public long IdRol { get; set; }
        public string Nombre { get; set; }
        public virtual ICollection<Usuario>? Usuarios { get; set; }
    }
}
