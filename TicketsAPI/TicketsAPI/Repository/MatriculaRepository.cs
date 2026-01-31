using Microsoft.EntityFrameworkCore;
using TicketsAPI.DTO;
using TicketsAPI.Entities;
using TicketsAPI.Enum;
using TicketsAPI.Interfaces;

namespace TicketsAPI.Repository
{
    public class MatriculaRepository : IMatricula
    {
        private readonly ApplicationDbContext _context;

        public MatriculaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<MatriculaResponseDto> CrearMatriculaAsync(CrearMatriculaDto dto)
        {
            await using var tx = await _context.Database.BeginTransactionAsync();

            // 1) valida estudiante
            var estudiante = await _context.Estudiantes
                .FirstOrDefaultAsync(e => e.IsActive == true && e.Id == dto.EstudianteId);

            if (estudiante == null)
                throw new Exception("Estudiante no existe o está inactivo.");

            // Opcional: evitar matricular retirados/graduados/suspendidos
            if (estudiante.Estado == EstadoEstudiante.Retirado ||
                estudiante.Estado == EstadoEstudiante.Graduado ||
                estudiante.Estado == EstadoEstudiante.Suspendido)
                throw new Exception("El estudiante no puede ser matriculado por su estado actual.");

            // 2) valida oferta (grado-paralelo)
            var oferta = await _context.GradoParalelos
                .Include(x => x.Grado)
                .Include(x => x.Paralelo)
                .Include(x => x.AnioLectivo)
                .FirstOrDefaultAsync(x => x.IsActive == true && x.Id == dto.GradoParaleloId);

            if (oferta == null)
                throw new Exception("La oferta (Grado/Paralelo/Año) no existe o está inactiva.");

            // 3) evita duplicado por año lectivo
            var yaMatriculado = await _context.Matriculas.AnyAsync(m =>
                m.IsActive == true &&
                m.EstudianteId == dto.EstudianteId &&
                m.GradoParalelo.AnioLectivoId == oferta.AnioLectivoId &&
                m.EstadoMatricula != "Anulada"
            );

            if (yaMatriculado)
                throw new Exception("El estudiante ya tiene una matrícula en este año lectivo.");

            // 4) cupos disponibles
            var confirmadas = await _context.Matriculas.CountAsync(m =>
                m.IsActive == true &&
                m.GradoParaleloId == dto.GradoParaleloId &&
                m.EstadoMatricula == "Confirmada"
            );

            var disponibles = oferta.Cupos - confirmadas;
            if (disponibles <= 0)
                throw new Exception("No hay cupos disponibles para este grado/paralelo.");

            // 5) crea matrícula en estado Pendiente
            var matricula = new Matricula
            {
                EstudianteId = dto.EstudianteId,
                GradoParaleloId = dto.GradoParaleloId,
                EstadoMatricula = "Pendiente",
                FechaMatricula = DateTime.UtcNow,
                FechaConfirmacion = null,  // ✅ pendiente => sin confirmación
                PagoEstado = "Pendiente",
                IsActive = true
            };

            _context.Matriculas.Add(matricula);

            // 6) cambia estado del estudiante (✅ lo que pediste)
            estudiante.Estado = EstadoEstudiante.Matriculado;
            estudiante.FechaModificacion = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            return new MatriculaResponseDto
            {
                Id = matricula.Id,
                EstudianteId = estudiante.Id,
                EstudianteNombre = estudiante.Nombre,
                GradoParaleloId = oferta.Id,
                GradoNombre = oferta.Grado.Nombre,
                ParaleloNombre = oferta.Paralelo.Nombre,
                Periodo = oferta.AnioLectivo.Periodo,
                EstadoMatricula = matricula.EstadoMatricula,
                FechaMatricula = matricula.FechaMatricula
            };
        }


        public async Task<List<MatriculaResponseDto>> GetAll(string? periodo)
        {
            IQueryable<Matricula> query = _context.Matriculas
                .AsNoTracking()
                .Where(m => m.IsActive == true);

            // ✅ FILTRO ANTES DE LOS INCLUDE
            if (!string.IsNullOrEmpty(periodo))
            {
                query = query.Where(m =>
                    m.GradoParalelo.AnioLectivo.Periodo.Contains(periodo)
                );
            }

            // ✅ INCLUDES DESPUÉS
            query = query
                .Include(m => m.Estudiante)
                .Include(m => m.GradoParalelo)
                    .ThenInclude(gp => gp.Grado)
                .Include(m => m.GradoParalelo)
                    .ThenInclude(gp => gp.Paralelo);

            return await query
                .OrderByDescending(m => m.FechaMatricula)
                .Select(m => new MatriculaResponseDto
                {
                    Id = m.Id,
                    EstudianteId = m.EstudianteId,
                    EstudianteNombre = m.Estudiante.Nombre + " " + m.Estudiante.Apellido,

                    GradoParaleloId = m.GradoParaleloId,
                    GradoNombre = m.GradoParalelo.Grado.Nombre,
                    ParaleloNombre = m.GradoParalelo.Paralelo.Nombre,
                    Periodo = m.GradoParalelo.AnioLectivo.Periodo,

                    EstadoMatricula = m.EstadoMatricula.ToString(),
                    FechaMatricula = m.FechaMatricula
                })
                .ToListAsync();
        }

    }

}
