using Microsoft.EntityFrameworkCore;
using TicketsAPI.DTO;
using TicketsAPI.Entities;
using TicketsAPI.Enum;
using TicketsAPI.Interfaces;

namespace TicketsAPI.Repository
{
    public class EstudianteRepository : IEstudiante
    {

        private readonly ApplicationDbContext _context;

        public EstudianteRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================
        // CREATE
        // =========================
        public async Task<EstudianteResponseDto> CrearEstudianteAsync(EstudianteCreateDto dto)
        {
            // Validar cédula duplicada (activos)
            var existeCedula = await _context.Estudiantes
                .AnyAsync(e => e.IsActive == true && e.Cedula == dto.Cedula);

            if (existeCedula)
                throw new Exception("Ya existe un estudiante activo con esa cédula.");

            var entity = new Estudiante
            {
                Nombre = dto.Nombre.Trim(),
                Apellido = dto.Apellido.Trim(),
                Cedula = dto.Cedula.Trim(),
                FechaNacimiento = dto.FechaNacimiento,

                Representante = dto.Representante.Trim(),
                CedulaRepresentante = dto.CedulaRepresentante?.Trim(),
                TelefonoRepresentante = dto.TelefonoRepresentante?.Trim(),
                CorreoRepresentante = dto.CorreoRepresentante?.Trim(),

                Telefono = dto.Telefono?.Trim(),
                Correo = dto.Correo?.Trim(),
                Direccion = dto.Direccion?.Trim(),

                Nivel = dto.Nivel,
                UltimoGradoAprobado = dto.UltimoGradoAprobado,
                Genero = dto.Genero,

                Estado = Enum.EstadoEstudiante.Activo,
                IsActive = true,

                FechaCreacion = DateTime.UtcNow,
                UsuarioCreacion = "SYSTEM"
            };

            _context.Estudiantes.Add(entity);
            await _context.SaveChangesAsync();

            return MapToResponse(entity);
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
            var list = await _context.Estudiantes
                .AsNoTracking()
                .Where(x => x.IsActive == true)
                .OrderByDescending(x => x.Id)
                .Select(e => MapToResponse(e))
                .ToListAsync();

            return list;
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

            entity.Representante = dto.Representante.Trim();
            entity.CedulaRepresentante = dto.CedulaRepresentante?.Trim();
            entity.TelefonoRepresentante = dto.TelefonoRepresentante?.Trim();
            entity.CorreoRepresentante = dto.CorreoRepresentante?.Trim();

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
                    e.Representante.Contains(query) ||
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
                    Representante = e.Representante,
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

                Representante = e.Representante,
                CedulaRepresentante = e.CedulaRepresentante,
                TelefonoRepresentante = e.TelefonoRepresentante,
                CorreoRepresentante = e.CorreoRepresentante,

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

    }
}
