using TicketsAPI.DTO;

namespace TicketsAPI.Interfaces
{
    public interface IGradoParalelo
    {
        public Task<List<OfertaDTO>> GetDisponibles(long anioLectivoId, long gradoId);
        public Task<long> CrearOfertaAsync(CreateGradoParaleloDto dto);
    }
}
