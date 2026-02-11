namespace TicketsAPI.DTO
{
    public class ProfesorCreateDto
    {
        public string Nombres { get; set; } = null!;
        public string Apellidos { get; set; } = null!;
        public string TituloProfesional { get; set; } = null!;
        public string TipoDocumento { get; set; } = null!;
        public string NumeroDocumento { get; set; } = null!;
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public bool IsTutor { get; set; }  
    }

    public class ProfesorResponseDto
    {
        public long Id { get; set; }
        public string Nombres { get; set; } = null!;
        public string Apellidos { get; set; } = null!;
        public string NombreCompleto { get; set; } = null!;
        public string TituloProfesional { get; set; } = null!;
        public string TipoDocumento { get; set; } = null!;
        public string NumeroDocumento { get; set; } = null!;
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public bool? IsActive { get; set; }
        public bool IsTutor { get; set; }   
    }

    public class ProfesorSearchDto
    {
        public long Id { get; set; }
        public string NombreCompleto { get; set; } = null!;
        public string NumeroDocumento { get; set; } = null!;
        public string? Email { get; set; }
    }
}
