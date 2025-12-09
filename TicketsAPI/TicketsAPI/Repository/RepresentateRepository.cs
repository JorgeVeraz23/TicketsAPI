using TicketsAPI.DTO;
using TicketsAPI.Interfaces;

namespace TicketsAPI.Repository
{
    public class RepresentateRepository : IRepresentate
    {
        private readonly ApplicationDbContext _context;
        public RepresentateRepository(ApplicationDbContext context)
        {
            
        }
        public Task<bool> CrearRepresentate(RepresentanteDTO represetanteDTO)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteRepresentate(long representanteId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> EditarRepresentate(RepresentanteDTO representanteDTO)
        {
            throw new NotImplementedException();
        }

        public Task<List<RepresentanteDTO>> GetAllRepresentate()
        {
            throw new NotImplementedException();
        }

        public Task<RepresentanteDTO> GetRepresentateById(long representanteId)
        {
            throw new NotImplementedException();
        }
    }
}
