using Salesman.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salesman.SalesmanSolvers
{
    public class ThreeOptSolver : GradientDescentSolver
    {
        public ThreeOptSolver() : base()
        {
        }

        public ThreeOptSolver(SalesmanSolver solver) : base(solver)
        {
        }

        public static Boolean CheckTransition(Int32 from1, Int32 mid, Int32 to1, Int32 from2, Int32 to2)
        {
            return from1 != from2 && to1 != to2;
        }

        public static Int64 Difference(List<List<Int64>> graph, Int32 from1, Int32 mid, Int32 to1, Int32 from2, Int32 to2)
        {
            return Utils.SafeAdd(graph[from1][mid], graph[mid][to1], graph[from2][to2]) -
                Utils.SafeAdd(graph[from1][to1], graph[from2][mid], graph[mid][to2]);
        }

        public static void GoToState(List<List<Int64>> graph, SalesmanResult state, Int32 from1, Int32 mid, Int32 to1, Int32 from2, Int32 to2)
        {
            Int64 diff = Difference(graph, state.Path[from1], state.Path[mid], state.Path[to1], state.Path[from2], state.Path[to2]);

            Int32 midV = state.Path[mid];
            state.Path.RemoveAt(mid);
            if (mid < to2)
            {
                state.Path.Insert(to2 - 1, midV);
            }
            else
            {
                state.Path.Insert(to2, midV);
            }
            if (state.Length != Graph.INF_EDGE)
            {
                state.Length -= diff;
            }
            else
            {
                state.Reculc(graph);
            }
        }

        public override Boolean FindNewState(Graph inputGraph, SalesmanResult state)
        {
            List<List<Int64>> graph = inputGraph.AjacencyMatrix;

            for (Int32 from1 = 0; from1 < graph.Count; ++from1)
            {
                for (Int32 from2 = 0; from2 < graph.Count; ++from2)
                {
                    Int32 mid = (from1 + 1) % state.Path.Count;
                    Int32 to1 = (from1 + 2) % state.Path.Count;
                    Int32 to2 = (from2 + 1) % state.Path.Count;

                    if (CheckTransition(from1, mid, to1, from2, to2) &&
                        Difference(graph, state.Path[from1], state.Path[mid], state.Path[to1], state.Path[from2], state.Path[to2]) > 0)
                    {
                        GoToState(graph, state, from1, mid, to1, from2, to2);

                        return true;
                    }
                }
            }
            return false;
        }
    }
}
