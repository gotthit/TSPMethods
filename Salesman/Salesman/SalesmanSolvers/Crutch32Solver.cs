using Salesman.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salesman.SalesmanSolvers
{
    public class Crutch32Solver : ThreeTwoMetricSolver
    {
        protected override List<Int32> Matching(HashSet<Int32> vertices)
        {
            List<Int32> matching = base.Matching(vertices);

            Int32 cnt = 0;
            while (true)
            {
                List<Int64> distance = new List<Int64>();
                List<Int32> parent = new List<Int32>();
                Int32 vertexCount = 0;
                for (Int32 i = 0; i < graph.Count; ++i)
                {
                    if (matching[i] != -1)
                    {
                        ++vertexCount;
                        distance.Add(0);
                    }
                    else
                    {
                        distance.Add(Graph.INF_EDGE);
                    }
                    parent.Add(-1);
                }
                HashSet<Pair<Int32, Int32>> set = new HashSet<Pair<Int32, Int32>>();

                Int32 syclePoint = -1;
                for (Int32 i = 0; i < vertexCount; ++i)
                {
                    syclePoint = -1;
                    for (Int32 from = 0; from < graph.Count; ++from)
                    {
                        for (Int32 to = 0; to < graph.Count; ++to)
                        {
                            if (from != to && matching[to] != -1 && matching[from] != -1 && matching[to] != from && 
                                !set.Contains(new Pair<Int32, Int32>(to, matching[to])))
                            {
                                if (distance[matching[to]] > Utils.SafeAdd(distance[from], graph[from][to] - graph[to][matching[to]]))
                                {
                                    distance[matching[to]] = Math.Max(-Graph.INF_EDGE, Utils.SafeAdd(distance[from], graph[from][to] - graph[to][matching[to]]));
                                    parent[to] = from;
                                    syclePoint = matching[to];

                                    set.Add(new Pair<Int32, Int32>(matching[to], to));
                                }
                            }
                        }
                    }
                }

                if (syclePoint == -1)
                {
                    break;
                }
                else
                {
                    List<Int32> debuug = base.Matching(vertices);

                    for (Int32 i = 0; i < vertexCount; ++i)
                    {
                        syclePoint = parent[matching[syclePoint]];
                    }
                    Int32 last = -1;
                    Int32 to;
                    for (Int32 cur = syclePoint; ; cur = parent[to])
                    {
                        to = matching[cur];
                        if (last != -1)
                        {
                            matching[last] = cur;
                            matching[cur] = last;
                        }

                        if (cur == syclePoint && last != -1)
                        {
                            break;
                        }
                        last = to;
                    }
                }

                ++cnt;
            }
            Console.WriteLine("                                                 " + cnt);

            return matching;
        }
    }
}
