using Microsoft.EntityFrameworkCore;
using TicketsAPI.DTO;
using TicketsAPI.Entities;
using TicketsAPI.Interfaces;

namespace TicketsAPI.Repository
{
    public class CalificacionRepository : ICalificacion
    {
        private readonly ApplicationDbContext _context;

        public CalificacionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CalificacionResponseDto> CrearAsync(CalificacionCreateDto dto)
        {
            // Validaciones mínimas: existencia de FK
            var existeEst = await _context.Estudiantes.AnyAsync(e => e.IsActive == true && e.Id == dto.EstudianteId);
            if (!existeEst) throw new Exception("Estudiante no existe o no está activo.");

            var existeProf = await _context.Profesors.AnyAsync(p => p.IsActive == true && p.Id == dto.ProfesorId);
            if (!existeProf) throw new Exception("Profesor no existe o no está activo.");

            // (Opcional recomendado) evitar duplicado: misma materia+periodo para mismo estudiante
            var materia = dto.Materia.Trim();
            var periodo = dto.Periodo.Trim();

            var duplicado = await _context.Calificaciones.AnyAsync(c =>
                c.IsActive == true &&
                c.EstudianteId == dto.EstudianteId &&
                c.Materia == materia &&
                c.Periodo == periodo);

            if (duplicado) throw new Exception("Ya existe calificación para ese estudiante en esa materia y periodo.");

            var entity = new Calificacion
            {
                EstudianteId = dto.EstudianteId,
                ProfesorId = dto.ProfesorId,
                Materia = materia,
                Periodo = periodo,
                Nota = dto.Nota,
                Observacion = dto.Observacion?.Trim(),

                IsActive = true,
                FechaCreacion = DateTime.UtcNow,
                UsuarioCreacion = "SYSTEM"
            };

            _context.Calificaciones.Add(entity);
            await _context.SaveChangesAsync();

            return await MapToResponseAsync(entity.Id);
        }

        public async Task<CalificacionResponseDto?> ObtenerPorIdAsync(long id)
        {
            var c = await _context.Calificaciones
                .AsNoTracking()
                .Include(x => x.Estudiante)
                .Include(x => x.Profesor)
                .FirstOrDefaultAsync(x => x.IsActive == true && x.Id == id);

            return c is null ? null : MapToResponse(c);
        }

        public async Task<List<CalificacionResponseDto>> ListarAsync(string? periodo, string? materia)
        {
            var q = _context.Calificaciones
                .AsNoTracking()
                .Include(x => x.Estudiante)
                .Include(x => x.Profesor)
                .Where(x => x.IsActive == true);

            if (!string.IsNullOrWhiteSpace(periodo))
                q = q.Where(x => x.Periodo == periodo.Trim());

            if (!string.IsNullOrWhiteSpace(materia))
                q = q.Where(x => x.Materia.Contains(materia.Trim()));

            var list = await q
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            return list.Select(MapToResponse).ToList();
        }

        public async Task<List<CalificacionResponseDto>> ListarPorEstudianteAsync(long estudianteId, string? periodo)
        {
            var q = _context.Calificaciones
                .AsNoTracking()
                .Include(x => x.Estudiante)
                .Include(x => x.Profesor)
                .Where(x => x.IsActive == true && x.EstudianteId == estudianteId);

            if (!string.IsNullOrWhiteSpace(periodo))
                q = q.Where(x => x.Periodo == periodo.Trim());

            var list = await q.OrderByDescending(x => x.Id).ToListAsync();
            return list.Select(MapToResponse).ToList();
        }

        public async Task<List<CalificacionResponseDto>> ListarPorProfesorAsync(long profesorId, string? periodo)
        {
            var q = _context.Calificaciones
                .AsNoTracking()
                .Include(x => x.Estudiante)
                .Include(x => x.Profesor)
                .Where(x => x.IsActive == true && x.ProfesorId == profesorId);

            if (!string.IsNullOrWhiteSpace(periodo))
                q = q.Where(x => x.Periodo == periodo.Trim());

            var list = await q.OrderByDescending(x => x.Id).ToListAsync();
            return list.Select(MapToResponse).ToList();
        }

        public async Task<bool> ActualizarAsync(long id, CalificacionCreateDto dto)
        {
            var entity = await _context.Calificaciones
                .FirstOrDefaultAsync(x => x.IsActive == true && x.Id == id);

            if (entity == null) return false;

            // validar FK si cambian
            if (entity.EstudianteId != dto.EstudianteId)
            {
                var existeEst = await _context.Estudiantes.AnyAsync(e => e.IsActive == true && e.Id == dto.EstudianteId);
                if (!existeEst) throw new Exception("Estudiante no existe o no está activo.");
                entity.EstudianteId = dto.EstudianteId;
            }

            if (entity.ProfesorId != dto.ProfesorId)
            {
                var existeProf = await _context.Profesors.AnyAsync(p => p.IsActive == true && p.Id == dto.ProfesorId);
                if (!existeProf) throw new Exception("Profesor no existe o no está activo.");
                entity.ProfesorId = dto.ProfesorId;
            }

            var materia = dto.Materia.Trim();
            var periodo = dto.Periodo.Trim();

            // evitar duplicado al editar
            var duplicado = await _context.Calificaciones.AnyAsync(c =>
                c.IsActive == true &&
                c.Id != id &&
                c.EstudianteId == entity.EstudianteId &&
                c.Materia == materia &&
                c.Periodo == periodo);

            if (duplicado) throw new Exception("Ya existe calificación para ese estudiante en esa materia y periodo.");

            entity.Materia = materia;
            entity.Periodo = periodo;
            entity.Nota = dto.Nota;
            entity.Observacion = dto.Observacion?.Trim();

            entity.FechaModificacion = DateTime.UtcNow;
            entity.UsuarioModificacion = "SYSTEM";

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EliminarAsync(long id)
        {
            var entity = await _context.Calificaciones
                .FirstOrDefaultAsync(x => x.IsActive == true && x.Id == id);

            if (entity == null) return false;

            entity.IsActive = false;
            entity.FechaEliminacion = DateTime.UtcNow;
            entity.UsuarioEliminacion = "SYSTEM";

            await _context.SaveChangesAsync();
            return true;
        }

        // Helpers
        private async Task<CalificacionResponseDto> MapToResponseAsync(long id)
        {
            var c = await _context.Calificaciones
                .AsNoTracking()
                .Include(x => x.Estudiante)
                .Include(x => x.Profesor)
                .FirstAsync(x => x.Id == id);

            return MapToResponse(c);
        }

        private static CalificacionResponseDto MapToResponse(Calificacion c) => new()
        {
            Id = c.Id,
            EstudianteId = c.EstudianteId,
            Estudiante = $"{c.Estudiante.Nombre} {c.Estudiante.Apellido}",
            ProfesorId = c.ProfesorId,
            Profesor = $"{c.Profesor.Nombres} {c.Profesor.Apellidos}",
            Materia = c.Materia,
            Periodo = c.Periodo,
            Nota = c.Nota,
            Observacion = c.Observacion
        };
    }
}
