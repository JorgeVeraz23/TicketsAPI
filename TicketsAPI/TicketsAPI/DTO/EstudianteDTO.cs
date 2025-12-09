using System.ComponentModel.DataAnnotations.Schema;

namespace TicketsAPI.DTO
{
    public class EstudianteDTO
    {
        public long IdEstudiantes { get; set; }
        public string Nombre { get; set; }

        public long RepresentanteId { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string Identificacion { get; set; }
        public string Celular { get; set; }
        public int Edad { get; set; }
    }
}
