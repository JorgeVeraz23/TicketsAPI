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





}
