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
            return true;
        }

        public Task<bool> EditarCurso(CursoDTO cursotDTO)
        {
            throw new NotImplementedException();
        }

        public Task<bool> EliminarCurso(long idCurso)
        {
            throw new NotImplementedException();
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
