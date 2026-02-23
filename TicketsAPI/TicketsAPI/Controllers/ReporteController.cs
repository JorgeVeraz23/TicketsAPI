using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using TicketsAPI.DTO;
using TicketsAPI.Entities;
using TicketsAPI.Interfaces;
using System;
using System.Linq;
using System.Collections.Generic;
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

        public ReporteController(
            IReportesServices reportesServices,
            ApplicationDbContext context,
            IMatricula matricula,
            IMatriculaPdfService matriculaPdfService)
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
            if (filtro.Page <= 0) filtro.Page = 1;
            if (filtro.PageSize <= 0) filtro.PageSize = 20;
            if (filtro.PageSize > 200) filtro.PageSize = 200;

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
                    FechaNacimiento = x.FechaNacimiento,
                    Genero = x.Genero,
                    Telefono = x.Telefono ?? "",
                    Email = x.Correo ?? "",
                    Direccion = x.Direccion ?? "",
                    Representante = x.Representante != null ? ((x.Representante.Nombres + " " + x.Representante.Apellidos).Trim()) : "",
                    DocumentoRepresentante = x.Representante != null ? (x.Representante.NumeroDocumento ?? "") : "",
                    TieneMatricula = x.Matriculas.Any(m => m.IsActive == true)
                })
                .ToListAsync();

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

            ws.Range(1, 1, 1, 12).Merge();
            ws.Range(2, 1, 2, 12).Merge();
            ws.Range(3, 1, 3, 12).Merge();

            ws.Cell(1, 1).Style.Font.SetBold().Font.SetFontSize(16);
            ws.Cell(2, 1).Style.Font.SetBold().Font.SetFontSize(12);
            ws.Cell(3, 1).Style.Font.SetFontSize(10).Font.SetFontColor(XLColor.Gray);

            ws.Row(1).Height = 22;
            ws.Row(2).Height = 18;

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
                cell.Style.Fill.SetBackgroundColor(XLColor.FromHtml("#1E88E5"));
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

                var rowRange = ws.Range(r, 1, r, 12);
                rowRange.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                rowRange.Style.Border.BottomBorderColor = XLColor.FromHtml("#E6E6E6");
                rowRange.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                if ((r - (headerRow + 1)) % 2 == 1)
                    rowRange.Style.Fill.SetBackgroundColor(XLColor.FromHtml("#FAFAFA"));

                r++;
            }

            // ✅ SIN GRÁFICOS (se eliminó toda la sección de charts)

            ws.Columns().AdjustToContents();

            ws.Column(1).Width = Math.Max(ws.Column(1).Width, 16);
            ws.Column(2).Width = Math.Max(ws.Column(2).Width, 18);
            ws.Column(8).Width = Math.Max(ws.Column(8).Width, 22);
            ws.Column(9).Width = Math.Max(ws.Column(9).Width, 22);
            ws.Column(10).Width = Math.Max(ws.Column(10).Width, 22);

            ws.Column(3).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
            ws.Column(4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            ws.Column(5).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            ws.Column(6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            ws.Column(12).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            ws.SheetView.FreezeRows(headerRow);

            var tableRange = ws.Range(headerRow, 1, Math.Max(r - 1, headerRow), 12);
            tableRange.CreateTable();

            using var stream = new MemoryStream();
            wb.SaveAs(stream);
            var content = stream.ToArray();

            var fileName = $"Listado_Estudiantes_Activos_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";
            return File(content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }

        private static int CalcularEdad(DateTime fechaNacimiento)
        {
            var today = DateTime.Today;
            var age = today.Year - fechaNacimiento.Year;
            if (fechaNacimiento.Date > today.AddYears(-age)) age--;
            return age < 0 ? 0 : age;
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

            // ✅ SIN GRÁFICO

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
                return NotFound("No se encontraron tutores con cursos asignados.");

            using var wb = new XLWorkbook();
            var sheetName = ReporteChartHelper.GetSafeSheetName("Profesores Tutores y Cursos");
            var ws = wb.Worksheets.Add(sheetName);

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

            // ✅ SIN GRÁFICO

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

            // ✅ SIN GRÁFICO

            using var stream = new MemoryStream();
            wb.SaveAs(stream);
            var content = stream.ToArray();

            var fileName = "Profesores_Tutores.xlsx";
            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpGet("materias/por-maestro/{profesorId:long}")]
        public async Task<IActionResult> MateriasPorMaestroPorId(long profesorId)
        {
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
                        Paralelo = gp.Paralelo != null ? gp.Paralelo.Nombre : ""
                    })
                )
                .ToListAsync();

            if (rows.Count == 0)
                return NotFound("No se encontraron materias asignadas a este profesor.");

            using var wb = new XLWorkbook();
            var sheetName = ReporteChartHelper.GetSafeSheetName($"Materias - Prof {profesorId}");
            var ws = wb.Worksheets.Add(sheetName);

            ws.Style.Font.FontName = "Calibri";
            ws.Style.Font.FontSize = 11;
            ws.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

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

            if (prof.IsTutor)
            {
                ws.Cell("A4").Style.Fill.SetBackgroundColor(XLColor.FromHtml("#16A34A"));
                ws.Cell("A4").Style.Font.SetFontColor(XLColor.White);
            }
            else
            {
                ws.Cell("A4").Style.Fill.SetBackgroundColor(XLColor.FromHtml("#DC2626"));
                ws.Cell("A4").Style.Font.SetFontColor(XLColor.White);
            }

            ws.Range("A5:D5").Merge();
            ws.Cell("A5").Value = $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}";
            ws.Cell("A5").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A5").Style.Font.Italic = true;
            ws.Cell("A5").Style.Font.FontSize = 10;
            ws.Cell("A5").Style.Fill.SetBackgroundColor(XLColor.FromHtml("#F9FAFB"));

            ws.Row(6).Height = 6;

            var headerRow = 7;
            var headers = new[] { "Materia", "Profesor", "Grado", "Paralelo" };

            for (int i = 0; i < headers.Length; i++)
            {
                var cell = ws.Cell(headerRow, i + 1);
                cell.Value = headers[i];
                cell.Style.Font.SetBold();
                cell.Style.Font.SetFontColor(XLColor.White);
                cell.Style.Fill.SetBackgroundColor(XLColor.FromHtml("#111827"));
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

            if (lastDataRow >= headerRow + 1)
            {
                var range = ws.Range(headerRow, 1, lastDataRow, headers.Length);
                var table = range.CreateTable();
                table.Theme = XLTableTheme.TableStyleMedium9;
                table.ShowAutoFilter = true;
            }

            ws.Columns(1, headers.Length).AdjustToContents();
            ws.Column(1).Width = Math.Max(ws.Column(1).Width, 30);
            ws.Column(2).Width = Math.Max(ws.Column(2).Width, 26);
            ws.Column(3).Width = Math.Max(ws.Column(3).Width, 18);
            ws.Column(4).Width = Math.Max(ws.Column(4).Width, 12);

            ws.SheetView.FreezeRows(headerRow);
            ws.Column(3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Column(4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // ✅ SIN GRÁFICO (se eliminó el conteo por grado)

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

            // ✅ SIN GRÁFICO (se eliminó el RenderChartPng + AddPicture)

            ws.Columns().AdjustToContents();
            ws.Column(1).Width = Math.Max(ws.Column(1).Width, 28);

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

            ws2.Columns().AdjustToContents();
            ws2.Column(1).Width = Math.Max(ws2.Column(1).Width, 28);
            ws2.Column(4).Width = Math.Min(Math.Max(ws2.Column(4).Width, 35), 60);

            ws2.Range(headerRow, 1, Math.Max(r - 1, headerRow), 4)
                .CreateTable()
                .Theme = XLTableTheme.TableStyleMedium9;

            using var stream = new MemoryStream();
            wb.SaveAs(stream);

            var fileName = $"Estudiantes_Documentos_Faltantes_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName
            );
        }
    }
}