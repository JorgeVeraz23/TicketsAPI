using Microsoft.EntityFrameworkCore;
using TicketsAPI.DTO;
using TicketsAPI.Entities;
using TicketsAPI.Enum;
using TicketsAPI.Interfaces;
using TicketsAPI.Services;

namespace TicketsAPI.Repository
{
    public class EstudianteRepository : IEstudiante
    {

        private readonly ApplicationDbContext _context;
        private readonly DocumentoService _documentService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBlobStorageService _blob;

        public EstudianteRepository(ApplicationDbContext context, DocumentoService documentService, IHttpContextAccessor httpContextAccessor, IBlobStorageService blob)
        {
            _context = context;
            _documentService = documentService;
            _httpContextAccessor = httpContextAccessor;
            _blob = blob;
        }

        // =========================
        // CREATE
        // =========================
        //public async Task<EstudianteResponseDto> CrearEstudianteAsync(EstudianteCreateDto dto)
        //{
        //    // validar que el representante exista
        //    var existeRepresentante = await _context.Representantes
        //        .AnyAsync(r => r.IsActive == true && r.Id == dto.IdRepresentante);

        //    if (!existeRepresentante)
        //        throw new Exception("El representante no existe o no está activo.");

        //    var entity = new Estudiante
        //    {
        //        Nombre = dto.Nombre.Trim(),
        //        Apellido = dto.Apellido.Trim(),
        //        Cedula = dto.Cedula.Trim(),
        //        FechaNacimiento = dto.FechaNacimiento,

        //        RepresentanteId = dto.IdRepresentante, // ✅ OK

        //        Telefono = dto.Telefono?.Trim(),
        //        Correo = dto.Correo?.Trim(),
        //        Direccion = dto.Direccion?.Trim(),

        //        Nivel = dto.Nivel,
        //        UltimoGradoAprobado = dto.UltimoGradoAprobado,
        //        Genero = dto.Genero,

        //        Estado = EstadoEstudiante.SinMatricular,
        //        IsActive = true,

        //        FechaCreacion = DateTime.UtcNow,
        //        UsuarioCreacion = "SYSTEM"
        //    };


        //    _context.Estudiantes.Add(entity);
        //    await _context.SaveChangesAsync();

        //    return MapToResponse(entity);
        //}

        public async Task<EstudianteResponseDto> CrearEstudianteAsync(EstudianteCreateDto dto, CancellationToken ct)
        {
            var existeRepresentante = await _context.Representantes
                .AnyAsync(r => r.IsActive == true && r.Id == dto.IdRepresentante, ct);

            if (!existeRepresentante)
                throw new Exception("El representante no existe o no está activo.");

            var entity = new Estudiante
            {
                Nombre = dto.Nombre.Trim(),
                Apellido = dto.Apellido.Trim(),
                Cedula = dto.Cedula.Trim(),
                FechaNacimiento = dto.FechaNacimiento,
                RepresentanteId = dto.IdRepresentante,

                Telefono = dto.Telefono?.Trim(),
                Correo = dto.Correo?.Trim(),
                Direccion = dto.Direccion?.Trim(),

                Nivel = dto.Nivel,
                UltimoGradoAprobado = dto.UltimoGradoAprobado,
                Genero = dto.Genero,

                Estado = EstadoEstudiante.SinMatricular,
                IsActive = true,
                FechaCreacion = DateTime.UtcNow,
                UsuarioCreacion = "SYSTEM"
            };

            _context.Estudiantes.Add(entity);
            await _context.SaveChangesAsync(ct);

            // ✅ Si viene foto, se sube como Documento (TipoDocumento = FOTO_EST)
            if (dto.Foto != null && dto.Foto.Length > 0)
            {
                var tipoFotoId = await _context.TipoDocumentos
                    .Where(t => t.IsActive == true && t.Vigente == true && t.Codigo == "FOTO_EST")
                    .Select(t => t.Id)
                    .FirstOrDefaultAsync(ct);

                if (tipoFotoId == 0)
                    throw new Exception("No existe TipoDocumento FOTO_EST. Inserta el seed.");

                // (opcional) solo 1 foto activa: desactivar anteriores
                await DesactivarDocumentosDelTipoAsync(entity.Id, tipoFotoId, "SYSTEM", ct);

                await _documentService.UploadAsync(
                    estudianteId: entity.Id,
                    tipoDocumentoId: tipoFotoId,
                    file: dto.Foto,
                    observacion: "Foto del estudiante",
                    usuario: "SYSTEM",
                    ct: ct
                );
            }

            return MapToResponse(entity);
        }

        private async Task DesactivarDocumentosDelTipoAsync(long estudianteId, long tipoDocumentoId, string usuario, CancellationToken ct)
        {
            var anteriores = await _context.Documento
                .Where(d => d.IsActive == true && d.EstudianteId == estudianteId && d.TipoDocumentoId == tipoDocumentoId)
                .ToListAsync(ct);

            if (anteriores.Count == 0) return;

            foreach (var doc in anteriores)
            {
                doc.IsActive = false;
                doc.UsuarioEliminacion = usuario;
                doc.FechaEliminacion = DateTime.UtcNow;
                doc.UsuarioModificacion = usuario;
                doc.FechaModificacion = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync(ct);
        }


        // =========================
        // GET BY ID
        // =========================
        public async Task<EstudianteResponseDto?> ObtenerEstudiantePorIdAsync(long id)
        {
            const long FOTO_TIPO_DOCUMENTO_ID = 4; // FOTO_EST

            var row = await _context.Estudiantes
                .AsNoTracking()
                .Where(e => e.IsActive == true && e.Id == id)
                .Select(e => new
                {
                    e.Id,
                    e.Nombre,
                    e.Apellido,
                    e.Cedula,
                    e.FechaNacimiento,
                    e.RepresentanteId,
                    e.Telefono,
                    e.Correo,
                    e.Direccion,
                    e.Nacionalidad,
                    e.Observacion,
                    e.Nivel,
                    e.UltimoGradoAprobado,
                    e.Estado,
                    e.Genero,
                    Foto = _context.Documento
                        .Where(d => d.IsActive == true
                                    && d.EstudianteId == e.Id
                                    && d.TipoDocumentoId == FOTO_TIPO_DOCUMENTO_ID)
                        .OrderByDescending(d => d.FechaCreacion)
                        .Select(d => new { d.Id, d.StoragePath })
                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync();

            if (row == null) return null;

            return new EstudianteResponseDto
            {
                Id = row.Id,
                NombreCompleto = $"{row.Nombre} {row.Apellido}".Trim(),
                Cedula = row.Cedula,
                FechaNacimiento = row.FechaNacimiento,
                Edad = CalcularEdad(row.FechaNacimiento),

                IdRepresentante = row.RepresentanteId,

                Telefono = row.Telefono,
                Correo = row.Correo,
                Direccion = row.Direccion,
                Nacionalidad = row.Nacionalidad,
                Observacion = row.Observacion,

                Nivel = row.Nivel,
                UltimoGradoAprobado = row.UltimoGradoAprobado,
                Estado = row.Estado,
                Genero = row.Genero,

                FotoDocumentoId = row.Foto?.Id,
                FotoUrl = row.Foto?.StoragePath == null ? null : _blob.GetReadSasUrl(row.Foto.StoragePath, 10)
            };
        }


        // =========================
        // GET ALL
        // =========================
        public async Task<List<EstudianteResponseDto>> ObtenerTodosEstudiantesAsync()
        {
            const long FOTO_TIPO_DOCUMENTO_ID = 4; // tu "Foto del estudiante"

            var rows = await _context.Estudiantes
                .AsNoTracking()
                .Where(e => e.IsActive == true)
                .OrderByDescending(e => e.Id)
                .Select(e => new
                {
                    e.Id,
                    e.Nombre,
                    e.Apellido,
                    e.Cedula,
                    e.FechaNacimiento,
                    e.Telefono,
                    e.Correo,
                    e.Direccion,
                    e.Nacionalidad,
                    e.Observacion,
                    e.Nivel,
                    e.UltimoGradoAprobado,
                    e.Estado,
                    e.Genero,

                    Foto = _context.Documento
                        .Where(d => d.IsActive == true
                                    && d.EstudianteId == e.Id
                                    && d.TipoDocumentoId == FOTO_TIPO_DOCUMENTO_ID)
                        .OrderByDescending(d => d.FechaCreacion)
                        .Select(d => new { d.Id, d.StoragePath })
                        .FirstOrDefault()
                })
                .ToListAsync();

            return rows.Select(x => new EstudianteResponseDto
            {
                Id = x.Id,
                NombreCompleto = $"{x.Nombre} {x.Apellido}".Trim(),
                Cedula = x.Cedula,
                FechaNacimiento = x.FechaNacimiento,
                Edad = CalcularEdad(x.FechaNacimiento),

                Telefono = x.Telefono,
                Correo = x.Correo,
                Direccion = x.Direccion,
                Nacionalidad = x.Nacionalidad,
                Observacion = x.Observacion,

                Nivel = x.Nivel,
                UltimoGradoAprobado = x.UltimoGradoAprobado,
                Estado = x.Estado,
                Genero = x.Genero,

                FotoDocumentoId = x.Foto?.Id,
                FotoUrl = x.Foto?.StoragePath == null ? null : _blob.GetReadSasUrl(x.Foto.StoragePath, expiresMinutes: 10)
            })
            .ToList();
        }


        private static int CalcularEdad(DateTime fechaNacimiento)
        {
            var hoy = DateTime.UtcNow.Date;
            var edad = hoy.Year - fechaNacimiento.Date.Year;
            if (fechaNacimiento.Date > hoy.AddYears(-edad)) edad--;
            return edad < 0 ? 0 : edad;
        }



        public async Task<EstudianteResponseDto?> ActualizarEstudianteAsync(long id, EstudianteUpdateDto dto)
        {
            var entity = await _context.Estudiantes
                .FirstOrDefaultAsync(x => x.IsActive == true && x.Id == id);

            if (entity == null) return null;

            // Validar cédula duplicada (si cambió)
            var cedulaNueva = (dto.Cedula ?? string.Empty).Trim();
            if (!string.Equals(entity.Cedula, cedulaNueva, StringComparison.OrdinalIgnoreCase))
            {
                var existe = await _context.Estudiantes.AnyAsync(e =>
                    e.IsActive == true &&
                    e.Cedula == cedulaNueva &&
                    e.Id != id);

                if (existe)
                    throw new Exception("Ya existe otro estudiante activo con esa cédula.");
            }

            // Validar representante
            var existeRepresentante = await _context.Representantes
                .AnyAsync(r => r.IsActive == true && r.Id == dto.IdRepresentante);

            if (!existeRepresentante)
                throw new Exception("El representante no existe o no está activo.");

            // Actualizar campos
            entity.Nombre = (dto.Nombre ?? string.Empty).Trim();
            entity.Apellido = (dto.Apellido ?? string.Empty).Trim();
            entity.Cedula = cedulaNueva;
            entity.FechaNacimiento = dto.FechaNacimiento;
            entity.RepresentanteId = dto.IdRepresentante;

            entity.Telefono = dto.Telefono?.Trim();
            entity.Correo = dto.Correo?.Trim();
            entity.Direccion = dto.Direccion?.Trim();

            entity.Nivel = dto.Nivel;
            entity.UltimoGradoAprobado = dto.UltimoGradoAprobado;
            entity.Genero = dto.Genero;

            entity.FechaModificacion = DateTime.UtcNow;
            entity.UsuarioModificacion = "SYSTEM";

            await _context.SaveChangesAsync();

            // Si viene foto, subir como FOTO_EST (opcional)
            if (dto.Foto != null && dto.Foto.Length > 0)
            {
                var tipoFotoId = await _context.TipoDocumentos
                    .Where(t => t.IsActive == true && t.Vigente == true && t.Codigo == "FOTO_EST")
                    .Select(t => t.Id)
                    .FirstOrDefaultAsync();

                if (tipoFotoId == 0)
                    throw new Exception("No existe TipoDocumento FOTO_EST. Inserta el seed.");

                await DesactivarDocumentosDelTipoAsync(entity.Id, tipoFotoId, "SYSTEM", CancellationToken.None);

                await _documentService.UploadAsync(
                    estudianteId: entity.Id,
                    tipoDocumentoId: tipoFotoId,
                    file: dto.Foto,
                    observacion: "Foto del estudiante",
                    usuario: "SYSTEM",
                    ct: CancellationToken.None
                );
            }

            var result = await _context.Estudiantes
    .AsNoTracking()
    .Where(e => e.IsActive && e.Id == id)
    .Select(e => new
    {
        e.Id,
        e.Nombre,
        e.Apellido,
        e.Cedula,
        e.FechaNacimiento,
        e.Telefono,
        e.Correo,
        e.Direccion,
        e.Nacionalidad,
        e.Observacion,
        e.Nivel,
        e.UltimoGradoAprobado,
        e.Estado,
        e.Genero,
        Foto = _context.Documento
            .Where(d => d.IsActive && d.EstudianteId == e.Id && d.TipoDocumentoId == 4)
            .OrderByDescending(d => d.FechaCreacion)
            .Select(d => new { d.Id, d.StoragePath })
            .FirstOrDefault()
    })
    .FirstOrDefaultAsync();

            if (result == null) return null;

            return new EstudianteResponseDto
            {
                Id = result.Id,
                NombreCompleto = $"{result.Nombre} {result.Apellido}".Trim(),
                Cedula = result.Cedula,
                FechaNacimiento = result.FechaNacimiento,
                Edad = CalcularEdad(result.FechaNacimiento),
                Telefono = result.Telefono,
                Correo = result.Correo,
                Direccion = result.Direccion,
                Nacionalidad = result.Nacionalidad,
                Observacion = result.Observacion,
                Nivel = result.Nivel,
                UltimoGradoAprobado = result.UltimoGradoAprobado,
                Estado = result.Estado,
                Genero = result.Genero,
                FotoDocumentoId = result.Foto?.Id,
                FotoUrl = result.Foto?.StoragePath == null ? null : _blob.GetReadSasUrl(result.Foto.StoragePath, 10)
            };

        }


        // =========================
        // DELETE (soft delete)
        // =========================
        public async Task<bool> EliminarEstudianteAsync(long id)
        {
            var entity = await _context.Estudiantes
                .Where(x => x.IsActive == true && x.Id == id)
                .FirstOrDefaultAsync();

            if (entity == null) return false;

            entity.IsActive = false;
            entity.Estado = Enum.EstadoEstudiante.Retirado; // opcional
            entity.FechaEliminacion = DateTime.UtcNow;
            entity.UsuarioEliminacion = "SYSTEM";

            await _context.SaveChangesAsync();
            return true;
        }

        // =========================
        // SEARCH (para matrícula: cédula/nombre/apellido/representante)
        // =========================
        public async Task<List<EstudianteSearchDto>> SearchAsync(string query, int take = 10)
        {
            query = (query ?? "").Trim();

            if (string.IsNullOrWhiteSpace(query))
                return new List<EstudianteSearchDto>();

            // Búsqueda flexible
            var q = _context.Estudiantes
                .AsNoTracking()
                .Where(e => e.IsActive == true);

            // Si es número, prioriza por cédula
            var isNumeric = query.All(char.IsDigit);

            if (isNumeric)
            {
                q = q.Where(e => e.Cedula.Contains(query));
            }
            else
            {
                q = q.Where(e =>
                    e.Nombre.Contains(query) ||
                    e.Apellido.Contains(query) ||
                    e.Representante.Nombres.Contains(query) ||
                    e.Cedula.Contains(query));
            }

            var result = await q
                .OrderBy(e => e.Nombre)
                .ThenBy(e => e.Apellido)
                .Take(take)
                .Select(e => new EstudianteSearchDto
                {
                    Id = e.Id,
                    Cedula = e.Cedula,
                    Representante = e.Representante.Nombres,
                    Estado = e.Estado,
                    NombreCompleto = (e.Nombre + " " + e.Apellido)
                })
                .ToListAsync();

            return result;
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

            var existeEstudiante = await _context.Estudiantes
                .AnyAsync(e => e.IsActive == true && e.Id == estudianteId, ct);

            if (!existeEstudiante)
                throw new Exception("El estudiante no existe o no está activo.");

            var tipo = await _context.TipoDocumentos
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

            var existente = await _context.Documento
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

                await _context.SaveChangesAsync(ct);
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

            _context.Documento.Add(doc);
            await _context.SaveChangesAsync(ct);

            return doc.Id;
        }



        // =========================
        // MAPPING
        // =========================
        private static EstudianteResponseDto MapToResponse(Estudiante e)
        {
            return new EstudianteResponseDto
            {
                Id = e.Id,
                Cedula = e.Cedula,
                FechaNacimiento = e.FechaNacimiento,
                Edad = DateTime.Today.Year - e.FechaNacimiento.Year -
                       (e.FechaNacimiento.Date > DateTime.Today.AddYears(-(DateTime.Today.Year - e.FechaNacimiento.Year)) ? 1 : 0),

                NombreCompleto = $"{e.Nombre} {e.Apellido}",

               
                Telefono = e.Telefono,
                Correo = e.Correo,
                Direccion = e.Direccion,

                Nivel = e.Nivel,
                UltimoGradoAprobado = e.UltimoGradoAprobado,
                Estado = e.Estado,
                Genero = e.Genero
            };
        }
        public async Task<List<KeyValueDTO>> SelectorEstudiante()
        {
            return await _context.Estudiantes
                .AsNoTracking()
                .Where(e =>
                    e.IsActive == true &&
                    e.Estado == EstadoEstudiante.SinMatricular
                )
                .OrderBy(e => e.Apellido)
                .ThenBy(e => e.Nombre)
                .Select(e => new KeyValueDTO
                {
                    Key = e.Id,
                    Value = $"{e.Apellido} {e.Nombre} - {e.Cedula}"
                })
                .ToListAsync();
        }

        public async Task<List<KeyValueDTO>> SelectorEstudianteDocs()
        {
            return await _context.Estudiantes
               .AsNoTracking()
               .Where(e =>
                   e.IsActive == true &&
                   e.Estado != EstadoEstudiante.Matriculado
               )
               .OrderBy(e => e.Apellido)
               .ThenBy(e => e.Nombre)
               .Select(e => new KeyValueDTO
               {
                   Key = e.Id,
                   Value = $"{e.Apellido} {e.Nombre} - {e.Cedula}"
               })
               .ToListAsync();
        }
    }
}
