using Microsoft.EntityFrameworkCore;
using TicketsAPI.DTO;
using TicketsAPI.Entities;
using TicketsAPI.Interfaces;

namespace TicketsAPI.Repository
{
    public class CursoRepository : ICurso
    {
        private readonly ApplicationDbContext _context;

        public CursoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CrearCurso(CursoDTO cursoDTO)
        {
            Curso curso = new Curso();

            curso.Nombre = cursoDTO.Nombre;
            curso.Cupos = cursoDTO.Cupos;
            curso.DateRegister = DateTime.UtcNow;
            curso.IpRegister = "0000";
            curso.UserRegister = "SYSTEM";

            await _context.AddAsync(curso);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EditarCurso(CursoDTO cursotDTO)
        {
            var curso = _context.Curso.FirstOrDefault(x => x.IdCurso == cursotDTO.IdCurso);

            curso.Nombre = cursotDTO.Nombre;
            curso.Cupos = cursotDTO.Cupos;
            curso.DateModification = DateTime.UtcNow;
            curso.IpModification = "0000";
            curso.UserModification = "SYSTEM";

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> EliminarCurso(long idCurso)
        {
            var curso = await _context.Curso.FirstOrDefaultAsync(x => x.IdCurso == idCurso);    
            if (curso == null)
            {
                return false;
            }
            curso.Active = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<CursoDTO>> GetAllCursos()
        {
            var cursosList = await _context.Curso.Where(x => x.Active)
                .Select(x => new CursoDTO
                {
                    IdCurso = x.IdCurso,
                    Nombre = x.Nombre,
                    Cupos = x.Cupos,    
                })
                .ToListAsync(); 

            return cursosList;  

            
        }

        public async Task<CursoDTO> GetCursoById(long id)
        {
            var cursoSelected = await _context.Curso.Where(x => x.Active).Select(x => new CursoDTO
            {
                IdCurso = x.IdCurso,
                Nombre = x.Nombre,
                Cupos = x.Cupos,
            }).FirstOrDefaultAsync() ?? throw new ArgumentNullException();

            return cursoSelected;
        }
    }
}
