using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using TicketsAPI.DTO;
using TicketsAPI.Entities;
using TicketsAPI.Enum;
using TicketsAPI.Interfaces;
using TicketsAPI.Repository;
using System; // Para el uso de Enum y TryParse

using TicketsAPI.Services;

namespace TicketsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReporteController : ControllerBase
    {
        private readonly IReportesServices _reporteServices;
        private readonly IMatriculaPdfService _matriculaPdfService;
        private readonly IMatricula _matricula;
        private readonly ApplicationDbContext _context;

        public ReporteController(IReportesServices reportesServices, ApplicationDbContext context, IMatricula matricula, IMatriculaPdfService matriculaPdfService)
        {
            _reporteServices = reportesServices;
            _matricula = matricula;
            _matriculaPdfService = matriculaPdfService;
            _context = context;
        }

        // GET: api/reportes/matriculas?AnioLectivoId=1&GradoParaleloId=10&Texto=juan&Desde=2026-01-01&Hasta=2026-01-31&Page=1&PageSize=20
        [HttpGet("matriculas")]
        public async Task<ActionResult<PagedResultDto<ReporteMatriculaItemDto>>> ObtenerMatriculas([FromQuery] ReporteMatriculasFiltroDto filtro)
        {
            // Validaciones básicas (para evitar valores raros)
            if (filtro.Page <= 0) filtro.Page = 1;
            if (filtro.PageSize <= 0) filtro.PageSize = 20;
            if (filtro.PageSize > 200) filtro.PageSize = 200;

            // Validación opcional de rango de fechas
            if (filtro.Desde.HasValue && filtro.Hasta.HasValue && filtro.Desde.Value.Date > filtro.Hasta.Value.Date)
                return BadRequest("El parámetro 'Desde' no puede ser mayor que 'Hasta'.");

            var result = await _reporteServices.ObtenerMatriculasAsync(filtro);
            return Ok(result);
        }

        [HttpGet("matriculas/{matriculaId:int}/pdf")]
        public async Task<IActionResult> DescargarMatriculaPdf(int matriculaId)
        {
            try
            {
                var pdfBytes = await _matriculaPdfService.GenerarPdfAsync(matriculaId);
                return File(pdfBytes, "application/pdf", $"Matricula_AMA-{matriculaId:000000}.pdf");
            }
            catch (InvalidOperationException)
            {
                return NotFound("Matrícula no encontrada");
            }
        }

        // GET: /api/Reporte/matriculas/listado/excel?periodo=2025-2026
        [HttpGet("matriculas/listado/excel")]
        public async Task<IActionResult> ListadoMatriculasExcel([FromQuery] string periodo)
        {
            if (string.IsNullOrWhiteSpace(periodo))
                return BadRequest("El período es obligatorio.");

            var rows = await _context.Matriculas
                .AsNoTracking()
                .Where(m => m.IsActive == true
                            && m.GradoParalelo.AnioLectivo.Periodo == periodo)
                .Include(m => m.Estudiante)
                .Include(m => m.GradoParalelo).ThenInclude(gp => gp.Grado)
                .Include(m => m.GradoParalelo).ThenInclude(gp => gp.Paralelo)
                .Include(m => m.GradoParalelo).ThenInclude(gp => gp.AnioLectivo)
                .OrderBy(m => m.GradoParalelo.Grado.Nombre)
                .ThenBy(m => m.GradoParalelo.Paralelo.Nombre)
                .ThenBy(m => m.Estudiante.Apellido)
                .Select(m => new
                {
                    m.Id,
                    Estudiante = (m.Estudiante.Apellido + " " + m.Estudiante.Nombre),
                    Documento = m.Estudiante.Cedula,
                    Grado = m.GradoParalelo.Grado.Nombre,
                    Paralelo = m.GradoParalelo.Paralelo.Nombre,
                    Periodo = m.GradoParalelo.AnioLectivo.Periodo,
                    Estado = m.EstadoMatricula,
                    Fecha = m.FechaMatricula
                })
                .ToListAsync();

            using var wb = new XLWorkbook();
            var sheetName = ReporteChartHelper.GetSafeSheetName($"Matriculas {periodo}");
            var ws = wb.Worksheets.Add(sheetName);

            // Encabezado institucional
            ws.Cell(1, 1).Value = "Unidad Educativa AMA";
            ws.Cell(2, 1).Value = "Listado de Matrículas";
            ws.Cell(3, 1).Value = $"Período: {periodo}";
            ws.Cell(4, 1).Value = $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}";

            ws.Range(1, 1, 1, 8).Merge().Style.Font.SetBold().Font.SetFontSize(14);
            ws.Range(2, 1, 2, 8).Merge().Style.Font.SetBold().Font.SetFontSize(12);
            ws.Range(3, 1, 4, 8).Merge().Style.Font.SetFontSize(10).Font.SetFontColor(XLColor.Gray);

            // Header tabla (fila 6)
            var headerRow = 6;
            var headers = new[] { "ID", "Estudiante", "Documento", "Grado", "Paralelo", "Período", "Estado", "Fecha" };
            for (int i = 0; i < headers.Length; i++)
            {
                ws.Cell(headerRow, i + 1).Value = headers[i];
                ws.Cell(headerRow, i + 1).Style.Font.SetBold();
                ws.Cell(headerRow, i + 1).Style.Fill.SetBackgroundColor(XLColor.FromHtml("#EEEEEE"));
                ws.Cell(headerRow, i + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            }

            // Data
            var r = headerRow + 1;
            foreach (var item in rows)
            {
                ws.Cell(r, 1).Value = item.Id;
                ws.Cell(r, 2).Value = item.Estudiante;
                ws.Cell(r, 3).Value = item.Documento;
                ws.Cell(r, 4).Value = item.Grado;
                ws.Cell(r, 5).Value = item.Paralelo;
                ws.Cell(r, 6).Value = item.Periodo;
                ws.Cell(r, 7).Value = item.Estado;
                ws.Cell(r, 8).Value = item.Fecha;

                ws.Cell(r, 8).Style.DateFormat.Format = "dd/MM/yyyy HH:mm";
                r++;
            }

            // AutoFilter + Ajustes
            ws.Range(headerRow, 1, Math.Max(r - 1, headerRow), 8).SetAutoFilter();
            ws.Columns(1, 8).AdjustToContents();
            ws.SheetView.FreezeRows(headerRow);

            // Gráfico: conteo por grado (columna)
            var byGrado = rows.GroupBy(x => x.Grado).OrderBy(g => g.Key).ToDictionary(g => g.Key ?? "(sin grado)", g => (double)g.Count());
            using (var chartStream = ReporteChartHelper.RenderChartPng(ReporteChartHelper.ChartKind.ColumnVertical, byGrado, 700, 320))
            {
                ws.AddPicture(chartStream).MoveTo(ws.Cell(r + 2, 1)).WithSize(700, 320);
            }

            using var stream = new MemoryStream();
            wb.SaveAs(stream);
            var content = stream.ToArray();

            var safePeriodo = periodo.Replace("/", "-").Replace(" ", "");
            var fileName = $"Listado_Matriculas_{safePeriodo}.xlsx";

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName
            );
        }

        // GET: /api/Reporte/estudiantes/listado/excel?estado=Activo&q=juan
        [HttpGet("estudiantes/listado/excel")]
        public async Task<IActionResult> ListadoEstudiantesExcel(
            [FromQuery] string? estado,
            [FromQuery] string? q,
            [FromQuery] string? genero,
            [FromQuery] long? gradoParaleloId,
            [FromQuery] bool? conMatricula
        )
        {
            var query = _context.Estudiantes
                .AsNoTracking()
                .Include(x => x.Representante)
                .Include(x => x.Matriculas)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(estado))
            {
                var e = estado.Trim().ToLower();
                if (e == "activo") query = query.Where(x => x.IsActive == true);
                else if (e == "inactivo") query = query.Where(x => x.IsActive == false);
            }

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim().ToLower();
                query = query.Where(x =>
                    (x.Nombre + " " + x.Apellido).ToLower().Contains(term) ||
                    (x.Apellido + " " + x.Nombre).ToLower().Contains(term) ||
                    (x.Cedula ?? "").ToLower().Contains(term)
                );
            }
    

            if (gradoParaleloId.HasValue && gradoParaleloId.Value > 0)
            {
                query = query.Where(x => x.Matriculas.Any(m => m.IsActive == true && m.GradoParaleloId == gradoParaleloId.Value));
            }

            if (conMatricula.HasValue)
            {
                if (conMatricula.Value)
                    query = query.Where(x => x.Matriculas.Any(m => m.IsActive == true));
                else
                    query = query.Where(x => !x.Matriculas.Any(m => m.IsActive == true));
            }

            var rows = await query
                .OrderBy(x => x.Apellido)
                .ThenBy(x => x.Nombre)
                .Select(x => new
                {
                    // No incluir Id en el Excel según lo solicitado
                    x.Nombre,
                    x.Apellido,
                    Cedula = x.Cedula ?? "",
                    Estado = x.IsActive != null && x.IsActive == true ? "Activo" : "Inactivo",
                    Representante = x.Representante != null ? (x.Representante.Nombres + " " + x.Representante.Apellidos) : "",
                    DocumentoRepresentante = x.Representante != null ? (x.Representante.NumeroDocumento ?? "") : "",
                    Genero = x.Genero.ToString(),
                    TieneMatricula = x.Matriculas.Any(m => m.IsActive == true)
                })
                .ToListAsync();

            using var wb = new XLWorkbook();
            var sheetName = ReporteChartHelper.GetSafeSheetName("Estudiantes");
            var ws = wb.Worksheets.Add(sheetName);

            // Encabezado
            ws.Cell(1, 1).Value = "Unidad Educativa AMA";
            ws.Cell(2, 1).Value = "Listado general de estudiantes";
            ws.Cell(3, 1).Value = $"Filtros: Estado={(string.IsNullOrWhiteSpace(estado) ? "Todos" : estado)}" +
                                 $"{(!string.IsNullOrWhiteSpace(q) ? $" • Búsqueda='{q}'" : "")}" +
                                 $"{(!string.IsNullOrWhiteSpace(genero) ? $" • Género='{genero}'" : "")}" +
                                 $"{(gradoParaleloId.HasValue ? $" • GradoParaleloId={gradoParaleloId}" : "")}" +
                                 $"{(conMatricula.HasValue ? $" • ConMatricula={(conMatricula.Value ? "Sí" : "No")}" : "")}";
            ws.Cell(4, 1).Value = $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}";

            ws.Range(1, 1, 1, 8).Merge().Style.Font.SetBold().Font.SetFontSize(14);
            ws.Range(2, 1, 2, 8).Merge().Style.Font.SetBold().Font.SetFontSize(12);
            ws.Range(3, 1, 4, 8).Merge().Style.Font.SetFontSize(10).Font.SetFontColor(XLColor.Gray);

            var headerRow = 6;
            var headers = new[]
            {
                "Nombres", "Apellidos", "Cédula", "Estado",
                "Representante", "Doc. Representante", "Género", "Tiene Matrícula"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                ws.Cell(headerRow, i + 1).Value = headers[i];
                ws.Cell(headerRow, i + 1).Style.Font.SetBold();
                ws.Cell(headerRow, i + 1).Style.Fill.SetBackgroundColor(XLColor.FromHtml("#EEEEEE"));
                ws.Cell(headerRow, i + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            }

            var r = headerRow + 1;
            foreach (var x in rows)
            {
                ws.Cell(r, 1).Value = x.Nombre;
                ws.Cell(r, 2).Value = x.Apellido;
                ws.Cell(r, 3).Value = x.Cedula;
                ws.Cell(r, 4).Value = x.Estado;
                ws.Cell(r, 5).Value = x.Representante;
                ws.Cell(r, 6).Value = x.DocumentoRepresentante;
                ws.Cell(r, 7).Value = x.Genero;
                ws.Cell(r, 8).Value = x.TieneMatricula ? "Sí" : "No";
                r++;
            }

            ws.Range(headerRow, 1, Math.Max(r - 1, headerRow), headers.Length).SetAutoFilter();
            ws.Columns(1, headers.Length).AdjustToContents();
            ws.SheetView.FreezeRows(headerRow);

            // Gráfico: distribución por Género
            var statsGenero = rows
                .GroupBy(x => string.IsNullOrWhiteSpace(x.Genero) ? "(Sin género)" : x.Genero)
                .OrderBy(g => g.Key)
                .ToDictionary(g => g.Key, g => (double)g.Count());

            using (var chartStream = ReporteChartHelper.RenderChartPng(ReporteChartHelper.ChartKind.Pie, statsGenero, 500, 300))
            {
                ws.AddPicture(chartStream).MoveTo(ws.Cell(r + 2, 1)).WithSize(500, 300);
            }

            using var stream = new MemoryStream();
            wb.SaveAs(stream);
            var content = stream.ToArray();

            var fileName = $"Listado_Estudiantes_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName
            );
        }

        [HttpGet("materias/por-maestro")]
        public async Task<IActionResult> MateriasPorMaestro([FromQuery] long? profesorId)
        {
            var query = _context.Materias
                .AsNoTracking()
                .Include(m => m.Grado)
                .Include(m => m.Profesor)
                .OrderBy(m => m.Grado.Nombre);

            if (profesorId > 0 && profesorId != null)
            {
                query = (IOrderedQueryable<Materia>)query.Where(m => m.IdProfesor == profesorId);
            }

            var rows = await query
                .Select(m => new
                {
                    m.Id,
                    Materia = m.Nombre,
                    Profesor = m.Profesor.Nombres + " " + m.Profesor.Apellidos,
                    Grado = m.Grado.Nombre
                })
                .ToListAsync();

            using var wb = new XLWorkbook();
            var sheetName = ReporteChartHelper.GetSafeSheetName("Materias Asignadas");
            var ws = wb.Worksheets.Add(sheetName);

            ws.Cell(1, 1).Value = "Unidad Educativa AMA";
            ws.Cell(2, 1).Value = "Reporte de Materias Asignadas por Profesor";
            ws.Cell(3, 1).Value = $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}";

            var headerRow = 5;
            var headers = new[] { "ID", "Materia", "Profesor", "Grado" };
            for (int i = 0; i < headers.Length; i++)
            {
                ws.Cell(headerRow, i + 1).Value = headers[i];
                ws.Cell(headerRow, i + 1).Style.Font.SetBold();
                ws.Cell(headerRow, i + 1).Style.Fill.SetBackgroundColor(XLColor.FromHtml("#EEEEEE"));
                ws.Cell(headerRow, i + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            }

            var r = headerRow + 1;
            foreach (var item in rows)
            {
                ws.Cell(r, 1).Value = item.Id;
                ws.Cell(r, 2).Value = item.Materia;
                ws.Cell(r, 3).Value = item.Profesor;
                ws.Cell(r, 4).Value = item.Grado;
                r++;
            }

            ws.Range(headerRow, 1, Math.Max(r - 1, headerRow), headers.Length).SetAutoFilter();
            ws.Columns(1, headers.Length).AdjustToContents();
            ws.SheetView.FreezeRows(headerRow);

            // Gráfico: conteo por Grado (barra horizontal)
            var series = rows.GroupBy(x => x.Grado).OrderBy(g => g.Key).ToDictionary(g => g.Key ?? "(sin grado)", g => (double)g.Count());
            using (var chartStream = ReporteChartHelper.RenderChartPng(ReporteChartHelper.ChartKind.BarHorizontal, series, 600, 300))
            {
                ws.AddPicture(chartStream).MoveTo(ws.Cell(r + 2, 1)).WithSize(600, 300);
            }

            using var stream = new MemoryStream();
            wb.SaveAs(stream);
            var content = stream.ToArray();

            var fileName = profesorId.HasValue
                ? $"Materias_Asignadas_{profesorId}.xlsx"
                : "Materias_Asignadas_Todos.xlsx";

            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpGet("cursos/por-profesor/{profesorId:long}")]
        public async Task<IActionResult> CursosPorProfesor(long profesorId)
        {
            var rows = await _context.GradoParalelos
                .AsNoTracking()
                .Where(gp => gp.ProfesorId == profesorId)
                .Include(gp => gp.Grado)
                .Include(gp => gp.Paralelo)
                .OrderBy(gp => gp.Grado.Nombre)
                .ThenBy(gp => gp.Paralelo.Nombre)
                .Select(gp => new
                {
                    gp.Id,
                    Grado = gp.Grado.Nombre,
                    Paralelo = gp.Paralelo.Nombre
                })
                .ToListAsync();

            if (rows.Count == 0)
                return NotFound("Este profesor no tiene cursos asignados.");

            using var wb = new XLWorkbook();
            var sheetName = ReporteChartHelper.GetSafeSheetName($"Cursos Asignados - Profesor {profesorId}");
            var ws = wb.Worksheets.Add(sheetName);

            ws.Cell(1, 1).Value = "Unidad Educativa AMA";
            ws.Cell(2, 1).Value = $"Cursos Asignados a Profesor {profesorId}";
            ws.Cell(3, 1).Value = $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}";

            var headerRow = 5;
            var headers = new[] { "ID", "Grado", "Paralelo" };
            for (int i = 0; i < headers.Length; i++)
            {
                ws.Cell(headerRow, i + 1).Value = headers[i];
                ws.Cell(headerRow, i + 1).Style.Font.SetBold();
                ws.Cell(headerRow, i + 1).Style.Fill.SetBackgroundColor(XLColor.FromHtml("#EEEEEE"));
                ws.Cell(headerRow, i + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            }

            var r = headerRow + 1;
            foreach (var item in rows)
            {
                ws.Cell(r, 1).Value = item.Id;
                ws.Cell(r, 2).Value = item.Grado;
                ws.Cell(r, 3).Value = item.Paralelo;
                r++;
            }

            ws.Range(headerRow, 1, Math.Max(r - 1, headerRow), headers.Length).SetAutoFilter();
            ws.Columns(1, headers.Length).AdjustToContents();
            ws.SheetView.FreezeRows(headerRow);

            // Gráfico: contar cursos por Grado (column)
            var series = rows.GroupBy(x => x.Grado).OrderBy(g => g.Key).ToDictionary(g => g.Key ?? "(sin grado)", g => (double)g.Count());
            using (var chartStream = ReporteChartHelper.RenderChartPng(ReporteChartHelper.ChartKind.ColumnVertical, series, 600, 300))
            {
                ws.AddPicture(chartStream).MoveTo(ws.Cell(r + 2, 1)).WithSize(600, 300);
            }

            using var stream = new MemoryStream();
            wb.SaveAs(stream);
            var content = stream.ToArray();

            var fileName = $"Cursos_Asignados_{profesorId}.xlsx";

            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpGet("profesores/tutores/cursos")]
        public async Task<IActionResult> ProfesoresTutoresConCupos()
        {
            var tutorsWithCourses = await _context.GradoParalelos
                .AsNoTracking()
                .Where(gp => gp.Profesor.IsTutor)
                .Include(gp => gp.Grado)
                .Include(gp => gp.Paralelo)
                .Include(gp => gp.Profesor)
                .OrderBy(gp => gp.Grado.Nombre)
                .ThenBy(gp => gp.Paralelo.Nombre)
                .Select(gp => new
                {
                    ProfesorNombre = gp.Profesor.Nombres + " " + gp.Profesor.Apellidos,
                    Grado = gp.Grado.Nombre,
                    Paralelo = gp.Paralelo.Nombre,
                    CuposTotales = gp.Cupos
                })
                .ToListAsync();

            if (!tutorsWithCourses.Any())
            {
                return NotFound("No se encontraron tutores con cursos asignados.");
            }

            using var wb = new XLWorkbook();
            var sheetName = ReporteChartHelper.GetSafeSheetName("Profesores Tutores y Cursos");
            var ws = wb.Worksheets.Add(sheetName);

            // Encabezado
            ws.Cell(1, 1).Value = "Unidad Educativa AMA";
            ws.Cell(2, 1).Value = "Reporte de Profesores Tutores y Cursos";
            ws.Cell(3, 1).Value = $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}";

            var headerRow = 5;
            var headers = new[] { "Profesor", "Grado", "Paralelo", "Cupos Totales" };
            for (int i = 0; i < headers.Length; i++)
            {
                ws.Cell(headerRow, i + 1).Value = headers[i];
                ws.Cell(headerRow, i + 1).Style.Font.SetBold();
                ws.Cell(headerRow, i + 1).Style.Fill.SetBackgroundColor(XLColor.FromHtml("#EEEEEE"));
                ws.Cell(headerRow, i + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            }

            var r = headerRow + 1;
            foreach (var item in tutorsWithCourses)
            {
                ws.Cell(r, 1).Value = item.ProfesorNombre;
                ws.Cell(r, 2).Value = item.Grado;
                ws.Cell(r, 3).Value = item.Paralelo;
                ws.Cell(r, 4).Value = item.CuposTotales;
                r++;
            }

            ws.Range(headerRow, 1, Math.Max(r - 1, headerRow), headers.Length).SetAutoFilter();
            ws.Columns(1, headers.Length).AdjustToContents();
            ws.SheetView.FreezeRows(headerRow);

            // Gráfico: cupos totales por Grado-Paralelo (bar horizontal)
            var series = tutorsWithCourses
                .GroupBy(x => $"{x.Grado}-{x.Paralelo}")
                .OrderBy(g => g.Key)
                .ToDictionary(g => g.Key, g => (double)g.Sum(i => i.CuposTotales));
            using (var chartStream = ReporteChartHelper.RenderChartPng(ReporteChartHelper.ChartKind.BarHorizontal, series, 800, 320))
            {
                ws.AddPicture(chartStream).MoveTo(ws.Cell(r + 2, 1)).WithSize(800, 320);
            }

            using var stream = new MemoryStream();
            wb.SaveAs(stream);
            var content = stream.ToArray();

            var fileName = "Profesores_Tutores_Cursos.xlsx";

            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpGet("profesores/tutores")]
        public async Task<IActionResult> ProfesoresTutores()
        {
            var tutors = await _context.Profesors
                .AsNoTracking()
                .Where(p => p.IsTutor)
                .Include(p => p.Materias)
                .OrderBy(p => p.Apellidos)
                .ThenBy(p => p.Nombres)
                .Select(p => new
                {
                    p.Id,
                    Profesor = p.Nombres + " " + p.Apellidos,
                    Materias = string.Join(", ", p.Materias.Select(m => m.Nombre))
                })
                .ToListAsync();

            using var wb = new XLWorkbook();
            var sheetName = ReporteChartHelper.GetSafeSheetName("Profesores Tutores");
            var ws = wb.Worksheets.Add(sheetName);

            ws.Cell(1, 1).Value = "Unidad Educativa AMA";
            ws.Cell(2, 1).Value = "Reporte de Profesores Tutores";
            ws.Cell(3, 1).Value = $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}";

            var headerRow = 5;
            var headers = new[] { "ID", "Profesor", "Materias Asignadas" };
            for (int i = 0; i < headers.Length; i++)
            {
                ws.Cell(headerRow, i + 1).Value = headers[i];
                ws.Cell(headerRow, i + 1).Style.Font.SetBold();
                ws.Cell(headerRow, i + 1).Style.Fill.SetBackgroundColor(XLColor.FromHtml("#EEEEEE"));
                ws.Cell(headerRow, i + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            }

            var r = headerRow + 1;
            foreach (var item in tutors)
            {
                ws.Cell(r, 1).Value = item.Id;
                ws.Cell(r, 2).Value = item.Profesor;
                ws.Cell(r, 3).Value = item.Materias;
                r++;
            }

            ws.Range(headerRow, 1, Math.Max(r - 1, headerRow), headers.Length).SetAutoFilter();
            ws.Columns(1, headers.Length).AdjustToContents();
            ws.SheetView.FreezeRows(headerRow);

            // Gráfico: número de materias por profesor (column)
            var series = tutors.ToDictionary(t => t.Profesor ?? "(sin nombre)", t => (double)(string.IsNullOrEmpty(t.Materias) ? 0 : t.Materias.Split(',').Length));
            using (var chartStream = ReporteChartHelper.RenderChartPng(ReporteChartHelper.ChartKind.ColumnVertical, series, 800, 320))
            {
                ws.AddPicture(chartStream).MoveTo(ws.Cell(r + 2, 1)).WithSize(800, 320);
            }

            using var stream = new MemoryStream();
            wb.SaveAs(stream);
            var content = stream.ToArray();

            var fileName = "Profesores_Tutores.xlsx";

            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpGet("materias/por-maestro/{profesorId:long}")]
        public async Task<IActionResult> MateriasPorMaestroPorId(long profesorId)
        {
            var rows = await _context.Materias
                .AsNoTracking()
                .Where(m => m.IdProfesor == profesorId)
                .Include(m => m.Grado)
                .Include(m => m.Profesor)
                .OrderBy(m => m.Grado.Nombre)
                .Select(m => new
                {
                    m.Id,
                    Materia = m.Nombre,
                    Profesor = m.Profesor.Nombres + " " + m.Profesor.Apellidos,
                    Grado = m.Grado.Nombre
                })
                .ToListAsync();

            if (rows.Count == 0)
                return NotFound("No se encontraron materias asignadas a este profesor.");

            using var wb = new XLWorkbook();
            var sheetName = ReporteChartHelper.GetSafeSheetName($"Materias Asignadas - Profesor {profesorId}");
            var ws = wb.Worksheets.Add(sheetName);

            ws.Cell(1, 1).Value = "Unidad Educativa AMA";
            ws.Cell(2, 1).Value = $"Reporte de Materias Asignadas a Profesor {profesorId}";
            ws.Cell(3, 1).Value = $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}";

            var headerRow = 5;
            var headers = new[] { "ID", "Materia", "Profesor", "Grado" };
            for (int i = 0; i < headers.Length; i++)
            {
                ws.Cell(headerRow, i + 1).Value = headers[i];
                ws.Cell(headerRow, i + 1).Style.Font.SetBold();
                ws.Cell(headerRow, i + 1).Style.Fill.SetBackgroundColor(XLColor.FromHtml("#EEEEEE"));
                ws.Cell(headerRow, i + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            }

            var r = headerRow + 1;
            foreach (var item in rows)
            {
                ws.Cell(r, 1).Value = item.Id;
                ws.Cell(r, 2).Value = item.Materia;
                ws.Cell(r, 3).Value = item.Profesor;
                ws.Cell(r, 4).Value = item.Grado;
                r++;
            }

            ws.Range(headerRow, 1, Math.Max(r - 1, headerRow), headers.Length).SetAutoFilter();
            ws.Columns(1, headers.Length).AdjustToContents();
            ws.SheetView.FreezeRows(headerRow);

            // Gráfico: conteo por Grado (barra)
            var series = rows.GroupBy(x => x.Grado).OrderBy(g => g.Key).ToDictionary(g => g.Key ?? "(sin grado)", g => (double)g.Count());
            using (var chartStream = ReporteChartHelper.RenderChartPng(ReporteChartHelper.ChartKind.BarHorizontal, series, 600, 300))
            {
                ws.AddPicture(chartStream).MoveTo(ws.Cell(r + 2, 1)).WithSize(600, 300);
            }

            using var stream = new MemoryStream();
            wb.SaveAs(stream);
            var content = stream.ToArray();

            var fileName = $"Materias_Asignadas_{profesorId}.xlsx";

            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        // GET: /api/Reporte/estudiantes/sin-matricula/excel?periodo=2025-2026
        [HttpGet("estudiantes/sin-matricula/excel")]
        public async Task<IActionResult> EstudiantesSinMatriculaExcel([FromQuery] string periodo)
        {
            if (string.IsNullOrWhiteSpace(periodo))
                return BadRequest("El período es obligatorio.");

            // 1) Estudiantes activos
            var estudiantes = _context.Estudiantes
                .AsNoTracking()
                .Where(e => e.IsActive == true) // ajusta si tu modelo usa otro campo
                .Include(e => e.Representante)
                .AsQueryable();

            // 2) Matriculas activas del periodo
            var matriculasPeriodo = _context.Matriculas
                .AsNoTracking()
                .Where(m => m.IsActive == true && m.GradoParalelo.AnioLectivo.Periodo == periodo)
                .Select(m => new { m.EstudianteId }) // ajusta propiedad FK si se llama distinto
                .Distinct();

            // 3) Anti-join: estudiantes que NO están matriculados en ese periodo
            // 3) Anti-join: estudiantes que NO están matriculados en ese periodo
            var sinMatricula = await estudiantes
                .Where(e => !matriculasPeriodo.Any(mp => mp.EstudianteId == e.Id))
                .OrderBy(e => e.Apellido).ThenBy(e => e.Nombre)
                .Select(e => new
                {
                    e.Id,
                    e.Nombre,
                    e.Apellido,
                    Cedula = e.Cedula ?? "",
                    Representante = e.Representante != null ? (e.Representante.Nombres + " " + e.Representante.Apellidos) : "",
                    DocRepresentante = e.Representante != null ? (e.Representante.NumeroDocumento ?? "") : "",
                    Telefono = e.Representante != null ? (e.Representante.Telefono ?? "") : "" // si tienes
                })
                .ToListAsync();

            var totalEstudiantes = sinMatricula.Count;
            var estudiantesConDocumentosFaltantes = sinMatricula.Count(x => string.IsNullOrWhiteSpace(x.Cedula));

            // 4) Excel
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add($"Sin matrícula {periodo}");

            ws.Cell(1, 1).Value = "Unidad Educativa AMA";
            ws.Cell(2, 1).Value = "Estudiantes SIN matrícula";
            ws.Cell(3, 1).Value = $"Período: {periodo}";
            ws.Cell(4, 1).Value = $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}";
            ws.Cell(5, 1).Value = $"Total: {sinMatricula.Count}";

            ws.Range(1, 1, 1, 7).Merge().Style.Font.SetBold().Font.SetFontSize(14);
            ws.Range(2, 1, 2, 7).Merge().Style.Font.SetBold().Font.SetFontSize(12);
            ws.Range(3, 1, 5, 7).Merge().Style.Font.SetFontSize(10).Font.SetFontColor(XLColor.Gray);

            var headerRow = 7;
            var headers = new[]
            {
            "ID", "Nombres", "Apellidos", "Cédula",
            "Representante", "Doc. Representante", "Teléfono"
        };

            for (int i = 0; i < headers.Length; i++)
            {
                ws.Cell(headerRow, i + 1).Value = headers[i];
                ws.Cell(headerRow, i + 1).Style.Font.SetBold();
                ws.Cell(headerRow, i + 1).Style.Fill.SetBackgroundColor(XLColor.FromHtml("#EEEEEE"));
                ws.Cell(headerRow, i + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            }

            var r = headerRow + 1;
            foreach (var x in sinMatricula)
            {
                ws.Cell(r, 1).Value = x.Id;
                ws.Cell(r, 2).Value = x.Nombre;
                ws.Cell(r, 3).Value = x.Apellido;
                ws.Cell(r, 4).Value = x.Cedula;
                ws.Cell(r, 5).Value = x.Representante;
                ws.Cell(r, 6).Value = x.DocRepresentante;
                ws.Cell(r, 7).Value = x.Telefono;
                r++;
            }

            ws.Range(headerRow, 1, Math.Max(r - 1, headerRow), headers.Length).SetAutoFilter();
            ws.Columns(1, headers.Length).AdjustToContents();
            ws.SheetView.FreezeRows(headerRow);

            // Gráfico: distribución de documentos faltantes (pastel)
            var faltan = estudiantesConDocumentosFaltantes;
            var completos = totalEstudiantes - faltan;
            var series = new Dictionary<string, double>
            {
                { "Faltan", faltan },
                { "Completos", completos }
            };
            using (var chartStream = ReporteChartHelper.RenderChartPng(ReporteChartHelper.ChartKind.Pie, series, 400, 300))
            {
                ws.AddPicture(chartStream)
                    .MoveTo(ws.Cell(r + 2, 1))
                    .WithSize(400, 300);
            }

            using var stream = new MemoryStream();
            wb.SaveAs(stream);
            var content = stream.ToArray();

            var safePeriodo = periodo.Replace("/", "-").Replace(" ", "");
            var fileName = $"Estudiantes_SinMatricula_{safePeriodo}.xlsx";

            return File(content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }

        // GET: /api/Reporte/estudiantes/por-curso/excel?periodo=2025-2026
        [HttpGet("estudiantes/por-curso/excel")]
        public async Task<IActionResult> EstudiantesPorCursoExcel([FromQuery] string periodo)
        {
            if (string.IsNullOrWhiteSpace(periodo))
                return BadRequest("El período es obligatorio.");

            // 1) Traer matrículas del período (activos)
            var data = await _context.Matriculas
                .AsNoTracking()
                .Where(m => m.IsActive == true &&
                            m.GradoParalelo.AnioLectivo.Periodo == periodo)
                .Include(m => m.Estudiante)
                .Include(m => m.GradoParalelo).ThenInclude(gp => gp.Grado)
                .Include(m => m.GradoParalelo).ThenInclude(gp => gp.Paralelo)
                .Include(m => m.GradoParalelo).ThenInclude(gp => gp.AnioLectivo)
                .OrderBy(m => m.GradoParalelo.Grado.Nombre)
                .ThenBy(m => m.GradoParalelo.Paralelo.Nombre)
                .ThenBy(m => m.Estudiante.Apellido)
                .ThenBy(m => m.Estudiante.Nombre)
                .Select(m => new
                {
                    MatriculaId = m.Id,
                    EstudianteId = m.Estudiante.Id,
                    Nombres = m.Estudiante.Nombre,
                    Apellidos = m.Estudiante.Apellido,
                    Cedula = m.Estudiante.Cedula ?? "",
                    Estado = m.EstadoMatricula,
                    Fecha = m.FechaMatricula,

                    GradoParaleloId = m.GradoParaleloId,
                    Grado = m.GradoParalelo.Grado.Nombre,
                    Paralelo = m.GradoParalelo.Paralelo.Nombre,
                    Periodo = m.GradoParalelo.AnioLectivo.Periodo
                })
                .ToListAsync();

            if (data.Count == 0)                    
                return NotFound("No hay matrículas para ese período.");

            // 2) Agrupar por curso (Grado + Paralelo)
            var grupos = data
                .GroupBy(x => new { x.GradoParaleloId, x.Grado, x.Paralelo })
                .OrderBy(g => g.Key.Grado)
                .ThenBy(g => g.Key.Paralelo)
                .ToList();

            using var wb = new XLWorkbook();

            // =========================
            // Hoja RESUMEN
            // =========================
            var wsResumen = wb.Worksheets.Add("Resumen");

            wsResumen.Cell(1, 1).Value = "Unidad Educativa AMA";
            wsResumen.Cell(2, 1).Value = "Estudiantes por curso (Resumen)";
            wsResumen.Cell(3, 1).Value = $"Período: {periodo}";
            wsResumen.Cell(4, 1).Value = $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}";

            wsResumen.Range(1, 1, 1, 4).Merge().Style.Font.SetBold().Font.SetFontSize(14);
            wsResumen.Range(2, 1, 2, 4).Merge().Style.Font.SetBold().Font.SetFontSize(12);
            wsResumen.Range(3, 1, 4, 4).Merge().Style.Font.SetFontSize(10).Font.SetFontColor(XLColor.Gray);

            var hr = 6;
            wsResumen.Cell(hr, 1).Value = "Grado";
            wsResumen.Cell(hr, 2).Value = "Paralelo";
            wsResumen.Cell(hr, 3).Value = "GradoParaleloId";
            wsResumen.Cell(hr, 4).Value = "Total";

            wsResumen.Range(hr, 1, hr, 4).Style.Font.SetBold();
            wsResumen.Range(hr, 1, hr, 4).Style.Fill.SetBackgroundColor(XLColor.FromHtml("#EEEEEE"));

            var rr = hr + 1;
            foreach (var g in grupos)
            {
                wsResumen.Cell(rr, 1).Value = g.Key.Grado;
                wsResumen.Cell(rr, 2).Value = g.Key.Paralelo;
                wsResumen.Cell(rr, 3).Value = g.Key.GradoParaleloId;
                wsResumen.Cell(rr, 4).Value = g.Count();
                rr++;
            }

            wsResumen.Range(hr, 1, Math.Max(rr - 1, hr), 4).SetAutoFilter();
            wsResumen.Columns(1, 4).AdjustToContents();
            wsResumen.SheetView.FreezeRows(hr);

            // =========================
            // 1 hoja por CURSO
            // =========================
            foreach (var g in grupos)
            {
                // nombre hoja <= 31 chars
                var sheetName = $"{g.Key.Grado}-{g.Key.Paralelo}".Trim();
                if (sheetName.Length > 31) sheetName = sheetName.Substring(0, 31);

                var ws = wb.Worksheets.Add(sheetName);

                ws.Cell(1, 1).Value = "Unidad Educativa AMA";
                ws.Cell(2, 1).Value = $"Lista: {g.Key.Grado} - {g.Key.Paralelo}";
                ws.Cell(3, 1).Value = $"Período: {periodo}";
                ws.Cell(4, 1).Value = $"Total: {g.Count()}";
                ws.Cell(5, 1).Value = $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}";

                ws.Range(1, 1, 1, 5).Merge().Style.Font.SetBold().Font.SetFontSize(14);
                ws.Range(2, 1, 2, 5).Merge().Style.Font.SetBold().Font.SetFontSize(12);
                ws.Range(3, 1, 5, 5).Merge().Style.Font.SetFontSize(10).Font.SetFontColor(XLColor.Gray);

                var headerRow = 7;
                var headers = new[] { "#", "Estudiante", "Cédula", "Estado", "Fecha" };


                for (int i = 0; i < headers.Length; i++)
                {
                    ws.Cell(headerRow, i + 1).Value = headers[i];
                    ws.Cell(headerRow, i + 1).Style.Font.SetBold();
                    ws.Cell(headerRow, i + 1).Style.Fill.SetBackgroundColor(XLColor.FromHtml("#EEEEEE"));
                    ws.Cell(headerRow, i + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                }

                int row = headerRow + 1;
                int n = 1;

                foreach (var x in g)
                {
                    ws.Cell(row, 1).Value = n++;
                    ws.Cell(row, 2).Value = $"{x.Apellidos} {x.Nombres}";
                    ws.Cell(row, 3).Value = x.Cedula;
                    ws.Cell(row, 4).Value = x.Estado;
                    ws.Cell(row, 5).Value = x.Fecha;
                    ws.Cell(row, 5).Style.DateFormat.Format = "dd/MM/yyyy HH:mm";
                    row++;
                }

                ws.Range(headerRow, 1, Math.Max(row - 1, headerRow), headers.Length).SetAutoFilter();
                ws.Columns(1, headers.Length).AdjustToContents();
                ws.SheetView.FreezeRows(headerRow);
            }

            using var stream = new MemoryStream();
            wb.SaveAs(stream);

            var safePeriodo = periodo.Replace("/", "-").Replace(" ", "");
            var fileName = $"Estudiantes_PorCurso_{safePeriodo}.xlsx";

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName
            );
        }

        // GET: /api/reporte/estudiantes/documentos-faltantes
        [HttpGet("estudiantes/documentos-faltantes")]
        public async Task<IActionResult> EstudiantesDocumentosFaltantes()
        {
            var estudiantesConDocumentos = await _context.Estudiantes
                .AsNoTracking()
                .Where(e => e.Documentos.Any(d => d.Aprobado == null)) // Solo aquellos con documentos faltantes
                .Include(e => e.Documentos)
                .OrderBy(e => e.Apellido)
                .ThenBy(e => e.Nombre)
                .Select(e => new
                {
                    EstudianteId = e.Id,
                    Nombre = e.Nombre + " " + e.Apellido,
                    DocumentosFaltantes = e.Documentos
                        .Where(d => d.Aprobado == null) // Documentos que no han sido aprobados
                        .Select(d => d.Nombre)
                        .ToList()
                })
                .ToListAsync();

            var totalEstudiantes = estudiantesConDocumentos.Count();
            var estudiantesConDocumentosFaltantes = estudiantesConDocumentos.Count(e => e.DocumentosFaltantes.Any());
            var porcentajeFaltantes = totalEstudiantes > 0 ? (double)estudiantesConDocumentosFaltantes / totalEstudiantes * 100 : 0;

            // ====== Generar imagen del gráfico con SkiaSharp (cross-platform) ======
            // Nota: requiere paquete NuGet `SkiaSharp`.
            var faltan = estudiantesConDocumentosFaltantes;
            var completos = totalEstudiantes - faltan;
            var width = 500;
            var height = 300;

            using var chartStream = new MemoryStream();
            {
                var info = new SkiaSharp.SKImageInfo(width, height);
                using var surface = SkiaSharp.SKSurface.Create(info);
                var canvas = surface.Canvas;

                // Fondo
                canvas.Clear(SkiaSharp.SKColors.White);

                // Pie rectangle
                var pieRect = new SkiaSharp.SKRect(10, 10, 210, 210);

                var total = (float)(faltan + completos);
                float startAngle = -90f;
                float sweepFaltan = total > 0 ? (float)faltan / total * 360f : 0f;
                float sweepCompletos = 360f - sweepFaltan;

                using (var paint = new SkiaSharp.SKPaint { IsAntialias = true })
                {
                    // Slice "Faltan"
                    paint.Style = SkiaSharp.SKPaintStyle.Fill;
                    paint.Color = new SkiaSharp.SKColor(0xFF, 0x45, 0x00); // OrangeRed
                    if (sweepFaltan > 0.001f)
                    {
                        canvas.DrawArc(pieRect, startAngle, sweepFaltan, true, paint);
                    }

                    // Slice "Completos"
                    paint.Color = SkiaSharp.SKColors.LightGreen;
                    if (sweepCompletos > 0.001f)
                    {
                        canvas.DrawArc(pieRect, startAngle + sweepFaltan, sweepCompletos, true, paint);
                    }

                    // Borde del pie
                    paint.Style = SkiaSharp.SKPaintStyle.Stroke;
                    paint.Color = SkiaSharp.SKColors.Gray;
                    paint.StrokeWidth = 1;
                    canvas.DrawOval(pieRect, paint);
                }

                // Leyenda
                using (var textPaint = new SkiaSharp.SKPaint
                {
                    IsAntialias = true,
                    Color = SkiaSharp.SKColors.Black,
                    TextSize = 12
                })
                using (var legendPaint = new SkiaSharp.SKPaint { IsAntialias = true })
                {
                    var legendX = pieRect.Right + 20;
                    var legendY = pieRect.Top;

                    // Faltan
                    legendPaint.Color = new SkiaSharp.SKColor(0xFF, 0x45, 0x00);
                    canvas.DrawRect(new SkiaSharp.SKRect(legendX, legendY, legendX + 15, legendY + 15), legendPaint);
                    var pctFaltan = total > 0 ? (faltan / total * 100.0) : 0.0;
                    canvas.DrawText($"Faltan: {faltan} ({pctFaltan:0.##}%)", legendX + 22, legendY + 12, textPaint);

                    // Completos
                    legendY += 25;
                    legendPaint.Color = SkiaSharp.SKColors.LightGreen;
                    canvas.DrawRect(new SkiaSharp.SKRect(legendX, legendY, legendX + 15, legendY + 15), legendPaint);
                    var pctCompletos = total > 0 ? (completos / total * 100.0) : 0.0;
                    canvas.DrawText($"Completos: {completos} ({pctCompletos:0.##}%)", legendX + 22, legendY + 12, textPaint);
                }

                // Título del gráfico
                using (var titlePaint = new SkiaSharp.SKPaint { IsAntialias = true, Color = SkiaSharp.SKColors.Black, TextSize = 14, FakeBoldText = true })
                {
                    canvas.DrawText("Documentos faltantes", 10, 245, titlePaint);
                }

                using var image = surface.Snapshot();
                using var data = image.Encode(SkiaSharp.SKEncodedImageFormat.Png, 100);
                data.SaveTo(chartStream);
                chartStream.Position = 0;
            }

            // ====== Crear el archivo Excel e insertar la imagen ======
            using var wb = new XLWorkbook();
            var sheetName = GetSafeSheetName("Estudiantes con Documentos Faltantes");
            var ws = wb.Worksheets.Add(sheetName);

            ws.Cell(1, 1).Value = "Unidad Educativa AMA";
            ws.Cell(2, 1).Value = "Reporte de Estudiantes con Documentación Faltante";
            ws.Cell(3, 1).Value = $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}";

            // Resumen breve
            ws.Cell(5, 1).Value = "Total estudiantes";
            ws.Cell(5, 2).Value = totalEstudiantes;
            ws.Cell(6, 1).Value = "Con documentos faltantes";
            ws.Cell(6, 2).Value = estudiantesConDocumentosFaltantes;
            ws.Cell(7, 1).Value = "Porcentaje con faltantes";
            ws.Cell(7, 2).Value = $"{porcentajeFaltantes:0.##}%";

            // Lista de estudiantes con documentos faltantes
            var headerRow = 9;
            var headers = new[] { "Estudiante", "Documentos Faltantes" };
            for (int i = 0; i < headers.Length; i++)
            {
                ws.Cell(headerRow, i + 1).Value = headers[i];
                ws.Cell(headerRow, i + 1).Style.Font.SetBold();
                ws.Cell(headerRow, i + 1).Style.Fill.SetBackgroundColor(XLColor.FromHtml("#EEEEEE"));
                ws.Cell(headerRow, i + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            }

            var r = headerRow + 1;
            foreach (var item in estudiantesConDocumentos)
            {
                ws.Cell(r, 1).Value = item.Nombre;
                ws.Cell(r, 2).Value = string.Join("\n", item.DocumentosFaltantes); // Documentos faltantes, cada uno en una nueva línea
                r++;
            }

            // Insertar la imagen generada con SkiaSharp debajo de la tabla
            ws.AddPicture(chartStream)
                .MoveTo(ws.Cell(r + 1, 1))
                .WithSize(width, height);

            ws.Range(headerRow, 1, Math.Max(r - 1, headerRow), headers.Length).SetAutoFilter();
            ws.Columns(1, headers.Length).AdjustToContents();
            ws.SheetView.FreezeRows(headerRow);

            using var stream = new MemoryStream();
            wb.SaveAs(stream);
            var content = stream.ToArray();

            var fileName = "Estudiantes_Documentos_Faltantes.xlsx";

            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }


        // GET: /api/Reporte/cupos/excel?anioLectivoId=1&soloDisponibles=true
        [HttpGet("cupos/excel")]
        public async Task<IActionResult> CuposPorCursoExcel(
            [FromQuery] long anioLectivoId,
            [FromQuery] bool soloDisponibles = false
        )
        {
            if (anioLectivoId <= 0)
                return BadRequest("anioLectivoId es obligatorio.");

            // (Opcional) traer el periodo para mostrarlo en el header
            // Si Periodo puede ser null en BD, lo protegemos con ?? "—"
            var anioLectivo = await _context.AnioLectivo
                .AsNoTracking()
                .Where(a => a.Id == anioLectivoId)
                .Select(a => new { a.Id, Periodo = (a.Periodo ?? "—") })
                .FirstOrDefaultAsync();

            if (anioLectivo == null)
                return NotFound("Año lectivo no encontrado.");

            // ✅ Tu query, tal cual (con Includes para nombres)
            var rows = await _context.GradoParalelos
                .AsNoTracking()
                .Where(x => x.AnioLectivoId == anioLectivoId)
                .Include(x => x.Grado)
                .Include(x => x.Paralelo)
                .OrderBy(x => x.Grado.Nombre)
                .ThenBy(x => x.Paralelo.Nombre)
                .Select(x => new
                {
                    GradoParaleloId = x.Id,
                    Grado = x.Grado.Nombre,
                    Paralelo = x.Paralelo.Nombre,
                    CuposTotales = x.Cupos,

                    // ⚠️ Si Matricula.GradoParaleloId es nullable (long?),
                    // cambia a: m.GradoParaleloId.HasValue && m.GradoParaleloId.Value == x.Id
                    CuposOcupados = _context.Matriculas.Count(m =>
                        m.IsActive == true &&
                        m.GradoParaleloId == x.Id
                    )
                })
                .ToListAsync();

            // calcular disponibles y %
            var result = rows.Select(x =>
            {
                var disp = x.CuposTotales - x.CuposOcupados;
                if (disp < 0) disp = 0;

                var pct = x.CuposTotales > 0 ? (double)x.CuposOcupados / x.CuposTotales : 0;

                return new
                {
                    x.GradoParaleloId,
                    x.Grado,
                    x.Paralelo,
                    x.CuposTotales,
                    x.CuposOcupados,
                    CuposDisponibles = disp,
                    PorcentajeOcupacion = pct
                };
            }).ToList();

            if (soloDisponibles)
                result = result.Where(r => r.CuposDisponibles > 0).ToList();

            // =========================
            // Excel
            // =========================
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Cupos");

            ws.Cell(1, 1).Value = "Unidad Educativa AMA";
            ws.Cell(2, 1).Value = "Reporte de cupos por grado y paralelo";
            ws.Cell(3, 1).Value = $"Año lectivo: {anioLectivo.Periodo} (Id: {anioLectivoId})";
            ws.Cell(4, 1).Value = $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}";
            ws.Cell(5, 1).Value = $"Solo disponibles: {(soloDisponibles ? "Sí" : "No")}";

            ws.Range(1, 1, 1, 7).Merge().Style.Font.SetBold().Font.SetFontSize(14);
            ws.Range(2, 1, 2, 7).Merge().Style.Font.SetBold().Font.SetFontSize(12);
            ws.Range(3, 1, 5, 7).Merge().Style.Font.SetFontSize(10).Font.SetFontColor(XLColor.Gray);

            var headerRow = 7;
            var headers = new[]
            {
                "GradoParaleloId", "Grado", "Paralelo",
                "Cupos Totales", "Ocupados", "Disponibles", "% Ocupación"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                ws.Cell(headerRow, i + 1).Value = headers[i];
                ws.Cell(headerRow, i + 1).Style.Font.SetBold();
                ws.Cell(headerRow, i + 1).Style.Fill.SetBackgroundColor(XLColor.FromHtml("#EEEEEE"));
                ws.Cell(headerRow, i + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            }

            var r = headerRow + 1;
            foreach (var x in result)
            {
                ws.Cell(r, 1).Value = x.GradoParaleloId;
                ws.Cell(r, 2).Value = x.Grado;
                ws.Cell(r, 3).Value = x.Paralelo;
                ws.Cell(r, 4).Value = x.CuposTotales;
                ws.Cell(r, 5).Value = x.CuposOcupados;
                ws.Cell(r, 6).Value = x.CuposDisponibles;

                ws.Cell(r, 7).Value = x.PorcentajeOcupacion;
                ws.Cell(r, 7).Style.NumberFormat.Format = "0%";

                r++;
            }

            ws.Range(headerRow, 1, Math.Max(r - 1, headerRow), headers.Length).SetAutoFilter();
            ws.Columns(1, headers.Length).AdjustToContents();
            ws.SheetView.FreezeRows(headerRow);

            using var stream = new MemoryStream();
            wb.SaveAs(stream);

            var fileName = $"Cupos_{anioLectivoId}_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName
            );
        }

        private static string GetSafeSheetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "Sheet1";

            // Quitar caracteres inválidos para nombres de hoja de Excel: : \ / ? * [ ]
            var invalidChars = new[] { ':', '\\', '/', '?', '*', '[', ']' };
            var sb = new System.Text.StringBuilder(name.Length);
            foreach (var ch in name)
            {
                if (Array.IndexOf(invalidChars, ch) >= 0)
                    sb.Append('_');
                else
                    sb.Append(ch);
            }

            var cleaned = sb.ToString().Trim();

            // Limitar longitud a 31 caracteres (requisito de Excel)
            if (cleaned.Length == 0)
                return "Sheet1";

            if (cleaned.Length > 31)
                cleaned = cleaned.Substring(0, 31).Trim();

            // Si queda vacío después del recorte, usar un nombre por defecto
            if (string.IsNullOrEmpty(cleaned))
                return "Sheet1";

            return cleaned;
        }
    }
}
