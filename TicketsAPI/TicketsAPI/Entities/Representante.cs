using System.ComponentModel.DataAnnotations;
using TicketsAPI.Utils;

namespace TicketsAPI.Entities
{
    public class Representante : CrudEntities
    {
        [Key]
        public long Id { get; set; }
        [Required, MaxLength(100)]
        public string Nombres { get; set; }
        [Required, MaxLength(100)]
        public string Apellidos { get; set; } 
        [Required, MaxLength(20)]
        public string TipoDocumento { get; set; } 
        [Required, MaxLength(20)]
        public string NumeroDocumento { get; set; }
        [MaxLength(20)]
        public string? Telefono { get; set; }
        [MaxLength(200)]
        public string? Email { get; set; }
        [MaxLength(300)]
        public string? Direccion { get; set; }


        // Navegación
        public virtual ICollection<Estudiante>? Estudiantes { get; set; } 
    }

}
