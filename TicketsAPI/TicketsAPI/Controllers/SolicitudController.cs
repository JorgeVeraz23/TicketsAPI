using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicketsAPI.DTO;
using TicketsAPI.Interfaces;
using TicketsAPI.Repository;

namespace TicketsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SolicitudController : ControllerBase
    {

        private readonly SolicitudInterface _solicitudInterface;

        public SolicitudController(SolicitudInterface solicitudInterface)
        {
            _solicitudInterface = solicitudInterface;   
        }

        //[HttpPost("CrearSolicitud")]
        //public async Task<ActionResult> CrearSolicitud(SolicitudDTO solicitudDTO)
        //{
        //    try
        //    {
        //        if(solicitudDTO == null)
        //        {
        //            return BadRequest("La solicitud enviada es nula.");
        //        }

        //        var response = await _solicitudInterface.CreateSolicitud(solicitudDTO);

        //        return Ok(response);

        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.Message);
        //    }
        //}


        [HttpPost]
        [Route("crear")]
        public async Task<IActionResult> CreateSolicitud([FromForm] SolicitudDTO solicitud)
        {
            if (ModelState.IsValid)
            {
                var result = await _solicitudInterface.CreateSolicitud(solicitud);

                if (result.Cod == "201")
                {
                    return Ok(result);
                }
                else
                {
                    return StatusCode(500, result);
                }
            }

            return BadRequest("Datos inválidos.");
        }

    }
}
