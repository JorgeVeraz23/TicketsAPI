namespace TicketsAPI.DTO
{
    public class CalificacionCreateDto
    {
        public long EstudianteId { get; set; }
        public long ProfesorId { get; set; }
        public string Materia { get; set; } = null!;
        public string Periodo { get; set; } = "2025-2026";
        public decimal Nota { get; set; }
        public string? Observacion { get; set; }
    }

    public class CalificacionResponseDto
    {
        public long Id { get; set; }
        public long EstudianteId { get; set; }
        public string Estudiante { get; set; } = null!;
        public long ProfesorId { get; set; }
        public string Profesor { get; set; } = null!;
        public string Materia { get; set; } = null!;
        public string Periodo { get; set; } = null!;
        public decimal Nota { get; set; }
        public string? Observacion { get; set; }
    }

}
