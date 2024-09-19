using TicketsAPI.DTO;

namespace TicketsAPI.Interfaces
{
    public interface FieldTypeInterface
    {
        public Task<MessageInfoSolicitudDTO> CrearFieldType(FieldTypeDTO fieldTypeDTO);
        public Task<MessageInfoSolicitudDTO> ActualizarFieldType(FieldTypeDTO fieldTypeDTO);
        public Task<MessageInfoSolicitudDTO> EliminarFieldType(long id);
        public Task<FieldTypeDTO> MostrarFieldTypePorId(long id);
        public Task<List<FieldTypeDTO>> MostrarTodosLosFieldTypePorId();
        public Task<List<KeyValueDTO>> SelectorFieldType();
    }
}
