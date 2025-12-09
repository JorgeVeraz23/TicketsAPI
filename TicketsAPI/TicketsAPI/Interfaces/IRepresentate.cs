using TicketsAPI.DTO;

namespace TicketsAPI.Interfaces
{
    public interface IRepresentate
    {
        public Task<bool> CrearRepresentate(RepresentanteDTO represetanteDTO);
        public Task<bool> EditarRepresentate(RepresentanteDTO representanteDTO);
        public Task<bool> DeleteRepresentate(long representanteId);
        public Task<List<RepresentanteDTO>> GetAllRepresentate();
        public Task<RepresentanteDTO> GetRepresentateById(long representanteId);    
        
    }
}
