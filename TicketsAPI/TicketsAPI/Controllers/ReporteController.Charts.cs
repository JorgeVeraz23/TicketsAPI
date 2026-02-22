using System;
using System.Collections.Generic;
using System.IO;
using SkiaSharp;

namespace TicketsAPI.Controllers
{
    internal static class ReporteChartHelper
    {
        public enum ChartKind
        {
            Pie,
            BarHorizontal,
            ColumnVertical
        }

        public static MemoryStream RenderChartPng(
            ChartKind kind,
            IReadOnlyDictionary<string, double> series,
            int width = 500,
            int height = 300)
        {
            var ms = new MemoryStream();
            var info = new SKImageInfo(width, height);

            using var surface = SKSurface.Create(info);
            var canvas = surface.Canvas;
            canvas.Clear(SKColors.White);

            series ??= new Dictionary<string, double>();

            // Palette simple
            SKColor[] palette = new[]
            {
                new SKColor(0xFF,0x45,0x00), // OrangeRed
                SKColors.LightGreen,
                SKColors.SteelBlue,
                SKColors.Goldenrod,
                SKColors.MediumPurple,
                SKColors.Coral
            };

            if (kind == ChartKind.Pie)
            {
                var total = 0.0;
                foreach (var v in series.Values) total += v;

                var rect = new SKRect(10, 10, Math.Min(width, 320), Math.Min(height, 220));
                float startAngle = -90f;
                int i = 0;

                using var paint = new SKPaint { IsAntialias = true, Style = SKPaintStyle.Fill };

                foreach (var kv in series)
                {
                    var sweep = total > 0 ? (float)(kv.Value / total * 360.0) : 0f;
                    paint.Color = palette[i % palette.Length];

                    if (sweep > 0.001f)
                        canvas.DrawArc(rect, startAngle, sweep, true, paint);

                    startAngle += sweep;
                    i++;
                }

                // Legend
                DrawLegend(canvas, series, palette, rect.Right + 10, 10);
            }
            else if (kind == ChartKind.BarHorizontal || kind == ChartKind.ColumnVertical)
            {
                var items = new List<KeyValuePair<string, double>>(series);

                int margin = 12;
                int top = margin;
                int bottom = height - margin - 40;

                using var fill = new SKPaint { IsAntialias = true, Style = SKPaintStyle.Fill };
                using var stroke = new SKPaint { IsAntialias = true, Style = SKPaintStyle.Stroke, Color = SKColors.Gray, StrokeWidth = 1 };

                // Texto (un poco más pequeño para que no se encime)
                using var text = new SKPaint { IsAntialias = true, Color = SKColors.Black, TextSize = 11 };

                if (items.Count == 0)
                {
                    canvas.DrawText("Sin datos", margin, (top + bottom) / 2, text);
                }
                else
                {
                    // Calcular maxVal
                    double maxVal = 0;
                    foreach (var kv in items) maxVal = Math.Max(maxVal, kv.Value);

                    if (kind == ChartKind.BarHorizontal)
                    {
                        // 1) Área de etiquetas dinámica (según ancho real del texto)
                        float maxLabelWidth = 0;
                        foreach (var kv in items)
                        {
                            var w = text.MeasureText(kv.Key ?? "");
                            if (w > maxLabelWidth) maxLabelWidth = w;
                        }

                        // Limitar para no “comerse” todo el gráfico si hay nombres enormes
                        // (máx 45% del ancho del canvas)
                        float labelAreaWidth = Math.Min(maxLabelWidth + 14, width * 0.45f);

                        float labelX = margin;                 // donde empieza el texto
                        float plotLeft = labelX + labelAreaWidth + 10; // donde empiezan las barras
                        float plotRight = width - margin;

                        int availableHeight = Math.Max(60, bottom - top);

                        // 2) Alto de barra basado en altura de fuente para que NUNCA se encime
                        int minBarHeight = (int)(text.TextSize + 8); // texto + padding
                        int gap = 10;
                        int barHeight = Math.Max(minBarHeight, (availableHeight - (items.Count - 1) * gap) / items.Count);
                        barHeight = Math.Min(barHeight, 34); // opcional: evita barras demasiado gordas

                        float y = top;
                        int idx = 0;

                        foreach (var kv in items)
                        {
                            var label = kv.Key ?? "";
                            var val = kv.Value;

                            // 3) Truncar etiqueta si es demasiado larga para el área
                            label = Ellipsize(text, label, labelAreaWidth);

                            float wBar = maxVal > 0 ? (float)((plotRight - plotLeft) * (val / maxVal)) : 0f;

                            fill.Color = palette[idx % palette.Length];

                            var barRect = new SKRect(plotLeft, y, plotLeft + wBar, y + barHeight);
                            canvas.DrawRect(barRect, fill);
                            canvas.DrawRect(barRect, stroke);

                            // baseline centrado verticalmente
                            float textY = y + (barHeight / 2f) + (text.TextSize / 2.8f);

                            // Etiqueta a la izquierda, alineada al inicio del área
                            canvas.DrawText(label, labelX, textY, text);

                            // Valor al final de la barra (con un poco de margen)
                            canvas.DrawText($"{val:0.##}", plotLeft + wBar + 8, textY, text);

                            y += barHeight + gap;
                            idx++;
                        }
                    }
                    else // ColumnVertical
                    {
                        int left = margin + 60;
                        int right = width - margin;

                        int availableWidth = Math.Max(80, right - left);
                        int colWidth = Math.Max(20, availableWidth / Math.Max(1, items.Count) - 8);

                        int x = left;
                        int idx = 0;

                        foreach (var kv in items)
                        {
                            var val = kv.Value;
                            float hCol = maxVal > 0 ? (float)((bottom - top) * (val / maxVal)) : 0f;

                            fill.Color = palette[idx % palette.Length];

                            var colRect = new SKRect(x, bottom - hCol, x + colWidth, bottom);
                            canvas.DrawRect(colRect, fill);
                            canvas.DrawRect(colRect, stroke);

                            // Labels rotadas
                            canvas.Save();
                            canvas.Translate(x + colWidth / 2f, bottom + 16);
                            canvas.RotateDegrees(-45);

                            var label = kv.Key ?? "";
                            label = Ellipsize(text, label, 140); // corte preventivo
                            canvas.DrawText(label, 0, 0, text);

                            canvas.Restore();

                            canvas.DrawText($"{val:0.##}", x + 2, bottom - hCol - 4, text);

                            x += colWidth + 8;
                            idx++;
                        }
                    }
                }

                using var title = new SKPaint
                {
                    IsAntialias = true,
                    Color = SKColors.Black,
                    TextSize = 14,
                    FakeBoldText = true
                };

                canvas.DrawText("Gráfica", 12, height - 12, title);
            }

            using var img = surface.Snapshot();
            using var encoded = img.Encode(SKEncodedImageFormat.Png, 100);
            encoded.SaveTo(ms);
            ms.Position = 0;
            return ms;
        }

        // Trunca con “…” para que el texto nunca invada el área de barras
        private static string Ellipsize(SKPaint paint, string text, float maxWidth)
        {
            if (string.IsNullOrEmpty(text)) return "";
            if (paint.MeasureText(text) <= maxWidth) return text;

            const string ell = "…";
            float ellW = paint.MeasureText(ell);

            int lo = 0, hi = text.Length;
            while (lo < hi)
            {
                int mid = (lo + hi) / 2;
                var candidate = text.Substring(0, mid).TrimEnd() + ell;
                if (paint.MeasureText(candidate) <= maxWidth)
                    lo = mid + 1;
                else
                    hi = mid;
            }

            int cut = Math.Max(1, lo - 1);
            return text.Substring(0, cut).TrimEnd() + ell;
        }

        private static void DrawLegend(SKCanvas canvas, IReadOnlyDictionary<string, double> series, SKColor[] palette, float x, float startY)
        {
            using var text = new SKPaint { IsAntialias = true, Color = SKColors.Black, TextSize = 12 };
            using var paint = new SKPaint { IsAntialias = true, Style = SKPaintStyle.Fill };

            float y = startY;
            int i = 0;

            foreach (var kv in series)
            {
                paint.Color = palette[i % palette.Length];
                canvas.DrawRect(new SKRect(x, y, x + 14, y + 14), paint);

                var label = $"{kv.Key}: {kv.Value:0.##}";
                canvas.DrawText(label, x + 18, y + 12, text);

                y += 20;
                i++;
            }
        }

        public static string GetSafeSheetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "Sheet1";
            var invalid = new[] { ':', '\\', '/', '?', '*', '[', ']' };

            var sb = new System.Text.StringBuilder(name.Length);
            foreach (var c in name)
                sb.Append(Array.IndexOf(invalid, c) >= 0 ? '_' : c);

            var cleaned = sb.ToString().Trim();
            if (cleaned.Length == 0) return "Sheet1";
            if (cleaned.Length > 31) cleaned = cleaned.Substring(0, 31).Trim();

            return string.IsNullOrEmpty(cleaned) ? "Sheet1" : cleaned;
        }
    }
}