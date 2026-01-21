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
        public string Nombre { get; set; }

        [Required, MaxLength(100)]
        public string Apellido { get; set; }

        [Required, MaxLength(10)]
        public string Cedula { get; set; }

        [Required]
        public DateTime FechaNacimiento { get; set; }

        [NotMapped]
        public int Edad =>
            DateTime.Today.Year - FechaNacimiento.Year -
            (FechaNacimiento.Date > DateTime.Today.AddYears(
                -(DateTime.Today.Year - FechaNacimiento.Year)) ? 1 : 0);

        // REPRESENTANTE
        [Required, MaxLength(100)]
        public string Representante { get; set; }

        [MaxLength(10)]
        public string CedulaRepresentante { get; set; }

        [MaxLength(20)]
        public string TelefonoRepresentante { get; set; }

        [MaxLength(200)]
        public string CorreoRepresentante { get; set; }

        // CONTACTO
        [MaxLength(20)]
        public string Telefono { get; set; }

        [MaxLength(200)]
        public string Correo { get; set; }

        [MaxLength(300)]
        public string Direccion { get; set; }

        // ACADEMICO
        public int Nivel { get; set; }

        public int UltimoGradoAprobado { get; set; }

        public EstadoEstudiante Estado { get; set; }

        public Genero Genero { get; set; }

        // RELACIONES
        public virtual ICollection<Documento>? Documentos { get; set; }
        public virtual ICollection<Matricula>? Matriculas { get; set; }


    }
}
