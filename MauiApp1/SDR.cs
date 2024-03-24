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
        public int MaxCycles { get; set; } = 10;
        public string GraphName { get; set; } = "ActivityGraph";
        public SKPaint ActiveCellPaint { get; set; } = new SKPaint { Color = SKColors.Purple, StrokeWidth = 2 };
        public SKPaint HighlightPaint { get; set; } = new SKPaint { Color = SKColors.Red.WithAlpha(128), StrokeWidth = 3 };
        public string SubPlotTitle { get; set; } = "Column 1";
        public string XAxisTitle { get; set; } = "Number of Touches";
        public string YAxisTitle { get; set; } = "Neuron #";
        public int MinCell { get; set; } = -100;
        public int MaxCell { get; set; } = 4200;

        void Main(string[] args)
        {
            // Reading input from user
            Console.WriteLine("Enter the filename (including path):");
            string filename = Console.ReadLine();

            Console.WriteLine("Enter the graph name:");
            string graphName = Console.ReadLine();

            Console.WriteLine("Enter the number of maximum cycles:");
            int maxCycles = int.Parse(Console.ReadLine());

            Console.WriteLine("Enter the number of highlight touches:");
            int highlightTouch = int.Parse(Console.ReadLine());

            Console.WriteLine("Enter the title of the y-axis:");
            string yAxisTitle = Console.ReadLine();

            Console.WriteLine("Enter the title of the x-axis:");
            string xAxisTitle = Console.ReadLine();

            Console.WriteLine("Enter the title of the subplot:");
            string subPlotTitle = Console.ReadLine();

            Console.WriteLine("Enter the name of the figure:");
            string figureName = Console.ReadLine();

            Console.WriteLine("Enter 'x' to plot horizontally, any other key for vertical:");
            string axis = Console.ReadLine();

            Console.WriteLine("Enter the minimum range of the cells (optional, press Enter to skip):");
            string minCellInput = Console.ReadLine();
            int? minCellRange = string.IsNullOrEmpty(minCellInput) ? (int?)null : int.Parse(minCellInput);

            Console.WriteLine("Enter the maximum range of the cells (optional, press Enter to skip):");
            string maxCellInput = Console.ReadLine();
            int? maxCellRange = string.IsNullOrEmpty(maxCellInput) ? (int?)null : int.Parse(maxCellInput);

            var dataSets = ReadDataFromCsv(filename);

            var maxCell = maxCellRange ?? dataSets.Max(set => set.Max()) + 100;
            var minCell = minCellRange ?? dataSets.Min(set => set.Min()) - 100;

            // Plotting
            if (axis.ToLower() == "x")
            {
                PlotActivityHorizontally(dataSets, highlightTouch, maxCell, minCell);
            }
            else
            {
                PlotActivityVertically(dataSets, highlightTouch, maxCell, minCell);
            }
        }
        List<List<int>> ReadDataFromCsv(string filename)
        {
            var dataSets = new List<List<int>>();
            using (var reader = new StreamReader(filename))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');
                    var dataSet = new List<int>();

                    foreach (var value in values)
                    {
                        if (int.TryParse(value, out int result))
                        {
                            dataSet.Add(result);
                        }
                    }

                    if (dataSet.Count > 0)
                    {
                        dataSets.Add(dataSet);
                    }
                }
            }
            return dataSets;
        }
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