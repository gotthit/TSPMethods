using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salesman.Utility
{
    public class MetricGraph : Graph
    {
        public MetricGraph(List<List<Int64>> matrix) : base(matrix)
        {
        }
    }
}
