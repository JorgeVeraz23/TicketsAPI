using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicketsAPI.DTO;
using TicketsAPI.Interfaces;

namespace TicketsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FormFieldController : ControllerBase
    {

        private readonly FormFieldInterface _formFieldInterface;
        public FormFieldController(FormFieldInterface formFieldInterface)
        {
            _formFieldInterface = formFieldInterface;
        }

        [HttpGet("ObtenerTodosLosFormFields")]
        public async Task<ActionResult> ObtenerTodosLosFormFields()
        {
            var response = await _formFieldInterface.ObtenerTodosLosFormField();

            return Ok(response);
        }

        [HttpGet("ObtenerFormField")]
        public async Task<ActionResult> ObtenerFormField(long id)
        {
            var response = await _formFieldInterface.ObtenerFormField(id);

            return Ok(response);    


        }

        [HttpGet("SelectorFormField")]
        public async Task<ActionResult> SelectorFormField()
        {
            var response = await _formFieldInterface.SelectorFormField();

            return Ok(response);
        }

        [HttpDelete("EliminarFormField")]
        public async Task<ActionResult> EliminarFormField(long id)
        {
            var response = await _formFieldInterface.EliminarFormField(id);

            return Ok(response);
        }

        [HttpPost("CrearFormField")]
        public async Task<ActionResult> CrearFormField(FormFieldDTO formFieldDTO)
        {
            var response = await _formFieldInterface.CrearFormField(formFieldDTO);

            return Ok(response);
        }

        [HttpPut("ActualizarFormField")]
        public async Task<ActionResult> ActualizarFormField(FormFieldDTO formFieldDTO)
        {
            var response = await _formFieldInterface.ActualizarFormField(formFieldDTO);

            return Ok(response);
        }


    }
}
