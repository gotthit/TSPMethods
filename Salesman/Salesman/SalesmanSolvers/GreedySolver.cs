using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Salesman.Utility;
using static Salesman.Utility.Graph;

namespace Salesman.SalesmanSolvers
{
    class GreedySolver : SalesmanSolver
    {
        private Boolean iterate;

        public GreedySolver(Boolean iterate = true)
        {
            this.iterate = iterate;
        }

        protected override SalesmanResult GetBestPath(Graph inputGraph)
        {
            List<List<Int64>> graph = inputGraph.AjacencyMatrix;
            SalesmanResult best = new SalesmanResult(new List<Int32>(), Graph.INF_EDGE);

            for (Int32 start = 0; start < (iterate ? graph.Count : 1); ++start)
            {
                SalesmanResult result = new SalesmanResult();
                HashSet<Int32> used = new HashSet<Int32>();

                Int32 curNode = start;
                Int64 minLength;
                Int32 toNode;

                used.Add(curNode);
                result.Path.Add(curNode);

                for (int i = 0; i < graph.Count - 1; ++i)
                {
                    toNode = -1;
                    minLength = Graph.INF_EDGE;
                    for (Int32 nextNode = 0; nextNode < graph[curNode].Count; ++nextNode)
                    {
                        Int64 length = graph[curNode][nextNode];

                        if (!used.Contains(nextNode) && length <= minLength)
                        {
                            minLength = length;
                            toNode = nextNode;
                        }
                    }
                    result.Length = Utils.SafeAdd(minLength, result.Length);
                    curNode = toNode;
                    used.Add(curNode);
                    result.Path.Add(curNode);
                }
                result.Length = Utils.SafeAdd(graph[curNode][start], result.Length);

                if (result.Length <= best.Length)
                {
                    best = result;
                }
            }

            return best;
        }
    }
}
