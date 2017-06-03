using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Salesman.Utility;

namespace Salesman.SalesmanSolvers
{
    public class SimulatedAnnealingTest : SalesmanSolver
    {
        private Temperature temperature;

        public SimulatedAnnealingTest()
        {
            temperature = new Temperature();
        }

        public SimulatedAnnealingTest(Temperature.TemperatureFunc type)
        {
            this.temperature = new Temperature(type);
        }

        public SimulatedAnnealingTest(Temperature temperature)
        {
            this.temperature = temperature;
        }

        protected override SalesmanResult GetBestPath(Graph inputGraph)
        {
            Random random = new Random(DateTime.Now.Millisecond ^ DateTime.Now.DayOfYear + DateTime.Now.Minute);

            SalesmanResult state = new RandomSolver(1).Solve(inputGraph);
            SalesmanResult best = state.Clone();

            Int32 cnt = 0;
            while (!temperature.IsCold())
            {
                ++cnt;

                for (Int32 from1 = 0; from1 < state.Path.Count && !temperature.IsCold(); ++from1)
                {
                    for (Int32 from2 = 0; from2 < state.Path.Count && !temperature.IsCold(); ++from2)
                    {
                        Int32 to1 = (from1 + 1) % state.Path.Count;
                        Int32 to2 = (from2 + 1) % state.Path.Count;

                        if (!TwoOptSolver.CheckTransition(from1, to1, from2, to2))
                        {
                            continue;
                        }
                        Int64 diff = TwoOptSolver.Difference(inputGraph.AjacencyMatrix, state.Path[from1], state.Path[to1], state.Path[from2], state.Path[to2]);

                        if (diff > 0 || random.NextDouble() <= Math.Exp((Double)diff / temperature.Current))
                        {
                            TwoOptSolver.GoToState(inputGraph.AjacencyMatrix, state, from1, to1, from2, to2);
                        }
                        if (best.Length > state.Length)
                        {
                            best = state.Clone();
                        }
                        temperature.Freezing();
                    }
                }
            }

            return best;
        }
    }
}
