using Microsoft.EntityFrameworkCore;
using System;
using TicketsAPI.DTO;
using TicketsAPI.Interfaces;

namespace TicketsAPI.Services
{
    public class ReporteService : IReportesServices
    {
        private readonly ApplicationDbContext _db; // cambia al nombre real

        public ReporteService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<PagedResultDto<ReporteMatriculaItemDto>> ObtenerMatriculasAsync(ReporteMatriculasFiltroDto filtro)
        {
            var page = Math.Max(1, filtro.Page);
            var pageSize = Math.Clamp(filtro.PageSize, 1, 200);

            var q = _db.Matriculas
                .AsNoTracking()
                .Include(m => m.Estudiante)
                    .ThenInclude(e => e.Representante) // si NO existe navegación, elimina esta línea
                .Include(m => m.GradoParalelo)
                    .ThenInclude(gp => gp.AnioLectivo) // ✅ aquí está AnioLectivo
                .AsQueryable();

            // ✅ filtro por AñoLectivo: viene desde GradoParalelo
            if (filtro.AnioLectivoId.HasValue)
                q = q.Where(m => m.GradoParalelo.AnioLectivoId == filtro.AnioLectivoId.Value);

            if (filtro.GradoParaleloId.HasValue)
                q = q.Where(m => m.GradoParaleloId == filtro.GradoParaleloId.Value);

            if (!string.IsNullOrWhiteSpace(filtro.CedulaEstudiante))
                q = q.Where(m => m.Estudiante.Cedula == filtro.CedulaEstudiante);

            if (!string.IsNullOrWhiteSpace(filtro.Texto))
            {
                var t = filtro.Texto.Trim().ToLower();
                q = q.Where(m =>
                    (m.Estudiante.Nombre + " " + m.Estudiante.Apellido).ToLower().Contains(t)
                    || (m.Estudiante.Apellido + " " + m.Estudiante.Nombre).ToLower().Contains(t));
            }

            // ✅ tus fechas se llaman FechaMatricula
            if (filtro.Desde.HasValue)
                q = q.Where(m => m.FechaMatricula >= filtro.Desde.Value.Date);

            if (filtro.Hasta.HasValue)
                q = q.Where(m => m.FechaMatricula < filtro.Hasta.Value.Date.AddDays(1));

            // orden
            q = q.OrderByDescending(m => m.FechaMatricula).ThenByDescending(m => m.Id);

            var total = await q.CountAsync();

            var items = await q
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(m => new ReporteMatriculaItemDto
                {
                    MatriculaId = (int)m.Id, // ⚠️ si tu DTO usa long, mejor cámbialo a long para no castear

                    // si NO tienes CodigoMatricula en tu entidad, usa el Id
                    CodigoMatricula = m.Id.ToString(),

                    Fecha = m.FechaMatricula,

                    EstudianteId = (int)m.EstudianteId,
                    Estudiante = (m.Estudiante.Nombre + " " + m.Estudiante.Apellido).Trim(),
                    CedulaEstudiante = m.Estudiante.Cedula,

                    RepresentanteId = m.Estudiante.RepresentanteId,
                    Representante = m.Estudiante.Representante != null
                        ? (m.Estudiante.Representante.Nombres + " " + m.Estudiante.Representante.Apellidos).Trim()
                        : null,
                    CedulaRepresentante = m.Estudiante.Representante != null
                        ? m.Estudiante.Representante.NumeroDocumento
                        : null,
                        
                    // ✅ estos salen desde GradoParalelo.AnioLectivo
                    AnioLectivoId = (int)m.GradoParalelo.AnioLectivoId,
                    AnioLectivo = m.GradoParalelo.AnioLectivo.Periodo,

                    GradoParaleloId = (int)m.GradoParaleloId,
                    GradoParalelo = m.GradoParalelo.Id.ToString(), // mejorable abajo

                    // ✅ tu estado se llama EstadoMatricula
                    Estado = m.EstadoMatricula
                })
                .ToListAsync();

            return new PagedResultDto<ReporteMatriculaItemDto>
            {
                Page = page,
                PageSize = pageSize,
                Total = total,
                Items = items
            };
        }

    }
}
