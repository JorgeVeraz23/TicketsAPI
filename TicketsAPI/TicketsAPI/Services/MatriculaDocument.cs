using QuestPDF.Helpers;
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

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);
                page.Footer().Element(ComposeFooter);
            });
        }

        void ComposeHeader(IContainer container)
        {
            container
                .Background(Colors.Grey.Lighten4)
                .Padding(10)
                .Column(col =>
                {
                    col.Item().Row(row =>
                    {
                        row.ConstantItem(44).Height(44).Placeholder(); // logo más pequeño

                        row.RelativeItem().PaddingLeft(8).Column(c =>
                        {
                            c.Spacing(1);
                            c.Item().Text(_m.Institucion).SemiBold().FontSize(14);
                            c.Item().Text("MATRÍCULA").SemiBold().FontSize(12);
                            c.Item().Text(_m.Periodo).FontSize(10).FontColor(Colors.Grey.Darken2);

                            c.Item().PaddingTop(4)
                                .Background(Colors.Blue.Lighten4)
                                .PaddingVertical(2).PaddingHorizontal(6)
                                .Text($"Código: {_m.CodigoMatricula}")
                                .FontSize(9).SemiBold().FontColor(Colors.Blue.Darken2);
                        });

                        row.ConstantItem(92).AlignRight().Column(c =>
                        {
                            c.Item().Text(_m.Fecha.ToString("dd/MM/yyyy"))
                                .FontSize(9).FontColor(Colors.Grey.Darken2);

                            // QR más chico o quítalo mientras:
                            c.Item().PaddingTop(6).Height(44).Width(44).Placeholder();
                        });
                    });
                });
        }




        void ComposeContent(IContainer container)
        {
            container.PaddingTop(10).Column(col =>
            {
                col.Spacing(8);

                col.Item().Element(c => SectionCardCompact(c, "Datos del Estudiante", content =>
                {
                    TwoColInfo(content,
                        ("Estudiante", _m.Estudiante),
                        ("Documento", _m.DocumentoEstudiante));
                }));

                col.Item().Element(c => SectionCardCompact(c, "Datos del Representante", content =>
                {
                    TwoColInfo(content,
                        ("Representante", _m.Representante),
                        ("Documento", _m.DocumentoRepresentante));
                }));

                col.Item().Element(c => SectionCardCompact(c, "Información Académica", content =>
                {
                    TwoColInfo(content,
                        ("Grado", _m.Grado),
                        ("Paralelo", _m.Paralelo));
                }));

                col.Item().Text("Materias").SemiBold().FontSize(11);

                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        cols.ConstantColumn(28);
                        cols.RelativeColumn();
                    });

                    table.Header(h =>
                    {
                        h.Cell().Background(Colors.Grey.Lighten3).Padding(6).Text("#").SemiBold().FontSize(10);
                        h.Cell().Background(Colors.Grey.Lighten3).Padding(6).Text("Materia").SemiBold().FontSize(10);
                    });

                    for (int i = 0; i < _m.Materias.Count; i++)
                    {
                        var bg = (i % 2 == 0) ? Colors.White : Colors.Grey.Lighten5;

                        table.Cell().Background(bg).BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                            .Padding(6).Text((i + 1).ToString()).FontSize(10);

                        table.Cell().Background(bg).BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                            .Padding(6).Text(_m.Materias[i]).FontSize(10);
                    }
                });

                col.Item().PaddingTop(10).Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Height(55); // 👈 ESPACIO PARA FIRMA (ajusta entre 50–70)
                        c.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                        c.Item().PaddingTop(4)
                            .Text("Firma Representante")
                            .FontSize(9)
                            .FontColor(Colors.Grey.Darken1);
                    });

                    row.ConstantItem(24);

                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Height(55); // 👈 ESPACIO PARA FIRMA
                        c.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                        c.Item().PaddingTop(4)
                            .Text("Secretaría / Rectorado")
                            .FontSize(9)
                            .FontColor(Colors.Grey.Darken1);
                    });
                });

            });
        }

        static void SectionCardCompact(IContainer container, string title, Action<IContainer> content)
        {
            container
                .Border(1).BorderColor(Colors.Grey.Lighten2)
                .Background(Colors.White)
                .Padding(10)
                .Column(col =>
                {
                    col.Item().Text(title).SemiBold().FontSize(11);
                    col.Item().PaddingTop(6).Element(content);
                });
        }

        void ComposeFooter(IContainer container)
        {
            container.Column(col =>
            {
                col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                col.Item().PaddingTop(8).AlignCenter().Text(text =>
                {
                    text.DefaultTextStyle(x => x.FontSize(9).FontColor(Colors.Grey.Darken2));

                    text.Span("Documento generado por Matriculas AMA • ");
                    text.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm"))
                        .FontColor(Colors.Grey.Darken1);
                });
            });
        }



        static void TwoColInfo(IContainer container, (string k, string v) a, (string k, string v) b)
        {
            container.Row(row =>
            {
                row.RelativeItem().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8).Column(c =>
                {
                    c.Item().Text(a.k).FontSize(9).FontColor(Colors.Grey.Darken1);
                    c.Item().Text(a.v).SemiBold();
                });

                row.ConstantItem(10);

                row.RelativeItem().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8).Column(c =>
                {
                    c.Item().Text(b.k).FontSize(9).FontColor(Colors.Grey.Darken1);
                    c.Item().Text(b.v).SemiBold();
                });
            });
        }
    }
}
