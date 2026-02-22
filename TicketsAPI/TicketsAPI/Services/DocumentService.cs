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
            CancellationToken ct = default)
        {
            if (file == null || file.Length == 0)
                throw new Exception("Archivo inválido.");

            var existeEstudiante = await _db.Estudiantes
                .AnyAsync(e => e.IsActive == true && e.Id == estudianteId, ct);

            if (!existeEstudiante)
                throw new Exception("El estudiante no existe o no está activo.");

            var tipo = await _db.TipoDocumentos
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.IsActive == true && t.Vigente == true && t.Id == tipoDocumentoId, ct);

            if (tipo == null)
                throw new Exception("Tipo de documento no válido.");

            var ext = Path.GetExtension(file.FileName)?.ToLowerInvariant() ?? "";
            var mime = file.ContentType ?? "application/octet-stream";
            var size = file.Length;

            byte[] fileBytes;
            byte[] hashBytes;

            await using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms, ct);
                fileBytes = ms.ToArray();

                using var sha = System.Security.Cryptography.SHA256.Create();
                hashBytes = sha.ComputeHash(fileBytes);
            }

            var newStoragePath = $"documents/estudiante-{estudianteId}/doc-{tipoDocumentoId}/{Guid.NewGuid():N}{ext}";
            await using (var uploadStream = new MemoryStream(fileBytes))
            {
                await _blob.UploadAsync(uploadStream, mime, newStoragePath, ct);
            }

            var existente = await _db.Documento
                .FirstOrDefaultAsync(d =>
                    d.EstudianteId == estudianteId &&
                    d.TipoDocumentoId == tipoDocumentoId, ct);

            if (existente != null)
            {
                if (!string.IsNullOrWhiteSpace(existente.StoragePath))
                {
                    try { await _blob.DeleteIfExistsAsync(existente.StoragePath, ct); } catch { }
                }

                existente.Nombre = file.FileName;
                existente.StorageProvider = "AzureBlob";
                existente.StoragePath = newStoragePath;
                existente.MimeType = mime;
                existente.Extension = ext;
                existente.TamanoBytes = size;
                existente.HashArchivo = hashBytes;
                existente.Observacion = observacion;
                existente.Estado = "Pendiente";
                existente.Aprobado = null;
                existente.FechaRevision = null;
                existente.UsuarioRevision = null;

                existente.IsActive = true;
                existente.UsuarioEliminacion = null;
                existente.FechaEliminacion = null;

                existente.UsuarioModificacion = usuario;
                existente.FechaModificacion = DateTime.UtcNow;

                await _db.SaveChangesAsync(ct);
                return existente.Id;
            }

            var doc = new Documento
            {
                Nombre = file.FileName,
                Estado = "Pendiente",
                StorageProvider = "AzureBlob",
                StoragePath = newStoragePath,
                EstudianteId = estudianteId,
                TipoDocumentoId = tipoDocumentoId,
                MimeType = mime,
                Extension = ext,
                TamanoBytes = size,
                HashArchivo = hashBytes,
                Observacion = observacion,
                Aprobado = null,
                FechaRevision = null,
                UsuarioRevision = null,
                UsuarioCreacion = usuario,
                FechaCreacion = DateTime.UtcNow,
                IsActive = true
            };

            _db.Documento.Add(doc);
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


        public async Task<(byte[] bytes, string contentType, string fileName)> DescargarBytesAsync(long documentoId)
        {
            var doc = await _db.Documento.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == documentoId && x.IsActive == true)
                ?? throw new KeyNotFoundException("Documento no existe.");

            if (string.IsNullOrWhiteSpace(doc.StoragePath))
                throw new InvalidOperationException("Documento sin ruta en storage.");

            // Si tu DownloadAsync requiere CancellationToken, pásale CancellationToken.None
            var (stream, contentType) = await _blob.DownloadAsync(doc.StoragePath, CancellationToken.None);

            await using (stream)
            await using (var ms = new MemoryStream())
            {
                await stream.CopyToAsync(ms);
                var name = string.IsNullOrWhiteSpace(doc.Nombre)
                    ? $"documento_{doc.Id}{doc.Extension}"
                    : doc.Nombre;

                return (ms.ToArray(), contentType, name);
            }
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
