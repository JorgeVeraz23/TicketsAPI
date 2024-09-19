using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicketsAPI.DTO;
using TicketsAPI.Interfaces;
using TicketsAPI.Repository;

namespace TicketsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FormGroupController : ControllerBase
    {
        private readonly FormGroupInterface _formGroupInterface;

        public FormGroupController(FormGroupInterface formGroupInterface)
        {
            _formGroupInterface = formGroupInterface; 
        }

        [HttpPost("CrearFormulario")]
        public async Task<ActionResult> CrearFormulario(FormGroupDTO formGroupDTO)
        {
            var response = await _formGroupInterface.CrearFormGroup(formGroupDTO);

            return Ok(response);    
        }

        [HttpPut("ActualizarFormulario")]
        public async Task<ActionResult> ActualizarFormulario(FormGroupDTO formGroup)
        {
            var response = await _formGroupInterface.ActualizarFormulario(formGroup);

            return Ok(response);
        }


        [HttpGet("ObtenerTodosLosFormularios")]
        public async Task<ActionResult> ObtenerTodosLosFormularios()
        {
            var response = await _formGroupInterface.ObtenerTodosLosFormularios();

            return Ok(response);    
        }

        [HttpGet("ObtenerFormularioPorId")]
        public async Task<ActionResult> ObtenerFormularioPorId(long id)
        {
            var response = await _formGroupInterface.ObtenerFormularioPorId(id);

            return Ok(response);
        }

        [HttpDelete("EliminarFormulario")]
        public async Task<ActionResult> EliminarFormularioPorId(long id)
        {
            var response = await _formGroupInterface.EliminarFormGroup(id);

            return Ok(response);
        }

        [HttpGet("SelectorFormulario")]
        public async Task<ActionResult> SelectorFormulario()
        {
            var response = await _formGroupInterface.KeyValueFormGroup();

            return Ok(response);
        }
    }
}
