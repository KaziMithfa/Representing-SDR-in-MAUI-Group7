using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Services
{
    public class CsvDataService
    {
        public (List<HashSet<int>>, int, int) ReadDataFromCsv(string filename)
        {
            var dataSets = new List<HashSet<int>>();
            var allCells = new List<int>();
            using (var reader = new StreamReader(filename))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                     .Select(v => v.Trim())
                                     .Where(v => !string.IsNullOrWhiteSpace(v) && int.TryParse(v, out _))
                                     .Select(int.Parse)
                                     .ToList();

                    if (values.Count > 0)
                    {
                        var cell = new HashSet<int>(values);
                        dataSets.Add(cell);
                        allCells.AddRange(cell);
                    }
                }
            }
            var maxCell = allCells.Count > 0 ? allCells.Max() + 100 : 0;
            var minCell = allCells.Count > 0 ? allCells.Min() - 100 : 0;

            return (dataSets, maxCell, minCell);
        }
    }
}
