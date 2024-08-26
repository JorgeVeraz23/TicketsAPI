using Microsoft.EntityFrameworkCore;
using TicketsAPI.DTO;
using TicketsAPI.Entities;
using TicketsAPI.Interfaces;

namespace TicketsAPI.Repository
{

   
    public class UsuarioRepository : UsuarioInterface
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SolicitudRepository> _logger;

        public UsuarioRepository( ApplicationDbContext context, ILogger<SolicitudRepository> logger)
        {
            _context = context;

            _logger = logger;

        }

        public async Task<bool> CrearUsuario(UsuarioDTO usuarioDTO)
        {


            try
            {

                var validacionUsuario = await _context.Usuarios.FirstOrDefaultAsync(x => x.UserName == usuarioDTO.UserName);

                if(validacionUsuario != null)
                {
                    return false;
                }

                Usuario usuarioEntity = new Usuario
                {

                    UserName = usuarioDTO.UserName,
                    Password = usuarioDTO.Password,
                    Rol = usuarioDTO.Rol,

                };

                await _context.Usuarios.AddAsync(usuarioEntity);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el usuario con {UserName}" + usuarioDTO.UserName);
                return false;
            }
        }

        public async Task<UsuarioDTO> LoginUsuario(string username, string password)
        {
            try
            {
                var validacion = await _context.Usuarios.FirstOrDefaultAsync(x => x.UserName == username && x.Password == password) ?? throw new Exception("No se encontro el usuario");


                
                UsuarioDTO usuarioEntity = new UsuarioDTO
                {
                    UserName = validacion.UserName,
                    Password = validacion.Password,
                    Rol = validacion.Rol,
                };

               

                if(validacion != null)
                {
                    return usuarioEntity;
                }
                else
                {
                    return null;
                }
                

            }catch(Exception ex)
            {
                _logger.LogError(ex, "Error al intentar iniciar sesion con {UserName}"+ username);
                return null;
            }
        }
    }
}
