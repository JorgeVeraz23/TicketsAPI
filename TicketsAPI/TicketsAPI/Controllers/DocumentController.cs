using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicketsAPI.DTO;
using TicketsAPI.Services;

namespace TicketsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : ControllerBase
    {

        private readonly DocumentoService _svc;

        public DocumentController(DocumentoService svc) => _svc = svc;

        // Ajusta a tu auth real
        private string UserName => User?.Identity?.Name ?? "system";

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Upload([FromForm] DocumentoUploadRequest request, CancellationToken ct)
        {
            // ejemplo: llama a tu servicio
            var id = await _svc.UploadAsync(
                request.EstudianteId,
                request.TipoDocumentoId,
                request.File,
                request.Observacion,
                User?.Identity?.Name ?? "system",
                ct);

            return Ok(new { id });
        }


        [HttpGet("estudiante/{estudianteId:long}")]
        public async Task<IActionResult> Listar(long estudianteId, CancellationToken ct)
        {
            var docs = await _svc.ListarPorEstudianteAsync(estudianteId, ct);
            return Ok(docs);
        }

        [HttpPost("{id:long}/aprobar")]
        public async Task<IActionResult> Aprobar(long id, [FromBody] AprobarDto dto, CancellationToken ct)
        {
            await _svc.AprobarAsync(id, dto.Aprobado, dto.Observacion, UserName, ct);
            return NoContent();
        }

        [HttpGet("{id:long}/download")]
        public async Task<IActionResult> Descargar(long id, CancellationToken ct)
        {
            var (stream, contentType, downloadName) = await _svc.DescargarAsync(id, ct);
            return File(stream, contentType, downloadName);
        }
    }


    public record AprobarDto(bool Aprobado, string? Observacion);

}
