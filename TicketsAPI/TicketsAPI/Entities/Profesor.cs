using System.ComponentModel.DataAnnotations;
using TicketsAPI.Utils;

namespace TicketsAPI.Entities
{
    public class Profesor : CrudEntities
    {
        [Key]
        public long Id { get; set; }

        public string Nombres { get; set; } = null!;
        public string Apellidos { get; set; } = null!;

        public string TituloProfesional { get; set; } = null!;
        public string TipoDocumento { get; set; } = null!;
        public string NumeroDocumento { get; set; } = null!;

        public string? Telefono { get; set; }
        public string? Email { get; set; }



        // Navegación
        public ICollection<Calificacion> Calificaciones { get; set; } = new List<Calificacion>();
    }

}
