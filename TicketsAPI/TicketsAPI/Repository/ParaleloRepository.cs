using Microsoft.EntityFrameworkCore;
using TicketsAPI.DTO;
using TicketsAPI.Entities;
using TicketsAPI.Interfaces;

namespace TicketsAPI.Repository
{
    public class ParaleloRepository : IParalelo
    {

        private readonly ApplicationDbContext _context;
        public ParaleloRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<bool> CrearParalelo(ParaleloDTO paralelo)
        {
            Paralelos pa = new Paralelos();

            pa.Nombre = paralelo.Nombre;
            pa.UserRegister = "SYSTEM";
            pa.DateRegister = DateTime.UtcNow;
            pa.IpRegister = "127.0.0.1";
            pa.Active = true;

            await _context.Paralelos.AddAsync(pa);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> EditarParalelo(ParaleloDTO paralelo)
        {
            var pa = await _context.Paralelos.Where(x => x.IdParalelo == paralelo.IdParalelo).FirstOrDefaultAsync() ?? throw new ArgumentNullException();

            pa.Nombre = paralelo.Nombre;
            pa.IpModification = paralelo.Nombre;
            pa.UserModification = "SYSTEM";
            pa.DateModification = DateTime.UtcNow;

            await _context.Paralelos.AddAsync(pa);
            await _context.SaveChangesAsync();

            return true;

           
        }

        public async Task<bool> EliminarParalelo(long id)
        {
            var paralelo = await _context.Paralelos.Where(x => x.IdParalelo == id).FirstOrDefaultAsync() ?? throw new ArgumentNullException();

            paralelo.Active = false;
            paralelo.UserDelete = "SYSTEM";
            paralelo.DateDelete = DateTime.UtcNow;
            paralelo.IpDelete = "0.0.0.0";


            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ParaleloDTO> GetParalelo(long id)
        {
            var paralelo = await _context.Paralelos.Where(x => x.IdParalelo == id).Select(x => new ParaleloDTO
            {
                IdParalelo = x.IdParalelo,
                Nombre = x.Nombre
            }).FirstOrDefaultAsync() ?? throw  new ArgumentNullException();


            return paralelo;
        }

        public async Task<List<ParaleloDTO>> GetParaleloList()
        {
            var paraleloList = await _context.Paralelos.Where(x => x.Active == true).Select(x => new ParaleloDTO
            {
                IdParalelo = x.IdParalelo,
                Nombre = x.Nombre
            }).ToListAsync();

            return paraleloList;
        }
    }
}
