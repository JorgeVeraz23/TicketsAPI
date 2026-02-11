using Microsoft.EntityFrameworkCore;
using TicketsAPI.DTO;
using TicketsAPI.Entities;
using TicketsAPI.Interfaces;

namespace TicketsAPI.Repository
{
    public class ProfesorRepository : IProfesor
    {
        private readonly ApplicationDbContext _context;

        public ProfesorRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================
        // CREATE
        // =========================
        public async Task<ProfesorResponseDto> CrearProfesorAsync(ProfesorCreateDto dto)
        {
            var tipoDoc = dto.TipoDocumento.Trim();
            var numDoc = dto.NumeroDocumento.Trim();

            var existe = await _context.Profesors
                .AnyAsync(p => p.IsActive == true &&
                               p.TipoDocumento == tipoDoc &&
                               p.NumeroDocumento == numDoc);

            if (existe)
                throw new Exception("Ya existe un profesor activo con ese documento.");

            var entity = new Profesor
            {
                Nombres = dto.Nombres.Trim(),
                Apellidos = dto.Apellidos.Trim(),
                TituloProfesional = dto.TituloProfesional.Trim(),
                TipoDocumento = tipoDoc,
                NumeroDocumento = numDoc,
                Telefono = string.IsNullOrWhiteSpace(dto.Telefono) ? null : dto.Telefono.Trim(),
                Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim(),
                IsTutor =dto.IsTutor,
                IsActive = true,
                FechaCreacion = DateTime.UtcNow,
                UsuarioCreacion = "SYSTEM"
            };

            _context.Profesors.Add(entity);
            await _context.SaveChangesAsync();

            return MapToResponse(entity);
        }

        // =========================
        // GET BY ID
        // =========================
        public async Task<ProfesorResponseDto?> ObtenerProfesorPorIdAsync(long id)
        {
            var p = await _context.Profesors
                .AsNoTracking()
                .Where(x => x.IsActive == true && x.Id == id)
                .FirstOrDefaultAsync();

            return p is null ? null : MapToResponse(p);
        }

        // =========================
        // GET ALL
        // =========================
        public async Task<List<ProfesorResponseDto>> ObtenerTodosProfesoresAsync()
        {
            var response = await _context.Profesors
                .AsNoTracking()
                .Where(x => x.IsActive == true)
                .OrderByDescending(x => x.Id)
                .Select(p => new ProfesorResponseDto
                {
                    Id = p.Id,
                    NombreCompleto = p.Nombres + p.Apellidos,
                    Nombres = p.Nombres,
                    Apellidos = p.Apellidos,
                    IsTutor = p.IsTutor,
                    Telefono = p.Telefono,
                    TipoDocumento = p.TipoDocumento,
                    TituloProfesional = p.TituloProfesional,
                    Email = p.Email,
                    IsActive = p.IsActive,
                    NumeroDocumento = p.NumeroDocumento,
                })
                .ToListAsync();

            return response;
        }

        // =========================
        // UPDATE
        // =========================
        public async Task<bool> ActualizarProfesorAsync(long id, ProfesorCreateDto dto)
        {
            var entity = await _context.Profesors
                .Where(x => x.IsActive == true && x.Id == id)
                .FirstOrDefaultAsync();

            if (entity == null) return false;

            var tipoDoc = dto.TipoDocumento.Trim();
            var numDoc = dto.NumeroDocumento.Trim();

            // validar duplicado si cambian datos de documento
            if (!string.Equals(entity.TipoDocumento, tipoDoc, StringComparison.OrdinalIgnoreCase) ||
                !string.Equals(entity.NumeroDocumento, numDoc, StringComparison.OrdinalIgnoreCase))
            {
                var existe = await _context.Profesors.AnyAsync(p =>
                    p.IsActive == true &&
                    p.TipoDocumento == tipoDoc &&
                    p.NumeroDocumento == numDoc &&
                    p.Id != id);

                if (existe)
                    throw new Exception("Ya existe otro profesor activo con ese documento.");
            }

            entity.Nombres = dto.Nombres.Trim();
            entity.Apellidos = dto.Apellidos.Trim();
            entity.TituloProfesional = dto.TituloProfesional.Trim();
            entity.TipoDocumento = tipoDoc;
            entity.NumeroDocumento = numDoc;
            entity.Telefono = string.IsNullOrWhiteSpace(dto.Telefono) ? null : dto.Telefono.Trim();
            entity.Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim();
            entity.IsTutor = dto.IsTutor;
            entity.FechaModificacion = DateTime.UtcNow;
            entity.UsuarioModificacion = "SYSTEM";

            await _context.SaveChangesAsync();
            return true;
        }

        // =========================
        // DELETE (soft delete)
        // =========================
        public async Task<bool> EliminarProfesorAsync(long id)
        {
            var entity = await _context.Profesors
                .Where(x => x.IsActive == true && x.Id == id)
                .FirstOrDefaultAsync();

            if (entity == null) return false;

            entity.IsActive = false;
            entity.FechaEliminacion = DateTime.UtcNow;
            entity.UsuarioEliminacion = "SYSTEM";

            await _context.SaveChangesAsync();
            return true;
        }

        // =========================
        // SEARCH (selector/autocomplete)
        // =========================
        public async Task<List<ProfesorSearchDto>> SearchAsync(string query, int take = 10)
        {
            query = (query ?? "").Trim();
            if (string.IsNullOrWhiteSpace(query))
                return new List<ProfesorSearchDto>();

            var q = _context.Profesors
                .AsNoTracking()
                .Where(p => p.IsActive == true);

            var isNumeric = query.All(char.IsDigit);

            if (isNumeric)
            {
                q = q.Where(p => p.NumeroDocumento.Contains(query));
            }
            else
            {
                q = q.Where(p =>
                    p.Nombres.Contains(query) ||
                    p.Apellidos.Contains(query) ||
                    p.TituloProfesional.Contains(query) ||
                    (p.Email != null && p.Email.Contains(query)) ||
                    p.NumeroDocumento.Contains(query));
            }

            return await q
                .OrderBy(p => p.Apellidos)
                .ThenBy(p => p.Nombres)
                .Take(take)
                .Select(p => new ProfesorSearchDto
                {
                    Id = p.Id,
                    NombreCompleto = (p.Nombres + " " + p.Apellidos),
                    NumeroDocumento = p.NumeroDocumento,
                    Email = p.Email
                })
                .ToListAsync();
        }

        // =========================
        // MAPPING
        // =========================
        private static ProfesorResponseDto MapToResponse(Profesor p)
        {
            return new ProfesorResponseDto
            {
                Id = p.Id,
                Nombres = p.Nombres,
                Apellidos = p.Apellidos,
                NombreCompleto = $"{p.Nombres} {p.Apellidos}",
                TituloProfesional = p.TituloProfesional,
                TipoDocumento = p.TipoDocumento,
                NumeroDocumento = p.NumeroDocumento,
                Telefono = p.Telefono,
                Email = p.Email,
                IsActive = p.IsActive
            };
        }

        public async Task<List<KeyValueDTO>> SelectorProfesor()
        {
            var selector = await _context.Profesors
                .AsNoTracking()
                .Where(p => p.IsActive == true)
                .OrderBy(p => p.Apellidos)
                .ThenBy(p => p.Nombres)
                .Select(p => new KeyValueDTO
                {
                    Key = p.Id,
                    Value = $"{p.Nombres} {p.Apellidos}"
                })
                .ToListAsync();

            return selector;
        }

        public async Task<List<KeyValueDTO>> SelectorProfesorTutor()
        {
            var selector = await _context.Profesors
               .AsNoTracking()
               .Where(p => p.IsActive == true && p.IsTutor == true)
               .OrderBy(p => p.Apellidos)
               .ThenBy(p => p.Nombres)
               .Select(p => new KeyValueDTO
               {
                   Key = p.Id,
                   Value = $"{p.Nombres} {p.Apellidos}"
               })
               .ToListAsync();

            return selector;
        }
    }
}
