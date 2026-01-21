using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TicketsAPI.Enum;

namespace TicketsAPI.DTO
{
    public class EstudianteCreateDto
    {
        [Required, MaxLength(100)]
        public string Nombre { get; set; }

        [Required, MaxLength(100)]
        public string Apellido { get; set; }

        [Required, MaxLength(10)]
        public string Cedula { get; set; }

        [Required]
        public DateTime FechaNacimiento { get; set; }

        // REPRESENTANTE
        [Required, MaxLength(100)]
        public string Representante { get; set; }

        [MaxLength(10)]
        public string CedulaRepresentante { get; set; }

        [MaxLength(20)]
        public string TelefonoRepresentante { get; set; }

        [EmailAddress, MaxLength(200)]
        public string CorreoRepresentante { get; set; }

        // CONTACTO ESTUDIANTE
        [MaxLength(20)]
        public string Telefono { get; set; }

        [EmailAddress, MaxLength(200)]
        public string Correo { get; set; }

        [MaxLength(300)]
        public string Direccion { get; set; }

        // ACADÉMICO
        [Required]
        public int Nivel { get; set; }

        [Required]
        public int UltimoGradoAprobado { get; set; }

        [Required]
        public Genero Genero { get; set; } // o enum si ya lo tienes

    }


    public class EstudianteResponseDto
    {
        public long Id { get; set; }

        public string NombreCompleto { get; set; }

        public string Cedula { get; set; }

        public int Edad { get; set; }

        public DateTime FechaNacimiento { get; set; }

        // REPRESENTANTE
        public string Representante { get; set; }
        public string CedulaRepresentante { get; set; }
        public string TelefonoRepresentante { get; set; }
        public string CorreoRepresentante { get; set; }

        // CONTACTO
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public string Direccion { get; set; }

        // ACADÉMICO
        public int Nivel { get; set; }
        public int UltimoGradoAprobado { get; set; }
        public EstadoEstudiante Estado { get; set; }
        public Genero Genero { get; set; }
    }


    public class EstudianteSearchDto
{
    public long Id { get; set; }
    public string NombreCompleto { get; set; }
    public string Cedula { get; set; }
    public string Representante { get; set; }
    public EstadoEstudiante Estado { get; set; }
}


}
