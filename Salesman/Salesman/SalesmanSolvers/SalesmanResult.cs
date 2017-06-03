using Salesman.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salesman.SalesmanSolvers
{
    public class SalesmanResult
    {
        public List<Int32> Path;
        public Int64 Length;

        public SalesmanResult()
        {
            this.Path = new List<Int32>();
            this.Length = 0;
        }

        public SalesmanResult(List<Int32> path, Int64 length)
        {
            this.Path = path;
            this.Length = length;
        }

        public void Reculc(List<List<Int64>> graph)
        {
            Length = 0;
            for (Int32 i = 0; i < Path.Count - 1; ++i)
            {
                Length = Utils.SafeAdd(Length, graph[Path[i]][Path[i + 1]]);
            }
            Length = Utils.SafeAdd(Length, graph[Path.GetLast()][Path[0]]);
        }

        public SalesmanResult Clone()
        {
            SalesmanResult copy = new SalesmanResult(new List<Int32>(), Length);
            foreach (Int32 numb in Path)
            {
                copy.Path.Add(numb);
            }
            return copy;
        }
    }
}
