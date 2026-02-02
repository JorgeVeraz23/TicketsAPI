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
        public async Task<EstudianteResponseDto> ObtenerEstudiantePorIdAsync(long id)
        {
            var e = await _context.Estudiantes
                .AsNoTracking()
                .Where(x => x.IsActive == true && x.Id == id)
                .FirstOrDefaultAsync();

            if (e == null) return null;

            return MapToResponse(e);
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



        // =========================
        // UPDATE
        // =========================
        public async Task<bool> ActualizarEstudianteAsync(long id, EstudianteCreateDto dto)
        {
            var entity = await _context.Estudiantes
                .Where(x => x.IsActive == true && x.Id == id)
                .FirstOrDefaultAsync();

            if (entity == null) return false;

            // Si cambia la cédula, validar duplicado
            var cedulaNueva = dto.Cedula.Trim();
            if (!string.Equals(entity.Cedula, cedulaNueva, StringComparison.OrdinalIgnoreCase))
            {
                var existe = await _context.Estudiantes.AnyAsync(e =>
                    e.IsActive == true &&
                    e.Cedula == cedulaNueva &&
                    e.Id != id);

                if (existe)
                    throw new Exception("Ya existe otro estudiante activo con esa cédula.");
            }

            entity.Nombre = dto.Nombre.Trim();
            entity.Apellido = dto.Apellido.Trim();
            entity.Cedula = cedulaNueva;
            entity.FechaNacimiento = dto.FechaNacimiento;


            entity.Telefono = dto.Telefono?.Trim();
            entity.Correo = dto.Correo?.Trim();
            entity.Direccion = dto.Direccion?.Trim();

            entity.Nivel = dto.Nivel;
            entity.UltimoGradoAprobado = dto.UltimoGradoAprobado;
            entity.Genero = dto.Genero;

            entity.FechaModificacion = DateTime.UtcNow;
            entity.UsuarioModificacion = "SYSTEM";

            await _context.SaveChangesAsync();
            return true;
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
