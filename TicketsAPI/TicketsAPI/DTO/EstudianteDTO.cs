using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TicketsAPI.Enum;

namespace TicketsAPI.DTO
{
    //   public record EstudianteCreateDto(
    //    string Nombre,
    //    string Apellido,
    //    string Cedula,
    //    DateTime FechaNacimiento,
    //    long IdRepresentante,
    //    string? Telefono,
    //    string? Correo,
    //    string? Direccion,
    //    int Nivel,
    //    int UltimoGradoAprobado,
    //    EstadoEstudiante Estado,
    //    Genero Genero
    //);

    public class EstudianteCreateDto
    {
        public string Nombre { get; set; } = default!;
        public string Apellido { get; set; } = default!;
        public string Cedula { get; set; } = default!;
        public DateTime FechaNacimiento { get; set; }
        public long IdRepresentante { get; set; }

        public string? Telefono { get; set; }
        public string? Correo { get; set; }
        public string? Direccion { get; set; }

        public int Nivel { get; set; }
        public int UltimoGradoAprobado { get; set; }
        public Genero Genero { get; set; }

        // ✅ Foto opcional (se guarda como Documento)
        public IFormFile? Foto { get; set; }
    }



        public class EstudianteUpdateDto
        {
            [Required]
            public long Id { get; set; }

            [Required]
            [MaxLength(100)]
            public string Nombre { get; set; } = string.Empty;

            [Required]
            [MaxLength(100)]
            public string Apellido { get; set; } = string.Empty;

            [Required]
            [MaxLength(20)]
            public string Cedula { get; set; } = string.Empty;

            [Required]
            public DateTime FechaNacimiento { get; set; }

            [Required]
            public long IdRepresentante { get; set; }

            [MaxLength(20)]
            public string? Telefono { get; set; }

            [EmailAddress]
            [MaxLength(150)]
            public string? Correo { get; set; }

            [MaxLength(250)]
            public string? Direccion { get; set; }

            [Required]
            [Range(1, 20)]
            public int Nivel { get; set; }

            [Required]
            [Range(0, 20)]
            public int UltimoGradoAprobado { get; set; }

            [Required]
            public Genero Genero { get; set; }   // enum

            public IFormFile? Foto { get; set; } // opcional
        }




    public class EstudianteResponseDto
    {
        public long Id { get; set; }
        public string NombreCompleto { get; set; } = default!;
        public string Cedula { get; set; } = default!;
        public int Edad { get; set; }
        public DateTime FechaNacimiento { get; set; }

        public long IdRepresentante { get; set; }   // <-- agregar

        public string? Telefono { get; set; }
        public string? Correo { get; set; }
        public string? Direccion { get; set; }

        public string? Nacionalidad { get; set; }
        public string? Observacion { get; set; }

        public int Nivel { get; set; }
        public int UltimoGradoAprobado { get; set; }
        public EstadoEstudiante Estado { get; set; }
        public Genero Genero { get; set; }

        public long? FotoDocumentoId { get; set; }
        public string? FotoUrl { get; set; }
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
