using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TicketsAPI.Utils;

namespace TicketsAPI.Entities
{
    public class Calificacion : CrudEntities
    {
        [Key]
        public long Id { get; set; }

        // Relaciones
        public long EstudianteId { get; set; }
        public Estudiante Estudiante { get; set; } = null!;

        [ForeignKey("Profesor")]
        public long ProfesorId { get; set; }
        public Profesor Profesor { get; set; } = null!;

        // Datos académicos mínimos
        [Required, MaxLength(120)]
        public string Materia { get; set; } = null!;  // luego lo cambias a MateriaId si creas tabla Materia

        [Required, MaxLength(50)]
        public string Periodo { get; set; } = "2025-2026"; // ejemplo

        [Required, Range(0, 10)]
        public decimal Nota { get; set; }

        [MaxLength(300)]
        public string? Observacion { get; set; }
    }
}
