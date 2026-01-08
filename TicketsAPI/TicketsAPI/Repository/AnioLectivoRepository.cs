using Microsoft.EntityFrameworkCore;
using TicketsAPI.DTO;
using TicketsAPI.Interfaces;

namespace TicketsAPI.Repository
{
    public class AnioLectivoRepository : IAnioLectiivo
    {
        private readonly ApplicationDbContext _context;

        public AnioLectivoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<bool> CrearAnioLectivo(AnioLectivoDTO anioLectivoDTO)
        {
            throw new NotImplementedException();
        }

        public Task<bool> EditarAnioLectivo(AnioLectivoDTO anioLectivoDTO)
        {
            throw new NotImplementedException();
        }

        public Task<bool> EliminarAnioLectivo(long id)
        {
            throw new NotImplementedException();
        }

        public Task<List<AnioLectivoDTO>> GetAllAnioLectivo()
        {
            var anioLectivoList = _context.AnioLectivo
                .Where(a => a.IsActive == true)
                .Select(a => new AnioLectivoDTO
                {
                    Id = a.Id,
                    Periodo = a.Periodo
                })
                .ToListAsync();
            
            return anioLectivoList;
        }

        public Task<AnioLectivoDTO> GetAnioLectivoById(long id)
        {
            throw new NotImplementedException();
        }

        public Task<List<KeyValueDTO>> SelectorAnioLectivo()
        {
            var selectorList = _context.AnioLectivo
                .Where(a => a.IsActive == true)
                .Select(a => new KeyValueDTO
                {
                    Key = a.Id,
                    Value = a.Periodo
                })
                .ToListAsync();

            return selectorList;
        }
    }
}
