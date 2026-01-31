namespace TicketsAPI.DTO
{
    public class DocumentoUploadRequest
    {
        public long EstudianteId { get; set; }
        public long TipoDocumentoId { get; set; }
        public string? Observacion { get; set; }
        public IFormFile File { get; set; } = default!;
    }

}
