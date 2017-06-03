using Salesman.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salesman.SalesmanSolvers
{
    public abstract class SalesmanSolver
    {
        protected abstract SalesmanResult GetBestPath(Graph inputGraph);

        public SalesmanResult Solve(Graph graph, out Int64 expendedMemory, out TimeSpan workingTime)
        {
            Int64 memoryBefore = GC.GetTotalMemory(true);
            Int64 memoryAfter;

            Stopwatch timer = new Stopwatch();
            timer.Start();

            SalesmanResult result = GetBestPath(graph);

            memoryAfter = GC.GetTotalMemory(false);

            timer.Stop();
            workingTime = timer.Elapsed;

            expendedMemory = memoryAfter - memoryBefore;

            return result;
        }

        public SalesmanResult Solve(Graph graph)
        {
            return GetBestPath(graph);
        }
    }
}
