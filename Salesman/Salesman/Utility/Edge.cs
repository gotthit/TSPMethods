using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salesman.Utility
{
    public struct Edge : IComparable<Edge>
    {
        public Int64 Length;

        public Int32 From;

        public Int32 To;

        public Edge(Int32 from, Int32 to, Int64 length)
        {
            From = from;
            To = to;
            Length = length;
        }

        public int CompareTo(Edge other)
        {
            return Length.CompareTo(other.Length);
        }
    }
}
