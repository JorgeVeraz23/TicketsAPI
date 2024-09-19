using TicketsAPI.DTO;

namespace TicketsAPI.Interfaces
{
    public interface FormGroupInterface
    {
        public Task<MessageInfoSolicitudDTO> CrearFormGroup(FormGroupDTO formGroupDTO);
        public Task<MessageInfoSolicitudDTO> ActualizarFormulario(FormGroupDTO formGroupDTO);
        public Task<List<KeyValueDTO>> KeyValueFormGroup();
        public Task<MessageInfoSolicitudDTO> EliminarFormGroup(long id);
        public Task<FormGroupDTO> ObtenerFormularioPorId(long id);
        public Task<List<FormGroupDTO>> ObtenerTodosLosFormularios();
    }
}
