using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Salesman.Utility;

namespace Salesman.SalesmanSolvers
{
    public class WrongBBSolver : SalesmanSolver
    {
        private SalesmanResult record;

        private List<List<Int64>> graph;

        private Int64 loverBound(List<Int32> passed, ISet<Int32> blocked, Int32 last, Int64 passedLength)
        {
            List<Int64> alpha = new List<Int64>();
            List<Int64> beta = new List<Int64>();

            for (Int32 i = 0; i < graph.Count; ++i)
            {
                Boolean exist = false;
                alpha.Add(Graph.INF_EDGE);
                if (!blocked.Contains(i))
                {
                    for (Int32 j = 0; j < graph.Count; ++j)
                    {
                        if (i != j && ((j == passed[0] && i != last) || (!blocked.Contains(j) && j != last)))
                        {
                            alpha[i] = Math.Min(graph[i][j], alpha[i]);
                            exist = true;
                        }
                    }
                }
                if (!exist)
                {
                    alpha[i] = 0;
                }
            }
            for (Int32 j = 0; j < graph.Count; ++j)
            {
                Boolean exist = false;
                beta.Add(Graph.INF_EDGE);
                if (j == passed[0] || (!blocked.Contains(j) && j != last))
                {
                    for (Int32 i = 0; i < graph.Count; ++i)
                    {
                        if (i != j && ((last == i && j != passed[0]) || (!blocked.Contains(i) && i != last)))
                        {
                            beta[j] = Math.Min(graph[i][j] - alpha[i], beta[j]);
                            exist = true;
                        }
                    }
                }
                if (!exist)
                {
                    beta[j] = 0;
                }
            }
            Int64 result = passedLength;
            for (Int32 i = 0; i < graph.Count; ++i)
            {
                result = Utils.SafeAdd(result, alpha[i]);
                result = Utils.SafeAdd(result, beta[i]);
            }
            return result;
        }

        private void goDown(List<Int32> passed, ISet<Int32> blocked, Int64 passedLength)
        {
            if (passed.Count == graph.Count && record.Length > Utils.SafeAdd(graph[passed.GetLast()][passed[0]], passedLength))
            {
                record.Path.Clear();
                foreach (Int32 numb in passed)
                {
                    record.Path.Add(numb);
                }
                record.Length = Utils.SafeAdd(graph[passed.GetLast()][passed[0]], passedLength);
            }
            if (passed.Count < graph.Count)
            {
                List<Pair<Int64, Int32>> branches = new List<Pair<Int64, Int32>>();

                for (Int32 next = 0; next < graph.Count; ++next)
                {
                    if (!blocked.Contains(next))
                    {
                        Int64 lover = loverBound(passed, blocked, next, Utils.SafeAdd(passedLength, graph[passed.GetLast()][next]));
                        branches.Add(new Pair<Int64, Int32>(lover, next));
                    }
                }
                branches.Sort();
                foreach (Pair<Int64, Int32> branch in branches)
                {
                    if (branch.First >= record.Length)
                    {
                        break;
                    }

                    Int32 next = branch.Second;
                    Int64 newLength = Utils.SafeAdd(passedLength, graph[passed.GetLast()][next]);
                    blocked.Add(next);
                    passed.Add(next);
                    goDown(passed, blocked, newLength);
                    passed.RemoveAt(passed.Count - 1);
                    blocked.Remove(next);
                }
            }
        }

        protected override SalesmanResult GetBestPath(Graph inputGraph)
        {
            graph = inputGraph.AjacencyMatrix;
            record = new SalesmanResult(new List<Int32>(), Graph.INF_EDGE);

            goDown(new List<Int32>() { 0 }, new HashSet<Int32>() { 0 }, 0);

            return record;
        }
    }
}
