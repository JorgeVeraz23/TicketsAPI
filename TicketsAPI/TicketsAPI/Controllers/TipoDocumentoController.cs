using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicketsAPI.Interfaces;

namespace TicketsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TipoDocumentoController : ControllerBase
    {
        private readonly ITipoDocumento _tipoDocumento;

        public TipoDocumentoController(ITipoDocumento tipoDocumento)
        {
            _tipoDocumento = tipoDocumento;

        }


        [HttpGet("list")]
        public async Task<IActionResult> ListarTiposDocumento()
        {
            var tiposDocumento = await _tipoDocumento.ListAsync();
            return Ok(tiposDocumento);
        }

    }
}
