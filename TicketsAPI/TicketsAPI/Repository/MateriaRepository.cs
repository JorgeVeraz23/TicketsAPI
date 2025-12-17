using Microsoft.EntityFrameworkCore;
using TicketsAPI.DTO;
using TicketsAPI.Entities;
using TicketsAPI.Interfaces;

namespace TicketsAPI.Repository
{
    public class MateriaRepository : IMateria
    {
        private readonly ApplicationDbContext _context;
        public MateriaRepository(ApplicationDbContext context)
        {
           _context = context; 
        }

        public async Task<bool> CrearMateria(MateriaDTO materiaDTO)
        {
            Materia materia = new Materia();

            materia.Nombre = materiaDTO.Name;
            materia.Active = true;
            materia.DateRegister = DateTime.UtcNow;
            materia.UserRegister = "SYSTEM";
            materia.IpRegister = "0.0.0.0";
            
            await _context.Materias.AddAsync(materia);   
            await _context.SaveChangesAsync();

            return true;

        }

        public async Task<bool> EditarMateria(MateriaDTO materiaDTO)
        {
            var materia = await _context.Materias.Where(x => x.IdMateria == materiaDTO.IdMateria).FirstOrDefaultAsync() ?? throw new ArgumentNullException();

            materia.Nombre = materiaDTO.Name;
            materia.UserModification = "SYSTEM";
            materia.DateModification = DateTime.UtcNow;
            materia.IpModification = "0.0.0.0";

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EliminarMateria(long id)
        {
            var materia = await _context.Materias.Where(x => x.IdMateria == id).FirstOrDefaultAsync() ?? throw new ArgumentNullException();

            materia.Active = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<MateriaDTO>> GetAllMaterias()
        {
            var materias = await _context.Materias.Where(x => x.Active == true).Select(x => new MateriaDTO
            {
                IdMateria = x.IdMateria,
                Name = x.Nombre
            }).ToListAsync();

            return materias;
        }

        public async Task<MateriaDTO> GetMateria(long id)
        {
            var materia = await _context.Materias.Where(x => x.IdMateria == id)
                .Select(x => new MateriaDTO
                {
                    IdMateria = x.IdMateria,
                    Name = x.Nombre
                })
                .FirstOrDefaultAsync() ?? throw new ArgumentNullException();

            return materia;


        }
    }
}
