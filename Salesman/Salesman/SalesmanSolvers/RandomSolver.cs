using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Salesman.Utility;
using static Salesman.Utility.Graph;

namespace Salesman.SalesmanSolvers
{
    class RandomSolver : SalesmanSolver
    {
        private readonly Int32 steps;

        public RandomSolver(Int32 steps = 100) : base()
        {
            this.steps = steps;
        }

        protected override SalesmanResult GetBestPath(Graph inputGraph)
        {
            List<List<Int64>> graph = inputGraph.AjacencyMatrix;
            SalesmanResult best = new SalesmanResult(new List<Int32>(), Graph.INF_EDGE);

            for (Int32 i = 0; i < steps; ++i)
            {
                SalesmanResult result = GetRandom(inputGraph);

                if (result.Length <= best.Length)
                {
                    best = result;
                }
            }
            return best;
        }

        public SalesmanResult GetRandom(Graph inputGraph)
        {
            Random random = new Random(DateTime.Now.Millisecond ^ DateTime.Now.DayOfYear + DateTime.Now.Minute);

            List<List<Int64>> graph = inputGraph.AjacencyMatrix;

            SalesmanResult result = new SalesmanResult();
            HashSet<Int32> used = new HashSet<Int32>();

            Int32 curNode = 0;
            Int32 toNode;

            used.Add(curNode);
            result.Path.Add(curNode);

            for (int i = 0; i < graph.Count - 1; ++i)
            {
                toNode = -1;
                Int32 total = 0;
                for (Int32 nextNode = 0; nextNode < graph[curNode].Count; ++nextNode)
                {
                    if (!used.Contains(nextNode))
                    {
                        ++total;
                    }
                }
                Int32 curi = 0, toStop = random.Next(0, total);
                for (Int32 nextNode = 0; nextNode < graph[curNode].Count; ++nextNode)
                {
                    if (!used.Contains(nextNode))
                    {
                        if (curi == toStop)
                        {
                            toNode = nextNode;
                            break;
                        }
                        ++curi;
                    }
                }

                result.Length = Utils.SafeAdd(graph[curNode][toNode], result.Length);
                curNode = toNode;
                used.Add(curNode);
                result.Path.Add(curNode);
            }
            result.Length = Utils.SafeAdd(graph[curNode][0], result.Length);
            return result;
        }
    }
}
