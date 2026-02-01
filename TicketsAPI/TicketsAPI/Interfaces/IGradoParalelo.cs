using TicketsAPI.DTO;

namespace TicketsAPI.Interfaces
{
    public interface IGradoParalelo
    {
        public Task<List<OfertaDTO>> GetDisponibles(long anioLectivoId, long gradoId);
        public Task<List<OfertaDTO>> GetCuposDisponibles(long idEstudiante);
        public Task<List<KeyValueDTO>> SelectorGradoParalelo(long? anioLectivoId);
        public Task<long> CrearOfertaAsync(CreateGradoParaleloDto dto);
    }
}
