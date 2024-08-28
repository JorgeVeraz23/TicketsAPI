using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicketsAPI.DTO;
using TicketsAPI.Interfaces;

namespace TicketsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly UsuarioInterface _usuarioInterface;

        public UsuarioController(UsuarioInterface usuarioInterface)
        {
            _usuarioInterface = usuarioInterface;
        }


        [HttpPost("CrearUsuario")]
        public async Task<ActionResult> CrearUsuario(UsuarioDTO usuarioDTO)
        {
            try
            {
                var response = await _usuarioInterface.CrearUsuario(usuarioDTO);

                return Ok(response);

            }catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPost("LoginUsuario")]
        public async Task<ActionResult> LoginUsuario(string username, string password)
        {
            try
            {
                var response = await _usuarioInterface.LoginUsuario(username, password);
               
                 return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("KeyValueCliente")]
        public async Task<ActionResult> KeyValueCliente()
        {
            try
            {
                var response = await _usuarioInterface.KeyValueCliente();

                return Ok(response);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
