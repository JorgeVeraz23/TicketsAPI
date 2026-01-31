using System;
using TicketsAPI.Entities;
using TicketsAPI.Interfaces;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;

namespace TicketsAPI.Services
{
    public class DocumentoService
    {
        private readonly ApplicationDbContext _db;
        private readonly IBlobStorageService _blob;

        public DocumentoService(ApplicationDbContext db, IBlobStorageService blob)
        {
            _db = db;
            _blob = blob;
        }

        public async Task<long> UploadAsync(
            long estudianteId,
            long tipoDocumentoId,
            IFormFile file,
            string? observacion,
            string usuario,
            CancellationToken ct)
        {
            if (file == null || file.Length <= 0) throw new ArgumentException("Archivo vacío.");

            // Recomendado (mínimo):
            var allowed = new[] { "application/pdf", "image/png", "image/jpeg" };
            if (!allowed.Contains(file.ContentType))
                throw new InvalidOperationException("Tipo de archivo no permitido (solo PDF/PNG/JPG).");

            // 1) Crear registro en BD primero (para obtener Id si quieres usarlo en path)
            var ext = Path.GetExtension(file.FileName);
            var doc = new Documento
            {
                Nombre = Path.GetFileName(file.FileName),
                Estado = "Pendiente",
                EstudianteId = estudianteId,
                TipoDocumentoId = tipoDocumentoId,
                MimeType = file.ContentType,
                Extension = ext,
                TamanoBytes = file.Length,
                Observacion = observacion,
                Aprobado = null,
                FechaRevision = null,
                UsuarioRevision = null,
                UsuarioCreacion = usuario,
                FechaCreacion = DateTime.UtcNow,
                IsActive = true,
                StorageProvider = "AzureBlob",
                StoragePath = "TEMP" // se actualiza luego
            };

            _db.Documento.Add(doc);
            await _db.SaveChangesAsync(ct);

            // 2) Subir a Blob (path ordenado)
            var safeExt = string.IsNullOrWhiteSpace(ext) ? "" : ext.ToLowerInvariant();
            var blobPath = $"documents/estudiante-{estudianteId}/doc-{doc.Id}/{Guid.NewGuid():N}{safeExt}";

            await using var stream = file.OpenReadStream();
            await _blob.UploadAsync(stream, file.ContentType, blobPath, ct);

            // 3) Calcular hash opcional (SHA256) (si lo usas)
            // OJO: hay que reabrir stream para calcular hash
            await using var stream2 = file.OpenReadStream();
            doc.HashArchivo = await ComputeSha256Async(stream2, ct);

            doc.StoragePath = blobPath;
            await _db.SaveChangesAsync(ct);

            return doc.Id;
        }

        public async Task<List<Documento>> ListarPorEstudianteAsync(long estudianteId, CancellationToken ct)
        {
            return await _db.Documento
                .AsNoTracking()
                .Include(x => x.TipoDocumento)
                .Where(x => x.EstudianteId == estudianteId && x.IsActive == true)
                .OrderByDescending(x => x.FechaCreacion)
                .ToListAsync(ct);
        }

        public async Task AprobarAsync(long documentoId, bool aprobado, string? observacion, string usuarioRevision, CancellationToken ct)
        {
            var doc = await _db.Documento.FirstOrDefaultAsync(x => x.Id == documentoId && x.IsActive == true, ct)
                      ?? throw new KeyNotFoundException("Documento no existe.");

            doc.Aprobado = aprobado;
            doc.Estado = aprobado ? "Aprobado" : "Rechazado";
            doc.Observacion = observacion;
            doc.UsuarioRevision = usuarioRevision;
            doc.FechaRevision = DateTime.UtcNow;

            doc.UsuarioModificacion = usuarioRevision;
            doc.FechaModificacion = DateTime.UtcNow;

            await _db.SaveChangesAsync(ct);
        }

        public async Task<(Stream stream, string contentType, string downloadName)> DescargarAsync(long documentoId, CancellationToken ct)
        {
            var doc = await _db.Documento.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == documentoId && x.IsActive == true, ct)
                ?? throw new KeyNotFoundException("Documento no existe.");

            if (string.IsNullOrWhiteSpace(doc.StoragePath))
                throw new InvalidOperationException("Documento sin ruta en storage.");

            var (stream, contentType) = await _blob.DownloadAsync(doc.StoragePath, ct);
            var name = string.IsNullOrWhiteSpace(doc.Nombre) ? $"documento_{doc.Id}{doc.Extension}" : doc.Nombre;

            return (stream, contentType, name);
        }

        private static async Task<byte[]> ComputeSha256Async(Stream stream, CancellationToken ct)
        {
            using var sha = SHA256.Create();
            // SHA256 no tiene async nativo; copiamos a un buffer
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms, ct);
            ms.Position = 0;
            return sha.ComputeHash(ms);
        }
    }
}
