using System.ComponentModel.DataAnnotations;
using TicketsAPI.Utils;

namespace TicketsAPI.Entities
{
    public class Representante : CrudEntities
    {
        [Key]
        public long Id { get; set; }

        public string Nombres { get; set; } = null!;
        public string Apellidos { get; set; } = null!;

        public string TipoDocumento { get; set; } = null!;
        public string NumeroDocumento { get; set; } = null!;

        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public string? Direccion { get; set; }


        // Navegación
        public ICollection<Estudiante> Estudiantes { get; set; } = new List<Estudiante>();
    }

}
