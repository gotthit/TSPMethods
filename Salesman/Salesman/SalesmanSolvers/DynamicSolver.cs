using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Salesman.Utility;
using static Salesman.Utility.Graph;

namespace Salesman.SalesmanSolvers
{
    public class DynamicSolver : SalesmanSolver
    {
        protected override SalesmanResult GetBestPath(Graph inputGraph)
        {
            Int32 start = 0;

            List<List<Int64>> graph = inputGraph.AjacencyMatrix;
            List<List<Int64>> dp = new List<List<Int64>>();

            for (Int32 mask = 0; mask < 1 << graph.Count; ++mask)
            {
                dp.Add(new List<Int64>());
                for (Int32 i = 0; i < graph.Count; ++i)
                {
                    dp[mask].Add(Graph.INF_EDGE);
                }
            }
            dp[0][start] = 0;
            for (Int32 nextNode = 0; nextNode < graph[start].Count; ++nextNode)
            {
                dp[1 << nextNode][nextNode] = graph[start][nextNode];
            }


            for (Int32 mask = 0; mask < 1 << graph.Count; ++mask)
            {
                for (Int32 i = 0; i < graph.Count; ++i)
                {
                    if ((mask & (1 << i)) != 0)
                    {
                        for (Int32 nextNode = 0; nextNode < graph[i].Count; ++nextNode)
                        {
                            Int64 length = graph[i][nextNode];
                            if ((mask & (1 << nextNode)) == 0)
                            {
                                Int32 toMask = mask | (1 << nextNode);
                                dp[toMask][nextNode] = Math.Min(dp[toMask][nextNode], Utils.SafeAdd(dp[mask][i], length));
                            }
                        }
                    }
                }
            }


            SalesmanResult result = new SalesmanResult();
            result.Length = dp[(1 << graph.Count) - 1][start];
            Int32 curMask = (1 << graph.Count) - 1;
            Int32 curNode = start;
            result.Path.Add(curNode);
            while (curMask != (1 << curNode))
            {
                for (Int32 prevNode = 0; prevNode < graph[curNode].Count; ++prevNode)
                {
                    if (prevNode != curNode)
                    {
                        Int64 length = graph[prevNode][curNode];

                        if ((curMask & (1 << prevNode)) != 0 &&
                            dp[curMask][curNode] == Utils.SafeAdd(dp[curMask - (1 << curNode)][prevNode], length))
                        {
                            curMask = curMask - (1 << curNode);
                            curNode = prevNode;
                            result.Path.Add(curNode);
                            break;
                        }
                    }
                }
            }
            result.Path.Reverse();

            return result;
        }
    }
}
