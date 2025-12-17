using TicketsAPI.DTO;

namespace TicketsAPI.Interfaces
{
    public interface IParalelo
    {
        public Task<ParaleloDTO> GetParalelo(long id);
        public Task<List<ParaleloDTO>> GetParaleloList();
        public Task<bool> CrearParalelo(ParaleloDTO paralelo);
        public Task<bool> EditarParalelo(ParaleloDTO paralelo);
        public Task<bool> EliminarParalelo(long id);
    }
}
