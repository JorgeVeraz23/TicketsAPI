using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TicketsAPI.DTO;

namespace TicketsAPI.Services
{
    public class MatriculaDocument : IDocument
    {
        private readonly MatriculaPdfDto _m;

        public MatriculaDocument(MatriculaPdfDto m) => _m = m;

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        // Paleta
        private static readonly string Primary = Colors.Blue.Darken2;
        private static readonly string BorderSoft = Colors.Grey.Lighten3;
        private static readonly string TextSoft = Colors.Grey.Darken2;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(24); // ✅ menos margen para que quepa en 1 hoja
                page.PageColor(Colors.White);

                page.DefaultTextStyle(x => x.FontSize(10.0f).FontColor(Colors.Grey.Darken4)); // ✅ un poco más pequeño

                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);
                page.Footer().Element(ComposeFooter);
            });
        }

        // ================= HEADER =================
        void ComposeHeader(IContainer container)
        {
            container
                .PaddingBottom(8)
                .Column(col =>
                {
                    col.Item().Height(5).Background(Primary).CornerRadius(10);

                    col.Item().PaddingTop(8).Row(row =>
                    {
                        // 🔴 ELIMINADO: LogoBox

                        // 🔹 Ahora el bloque textual ocupa todo el espacio izquierdo
                        row.RelativeItem().Column(c =>
                        {
                            c.Spacing(1);
                            c.Item().Text(_m.Institucion).SemiBold().FontSize(14);

                            c.Item().Row(r =>
                            {
                                r.AutoItem().Element(x => Chip(x, "MATRÍCULA", Colors.Blue.Darken2, Colors.Blue.Lighten5));
                                r.AutoItem().PaddingLeft(8).Element(x => Chip(x, _m.Periodo, TextSoft, Colors.Grey.Lighten4));
                            });

                            c.Item().PaddingTop(4).Row(r =>
                            {
                                r.AutoItem().Element(x => Chip(x, $"Código: {_m.CodigoMatricula}", Colors.White, Primary, solid: true));
                                r.AutoItem().PaddingLeft(10)
                                    .Text(_m.Fecha.ToString("dd/MM/yyyy"))
                                    .FontSize(9.2f)
                                    .FontColor(TextSoft);
                            });
                        });

                        // 🔹 Foto del estudiante en el extremo derecho
                        row.ConstantItem(52)
                           .Height(52)
                           .AlignRight()
                           .AlignMiddle()
                           .Element(HeaderFotoBox);
                    });

                    col.Item().PaddingTop(10).LineHorizontal(1).LineColor(BorderSoft);
                });
        }

        // ================= CONTENT =================
        void ComposeContent(IContainer container)
        {
            container
                .PaddingTop(10) // ✅ menos espacio
                .Column(col =>
                {
                    col.Spacing(10); // ✅ menos separación para 1 hoja

                    // ====== BLOQUE EN CELDAS (2 columnas) ======
                    col.Item().Row(row =>
                    {
                        // Datos del estudiante (más compacto)
                        row.RelativeItem().Element(card => CardCell(card, "Datos del Estudiante", body =>
                        {
                            body.Column(c =>
                            {
                                c.Spacing(8);

                                c.Item().Element(x => InfoBoxOneLine(x, "Estudiante", _m.Estudiante));
                                c.Item().Element(x => InfoBoxOneLine(x, "Documento", _m.DocumentoEstudiante));
                            });
                        }));

                        row.ConstantItem(12);

                        row.RelativeItem().Element(card => CardCell(card, "Datos del Representante", body =>
                        {
                            //InfoGrid(body,
                            //    ("Representante", _m.Representante),
                            //    ("Documento", _m.DocumentoRepresentante));
                            body.Column(c =>
                            {
                                c.Spacing(8);

                                c.Item().Element(x => InfoBoxOneLine(x, "Estudiante", _m.Estudiante));
                                c.Item().Element(x => InfoBoxOneLine(x, "Documento", _m.DocumentoEstudiante));
                            });

                        }));
                    });

                    col.Item().Element(card => CardCell(card, "Información Académica", body =>
                    {
                        InfoGrid(body,
                            ("Grado", _m.Grado),
                            ("Paralelo", _m.Paralelo));
                    }));

                    // Materias
                    col.Item().Row(r =>
                    {
                        r.RelativeItem().Text("Materias registradas").SemiBold().FontSize(11.2f);
                        r.AutoItem().Element(x => Chip(x, $"{_m.Materias?.Count ?? 0} materias", TextSoft, Colors.Grey.Lighten4));
                    });

                    col.Item().Element(ComposeMateriasTableCompact);

                    // ✅ Firmas compactas y pegadas abajo
                    col.Item().ExtendVertical().AlignBottom().Element(ComposeSignaturesCompact);
                });
        }

        // ================= TABLE MATERIAS (compacta) =================
        void ComposeMateriasTableCompact(IContainer container)
        {
            container
                .Border(1).BorderColor(BorderSoft)
                .CornerRadius(14)
                .Padding(10) // ✅ menos padding
                .Background(Colors.White)
                .Table(t =>
                {
                    t.ColumnsDefinition(cols =>
                    {
                        cols.ConstantColumn(30); // #
                        cols.RelativeColumn(2);  // Materia
                        cols.RelativeColumn(2);  // Profesor
                    });

                    t.Header(h =>
                    {
                        h.Cell().Element(x => TableHeaderCellCompact(x, "#"));
                        h.Cell().Element(x => TableHeaderCellCompact(x, "Materia"));
                        h.Cell().Element(x => TableHeaderCellCompact(x, "Profesor"));
                    });

                    var materias = _m.Materias ?? new List<string>();
                    var profesores = _m.Profesor ?? new List<string>();

                    if (materias.Count == 0)
                    {
                        t.Cell().ColumnSpan(3)
                            .Padding(10)
                            .Text("No hay materias registradas.")
                            .FontColor(TextSoft)
                            .Italic();
                        return;
                    }

                    for (int i = 0; i < materias.Count; i++)
                    {
                        var bg = (i % 2 == 0) ? Colors.White : Colors.Grey.Lighten5;

                        var materia = materias[i];
                        var profesor = (i < profesores.Count && !string.IsNullOrWhiteSpace(profesores[i]))
                            ? profesores[i]
                            : "—";

                        t.Cell()
                            .Background(bg)
                            .PaddingVertical(8)   // ✅ menos alto por fila
                            .PaddingHorizontal(6)
                            .BorderBottom(1).BorderColor(BorderSoft)
                            .Text((i + 1).ToString()).FontSize(9.6f);

                        t.Cell()
                            .Background(bg)
                            .PaddingVertical(8)
                            .PaddingHorizontal(6)
                            .BorderBottom(1).BorderColor(BorderSoft)
                            .Text(materia).FontSize(9.6f);

                        t.Cell()
                            .Background(bg)
                            .PaddingVertical(8)
                            .PaddingHorizontal(6)
                            .BorderBottom(1).BorderColor(BorderSoft)
                            .Text(profesor).FontSize(9.6f);
                    }
                });
        }

        // ================= SIGNATURES (compactas) =================
        void ComposeSignaturesCompact(IContainer container)
        {
            container.Column(col =>
            {
                col.Item().LineHorizontal(1).LineColor(BorderSoft);

                col.Item().PaddingTop(10).Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Height(42); // ✅ menos alto
                        c.Item().LineHorizontal(1).LineColor(BorderSoft);
                        c.Item().PaddingTop(3).Text("Firma Representante").FontSize(8.6f).FontColor(TextSoft);
                    });

                    row.ConstantItem(24);

                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Height(42);
                        c.Item().LineHorizontal(1).LineColor(BorderSoft);
                        c.Item().PaddingTop(3).Text("Secretaría / Rectorado").FontSize(8.6f).FontColor(TextSoft);
                    });
                });
            });
        }

        // ================= FOOTER =================
        void ComposeFooter(IContainer container)
        {
            container
                .PaddingTop(6)
                .Row(row =>
                {
                    row.RelativeItem().Text(t =>
                    {
                        t.DefaultTextStyle(x => x.FontSize(8.8f).FontColor(TextSoft));
                        t.Span("Documento generado por ");
                        t.Span("Matriculas AMA").SemiBold();
                        t.Span(" • ");
                        t.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
                    });

                    row.AutoItem().Text(t =>
                    {
                        t.DefaultTextStyle(x => x.FontSize(8.8f).FontColor(TextSoft));
                        t.Span("Página ");
                        t.CurrentPageNumber();
                        t.Span(" / ");
                        t.TotalPages();
                    });
                });
        }

        // ================= HELPERS UI =================

        void HeaderFotoBox(IContainer c)
        {
            c
             .Width(52)
             .Height(52)
             .Border(1)
             .BorderColor(BorderSoft)
             .CornerRadius(999) // circular
             .AlignCenter()
             .AlignMiddle()
             .Element(inner =>
             {
                 if (_m.FotoEstudiante != null && _m.FotoEstudiante.Length > 0)
                 {
                     inner.Image(_m.FotoEstudiante)
                          .FitArea();
                 }
                 else
                 {
                     inner.Text("FOTO")
                          .FontSize(8)
                          .FontColor(TextSoft);
                 }
             });
        }

        void FotoEstudianteBox(IContainer c)
        {
            c.Border(1).BorderColor(BorderSoft)
             .CornerRadius(12)
             .Background(Colors.White)
             .Padding(4)
             .AlignCenter()
             .AlignMiddle()
             .Element(box =>
             {
                 if (_m.FotoEstudiante != null && _m.FotoEstudiante.Length > 0)
                     box.Image(_m.FotoEstudiante).FitArea();
                 else
                     box.Text("SIN FOTO").FontSize(9).FontColor(TextSoft);
             });
        }

        void LogoBox(IContainer c)
        {
            c.Border(1).BorderColor(BorderSoft).CornerRadius(12)
             .AlignCenter().AlignMiddle()
             .Text("LOGO").FontSize(9).FontColor(TextSoft);
        }

        static void Chip(IContainer c, string text, string fg, string bg, bool solid = false)
        {
            c.Background(bg)
             .CornerRadius(999)
             .PaddingVertical(3)
             .PaddingHorizontal(10)
             .Text(text)
             .FontSize(9)
             .SemiBold()
             .FontColor(fg);
        }

        static void TableHeaderCellCompact(IContainer c, string text)
        {
            c.Background(Colors.Grey.Lighten4)
             .PaddingVertical(7)
             .PaddingHorizontal(6)
             .Text(text)
             .SemiBold()
             .FontSize(9.4f)
             .FontColor(TextSoft);
        }

        static void CardCell(IContainer container, string title, Action<IContainer> body)
        {
            container
                .Border(1).BorderColor(BorderSoft)
                .CornerRadius(14)
                .Padding(12) // ✅ más compacto
                .Background(Colors.White)
                .Column(col =>
                {
                    col.Spacing(8);

                    col.Item().Row(r =>
                    {
                        r.ConstantItem(4).Height(16).Background(Primary).CornerRadius(2);
                        r.RelativeItem().PaddingLeft(10)
                            .Text(title)
                            .SemiBold()
                            .FontSize(11.4f)
                            .FontColor(Colors.Grey.Darken4);
                    });

                    col.Item().Element(body);
                });
        }

        static void InfoGrid(IContainer container, (string k, string v) a, (string k, string v) b)
        {
            container.Row(row =>
            {
                row.RelativeItem().Element(x => InfoBox(x, a.k, a.v));
                row.ConstantItem(10);
                row.RelativeItem().Element(x => InfoBox(x, b.k, b.v));
            });
        }

        static void InfoBox(IContainer container, string label, string value)
        {
            container
                .Border(1).BorderColor(Colors.Grey.Lighten2)
                .CornerRadius(12)
                .Padding(10)
                .MinHeight(54)
                .Column(col =>
                {
                    col.Spacing(3);

                    col.Item().Text(label)
                        .FontSize(9.2f)
                        .FontColor(TextSoft);

                    col.Item().Text(t =>
                    {
                        t.DefaultTextStyle(x => x.FontSize(10.4f).SemiBold().FontColor(Colors.Grey.Darken4));
                        var safe = string.IsNullOrWhiteSpace(value) ? "—" : value;
                        t.Span(safe);
                    });
                });
        }

        static void InfoBoxOneLine(IContainer container, string label, string value)
        {
            container
                .Border(1).BorderColor(Colors.Grey.Lighten2)
                .CornerRadius(12)
                .Padding(10)
                .MinHeight(54)
                .Column(col =>
                {
                    col.Spacing(3);

                    col.Item().Text(label)
                        .FontSize(9.2f)
                        .FontColor(TextSoft);

                    var safe = string.IsNullOrWhiteSpace(value) ? "—" : value;

                    col.Item().Text(t =>
                    {
                        t.DefaultTextStyle(x => x.FontSize(10.4f).SemiBold().FontColor(Colors.Grey.Darken4));

                        // ✅ 1 línea “bonita” con ellipsis por fallback
                        var v = safe.Length > 45 ? safe.Substring(0, 45) + "…" : safe;
                        t.Span(v);
                    });
                });
        }
    }
}