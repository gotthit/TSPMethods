using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salesman.Utility
{
    public class Graph
    {
        public const Int64 INF_EDGE = Int64.MaxValue / 10;

        public List<List<Int64>> AjacencyMatrix { get; }

        private Int32 isSymetric = 0;

        public Graph(List<List<Int64>> matrix)
        {
            AjacencyMatrix = matrix;
        }

        public Boolean IsSymetric
        {
            get
            {
                if (isSymetric == 0)
                {
                    isSymetric = 1;
                    for (Int32 i = 0; i < AjacencyMatrix.Count; ++i)
                    {
                        for (Int32 j = i + 1; j < AjacencyMatrix.Count; ++j)
                        {
                            if (AjacencyMatrix[i][j] != AjacencyMatrix[j][i])
                            {
                                isSymetric = -1;
                                return false;
                            }
                        }
                    }
                }
                return isSymetric == 1;
            }
        }

        public static Graph RandomGraph(Int32 vertexCount, Double edgePropability, Int32 minLength, Int32 maxLength, Boolean undirected = false)
        {
            Random random = new Random(DateTime.Now.Millisecond ^ DateTime.Now.DayOfYear + DateTime.Now.Minute);
            List<List<Int64>> matrix = new List<List<Int64>>();
            for (Int32 i = 0; i < vertexCount; ++i)
            {
                matrix.Add(new List<Int64>());
                for (Int32 j = 0; j < vertexCount; ++j)
                {
                    if (j < i && undirected)
                    {
                        matrix[i].Add(matrix[j][i]);
                    }
                    else if (i != j && random.NextDouble() <= edgePropability)
                    {
                        matrix[i].Add(random.Next(minLength, maxLength));
                    }
                    else
                    {
                        matrix[i].Add(Graph.INF_EDGE);
                    }
                }
            }
            return new Graph(matrix);
        }

        public static MetricGraph MetricGraph(List<Point> points)
        {
            List<List<Int64>> matrix = new List<List<Int64>>();
            for (Int32 i = 0; i < points.Count; ++i)
            {
                matrix.Add(new List<Int64>());
                for (Int32 j = 0; j < points.Count; ++j)
                {
                    matrix[i].Add(Graph.INF_EDGE);
                }
            }
            for (Int32 i = 0; i < points.Count; ++i)
            {
                for (Int32 j = 0; j < points.Count; ++j)
                {
                    matrix[i][j] = points[i].DistanceTo(points[j]);
                }
                matrix[i][i] = Graph.INF_EDGE;
            }
            return new MetricGraph(matrix);
        }

        public static MetricGraph MetricGraph(Int32 vertexCount, Int32 XMin, Int32 XMax, Int32 YMin, Int32 YMax)
        {
            Random random = new Random(DateTime.Now.Millisecond ^ DateTime.Now.DayOfYear + DateTime.Now.Minute);
            List<Point> points = new List<Point>();
            for (Int32 i = 0; i < vertexCount; ++i)
            {
                points.Add(new Point(random.Next(XMin, XMax), random.Next(YMin, YMax)));
            }
            return MetricGraph(points);
        }

        public static Graph HamiltonianGraph(Int32 vertexCount, Double edgePropability, Int32 minLength, Int32 maxLength)
        {
            Random random = new Random(DateTime.Now.Millisecond ^ DateTime.Now.DayOfYear + DateTime.Now.Minute);
            List<Int32> permutation = new List<Int32>();
            for (Int32 i = 0; i < vertexCount; ++i)
            {
                permutation[i] = i;
            }
            Utils.Shufle<Int32>(permutation);

            Graph graph = RandomGraph(vertexCount, edgePropability, minLength, maxLength);
            for (Int32 i = 0; i < permutation.Count - 1; ++i)
            {
                graph.AjacencyMatrix[permutation[i]][permutation[i + 1]] = random.Next(minLength, maxLength);
            }
            graph.AjacencyMatrix[permutation.GetLast()][0] = random.Next(minLength, maxLength);

            return graph;
        }

        public static Graph HamiltonGraph(Int32 vertexCount, Int32 variants, Int32 minLength, Int32 maxLength)
        {
            Random random = new Random(DateTime.Now.Millisecond ^ DateTime.Now.DayOfYear + DateTime.Now.Minute);

            List<List<Int64>> matrix = new List<List<Int64>>();
            for (Int32 i = 0; i < vertexCount; ++i)
            {
                matrix.Add(new List<Int64>());
                for (Int32 j = 0; j < vertexCount; ++j)
                {
                    matrix[i].Add(Graph.INF_EDGE);
                }
            }

            List<Int32> permutation = new List<Int32>();
            for (Int32 i = 0; i < vertexCount; ++i)
            {
                permutation[i] = i;
            }
            for (Int32 j = 0; j < variants; ++j)
            {
                Utils.Shufle<Int32>(permutation);
                for (Int32 i = 0; i < permutation.Count - 1; ++i)
                {
                    matrix[permutation[i]][permutation[i + 1]] = random.Next(minLength, maxLength);
                }
                matrix[permutation.GetLast()][0] = random.Next(minLength, maxLength);
            }
            return new Graph(matrix);
        }
    }
}
