using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeocortexApi
{
    public interface ISdrDrawerLib
    {
        Task<string> GenerateSDRChartAsync(string csvContent, bool useFileInput, int highlightTouch, string figureName, string xAxisTitle,
            string yAxisTitle, bool isHorizontal, int? maxCycles);
    }
}
