using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Salesman.Utility;

namespace Salesman.SalesmanSolvers
{
    public class ApproximateBBSolver : SalesmanSolver
    {
        private List<List<Int64>> graph;

        private Pair<Int32, Int32> loverBound(List<List<Int64>> matrix, Dictionary<Int32, Int32> fromTo, Dictionary<Int32, Int32> toFrom)
        {
            List<Int64> firstColVal = new List<Int64>();
            List<Int64> firstRowVal = new List<Int64>();
            List<Int64> secondColVal = new List<Int64>();
            List<Int64> secondRowVal = new List<Int64>();

            for (Int32 i = 0; i < matrix.Count; ++i)
            {
                firstColVal.Add(Graph.INF_EDGE);
                secondColVal.Add(Graph.INF_EDGE);
                for (Int32 j = 0; j < matrix.Count; ++j)
                {
                    if (firstColVal[i] > matrix[i][j])
                    {
                        firstColVal[i] = matrix[i][j];
                    }
                }
            }
            for (Int32 i = 0; i < matrix.Count; ++i)
            {
                for (Int32 j = 0; j < matrix.Count; ++j)
                {
                    matrix[i][j] -= firstColVal[i];
                }
            }

            for (Int32 j = 0; j < matrix.Count; ++j)
            {
                firstRowVal.Add(Graph.INF_EDGE);
                secondRowVal.Add(Graph.INF_EDGE);
                for (Int32 i = 0; i < matrix.Count; ++i)
                {
                    if (firstRowVal[j] > matrix[i][j])
                    {
                        firstRowVal[j] = matrix[i][j];
                    }
                }
            }
            for (Int32 j = 0; j < matrix.Count; ++j)
            {
                for (Int32 i = 0; i < matrix.Count; ++i)
                {
                    matrix[i][j] -= firstRowVal[j];
                }
            }

            Int64 smallest = Graph.INF_EDGE;
            for (Int32 i = 0; i < matrix.Count; ++i)
            {
                smallest = Graph.INF_EDGE;
                for (Int32 j = 0; j < matrix.Count; ++j)
                {
                    if (matrix[i][j] < smallest)
                    {
                        secondColVal[i] = smallest;
                        smallest = matrix[i][j];
                    }
                    else if (matrix[i][j] < secondColVal[i])
                    {
                        secondColVal[i] = matrix[i][j];
                    }
                }
            }
            for (Int32 j = 0; j < matrix.Count; ++j)
            {
                smallest = Graph.INF_EDGE;
                for (Int32 i = 0; i < matrix.Count; ++i)
                {
                    if (matrix[i][j] < smallest)
                    {
                        secondRowVal[j] = smallest;
                        smallest = matrix[i][j];
                    }
                    else if (matrix[i][j] < secondRowVal[j])
                    {
                        secondRowVal[j] = matrix[i][j];
                    }
                }
            }


            Int32 mxZoeroCol = 0, mxZeroRow = 0;
            Int64 zeroVal = -1;
            for (Int32 i = 0; i < matrix.Count; ++i)
            {
                for (Int32 j = 0; j < matrix.Count; ++j)
                {
                    if ((!fromTo.ContainsKey(i) && !toFrom.ContainsKey(j)) &&
                        matrix[i][j] == 0 &&
                        (zeroVal < (secondColVal[i] + secondRowVal[j])))
                    {
                        zeroVal = Utils.SafeAdd(secondColVal[i], secondRowVal[j]);
                        mxZoeroCol = i;
                        mxZeroRow = j;
                    }
                }
            }
            Int64 loverBound = 0;
            for (Int32 i = 0; i < matrix.Count; ++i)
            {
                loverBound = Utils.SafeAdd(firstColVal[i] + firstRowVal[i], loverBound);
            }

            return new Pair<Int32, Int32>(mxZoeroCol, mxZeroRow);
        }

        private void reduceMatrix(List<List<Int64>> matrix, Dictionary<Int32, Int32> fromTo, Dictionary<Int32, Int32> toFrom, Int32 col, Int32 row)
        { 
            for (Int32 i = 0; i < matrix.Count; ++i)
            {
                for (Int32 j = 0; j < matrix.Count; ++j)
                {
                    if ((i == col) != (j == row))
                    {
                        matrix[i][j] = Graph.INF_EDGE * 2;
                    }
                }
            }
            Int32 first = col;
            while (toFrom.ContainsKey(first))
            {
                first = toFrom[first];
            }
            Int32 last = row;
            while (fromTo.ContainsKey(last))
            {
                last = fromTo[last];
            }
            matrix[last][first] = Graph.INF_EDGE * 2;
        }

        private SalesmanResult goDown(List<List<Int64>> matrix, Dictionary<Int32, Int32> fromTo, Dictionary<Int32, Int32> toFrom)
        {
            if (fromTo.Count == graph.Count - 1)
            {
                Int32 first = 0, last = 0;
                for (Int32 i = 0; i < matrix.Count; ++i)
                {
                    if (!fromTo.ContainsKey(i))
                    {
                        last = i;
                    }
                    if (!toFrom.ContainsKey(i))
                    {
                        first = i;
                    }
                }
                SalesmanResult record = new SalesmanResult();
                Int32 cur = first;
                record.Path.Add(cur);
                record.Length = Utils.SafeAdd(record.Length, graph[cur][fromTo[cur]]);
                cur = fromTo[cur];
                while (cur != last)
                {
                    record.Path.Add(cur);
                    record.Length = Utils.SafeAdd(record.Length, graph[cur][fromTo[cur]]);
                    cur = fromTo[cur];
                }
                record.Path.Add(last);
                record.Length = Utils.SafeAdd(record.Length, graph[last][first]);

                return record;
            }
            else
            {
                Pair<Int32, Int32> goTo = loverBound(matrix, fromTo, toFrom);

                Int32 from = goTo.First;
                Int32 to = goTo.Second;
                fromTo[from] = to;
                toFrom[to] = from;
                reduceMatrix(matrix, fromTo, toFrom, from, to);

                return goDown(matrix, fromTo, toFrom);
            }
        }

        protected override SalesmanResult GetBestPath(Graph inputGraph)
        {
            graph = inputGraph.AjacencyMatrix;

            List<List<Int64>> matrix = new List<List<Int64>>();
            foreach (var row in graph)
            {
                matrix.Add(new List<Int64>());
                foreach (var numb in row)
                {
                    matrix.GetLast().Add(numb);
                }
            }
            for (Int32 i = 0; i < matrix.Count; ++i)
            {
                matrix[i][i] = Graph.INF_EDGE * 2;
            }


            SalesmanResult res = goDown(matrix, new Dictionary<Int32, Int32>(1000), new Dictionary<Int32, Int32>(1000));

            return res;
        }
    }
}
