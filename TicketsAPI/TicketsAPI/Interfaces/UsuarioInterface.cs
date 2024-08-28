using TicketsAPI.DTO;

namespace TicketsAPI.Interfaces
{
    public interface UsuarioInterface
    {

        public Task<bool> CrearUsuario(UsuarioDTO usuarioDTO);
        public Task<MessageInfoDTO> LoginUsuario(string username, string password);
        public Task<List<KeyValueDTO>> KeyValueCliente();
        

    }
}
