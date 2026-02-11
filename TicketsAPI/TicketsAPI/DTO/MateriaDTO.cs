using System.ComponentModel.DataAnnotations;

namespace TicketsAPI.DTO
{
    public class MateriaDTO
    {
        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }

        [Required]
        public long GradoId { get; set; }  // El grado al que pertenece la materia
        [Required]
        public long ProfesorId { get; set; }  // El profesor que imparte la materia
    }

    public class MateriaResponseDto
    {
        public long Id { get; set; }
        public string Nombre { get; set; }
        public long GradoId { get; set; }
        public string GradoNombre { get; set; }  // Nombre del grado, como "Primero de Básica"
        public string ProfesorNombre { get; set; }  // Nombre del profesor, como "Juan Pérez"
    }

}
