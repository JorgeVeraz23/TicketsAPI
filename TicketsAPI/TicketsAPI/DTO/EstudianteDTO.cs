using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TicketsAPI.Enum;

namespace TicketsAPI.DTO
{
    public record EstudianteCreateDto(
     string Nombre,
     string Apellido,
     string Cedula,
     DateTime FechaNacimiento,
     long IdRepresentante,
     string? Telefono,
     string? Correo,
     string? Direccion,
     int Nivel,
     int UltimoGradoAprobado,
     EstadoEstudiante Estado,
     Genero Genero
 );

    public record EstudianteUpdateDto(
        string Nombre,
        string Apellido,
        string Cedula,
        DateTime FechaNacimiento,
        string Representante,
        string CedulaRepresentante,
        string? TelefonoRepresentante,
        string? CorreoRepresentante,
        string? Telefono,
        string? Correo,
        string? Direccion,
        int Nivel,
        int UltimoGradoAprobado,
        EstadoEstudiante Estado,
        Genero Genero
    );

    public class EstudianteResponseDto
    {
        public long Id { get; set; }

        public string NombreCompleto { get; set; }

        public string Cedula { get; set; }

        public int Edad { get; set; }

        public DateTime FechaNacimiento { get; set; }

 
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
