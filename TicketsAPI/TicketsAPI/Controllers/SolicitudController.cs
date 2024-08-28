using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicketsAPI.DTO;
using TicketsAPI.Entities;
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



        [HttpPost]
        [Route("CrearSolicitud")]
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

        [HttpPut]
        [Route("ActualizarSolicitud")]
        public async Task<IActionResult> ActualizarSolicitud(ActualizarSolicitudDTO solicitud)
        {
            if (ModelState.IsValid)
            {
                var result = await _solicitudInterface.ActualizarSolicitud(solicitud);

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



        [HttpGet("GetSolicitudesDeUsuario")]
        public async Task<ActionResult> MostrarSolicitud(long idUsuario)
        {
            try
            {
      
               var result = await _solicitudInterface.GetAllSolicitudes(idUsuario);


                return Ok(result);
                
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetAllSolicitudesByFilterAdministrador")]
        public async Task<ActionResult> MostrarSolicitudesAdministrador(long idUsuario, DateTime fechaIngreso)
        {
            try
            {
                var result = await _solicitudInterface.GetAllSolitudesByFilter(idUsuario, fechaIngreso);


                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("MostrarJustificativo")]
        public async Task<ActionResult> MostrarJustificativo( long idSolicitud)
        {
            try
            {
                var result = await _solicitudInterface.GetJustificativo(idSolicitud);


                return Ok(result);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("VerDetalleSolicitud")]
        public async Task<ActionResult> VerDetalleSolicitud(long idSolicitud)
        {
            try
            {
                var result = await _solicitudInterface.VerDetalleSolicitud(idSolicitud);


                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("KeyValueEstados")]
        public ActionResult<IEnumerable<KeyValueDTO>> GetEstados()
        {
            try
            {
                var estados = Enum.GetValues(typeof(EnumEstadoSolicitud))
                    .Cast<EnumEstadoSolicitud>()
                    .Select(e => new KeyValueDTO
                    {
                        Key = (int)e,
                        Value = e.ToString()
                    });

                return Ok(estados);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetAllSolicitudesByFilterCliente")]
        public async Task<ActionResult> GetAllSolicitudesByFilter(long idUsuario, DateTime fechaIngreso, EnumEstadoSolicitud Estado)
        {
            try {
                var result = await _solicitudInterface.GetAllSolicitudesByFilterCliente(idUsuario, fechaIngreso, Estado);


                return Ok(result);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("ObtenerTodasLasSolicitudesAdministador")]
        public async Task<ActionResult> MostrarTodasLasSolicitudes()
        {
            try
            {
                var result = await _solicitudInterface.GetAllSolicitudesAdministrador();

                return Ok(result);

            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
