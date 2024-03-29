using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeocortexApi
{
    public class SdrDrawerLib : ISdrDrawerLib
    {
        private List<HashSet<int>> activeCellsColumn = new();
        private bool isHorizontal = false;
        private int numTouches;
        private int count;
        private int highlightTouch = 0;
        private string xAxisTitle = "X-Axis";
        private string yAxisTitle = "Y-Axis";
        private string figureName = "Activity Graph";

        private int maxCycles = 0;
        private int minCell = 0;
        private int maxCell = 100;
        private int minTouch = 0;
        private int maxTouch = 100;
        private double originalCellWidth = 15;
        private double originalCellHeight = 3;
        private double cellWidth = 15;
        private double cellHeight = 3;
        private const int cellPadding = 1;
        private double chartPadding = 110;
        private double chartWidth = 800;
        private double chartHeight = 600;
        private ICsvDataService csvDataService;

        public SdrDrawerLib(ICsvDataService csvDataService)
        {
            this.csvDataService = csvDataService;
        }

        public async Task<string> GenerateSDRChartAsync(string csvContent, bool useFileInput, int highlightTouch, string figureName, string xAxisTitle, 
            string yAxisTitle, bool isHorizontal, int? maxCycles)
        {
            var sb = new StringBuilder();

            if (!useFileInput && !string.IsNullOrWhiteSpace(csvContent))
            {
                var result = csvDataService.ReadDataFromCsvContent(csvContent);
                this.activeCellsColumn = result.Item1;
            }

            this.highlightTouch = highlightTouch - 1;
            this.figureName = figureName;
            this.xAxisTitle = xAxisTitle;
            this.yAxisTitle = yAxisTitle;
            this.isHorizontal = isHorizontal;
            this.numTouches = Math.Min(activeCellsColumn.Count, maxCycles.HasValue ?
                maxCycles.Value : 1000);
            if (isHorizontal)
            {
                this.cellWidth = originalCellHeight;
                this.cellHeight = originalCellWidth;
            }
            else
            {
                this.cellWidth = originalCellWidth;
                this.cellHeight = originalCellHeight;
            }

            CalculateChartDimensions();

            // Begin SVG element
            sb.AppendLine($"<svg id='heatmapSvg' width='{chartWidth}' height='{chartHeight}' xmlns='http://www.w3.org/2000/svg' style='border: 1px solid;'>");

            if (activeCellsColumn != null && activeCellsColumn.Any())
            {
                // Figure Name
                sb.AppendLine($"<text x='{(chartWidth / 2)}' y='30' text-anchor='middle' font-size='16'>{figureName}</text>");

                if (isHorizontal)
                {
                    for (int rowIndex = 0; rowIndex < numTouches; rowIndex++)
                    {
                        var yPos = chartHeight - (rowIndex * (cellHeight + cellPadding)) - chartPadding;
                        foreach (var cell in activeCellsColumn[rowIndex])
                        {
                            var xPos = ((cell - minCell + 1) * (cellWidth + cellPadding)) + chartPadding - 50; // Offset for axis labels
                            sb.AppendLine($"<rect x='{xPos}' y='{yPos}' width='{cellWidth}' height='{cellHeight}' fill='{GetCellColor(rowIndex)}'/>");

                            if (rowIndex == highlightTouch)
                            {
                                sb.AppendLine($"<rect x='{chartPadding}' y='{yPos}' width='{chartWidth - (2 * chartPadding)}' height='{cellHeight}' fill='rgba(255, 0, 0, 0.5)'/>");
                            }
                        }
                    }

                    // Axes
                    sb.AppendLine($"<line x1='{chartPadding - 30}' y1='30' x2='{chartPadding - 30}' y2='{chartHeight - chartPadding + 30}' stroke='black'/>");
                    sb.AppendLine($"<line x1='{chartPadding - 30}' y1='{chartHeight - chartPadding + 30}' x2='{chartWidth - chartPadding + 50}' y2='{chartHeight - chartPadding + 30}' stroke='black'/>");

                    // Axis Labels
                    var labelsVertical = GenerateAxisLabelsVertical(isHorizontal ? minTouch : minCell, isHorizontal ? maxTouch : maxCell, true);
                    foreach (var label in labelsVertical)
                    {
                        sb.AppendLine($"<text x='{chartPadding - 35}' y='{chartHeight - label.pos}' text-anchor='end' font-size='12'>{label.label}</text>");
                    }

                    var labelsHorizontal = GenerateAxisLabelsVertical(isHorizontal ? minCell : minTouch, isHorizontal ? maxCell : maxTouch, false);
                    foreach (var label in labelsHorizontal)
                    {
                        sb.AppendLine($"<text x='{label.pos}' y='{chartHeight - chartPadding + 45}' text-anchor='middle' font-size='12'>{label.label}</text>");
                    }
                }
                else
                {
                    for (int touchIndex = 0; touchIndex < numTouches; touchIndex++)
                    {
                        var xPos = (touchIndex * (cellWidth + cellPadding)) + chartPadding;
                        foreach (var cell in activeCellsColumn[touchIndex])
                        {
                            var yPos = chartHeight - ((cell - minCell + 1) * (cellHeight - 2)) - chartPadding;
                            sb.AppendLine($"<rect x='{xPos}' y='{yPos}' width='{cellWidth}' height='{cellHeight}' fill='{GetCellColor(touchIndex)}'/>");

                            if (touchIndex == highlightTouch)
                            {
                                sb.AppendLine($"<rect x='{xPos}' y='{30}' width='{cellWidth}' height='{chartHeight - chartPadding + 30}' fill='rgba(255, 0, 0, 0.5)'/>");
                            }
                        }
                    }

                    // Axes
                    sb.AppendLine($"<line x1='{chartPadding - 30}' y1='30' x2='{chartPadding - 30}' y2='{chartHeight - chartPadding + 30}' stroke='black'/>");
                    sb.AppendLine($"<line x1='{chartPadding - 30}' y1='{chartHeight - chartPadding + 30}' x2='{chartWidth - chartPadding + 50}' y2='{chartHeight - chartPadding + 30}' stroke='black'/>");

                    // Axis Labels
                    var labelsVertical = GenerateAxisLabels(isHorizontal ? minTouch : minCell, isHorizontal ? maxTouch : maxCell, true);
                    foreach (var label in labelsVertical)
                    {
                        sb.AppendLine($"<text x='{chartPadding - 35}' y='{chartHeight - chartPadding - label.pos}' text-anchor='end' font-size='12'>{label.label}</text>");
                    }

                    var labelsHorizontal = GenerateAxisLabels(isHorizontal ? minCell : minTouch, isHorizontal ? maxCell : maxTouch, false);
                    foreach (var label in labelsHorizontal)
                    {
                        sb.AppendLine($"<text x='{label.pos}' y='{chartHeight - chartPadding + 45}' text-anchor='middle' font-size='12'>{label.label}</text>");
                    }
                }

                // X and Y Axis Titles
                sb.AppendLine($"<text x='{(chartWidth / 2)}' y='{chartHeight - 5}' text-anchor='middle' font-size='14'>{xAxisTitle}</text>");
                sb.AppendLine($"<text x='15' y='{(chartHeight / 2) - 50}' transform='rotate(-90 15, {(chartHeight / 2)})' text-anchor='middle' font-size='14'>{yAxisTitle}</text>");
            }

            // End SVG element
            sb.AppendLine("</svg>");

            return sb.ToString();
        }
        private string GetCellColor(int rowIndex) => rowIndex == highlightTouch ? "red" : "lightblue";

        private void CalculateChartDimensions()
        {
            minCell = int.MaxValue;
            maxCell = int.MinValue;

            foreach (var column in activeCellsColumn)
            {
                if (column.Any())
                {
                    int currentMin = column.Min();
                    int currentMax = column.Max();

                    if (currentMin < minCell)
                        minCell = currentMin;
                    if (currentMax > maxCell)
                        maxCell = currentMax;
                }
            }

            if (minCell == int.MaxValue && maxCell == int.MinValue)
            {
                minCell = 0;
                maxCell = 0;
            }

            minTouch = 0;
            maxTouch = numTouches;

            if (isHorizontal)
            {
                int cellRangeSpan = activeCellsColumn.Max(col => col.Max()) + 1 - minCell;
                chartWidth = (cellRangeSpan * (cellWidth - 2)) + chartPadding + 200;
                chartHeight = (numTouches * (cellHeight + cellPadding)) + chartPadding + 50;
            }
            else
            {
                int cellRangeSpan = activeCellsColumn.Max(col => col.Max()) + 1 - minCell;
                chartHeight = (cellRangeSpan * (cellHeight - 2)) + chartPadding + 50;
                chartWidth = (numTouches * (cellWidth + cellPadding)) + chartPadding + 200;
            }
        }

        private IEnumerable<(string label, int pos)> GenerateAxisLabels(int min, int max, bool isVertical)
        {
            var labels = new List<(string label, int pos)>();
            if (min > 0 && min < 10)
            {
                min = 0;
            }
            int range = max - min;

            // Determine the magnitude of the range to set an appropriate step value.
            int magnitude = (int)Math.Pow(10, (int)Math.Log10(range) - 1);
            int step = (range / magnitude < 5) ? magnitude : magnitude * 5;

            string label;
            int pos;

            // Generate labels based on the calculated step value.
            for (int i = min; i <= max; i += step)
            {
                label = i.ToString();

                if (isVertical)
                {
                    // Adjust position calculation for vertical orientation
                    pos = Convert.ToInt32(((float)(i - min) * (cellHeight - 2)));
                    //pos = Convert.ToInt32((float)(i - min) / range * (chartHeight - chartPadding)) + 8;
                }
                else
                {
                    // Adjust position calculation for horizontal orientation
                    pos = Convert.ToInt32(((float)(i - min) * ((cellWidth - 1) + cellPadding)) + chartPadding);
                    //pos = Convert.ToInt32((float)(i - min) / range * (chartWidth - chartPadding)) + 20;
                }

                labels.Add((label, pos));
            }

            // Ensure the last label is always added at the end of the axis.
            if (!labels.Any(l => l.label == max.ToString()))
            {
                if (isVertical)
                {
                    pos = Convert.ToInt32(((float)(max - min) * (cellHeight - 2)));
                    //pos = Convert.ToInt32((float)(max - min) / range * (chartHeight - chartPadding));
                    labels.Add((max.ToString(), pos));
                }
                else
                {
                    pos = Convert.ToInt32(((float)(max - min) * ((cellWidth - 1) + cellPadding)) + chartPadding);
                    //pos = Convert.ToInt32((float)(max - min) / range * (chartWidth - chartPadding));
                    labels.Add((max.ToString(), pos));
                }

                return labels;
            }
            return labels;
        }

        private IEnumerable<(string label, int pos)> GenerateAxisLabelsVertical(int min, int max, bool isVertical)
        {
            var labels = new List<(string label, int pos)>();
            if (min > 0 && min < 10)
            {
                min = 0;
            }
            int range = max - min;

            // Determine the magnitude of the range to set an appropriate step value.
            int magnitude = (int)Math.Pow(10, (int)Math.Log10(range) - 1);
            int step = (range / magnitude < 5) ? magnitude : magnitude * 5;

            string label;
            int pos;

            // Generate labels based on the calculated step value.
            for (int i = min; i <= max; i += step)
            {
                label = i.ToString();

                if (isVertical)
                {
                    pos = Convert.ToInt32(((float)(i - min) * ((cellHeight - 1) + cellPadding)) + chartPadding) - 12;
                    //pos = Convert.ToInt32((float)(i - min) * ((chartWidth) )) + 8;
                }
                else
                {
                    pos = Convert.ToInt32(((float)(i - min) * (cellWidth - 2))) + 100;
                    //pos = Convert.ToInt32((float)(i - min) / range * (chartWidth - chartPadding)) + 20;
                }

                labels.Add((label, pos));
            }

            // Ensure the last label is always added at the end of the axis.
            if (!labels.Any(l => l.label == max.ToString()))
            {
                if (isVertical)
                {
                    pos = Convert.ToInt32(((float)(max - min) * ((cellHeight - 1) + cellPadding)) + chartPadding) - 12;
                    //pos = Convert.ToInt32((float)(max - min) / range * (chartHeight - chartPadding));
                    labels.Add((max.ToString(), pos));
                }
                else
                {
                    pos = Convert.ToInt32(((float)(max - min) * (cellWidth - 2))) + 120;
                    //pos = Convert.ToInt32((float)(max - min) / range * (chartWidth - chartPadding));
                    labels.Add((max.ToString(), pos));
                }

                return labels;
            }
            return labels;
        }
    }
}
