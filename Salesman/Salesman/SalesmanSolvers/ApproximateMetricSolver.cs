using Salesman.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salesman.SalesmanSolvers
{
    public abstract class ApproximateMetricSolver : SalesmanSolver
    {
        protected List<List<Int64>> graph;

        private List<Edge> MinSpanningTree()
        {
            List<Edge> edges = new List<Edge>();
            for (Int32 i = 0; i < graph.Count; ++i)
            {
                for (Int32 j = 0; j < graph.Count; ++j)
                {
                    edges.Add(new Edge(i, j, graph[i][j]));
                }
            }
            edges.Sort();
            DSU dsu = new DSU(graph.Count);
            List<Edge> spanningTree = new List<Edge>();
            foreach (Edge edge in edges)
            {
                if (!dsu.IsInSame(edge.From, edge.To))
                {
                    spanningTree.Add(edge);
                    dsu.Merge(edge.From, edge.To);
                }
            }
            return spanningTree;
        }

        private List<Int32> EulerSycle(List<List<Int32>> eulerGraph)
        {
            List<Int32> eulerSycle = new List<Int32>();
            Stack<Int32> stack = new Stack<Int32>();
            stack.Push(0);
            while (stack.Count != 0)
            {
                Int32 cur = stack.Peek();
                if (eulerGraph[cur].Count == 0)
                {
                    eulerSycle.Add(cur);
                    stack.Pop();
                }
                else
                {
                    Int32 to = eulerGraph[cur][0];
                    stack.Push(to);
                    eulerGraph[cur].RemoveAt(0);
                    eulerGraph[to].Remove(cur);
                }
            }
            return eulerSycle;
        }

        private List<Int32> HamiltonianSycle(List<Int32> eulerSycle)
        {
            List<Int32> hamiltonSycle = new List<Int32>();
            HashSet<Int32> visited = new HashSet<Int32>();
            foreach (Int32 numb in eulerSycle)
            {
                if (!visited.Contains(numb))
                {
                    hamiltonSycle.Add(numb);
                    visited.Add(numb);
                }
            }
            return hamiltonSycle;
        }

        protected abstract List<List<Int32>> EulerGraph(List<Edge> spanningTree);

        protected override SalesmanResult GetBestPath(Graph inputGraph)
        {
            if (!(inputGraph is MetricGraph))
            {
                 throw new InvalidOperationException();
            }
            graph = inputGraph.AjacencyMatrix;
            List<Edge> mst = MinSpanningTree();
            List<List<Int32>> eulerGraph = EulerGraph(mst);

            List<Int32> path = HamiltonianSycle(EulerSycle(eulerGraph));

            SalesmanResult result = new SalesmanResult(path, 0);
            for (Int32 i = 0; i < path.Count - 1; ++i)
            {
                result.Length = Utils.SafeAdd(result.Length, graph[path[i]][path[i + 1]]);
            }
            result.Length = Utils.SafeAdd(result.Length, graph[path.GetLast()][path[0]]);

            return result;
        }
    }
}
