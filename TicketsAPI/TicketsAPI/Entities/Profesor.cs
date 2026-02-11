using System.ComponentModel.DataAnnotations;
using TicketsAPI.Utils;

namespace TicketsAPI.Entities
{
    public class Profesor : CrudEntities
    {
        [Key]
        public long Id { get; set; }
        [Required, MaxLength(100)]
        public string Nombres { get; set; }
        [Required, MaxLength(100)]
        public string Apellidos { get; set; }
        [MaxLength(150)]
        public string TituloProfesional { get; set; }
        [Required, MaxLength(20)]
        public string TipoDocumento { get; set; }
        [Required, MaxLength(20)]
        public string NumeroDocumento { get; set; }
        [MaxLength(20)]
        public string? Telefono { get; set; }
        public bool IsTutor { get; set; } = false;
        [MaxLength(200)]
        public string? Email { get; set; }
        public virtual ICollection<Materia> Materias { get; set; } = new List<Materia>();
        public virtual ICollection<GradoParalelo>? GradoParalelos { get; set; } 
        public virtual ICollection<Calificacion> Calificaciones { get; set; } = new List<Calificacion>();
    }

}
