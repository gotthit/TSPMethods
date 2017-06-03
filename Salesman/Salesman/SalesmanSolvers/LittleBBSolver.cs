using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Salesman.Utility;

namespace Salesman.SalesmanSolvers
{
    public class LittleBBSolver : SalesmanSolver
    {
        private struct Brunch
        {
            public Int32 zeroCol;
            public Int32 zeroRow;
            public Int64 loverBound;
            public Int64 zeroVal;

            public Brunch(Int32 zeroCol, Int32 zeroRow, Int64 loverBound, Int64 zeroVal)
            {
                this.zeroCol = zeroCol;
                this.zeroRow = zeroRow;
                this.zeroVal = zeroVal;
                this.loverBound = loverBound;
            }
        }

        private SalesmanResult record;

        private List<List<Int64>> graph;

        private Boolean symetric;

        private Brunch loverBound(List<List<Int64>> matrix, Dictionary<Int32, Int32> fromTo, Dictionary<Int32, Int32> toFrom)
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

            return new Brunch(mxZoeroCol, mxZeroRow, loverBound, zeroVal);
        }

        private List<List<Int64>> reduceMatrix(List<List<Int64>> matrix, Dictionary<Int32, Int32> fromTo, Dictionary<Int32, Int32> toFrom, Int32 col, Int32 row)
        {
            List<List<Int64>> reduced = new List<List<Int64>>();
            for (Int32 i = 0; i < matrix.Count; ++i)
            {
                reduced.Add(new List<Int64>());
                for (Int32 j = 0; j < matrix.Count; ++j)
                {
                    reduced[i].Add(Graph.INF_EDGE);
                    if ((i == col) == (j == row))
                    {
                        reduced[i][j] = matrix[i][j];
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
            reduced[last][first] = Graph.INF_EDGE;
            return reduced;
        }

        private void goDown(List<List<Int64>> matrix, Dictionary<Int32, Int32> fromTo, Dictionary<Int32, Int32> toFrom, Int64 curBound)
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

                record.Path.Clear();
                record.Length = 0;
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
            }
            else
            {
                Brunch brunch = loverBound(matrix, fromTo, toFrom);

                if (Utils.SafeAdd(curBound, brunch.loverBound) < record.Length)
                {
                    Int32 from = brunch.zeroCol;
                    Int32 to = brunch.zeroRow;
                    fromTo[from] = to;
                    toFrom[to] = from;
                    List<List<Int64>> newMatrix = reduceMatrix(matrix, fromTo, toFrom, from, to);

                    goDown(newMatrix, fromTo, toFrom, Utils.SafeAdd(curBound, brunch.loverBound));

                    fromTo.Remove(from);
                    toFrom.Remove(to);

                    if (Utils.SafeAdd(curBound, brunch.loverBound, brunch.zeroVal) < record.Length)
                    {
                        matrix[from][to] = Graph.INF_EDGE;
                        if (fromTo.Count == 0 && symetric)
                        {
                            matrix[to][from] = Graph.INF_EDGE;
                        }

                        goDown(matrix, fromTo, toFrom, Utils.SafeAdd(curBound, brunch.loverBound));
                    }
                }
            }
        }

        protected override SalesmanResult GetBestPath(Graph inputGraph)
        {
            graph = inputGraph.AjacencyMatrix;
            symetric = inputGraph.IsSymetric;
            record = new SalesmanResult(new List<Int32>(), Graph.INF_EDGE);

            List<List<Int64>> matrix = new List<List<Int64>>();
            foreach (var row in graph)
            {
                matrix.Add(new List<Int64>());
                foreach (var numb in row)
                {
                    matrix.GetLast().Add(numb);
                }
            }

            goDown(matrix, new Dictionary<Int32, Int32>(1000), new Dictionary<Int32, Int32>(1000), 0);

            return record;
        }
    }
}
