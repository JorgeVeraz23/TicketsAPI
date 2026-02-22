using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using TicketsAPI.DTO;
using TicketsAPI.Interfaces;

namespace TicketsAPI.Services
{
    public class MatriculaPdfService : IMatriculaPdfService
    {
        private readonly ApplicationDbContext _context;
        private readonly DocumentoService _documentoService;

        public MatriculaPdfService(ApplicationDbContext context, DocumentoService documentoService)
        {
            _context = context;
            _documentoService = documentoService;
        }

        public async Task<byte[]> GenerarPdfAsync(int matriculaId)
        {
            var matricula = await _context.Matriculas
                .AsNoTracking()
                .Where(m => m.IsActive == true && m.Id == matriculaId)
                .Include(x => x.GradoParalelo).ThenInclude(gp => gp.Grado).ThenInclude(g => g.Materias)!.ThenInclude(m => m.Profesor)
                .Include(x => x.GradoParalelo).ThenInclude(gp => gp.Paralelo)
                .Include(x => x.GradoParalelo).ThenInclude(gp => gp.AnioLectivo)
                .Include(x => x.Estudiante).ThenInclude(e => e.Representante)
                .FirstOrDefaultAsync();

            if (matricula == null)
                throw new InvalidOperationException("Matrícula no encontrada");

            // ====== Buscar foto del estudiante en tabla Documento ======
            const int TIPO_FOTO_ESTUDIANTE = 4;

            var fotoDoc = await _context.Documento
                .AsNoTracking()
                .Where(d => d.IsActive == true)
                .Where(d => d.EstudianteId == matricula.Estudiante.Id)
                .Where(d => d.TipoDocumentoId == TIPO_FOTO_ESTUDIANTE)
                // prioridad: aprobada primero
                .OrderByDescending(d => d.Aprobado == true)
                // luego la más reciente
                .ThenByDescending(d => d.FechaCreacion)
                .Select(d => new { d.Id, d.MimeType })
                .FirstOrDefaultAsync();

            byte[]? fotoBytes = null;

            if (fotoDoc != null)
            {
                // Descarga bytes del blob usando tu service
                var (bytes, contentType, _) = await _documentoService.DescargarBytesAsync(fotoDoc.Id); 

                // seguridad: solo imagen
                if (!string.IsNullOrWhiteSpace(contentType) && contentType.StartsWith("image/"))
                    fotoBytes = bytes;
                else if (!string.IsNullOrWhiteSpace(fotoDoc.MimeType) && fotoDoc.MimeType.StartsWith("image/"))
                    fotoBytes = bytes;
            }

            var dto = new MatriculaPdfDto
            {
                Institucion = "Unidad Educativa Ana Maria Iza",
                Periodo = matricula.GradoParalelo.AnioLectivo.Periodo,
                CodigoMatricula = $"AMA-{matricula.Id:000000}",
                Fecha = matricula.FechaMatricula,

                FotoEstudiante = fotoBytes, // ✅ ya es byte[]

                Estudiante = $"{matricula.Estudiante.Nombre} {matricula.Estudiante.Apellido}",
                DocumentoEstudiante = matricula.Estudiante.Cedula,

                Representante = matricula.Estudiante.Representante != null
                    ? $"{matricula.Estudiante.Representante.Nombres} {matricula.Estudiante.Representante.Apellidos}"
                    : "—",
                DocumentoRepresentante = matricula.Estudiante.Representante?.NumeroDocumento ?? "—",

                Grado = matricula.GradoParalelo.Grado.Nombre,
                Paralelo = matricula.GradoParalelo.Paralelo.Nombre,

                Materias = matricula.GradoParalelo.Grado.Materias!
                    .Select(mm => mm.Nombre)
                    .OrderBy(x => x)
                    .ToList(),

                Profesor = matricula.GradoParalelo.Grado.Materias!
                    .Select(mm => mm.Profesor)
                    .Where(p => p != null)
                    .Select(p => $"{p!.Nombres} {p.Apellidos}")
                    .OrderBy(x => x)
                    .ToList(),
            };

            var document = new MatriculaDocument(dto);
            return document.GeneratePdf();
        }
    }
}
