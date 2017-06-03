using Salesman.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salesman.SalesmanSolvers
{
    public class MixedOptSolver : GradientDescentSolver
    {
        public MixedOptSolver() : base()
        {
        }

        public MixedOptSolver(SalesmanSolver solver) : base(solver)
        {
        }

        public override Boolean FindNewState(Graph inputGraph, SalesmanResult state)
        {
            if (!inputGraph.IsSymetric)
            {
                return new ThreeOptSolver().FindNewState(inputGraph, state);
            }
            Boolean found = new TwoOptSolver().FindNewState(inputGraph, state);
            if (!found)
            {
                found = new ThreeOptSolver().FindNewState(inputGraph, state);
            }
            return found;
        }
    }
}
