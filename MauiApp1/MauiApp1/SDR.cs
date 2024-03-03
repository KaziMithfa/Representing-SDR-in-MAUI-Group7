using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1
{
    public class SDR
    {
        void PlotActivityVertically(List<List<int>> dataSets, int highlightTouch, int maxCell, int minCell)
        {
            int width = 800;
            int height = 600;
            using (var bitmap = new SKBitmap(width, height))
            using (var canvas = new SKCanvas(bitmap))
            {
                canvas.Clear(SKColors.White);

                int numColumns = dataSets.Count;
                float columnWidth = width / (float)numColumns;
                SKPaint paint = new SKPaint { Color = SKColors.Blue, StrokeWidth = 2 };
                SKPaint highlightPaint = new SKPaint { Color = SKColors.Red, StrokeWidth = 3 };

                for (int t = 0; t < dataSets.Count; t++)
                {
                    var cells = dataSets[t];
                    foreach (var cell in cells)
                    {
                        float x = t * columnWidth;
                        float y = Map(cell, minCell, maxCell, height, 0);
                        canvas.DrawRect(x, y, columnWidth, 5, paint);
                    }

                    if (t == highlightTouch)
                    {
                        canvas.DrawRect(t * columnWidth, 0, columnWidth, height, highlightPaint);
                    }
                }

                using (var image = SKImage.FromBitmap(bitmap))
                using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                using (var stream = File.OpenWrite("VerticalActivity.png"))
                {
                    data.SaveTo(stream);
                }
            }
        }

        void PlotActivityHorizontally(List<List<int>> dataSets, int highlightTouch, int maxCell, int minCell)
        {
            int width = 800;
            int height = 600;
            using (var bitmap = new SKBitmap(width, height))
            using (var canvas = new SKCanvas(bitmap))
            {
                canvas.Clear(SKColors.White);

                int numRows = dataSets.Count;
                float rowHeight = height / (float)numRows;
                SKPaint paint = new SKPaint { Color = SKColors.Blue, StrokeWidth = 2 };
                SKPaint highlightPaint = new SKPaint { Color = SKColors.Red, StrokeWidth = 3 };

                for (int t = 0; t < dataSets.Count; t++)
                {
                    var cells = dataSets[t];
                    foreach (var cell in cells)
                    {
                        float y = t * rowHeight;
                        float x = Map(cell, minCell, maxCell, 0, width);
                        canvas.DrawRect(x, y, 5, rowHeight, paint);
                    }

                    if (t == highlightTouch)
                    {
                        canvas.DrawRect(0, t * rowHeight, width, rowHeight, highlightPaint);
                    }
                }

                using (var image = SKImage.FromBitmap(bitmap))
                using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                using (var stream = File.OpenWrite("HorizontalActivity.png"))
                {
                    data.SaveTo(stream);
                }
            }
        }


        float Map(float value, float fromSource, float toSource, float fromTarget, float toTarget)
        {
            return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
        }

    }
}
