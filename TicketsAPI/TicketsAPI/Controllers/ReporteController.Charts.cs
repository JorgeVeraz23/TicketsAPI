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

        public static MemoryStream RenderChartPng(ChartKind kind, IReadOnlyDictionary<string, double> series, int width = 500, int height = 300)
        {
            var ms = new MemoryStream();
            var info = new SKImageInfo(width, height);
            using var surface = SKSurface.Create(info);
            var canvas = surface.Canvas;
            canvas.Clear(SKColors.White);

            if (series == null) series = new Dictionary<string, double>();

            // Simple palette
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

                // legend
                DrawLegend(canvas, series, palette, rect.Right + 10, 10);
            }
            else if (kind == ChartKind.BarHorizontal || kind == ChartKind.ColumnVertical)
            {
                var items = new List<KeyValuePair<string, double>>(series);
                int margin = 10;
                int left = margin + 120;
                int right = width - margin;
                int top = margin;
                int bottom = height - margin - 40;
                double maxVal = 0;
                foreach (var kv in items) maxVal = Math.Max(maxVal, kv.Value);

                using var fill = new SKPaint { IsAntialias = true, Style = SKPaintStyle.Fill };
                using var stroke = new SKPaint { IsAntialias = true, Style = SKPaintStyle.Stroke, Color = SKColors.Gray, StrokeWidth = 1 };
                using var text = new SKPaint { IsAntialias = true, Color = SKColors.Black, TextSize = 12 };

                if (items.Count == 0)
                {
                    canvas.DrawText("Sin datos", left, (top + bottom) / 2, text);
                }
                else
                {
                    if (kind == ChartKind.BarHorizontal)
                    {
                        int availableHeight = Math.Max(40, bottom - top);
                        int barHeight = Math.Max(12, availableHeight / items.Count - 10);
                        int y = top;
                        int idx = 0;
                        foreach (var kv in items)
                        {
                            var val = kv.Value;
                            float wBar = maxVal > 0 ? (float)((right - left) * (val / maxVal)) : 0f;
                            fill.Color = palette[idx % palette.Length];
                            var barRect = new SKRect(left, y, left + wBar, y + barHeight);
                            canvas.DrawRect(barRect, fill);
                            canvas.DrawRect(barRect, stroke);

                            canvas.DrawText(kv.Key, 8, y + barHeight * 0.75f, text);
                            canvas.DrawText($"{val:0.##}", left + wBar + 6, y + barHeight * 0.75f, text);

                            y += barHeight + 8;
                            idx++;
                        }
                    }
                    else // ColumnVertical
                    {
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

                            canvas.Save();
                            canvas.Translate(x + colWidth / 2, bottom + 14);
                            canvas.RotateDegrees(-45);
                            canvas.DrawText(kv.Key, 0, 0, text);
                            canvas.Restore();

                            canvas.DrawText($"{val:0.##}", x + 2, bottom - hCol - 4, text);

                            x += colWidth + 8;
                            idx++;
                        }
                    }
                }

                using var title = new SKPaint { IsAntialias = true, Color = SKColors.Black, TextSize = 14, IsStroke = false, FakeBoldText = true };
                canvas.DrawText("Gráfica", left, height - 12, title);
            }

            using var img = surface.Snapshot();
            using var encoded = img.Encode(SKEncodedImageFormat.Png, 100);
            encoded.SaveTo(ms);
            ms.Position = 0;
            return ms;
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
                canvas.DrawText($"{kv.Key}: {kv.Value:0.##}", x + 18, y + 12, text);
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