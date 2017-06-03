using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Salesman.Utility;

namespace Salesman.SalesmanSolvers
{
    public abstract class GradientDescentSolver : SalesmanSolver
    {
        private SalesmanSolver solver;

        public GradientDescentSolver() : base()
        {
            this.solver = new RandomSolver(1);
        }

        public GradientDescentSolver(SalesmanSolver solver) : base()
        {
            this.solver = solver;
        }

        protected override SalesmanResult GetBestPath(Graph inputGraph)
        {
            SalesmanResult greedyState = solver.Solve(inputGraph);

            return GradientDecent(greedyState, inputGraph);
        }

        public SalesmanResult GradientDecent(SalesmanResult someState, Graph inputGraph)
        {
            List<List<Int64>> graph = inputGraph.AjacencyMatrix;

            Boolean found = true;
            while (found)
            {
                found = FindNewState(inputGraph, someState);
            }
           
            return someState;
        }

        public abstract Boolean FindNewState(Graph graph, SalesmanResult fromTo);
    }
}
