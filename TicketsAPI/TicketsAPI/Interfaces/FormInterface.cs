using TicketsAPI.DTO;

namespace TicketsAPI.Interfaces
{
    public interface FormInterface
    {
        public Task<MessageInfoSolicitudDTO> CrearFormulario(FormDTO formDTO);
        public Task<List<FormDTO>> ObtenerFormularios();
        public Task<List<KeyValueDTO>> KeyValueFormuario();
        public Task<FormDTO> ObtenerFormularioPorId(long id);
        public Task<MessageInfoSolicitudDTO> ELiminarFormulario(long id);
        public Task<MessageInfoSolicitudDTO> ActualizarFormulario(FormDTO formDTO);
    }
}
