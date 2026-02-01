using System.ComponentModel.DataAnnotations;

namespace TicketsAPI.DTO
{
    public class CrearMatriculaDto
    {
        public long EstudianteId { get; set; }
        public long GradoParaleloId { get; set; }
    }

    public class MatriculaResponseDto
    {
        public long Id { get; set; }
        public long EstudianteId { get; set; }
        public string EstudianteNombre { get; set; }

        public long GradoParaleloId { get; set; }
        public string GradoNombre { get; set; }
        public string ParaleloNombre { get; set; }
        public string Periodo { get; set; }

        public string EstadoMatricula { get; set; }
        public DateTime FechaMatricula { get; set; }
    }

    public class MatriculaListadoInfoDto
    {
        public string Institucion { get; set; } = "";
        public string Periodo { get; set; } = "";
        public int? FiltroGradoParaleloId { get; set; }
        public string? FiltroEstado { get; set; }
    }

    public class MatriculaListadoRowDto
    {
        public int Id { get; set; }
        public string Estudiante { get; set; } = "";
        public string Documento { get; set; } = "";
        public string Grado { get; set; } = "";
        public string Paralelo { get; set; } = "";
        public string Estado { get; set; } = "";
        public DateTime Fecha { get; set; }
    }






}
