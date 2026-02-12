using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TicketsAPI.Enum;
using TicketsAPI.Utils;

namespace TicketsAPI.Entities
{
    public class Estudiante : CrudEntities
    {
        [Key]
        public long Id { get; set; }

        [Required, MaxLength(100)]
        public string Nombre { get; set; } = null!;

        [Required, MaxLength(100)]
        public string Apellido { get; set; } = null!;

        [Required, MaxLength(10)]
        public string Cedula { get; set; } = null!;

        [Required]
        public DateTime FechaNacimiento { get; set; }

        // ✅ FK al representante
        [ForeignKey("Representante")]
        public long RepresentanteId { get; set; }
        public virtual Representante Representante { get; set; } = null!;

        // CONTACTO estudiante
        [MaxLength(20)]
        public string? Telefono { get; set; }

        [MaxLength(200)]
        public string? Correo { get; set; }

        [MaxLength(300)]
        public string? Direccion { get; set; }
        [MaxLength(100)]
        public string? Nacionalidad { get; set; }
        [MaxLength(500)]
        public string? Observacion { get; set; }

        // ACADEMICO
        public int Nivel { get; set; }
        public int UltimoGradoAprobado { get; set; }
        public EstadoEstudiante Estado { get; set; }
        public Genero Genero { get; set; }

        public virtual ICollection<Documento>? Documentos { get; set; }
        public virtual ICollection<Matricula>? Matriculas { get; set; }
    }

}
