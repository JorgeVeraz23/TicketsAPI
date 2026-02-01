using Microsoft.EntityFrameworkCore;
using TicketsAPI.DTO;
using TicketsAPI.Entities;
using TicketsAPI.Interfaces;

namespace TicketsAPI.Repository
{
    public class OfertaRepository : IGradoParalelo
    {
        private readonly ApplicationDbContext _context;

        public OfertaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<long> CrearOfertaAsync(CreateGradoParaleloDto dto)
        {
            // 1️⃣ validar cupos
            if (dto.Cupos <= 0)
                throw new Exception("Los cupos deben ser mayores a cero.");

            // 2️⃣ evitar duplicados
            var existe = await _context.GradoParalelos.AnyAsync(x =>
                x.IsActive == true &&
                x.GradoId == dto.GradoId &&
                x.ParaleloId == dto.ParaleloId &&
                x.AnioLectivoId == dto.AnioLectivoId
            );

            if (existe)
                throw new Exception("Ya existe una oferta para ese grado, paralelo y año lectivo.");

            // 3️⃣ crear oferta
            var entity = new GradoParalelo
            {
                GradoId = dto.GradoId,
                ParaleloId = dto.ParaleloId,
                AnioLectivoId = dto.AnioLectivoId,
                Cupos = dto.Cupos,
                IsActive = true,
                FechaCreacion = DateTime.UtcNow
            };

            _context.GradoParalelos.Add(entity);
            await _context.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<List<OfertaDTO>> GetCuposDisponibles(long idEstudiante)
        {

            var estudiante = await _context.Estudiantes
                .Where(e => e.Id == idEstudiante && e.IsActive == true)
                .Select(x => new
                {
                    
                    x.UltimoGradoAprobado
                }).FirstOrDefaultAsync();


            var anioLectivoId = await _context.AnioLectivo.Where(x => x.Vigente == true).Select(c => c.Id).FirstOrDefaultAsync();

            var query = _context.GradoParalelos.Include(x => x.Grado)
                .Where(x => x.AnioLectivoId == anioLectivoId
                && x.Grado.Nivel == estudiante!.UltimoGradoAprobado + 1
                )
                .Select(x => new OfertaDTO
                {
                    gradoParaleloId = x.Id,
                    paraleloId = x.Paralelo.Id,
                    disponibles = 0, // se calcula despues
                    gradoNombre = x.Grado.Nombre,
                    anioLectivo = x.AnioLectivo.Periodo,
                    paraleloNombre = x.Paralelo.Nombre,
                    cupos = x.Cupos,
                    ocupados = _context.Matriculas.Count(m => m.IsActive == true && m.GradoParaleloId == x.Id)
                });

            var data = await query.ToListAsync();

            // calcula disponibles en memoria (simple y claro)
            data.ForEach(d => d.disponibles = d.cupos - d.ocupados);

            return data;
        }

        public async Task<List<OfertaDTO>> GetDisponibles(long anioLectivoId, long gradoId)
        {
            var query = _context.GradoParalelos
                .AsNoTracking()
                .Where(x => x.AnioLectivoId == anioLectivoId
                && x.GradoId == gradoId)
                .Select(x => new OfertaDTO
                {
                    gradoParaleloId = x.Id,
                    paraleloId = x.Paralelo.Id,
                    disponibles = 0, // se calcula despues
                    gradoNombre = x.Grado.Nombre,
                    anioLectivo = x.AnioLectivo.Periodo,
                    paraleloNombre = x.Paralelo.Nombre,
                    cupos = x.Cupos,
                    ocupados = _context.Matriculas.Count(m => m.IsActive == true && m.GradoParaleloId == x.Id)
                });

            var data = await query.ToListAsync();

            // calcula disponibles en memoria (simple y claro)
            data.ForEach(d => d.disponibles = d.cupos - d.ocupados);

            return data;
        }

        public async Task<List<KeyValueDTO>> SelectorGradoParalelo(long? anioLectivoId)
        {
            var q = _context.GradoParalelos
     .AsNoTracking()
     .Where(gp => gp.IsActive == true);

            if (anioLectivoId.HasValue)
                q = q.Where(gp => gp.AnioLectivoId == anioLectivoId.Value);

            var data = await q
                .Include(gp => gp.Grado)
                .Include(gp => gp.Paralelo)
                .OrderBy(gp => gp.Grado.Nombre)
                .ThenBy(gp => gp.Paralelo.Nombre)
                .Select(gp => new KeyValueDTO
                {
                    Key = gp.Id,
                    Value = gp.Grado.Nombre + " - " + gp.Paralelo.Nombre + " (Cupos: " + gp.Cupos + ")"
                })
                .ToListAsync();

            return data;
        }
    }
}
