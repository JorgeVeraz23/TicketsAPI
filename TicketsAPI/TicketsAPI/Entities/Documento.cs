using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TicketsAPI.Utils;

namespace TicketsAPI.Entities
{
    public class Documento : CrudEntities
    {
        [Key]
        public long Id { get; set; }

        [Required, MaxLength(200)]
        public string Nombre { get; set; } = default!;

        [Required, MaxLength(30)]
        public string Estado { get; set; } = "Pendiente";

        // ❌ QUITAR: ya no guardarás en BD
        // public byte[] Archivo { get; set; } = default!;

        // ✅ NUEVO: referencia al blob
        [MaxLength(40)]
        public string StorageProvider { get; set; } = "AzureBlob";

        [Required, MaxLength(600)]
        public string StoragePath { get; set; } = default!; // ej: documents/estudiante-10/doc-25.pdf

        [ForeignKey("Estudiante")]
        public long EstudianteId { get; set; }
        public virtual Estudiante Estudiante { get; set; } = default!;

        [ForeignKey("TipoDocumento")]
        public long TipoDocumentoId { get; set; }
        public virtual TipoDocumento TipoDocumento { get; set; } = default!;

        // ✅ Metadata (esto está bien)
        [MaxLength(100)]
        public string? MimeType { get; set; }

        [MaxLength(20)]
        public string? Extension { get; set; }

        public long? TamanoBytes { get; set; }

        public byte[]? HashArchivo { get; set; } // SHA256 ok

        [MaxLength(500)]
        public string? Observacion { get; set; }

        public bool? Aprobado { get; set; }
        public DateTime? FechaRevision { get; set; }

        [MaxLength(150)]
        public string? UsuarioRevision { get; set; }
    }

}
