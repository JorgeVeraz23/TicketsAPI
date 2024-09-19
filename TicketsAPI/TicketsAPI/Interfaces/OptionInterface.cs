using TicketsAPI.DTO;

namespace TicketsAPI.Interfaces
{
    public interface OptionInterface
    {
        public Task<MessageInfoSolicitudDTO> CrearOpcionDTO(OptionDTO optionDTO);
        public Task<MessageInfoSolicitudDTO> ActualizarOptionDTO(OptionDTO optionDTO);
        public Task<MessageInfoSolicitudDTO> EliminarOptionDTO(long id);
        public Task<List<OptionDTO>> ObtenerTodasLasOpciones();
        public Task<OptionDTO> ObtenerOpcionPorId(long id);
        public Task<List<KeyValueDTO>> SelectorOptionDTO();

    }
}
