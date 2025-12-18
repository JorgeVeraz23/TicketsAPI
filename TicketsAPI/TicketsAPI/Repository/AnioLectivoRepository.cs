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
            throw new NotImplementedException();
        }

        public Task<AnioLectivoDTO> GetAnioLectivoById(long id)
        {
            throw new NotImplementedException();
        }
    }
}
