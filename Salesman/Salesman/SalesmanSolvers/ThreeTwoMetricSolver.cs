using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Salesman.Utility;

namespace Salesman.SalesmanSolvers
{
    public class ThreeTwoMetricSolver : ApproximateMetricSolver
    {
        protected override List<List<Int32>> EulerGraph(List<Edge> spanningTree)
        {
            List<Int32> rangs = new List<Int32>();
            for (Int32 i = 0; i < graph.Count; ++i)
            {
                rangs.Add(0);
            }
            foreach (Edge edge in spanningTree)
            {
                ++rangs[edge.From];
                ++rangs[edge.To];
            }
            HashSet<Int32> vertices = new HashSet<Int32>();
            for (Int32 i = 0; i < rangs.Count; ++i)
            {
                if (rangs[i] % 2 != 0)
                {
                    vertices.Add(i);
                }
            }
            List<Int32> matching = Matching(vertices);

            List<List<Int32>> eulerGraph = new List<List<Int32>>();
            for (Int32 i = 0; i < graph.Count; ++i)
            {
                eulerGraph.Add(new List<Int32>());
            }
            foreach (Edge edge in spanningTree)
            {
                eulerGraph[edge.From].Add(edge.To);
                eulerGraph[edge.To].Add(edge.From);
            }
            for (Int32 i = 0; i < matching.Count; ++i)
            {
                if (matching[i] != -1)
                {
                    eulerGraph[i].Add(matching[i]);
                }
            }

            return eulerGraph;
        }

        protected virtual List<Int32> Matching(HashSet<Int32> vertices)
        {
            List<Edge> edges = new List<Edge>();
            for (Int32 i = 0; i < graph.Count; ++i)
            {
                for (Int32 j = 0; j < graph.Count; ++j)
                {
                    if (vertices.Contains(i) && vertices.Contains(j))
                    {
                        edges.Add(new Edge(i, j, graph[i][j]));
                    }
                }
            }
            edges.Sort();
            List<Int32> matching = new List<Int32>();
            for (Int32 i = 0; i < graph.Count; ++i)
            {
                matching.Add(-1);
            }
            foreach (Edge edge in edges)
            {
                if (matching[edge.From] == -1 && matching[edge.To] == -1)
                {
                    matching[edge.From] = edge.To;
                    matching[edge.To] = edge.From;
                }
            }
            return matching;
        }
    }
}
