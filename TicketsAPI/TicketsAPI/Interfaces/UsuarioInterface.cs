using TicketsAPI.DTO;

namespace TicketsAPI.Interfaces
{
    public interface UsuarioInterface
    {

        public Task<bool> CrearUsuario(UsuarioDTO usuarioDTO);
        public Task<UsuarioDTO> LoginUsuario(string username, string password);

    }
}
