using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Salesman.Utility;

namespace Salesman.SalesmanSolvers
{
    public class TwoMetricSolver : ApproximateMetricSolver
    {
        protected override List<List<Int32>> EulerGraph(List<Edge> spanningTree)
        {
            List<List<Int32>> eulerGraph = new List<List<Int32>>();
            for (Int32 i = 0; i < graph.Count; ++i)
            {
                eulerGraph.Add(new List<Int32>());
            }
            foreach (Edge edge in spanningTree)
            {
                eulerGraph[edge.From].Add(edge.To);
                eulerGraph[edge.To].Add(edge.From);
                eulerGraph[edge.From].Add(edge.To);
                eulerGraph[edge.To].Add(edge.From);
            }
            return eulerGraph;
        }
    }
}
