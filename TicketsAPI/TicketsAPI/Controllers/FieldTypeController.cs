using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicketsAPI.DTO;
using TicketsAPI.Interfaces;

namespace TicketsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FieldTypeController : ControllerBase
    {

        private readonly FieldTypeInterface _fieldTypeInterface;

        public FieldTypeController(FieldTypeInterface fieldTypeInterface)
        {
            _fieldTypeInterface = fieldTypeInterface;
        }



        [HttpGet("ObtenerTodosLosFieldType")]
        public async Task<ActionResult> ObtenerTodosLosFieldType()
        {
            var response = await _fieldTypeInterface.MostrarTodosLosFieldTypePorId();

            return Ok(response);    
        }

        [HttpGet("ObtenerFieldType")]
        public async Task<ActionResult> ObtenerFieldType(long id)
        {
            var response = await _fieldTypeInterface.MostrarFieldTypePorId(id);

            return Ok(response);
        }


        [HttpPost("CrearFieldType")]
        public async Task<ActionResult> CrearFieldType(FieldTypeDTO fieldTypeDTO)
        {
            var response = await _fieldTypeInterface.CrearFieldType(fieldTypeDTO);


            return Ok(response);
        }




    }
}
