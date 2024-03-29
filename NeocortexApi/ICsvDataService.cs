using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeocortexApi
{
    public interface ICsvDataService
    {
        (List<HashSet<int>>, int, int) ReadDataFromCsvContent(string csvContent);
    }
}
