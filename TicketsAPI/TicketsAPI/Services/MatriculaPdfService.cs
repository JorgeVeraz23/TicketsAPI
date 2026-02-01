using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using TicketsAPI.DTO;
using TicketsAPI.Interfaces;

namespace TicketsAPI.Services
{
    public class MatriculaPdfService : IMatriculaPdfService
    {
        private readonly ApplicationDbContext _context;

        public MatriculaPdfService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<byte[]> GenerarPdfAsync(int matriculaId)
        {
            var matricula = await _context.Matriculas
                .AsNoTracking()
                .Where(m => m.IsActive == true && m.Id == matriculaId)
                .Include(x => x.GradoParalelo).ThenInclude(gp => gp.Grado).ThenInclude(g => g.Materias)
                .Include(x => x.GradoParalelo).ThenInclude(gp => gp.Paralelo)
                .Include(x => x.GradoParalelo).ThenInclude(gp => gp.AnioLectivo)
                .Include(x => x.Estudiante).ThenInclude(e => e.Representante)
                .FirstOrDefaultAsync();

            if (matricula == null)
                throw new InvalidOperationException("Matrícula no encontrada");

            var dto = new MatriculaPdfDto
            {
                Institucion = "Unidad Educativa AMA",
                Periodo = matricula.GradoParalelo.AnioLectivo.Periodo,
                CodigoMatricula = $"AMA-{matricula.Id:000000}", // ✅ OJO: aquí era tu bug
                Fecha = matricula.FechaMatricula,

                Estudiante = $"{matricula.Estudiante.Nombre} {matricula.Estudiante.Apellido}",
                DocumentoEstudiante = matricula.Estudiante.Cedula,

                Representante = matricula.Estudiante.Representante != null
                    ? $"{matricula.Estudiante.Representante.Nombres} {matricula.Estudiante.Representante.Apellidos}"
                    : "—",
                DocumentoRepresentante = matricula.Estudiante.Representante?.NumeroDocumento ?? "—",

                Grado = matricula.GradoParalelo.Grado.Nombre,
                Paralelo = matricula.GradoParalelo.Paralelo.Nombre,

                Materias = matricula.GradoParalelo.Grado.Materias
                    .Select(mm => mm.Nombre)
                    .OrderBy(x => x)
                    .ToList()
            };

            var document = new MatriculaDocument(dto);
            return document.GeneratePdf();
        }
    }
}
