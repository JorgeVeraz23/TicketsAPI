using Microsoft.EntityFrameworkCore;
using TicketsAPI.DTO;
using TicketsAPI.Interfaces;

namespace TicketsAPI.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ApplicationDbContext _context;

        public DashboardService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardResumenDto> GetResumenAsync(long anioLectivoId, CancellationToken ct)
        {
            // Traer el año lectivo (para mostrar Periodo)
            var anio = await _context.AnioLectivo
                .AsNoTracking()
                .Where(a => a.Id == anioLectivoId && a.IsActive == true)
                .Select(a => new { a.Id, a.Periodo, a.Vigente })
                .FirstOrDefaultAsync(ct);

            if (anio == null) throw new Exception("Año lectivo no existe o no está activo.");

            // ---- KPI 1: estudiantes activos (con matrícula en ese año lectivo)
            // Ajusta si quieres contar solo matriculas confirmadas, etc.
            var estudiantesActivosTask = await _context.Matriculas
                .AsNoTracking()
                .Where(m => m.IsActive == true
                         && m.GradoParalelo.AnioLectivoId == anioLectivoId)
                .Select(m => m.EstudianteId)
                .Distinct()
                .LongCountAsync(ct);

            // ---- KPI 2: matrículas pendientes
            // Ajusta tu criterio de "pendiente":
            // Ej: EstadoMatricula == "Pendiente" o "EnRevision"
            var matriculasPendientesTask = await _context.Matriculas
                .AsNoTracking()
                .Where(m => m.IsActive == true
                         && m.GradoParalelo.AnioLectivoId == anioLectivoId
                         && m.EstadoMatricula == "Pendiente")
                .LongCountAsync(ct);

            // ---- KPI 3: documentos por validar
            // Criterio típico: Aprobado == null (no revisado)
            var docsPorValidarTask = await _context.Documento
                .AsNoTracking()
                .Where(d => d.IsActive == true
                         && d.Aprobado == null)
                .LongCountAsync(ct);

            // ---- Tabla: matrículas por grado
            var porGradoTask = await _context.Matriculas
                .AsNoTracking()
                .Where(m => m.IsActive == true
                         && m.GradoParalelo.AnioLectivoId == anioLectivoId)
                .GroupBy(m => new
                {
                    m.GradoParalelo.GradoId,
                    GradoNombre = m.GradoParalelo.Grado.Nombre
                })
                .Select(g => new DashboardMatriculasPorGradoDto
                {
                    GradoId = g.Key.GradoId,
                    GradoNombre = g.Key.GradoNombre,
                    Total = g.Count()
                })
                .OrderBy(x => x.GradoId)
                .ToListAsync(ct);

            // ---- Lista: últimos documentos cargados
            var ultDocsTask = await _context.Documento
                .AsNoTracking()
                .Where(d => d.IsActive == true)
                .OrderByDescending(d => d.FechaCreacion)
                .Take(5)
                .Select(d => new DashboardDocumentoRecienteDto
                {
                    DocumentoId = d.Id,
                    DocumentoNombre = d.Nombre,
                    Estado = d.Estado,
                    FechaCreacion = DateTime.UtcNow,

                    EstudianteId = d.EstudianteId,
                    EstudianteNombreCompleto = d.Estudiante.Nombre + " " + d.Estudiante.Apellido,

                    TipoDocumentoId = d.TipoDocumentoId,
                    TipoDocumentoNombre = d.TipoDocumento.Nombre
                })
                .ToListAsync(ct);


            return new DashboardResumenDto
            {
                Kpis = new DashboardKpisDto
                {
                    Periodo = anio.Periodo,
                    EstudiantesActivos = estudiantesActivosTask,
                    MatriculasPendientes = matriculasPendientesTask,
                    DocumentosPorValidar = docsPorValidarTask,
                },
                MatriculasPorGrado = porGradoTask,
                UltimosDocumentos = ultDocsTask
            };
        }


    }
}
