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


      
        // GET: /api/Reporte/estudiantes/listado/excel
        [HttpGet("estudiantes/listado/excel")]
        public async Task<IActionResult> ListadoEstudiantesExcel()
        {
            // SOLO ACTIVOS (sin filtros)
            var rows = await _context.Estudiantes
                .AsNoTracking()
                .Include(x => x.Representante)
                .Include(x => x.Matriculas)
                .Where(x => x.IsActive == true)
                .OrderBy(x => x.Apellido)
                .ThenBy(x => x.Nombre)
                .Select(x => new
                {
                    Nombres = (x.Nombre ?? "").Trim(),
                    Apellidos = (x.Apellido ?? "").Trim(),
                    Cedula = x.Cedula ?? "",
                    FechaNacimiento = x.FechaNacimiento, // ajusta el nombre real de tu campo si es distinto
                    Genero = x.Genero,                   // 0 masculino, 1 femenino (según tú)
                    Telefono = x.Telefono ?? "",
                    Email = x.Correo ?? "",
                    Direccion = x.Direccion ?? "",
                    Representante = x.Representante != null ? ((x.Representante.Nombres + " " + x.Representante.Apellidos).Trim()) : "",
                    DocumentoRepresentante = x.Representante != null ? (x.Representante.NumeroDocumento ?? "") : "",
                    TieneMatricula = x.Matriculas.Any(m => m.IsActive == true)
                })
                .ToListAsync();

            // Transformación “bonita” (edad, género legible)
            var data = rows.Select(x => new
            {
                x.Nombres,
                x.Apellidos,
                x.Cedula,
                Edad = CalcularEdad(x.FechaNacimiento),
                FechaNacimiento = x.FechaNacimiento.ToString("dd/MM/yyyy"),
                Genero = x.Genero == 0 ? "Masculino" : "Femenino",
                x.Telefono,
                x.Email,
                x.Direccion,
                x.Representante,
                DocRepresentante = x.DocumentoRepresentante,
                MatriculaActiva = x.TieneMatricula ? "Sí" : "No"
            }).ToList();

            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add(ReporteChartHelper.GetSafeSheetName("Estudiantes"));

            // ===================== ENCABEZADO =====================
            ws.Cell(1, 1).Value = "Unidad Educativa Ana Maria Iza";
            ws.Cell(2, 1).Value = "Listado general de estudiantes (Activos)";
            ws.Cell(3, 1).Value = $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}";

            // Merge a lo ancho de la tabla (12 columnas)
            ws.Range(1, 1, 1, 12).Merge();
            ws.Range(2, 1, 2, 12).Merge();
            ws.Range(3, 1, 3, 12).Merge();

            ws.Cell(1, 1).Style.Font.SetBold().Font.SetFontSize(16);
            ws.Cell(2, 1).Style.Font.SetBold().Font.SetFontSize(12);
            ws.Cell(3, 1).Style.Font.SetFontSize(10).Font.SetFontColor(XLColor.Gray);

            ws.Row(1).Height = 22;
            ws.Row(2).Height = 18;

            // Línea separadora
            ws.Range(4, 1, 4, 12).Merge();
            ws.Cell(4, 1).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            ws.Cell(4, 1).Style.Border.BottomBorderColor = XLColor.LightGray;

            // ===================== TABLA =====================
            var headerRow = 6;

            var headers = new[]
            {
        "Nombres", "Apellidos", "Cédula",
        "Edad", "Fec. Nacimiento", "Género",
        "Teléfono", "Email", "Dirección",
        "Representante", "Doc. Representante",
        "Matrícula Activa"
    };

            for (int i = 0; i < headers.Length; i++)
            {
                var cell = ws.Cell(headerRow, i + 1);
                cell.Value = headers[i];
                cell.Style.Font.SetBold();
                cell.Style.Font.SetFontColor(XLColor.White);
                cell.Style.Fill.SetBackgroundColor(XLColor.FromHtml("#1E88E5")); // azul
                cell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                cell.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                cell.Style.Border.OutsideBorderColor = XLColor.FromHtml("#D0D0D0");
            }

            ws.Row(headerRow).Height = 18;

            var r = headerRow + 1;
            foreach (var x in data)
            {
                ws.Cell(r, 1).Value = x.Nombres;
                ws.Cell(r, 2).Value = x.Apellidos;
                ws.Cell(r, 3).Value = x.Cedula;

                ws.Cell(r, 4).Value = x.Edad;
                ws.Cell(r, 5).Value = x.FechaNacimiento;
                ws.Cell(r, 6).Value = x.Genero;

                ws.Cell(r, 7).Value = x.Telefono;
                ws.Cell(r, 8).Value = x.Email;
                ws.Cell(r, 9).Value = x.Direccion;

                ws.Cell(r, 10).Value = x.Representante;
                ws.Cell(r, 11).Value = x.DocRepresentante;
                ws.Cell(r, 12).Value = x.MatriculaActiva;

                // Estilo por fila (zebra + bordes suaves)
                var rowRange = ws.Range(r, 1, r, 12);
                rowRange.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                rowRange.Style.Border.BottomBorderColor = XLColor.FromHtml("#E6E6E6");
                rowRange.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                if ((r - (headerRow + 1)) % 2 == 1)
                    rowRange.Style.Fill.SetBackgroundColor(XLColor.FromHtml("#FAFAFA"));

                r++;
            }

            // ===================== GRÁFICOS (PNG) =====================

            var chartsStartRow = r + 2;

            // Título general
            ws.Cell(chartsStartRow, 1).Value = "Análisis Estadístico de Estudiantes Activos";
            ws.Range(chartsStartRow, 1, chartsStartRow, 12).Merge();
            ws.Cell(chartsStartRow, 1).Style.Font.SetBold().Font.SetFontSize(13);
            ws.Cell(chartsStartRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            chartsStartRow += 2;

            // ===================== 1) GÉNERO =====================

            var statsGenero = data
                .GroupBy(x => string.IsNullOrWhiteSpace(x.Genero) ? "(Sin dato)" : x.Genero)
                .OrderBy(g => g.Key)
                .ToDictionary(g => g.Key, g => (double)g.Count());

            // Título gráfico
            ws.Cell(chartsStartRow, 1).Value = "Distribución por Género";
            ws.Range(chartsStartRow, 1, chartsStartRow, 6).Merge();
            ws.Cell(chartsStartRow, 1).Style.Font.SetBold();
            ws.Cell(chartsStartRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            using (var chartGenero = ReporteChartHelper.RenderChartPng(
                       ReporteChartHelper.ChartKind.Pie, statsGenero, 460, 280))
            {
                ws.AddPicture(chartGenero)
                  .MoveTo(ws.Cell(chartsStartRow + 1, 1))
                  .WithSize(460, 280);
            }

            // ===================== 2) MATRÍCULA =====================

            var statsMatricula = data
                .GroupBy(x => x.MatriculaActiva)
                .OrderBy(g => g.Key)
                .ToDictionary(g => g.Key, g => (double)g.Count());

            // Título gráfico
            ws.Cell(chartsStartRow, 7).Value = "Estudiantes con Matrícula Activa";
            ws.Range(chartsStartRow, 7, chartsStartRow, 12).Merge();
            ws.Cell(chartsStartRow, 7).Style.Font.SetBold();
            ws.Cell(chartsStartRow, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            using (var chartMat = ReporteChartHelper.RenderChartPng(
                       ReporteChartHelper.ChartKind.Pie, statsMatricula, 460, 280))
            {
                ws.AddPicture(chartMat)
                  .MoveTo(ws.Cell(chartsStartRow + 1, 7))
                  .WithSize(460, 280);
            }

            // ===================== 3) EDADES =====================

            chartsStartRow += 18;

            string AgeBucket(int edad)
            {
                if (edad <= 5) return "0–5";
                if (edad <= 10) return "6–10";
                if (edad <= 15) return "11–15";
                return "16+";
            }

            var statsEdad = data
                .GroupBy(x => AgeBucket(x.Edad))
                .OrderBy(g => g.Key)
                .ToDictionary(g => g.Key, g => (double)g.Count());

            // Título gráfico
            ws.Cell(chartsStartRow, 1).Value = "Distribución por Rangos de Edad";
            ws.Range(chartsStartRow, 1, chartsStartRow, 12).Merge();
            ws.Cell(chartsStartRow, 1).Style.Font.SetBold();
            ws.Cell(chartsStartRow, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            using (var chartEdad = ReporteChartHelper.RenderChartPng(
                       ReporteChartHelper.ChartKind.ColumnVertical, statsEdad, 960, 320))
            {
                ws.AddPicture(chartEdad)
                  .MoveTo(ws.Cell(chartsStartRow + 1, 1))
                  .WithSize(960, 320);
            }

            // Ajustes de columnas
            ws.Columns().AdjustToContents();

            // Anchos mínimos (para que “se vea bonito”)
            ws.Column(1).Width = Math.Max(ws.Column(1).Width, 16);
            ws.Column(2).Width = Math.Max(ws.Column(2).Width, 18);
            ws.Column(8).Width = Math.Max(ws.Column(8).Width, 22);
            ws.Column(9).Width = Math.Max(ws.Column(9).Width, 22);
            ws.Column(10).Width = Math.Max(ws.Column(10).Width, 22);

            // Alineaciones recomendadas
            ws.Column(3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
            ws.Column(4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            ws.Column(5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            ws.Column(6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            ws.Column(12).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            // Freeze header
            ws.SheetView.FreezeRows(headerRow);

            // ❌ NO AUTOFILTER (como pediste)
            // ws.Range(headerRow, 1, Math.Max(r - 1, headerRow), headers.Length).SetAutoFilter();

            // Opcional: “tabla” formal de Excel (se ve pro)
            var tableRange = ws.Range(headerRow, 1, Math.Max(r - 1, headerRow), 12);
            tableRange.CreateTable();

            // Export
            using var stream = new MemoryStream();
            wb.SaveAs(stream);
            var content = stream.ToArray();

            var fileName = $"Listado_Estudiantes_Activos_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";
            return File(content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }

        // Edad exacta (bien hecha)
        private static int CalcularEdad(DateTime fechaNacimiento)
        {
            var today = DateTime.Today;
            var age = today.Year - fechaNacimiento.Year;
            if (fechaNacimiento.Date > today.AddYears(-age)) age--;
            return age < 0 ? 0 : age;
        }

        //[HttpGet("materias/por-maestro")]
        //public async Task<IActionResult> MateriasPorMaestro([FromQuery] long? profesorId)
        //{
        //    var query = _context.Materias
        //        .AsNoTracking()
        //        .Include(m => m.Grado)
        //        .Include(m => m.Profesor)
        //        .OrderBy(m => m.Grado.Nombre);

        //    if (profesorId > 0 && profesorId != null)
        //    {
        //        query = (IOrderedQueryable<Materia>)query.Where(m => m.IdProfesor == profesorId);
        //    }

        //    var rows = await query
        //        .Select(m => new
        //        {
        //            m.Id,
        //            Materia = m.Nombre,
        //            Profesor = m.Profesor.Nombres + " " + m.Profesor.Apellidos,
        //            Grado = m.Grado.Nombre
        //        })
        //        .ToListAsync();

        //    using var wb = new XLWorkbook();
        //    var sheetName = ReporteChartHelper.GetSafeSheetName("Materias Asignadas");
        //    var ws = wb.Worksheets.Add(sheetName);

        //    ws.Cell(1, 1).Value = "Unidad Educativa AMA";
        //    ws.Cell(2, 1).Value = "Reporte de Materias Asignadas por Profesor";
        //    ws.Cell(3, 1).Value = $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}";

        //    var headerRow = 5;
        //    var headers = new[] { "ID", "Materia", "Profesor", "Grado" };
        //    for (int i = 0; i < headers.Length; i++)
        //    {
        //        ws.Cell(headerRow, i + 1).Value = headers[i];
        //        ws.Cell(headerRow, i + 1).Style.Font.SetBold();
        //        ws.Cell(headerRow, i + 1).Style.Fill.SetBackgroundColor(XLColor.FromHtml("#EEEEEE"));
        //        ws.Cell(headerRow, i + 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        //    }

        //    var r = headerRow + 1;
        //    foreach (var item in rows)
        //    {
        //        ws.Cell(r, 1).Value = item.Id;
        //        ws.Cell(r, 2).Value = item.Materia;
        //        ws.Cell(r, 3).Value = item.Profesor;
        //        ws.Cell(r, 4).Value = item.Grado;
        //        r++;
        //    }

        //    ws.Range(headerRow, 1, Math.Max(r - 1, headerRow), headers.Length).SetAutoFilter();
        //    ws.Columns(1, headers.Length).AdjustToContents();
        //    ws.SheetView.FreezeRows(headerRow);

        //    // Gráfico: conteo por Grado (barra horizontal)
        //    var series = rows.GroupBy(x => x.Grado).OrderBy(g => g.Key).ToDictionary(g => g.Key ?? "(sin grado)", g => (double)g.Count());
        //    using (var chartStream = ReporteChartHelper.RenderChartPng(ReporteChartHelper.ChartKind.BarHorizontal, series, 600, 300))
        //    {
        //        ws.AddPicture(chartStream).MoveTo(ws.Cell(r + 2, 1)).WithSize(600, 300);
        //    }

        //    using var stream = new MemoryStream();
        //    wb.SaveAs(stream);
        //    var content = stream.ToArray();

        //    var fileName = profesorId.HasValue
        //        ? $"Materias_Asignadas_{profesorId}.xlsx"
        //        : "Materias_Asignadas_Todos.xlsx";

        //    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        //}

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
            // 0) Datos del profesor (para mostrar Tutor + nombre arriba)
            var prof = await _context.Profesors
                .AsNoTracking()
                .Where(p => p.Id == profesorId)
                .Select(p => new
                {
                    p.Id,
                    p.Nombres,
                    p.Apellidos,
                    p.IsTutor
                })
                .FirstOrDefaultAsync();

            if (prof == null)
                return NotFound("No se encontró el profesor.");

            // 1) Filas planas (sin IDs) (una fila = una materia asignada)
            var rows = await _context.GradoParalelos
                .AsNoTracking()
                .Where(gp => gp.ProfesorId == profesorId)
                .OrderBy(gp => gp.Grado.Nivel)
                .SelectMany(gp => gp.Grado.Materias
                    .Where(m => m.IdProfesor == profesorId)
                    .Select(m => new
                    {
                        Materia = m.Nombre,
                        Profesor = (m.Profesor.Nombres + " " + m.Profesor.Apellidos),
                        Grado = gp.Grado.Nombre,
                        // AJUSTA según tu modelo:
                        Paralelo = gp.Paralelo != null ? gp.Paralelo.Nombre : ""
                    })
                )
                .ToListAsync();

            if (rows.Count == 0)
                return NotFound("No se encontraron materias asignadas a este profesor.");

            using var wb = new XLWorkbook();
            var sheetName = ReporteChartHelper.GetSafeSheetName($"Materias - Prof {profesorId}");
            var ws = wb.Worksheets.Add(sheetName);

            // ======= Estilo global =======
            ws.Style.Font.FontName = "Calibri";
            ws.Style.Font.FontSize = 11;
            ws.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            // ======= Header bonito =======
            ws.Range("A1:D1").Merge();
            ws.Cell("A1").Value = "UNIDAD EDUCATIVA Ana Maria IZA";
            ws.Cell("A1").Style.Font.Bold = true;
            ws.Cell("A1").Style.Font.FontSize = 16;
            ws.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A1").Style.Fill.SetBackgroundColor(XLColor.FromHtml("#0B3D91"));
            ws.Cell("A1").Style.Font.SetFontColor(XLColor.White);

            ws.Range("A2:D2").Merge();
            ws.Cell("A2").Value = "REPORTE DE MATERIAS ASIGNADAS";
            ws.Cell("A2").Style.Font.Bold = true;
            ws.Cell("A2").Style.Font.FontSize = 12;
            ws.Cell("A2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A2").Style.Fill.SetBackgroundColor(XLColor.FromHtml("#E8F0FE"));

            // Profesor + Tutor (visible, una sola vez)
            var nombreProfesor = $"{prof.Nombres} {prof.Apellidos}";
            var tutorText = prof.IsTutor ? "SÍ" : "NO";

            ws.Range("A3:D3").Merge();
            ws.Cell("A3").Value = $"PROFESOR: {nombreProfesor}";
            ws.Cell("A3").Style.Font.Bold = true;
            ws.Cell("A3").Style.Font.FontSize = 12;
            ws.Cell("A3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A3").Style.Fill.SetBackgroundColor(XLColor.FromHtml("#F3F4F6"));

            ws.Range("A4:D4").Merge();
            ws.Cell("A4").Value = $"ES TUTOR: {tutorText}";
            ws.Cell("A4").Style.Font.Bold = true;
            ws.Cell("A4").Style.Font.FontSize = 13;
            ws.Cell("A4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Color fuerte si es tutor
            if (prof.IsTutor)
            {
                ws.Cell("A4").Style.Fill.SetBackgroundColor(XLColor.FromHtml("#16A34A")); // verde
                ws.Cell("A4").Style.Font.SetFontColor(XLColor.White);
            }
            else
            {
                ws.Cell("A4").Style.Fill.SetBackgroundColor(XLColor.FromHtml("#DC2626")); // rojo
                ws.Cell("A4").Style.Font.SetFontColor(XLColor.White);
            }

            ws.Range("A5:D5").Merge();
            ws.Cell("A5").Value = $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}";
            ws.Cell("A5").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A5").Style.Font.Italic = true;
            ws.Cell("A5").Style.Font.FontSize = 10;
            ws.Cell("A5").Style.Fill.SetBackgroundColor(XLColor.FromHtml("#F9FAFB"));

            // Espacio
            ws.Row(6).Height = 6;

            // ======= Tabla =======
            var headerRow = 7;
            var headers = new[] { "Materia", "Profesor", "Grado", "Paralelo" };

            for (int i = 0; i < headers.Length; i++)
            {
                var cell = ws.Cell(headerRow, i + 1);
                cell.Value = headers[i];
                cell.Style.Font.SetBold();
                cell.Style.Font.SetFontColor(XLColor.White);
                cell.Style.Fill.SetBackgroundColor(XLColor.FromHtml("#111827")); // casi negro
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                cell.Style.Border.OutsideBorderColor = XLColor.FromHtml("#0F172A");
            }

            var r = headerRow + 1;
            foreach (var item in rows)
            {
                ws.Cell(r, 1).Value = item.Materia;
                ws.Cell(r, 2).Value = item.Profesor;
                ws.Cell(r, 3).Value = item.Grado;
                ws.Cell(r, 4).Value = item.Paralelo;

                ws.Range(r, 1, r, headers.Length).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.Range(r, 1, r, headers.Length).Style.Border.OutsideBorderColor = XLColor.FromHtml("#D1D5DB");
                r++;
            }

            var lastDataRow = r - 1;

            // Tabla con tema + bandas
            if (lastDataRow >= headerRow + 1)
            {
                var range = ws.Range(headerRow, 1, lastDataRow, headers.Length);
                var table = range.CreateTable();
                table.Theme = XLTableTheme.TableStyleMedium9;
                table.ShowAutoFilter = true;
            }

            // Ajustes visuales
            ws.Columns(1, headers.Length).AdjustToContents();
            ws.Column(1).Width = Math.Max(ws.Column(1).Width, 30); // Materia
            ws.Column(2).Width = Math.Max(ws.Column(2).Width, 26); // Profesor
            ws.Column(3).Width = Math.Max(ws.Column(3).Width, 18); // Grado
            ws.Column(4).Width = Math.Max(ws.Column(4).Width, 12); // Paralelo

            ws.SheetView.FreezeRows(headerRow);

            // Alineación por columna
            ws.Column(3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Column(4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // ======= Gráfico: conteo por Grado =======
            var series = rows
                .GroupBy(x => string.IsNullOrWhiteSpace(x.Grado) ? "(sin grado)" : x.Grado)
                .OrderBy(g => g.Key)
                .ToDictionary(g => g.Key, g => (double)g.Count());

            using (var chartStream = ReporteChartHelper.RenderChartPng(
                ReporteChartHelper.ChartKind.BarHorizontal, series, 900, 360))
            {
                ws.AddPicture(chartStream)
                  .MoveTo(ws.Cell(lastDataRow + 3, 1))
                  .WithSize(900, 360);
            }

            // Exportar
            using var stream = new MemoryStream();
            wb.SaveAs(stream);
            var content = stream.ToArray();

            var fileName = $"Materias_Asignadas_{profesorId}.xlsx";
            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }



        // GET: /api/Reporte/estudiantes/documentos-faltantes/excel
        [HttpGet("estudiantes/documentos-faltantes/excel")]
        public async Task<IActionResult> EstudiantesDocumentosFaltantesExcel()
        {
            // 1) Traer solo estudiantes activos + sus documentos (sin cargar de más)
            var estudiantes = await _context.Estudiantes
                .AsNoTracking()
                .Where(e => e.IsActive == true)
                .Select(e => new
                {
                    EstudianteId = e.Id,
                    Nombres = (e.Nombre ?? "").Trim(),
                    Apellidos = (e.Apellido ?? "").Trim(),
                    Cedula = e.Cedula ?? "",
                    DocumentosFaltantes = e.Documentos
                        .Where(d => d.IsActive == true && d.Aprobado == null)
                        .Select(d => (d.Nombre ?? "Documento").Trim())
                        .ToList()
                })
                .ToListAsync();

            // 2) Filtrar: solo los que tienen faltantes
            var estudiantesConFaltantes = estudiantes
                .Where(e => e.DocumentosFaltantes != null && e.DocumentosFaltantes.Count > 0)
                .OrderBy(e => e.Apellidos)
                .ThenBy(e => e.Nombres)
                .ToList();

            var totalEstudiantesActivos = estudiantes.Count;
            var totalConFaltantes = estudiantesConFaltantes.Count;
            var totalCompletos = totalEstudiantesActivos - totalConFaltantes;

            var porcentajeFaltantes = totalEstudiantesActivos > 0
                ? (double)totalConFaltantes / totalEstudiantesActivos * 100.0
                : 0.0;

            // 3) Serie para gráfica (pie)
            var stats = new Dictionary<string, double>
            {
                ["Completos"] = totalCompletos,
                ["Con faltantes"] = totalConFaltantes
            };

            // 4) Crear Excel
            using var wb = new XLWorkbook();

            // ===================== HOJA 1: RESUMEN =====================
            var ws = wb.Worksheets.Add(ReporteChartHelper.GetSafeSheetName("Doc. Faltantes"));

            ws.Cell(1, 1).Value = "Unidad Educativa Ana Maria Iza";
            ws.Cell(2, 1).Value = "Reporte de Estudiantes con Documentación Faltante";
            ws.Cell(3, 1).Value = $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}";

            ws.Range(1, 1, 1, 6).Merge();
            ws.Range(2, 1, 2, 6).Merge();
            ws.Range(3, 1, 3, 6).Merge();

            ws.Cell(1, 1).Style.Font.SetBold().Font.SetFontSize(16);
            ws.Cell(2, 1).Style.Font.SetBold().Font.SetFontSize(12);
            ws.Cell(3, 1).Style.Font.SetFontSize(10).Font.SetFontColor(XLColor.Gray);

            // KPIs
            ws.Cell(5, 1).Value = "Total estudiantes activos";
            ws.Cell(5, 2).Value = totalEstudiantesActivos;

            ws.Cell(6, 1).Value = "Con documentos faltantes";
            ws.Cell(6, 2).Value = totalConFaltantes;

            ws.Cell(7, 1).Value = "Completos";
            ws.Cell(7, 2).Value = totalCompletos;

            ws.Cell(8, 1).Value = "Porcentaje con faltantes";
            ws.Cell(8, 2).Value = porcentajeFaltantes / 100.0;
            ws.Cell(8, 2).Style.NumberFormat.Format = "0.00%";

            ws.Range(5, 1, 8, 2).Style.Font.SetFontSize(11);
            ws.Range(5, 1, 8, 1).Style.Font.SetBold();
            ws.Range(5, 1, 8, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            ws.Range(5, 1, 8, 2).Style.Border.OutsideBorderColor = XLColor.FromHtml("#D0D0D0");

            // Gráfica (usando tu helper SkiaSharp)
            ws.Cell(10, 1).Value = "Gráfico: Completos vs Con faltantes";
            ws.Range(10, 1, 10, 6).Merge();
            ws.Cell(10, 1).Style.Font.SetBold();
            ws.Cell(10, 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            using (var chart = ReporteChartHelper.RenderChartPng(ReporteChartHelper.ChartKind.Pie, stats, 900, 360))
            {
                ws.AddPicture(chart)
                  .MoveTo(ws.Cell(11, 1))
                  .WithSize(900, 360);
            }

            ws.Columns().AdjustToContents();
            ws.Column(1).Width = Math.Max(ws.Column(1).Width, 28);

            // ✅ NO sticky
            // ws.SheetView.FreezeRows(...);

            // ===================== HOJA 2: DETALLE =====================
            var ws2 = wb.Worksheets.Add(ReporteChartHelper.GetSafeSheetName("Detalle"));

            ws2.Cell(1, 1).Value = "Unidad Educativa Ana Maria Iza";
            ws2.Cell(2, 1).Value = "Detalle de Documentos Faltantes (Aprobado == NULL)";
            ws2.Cell(3, 1).Value = $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}";

            ws2.Range(1, 1, 1, 4).Merge();
            ws2.Range(2, 1, 2, 4).Merge();
            ws2.Range(3, 1, 3, 4).Merge();

            ws2.Cell(1, 1).Style.Font.SetBold().Font.SetFontSize(16);
            ws2.Cell(2, 1).Style.Font.SetBold().Font.SetFontSize(12);
            ws2.Cell(3, 1).Style.Font.SetFontSize(10).Font.SetFontColor(XLColor.Gray);

            var headerRow = 5;
            var headers = new[] { "Estudiante", "Cédula", "Cantidad faltantes", "Documentos faltantes" };

            for (int i = 0; i < headers.Length; i++)
            {
                var cell = ws2.Cell(headerRow, i + 1);
                cell.Value = headers[i];
                cell.Style.Font.SetBold().Font.SetFontColor(XLColor.White);
                cell.Style.Fill.SetBackgroundColor(XLColor.FromHtml("#1E88E5"));
                cell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                cell.Style.Border.OutsideBorderColor = XLColor.FromHtml("#D0D0D0");
            }

            int r = headerRow + 1;

            foreach (var e in estudiantesConFaltantes)
            {
                var nombreCompleto = $"{e.Apellidos} {e.Nombres}".Trim();
                var docs = e.DocumentosFaltantes ?? new List<string>();

                ws2.Cell(r, 1).Value = nombreCompleto;
                ws2.Cell(r, 2).Value = e.Cedula;
                ws2.Cell(r, 3).Value = docs.Count;
                ws2.Cell(r, 4).Value = string.Join(Environment.NewLine, docs);
                ws2.Cell(r, 4).Style.Alignment.WrapText = true;

                var rowRange = ws2.Range(r, 1, r, 4);
                rowRange.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                rowRange.Style.Border.BottomBorderColor = XLColor.FromHtml("#E6E6E6");

                if ((r - (headerRow + 1)) % 2 == 1)
                    rowRange.Style.Fill.SetBackgroundColor(XLColor.FromHtml("#FAFAFA"));

                r++;
            }

            // Tabla bonita
            ws2.Columns().AdjustToContents();
            ws2.Column(1).Width = Math.Max(ws2.Column(1).Width, 28);
            ws2.Column(4).Width = Math.Min(Math.Max(ws2.Column(4).Width, 35), 60);

            ws2.Range(headerRow, 1, Math.Max(r - 1, headerRow), 4).CreateTable().Theme = XLTableTheme.TableStyleMedium9;

            // ✅ NO sticky
            // ws2.SheetView.FreezeRows(headerRow);

            // 5) Export
            using var stream = new MemoryStream();
            wb.SaveAs(stream);

            var fileName = $"Estudiantes_Documentos_Faltantes_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";

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
