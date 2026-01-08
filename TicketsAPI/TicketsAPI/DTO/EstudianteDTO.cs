using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketsAPI.DTO
{
    public class EstudianteDTO
    {
        [Required]
        public string Nombre { get; set; }

        [Required]
        [MaxLength(20)]
        public string Cedula { get; set; }

        [Required]
        [MaxLength(100)]
        public string Representante { get; set; }

        [Required]
        [MaxLength(20)]
        public string Telefono { get; set; }

        [EmailAddress]
        [MaxLength(100)]
        public string Correo { get; set; }

        [Required]
        public int Nivel { get; set; }
    }

    public class EstudianteResponseDto
    {
        public long Id { get; set; }
        public string Nombre { get; set; }
        public string Cedula { get; set; }
        public string Representante { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public int Nivel { get; set; }
    }

    public class EstudianteSearchDto
    {
        public long Id { get; set; }
        public string Nombre { get; set; }
        public string Cedula { get; set; }
        public string Representante { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public int Nivel { get; set; }
    }

}
