using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salesman.Utility
{
    public struct Pair<TK, TV> : IComparable<Pair<TK, TV>> where TK : IComparable<TK> where TV : IComparable<TV>
    {
        public TK First;
        public TV Second;

        public Pair(TK first, TV second)
        {
            First = first;
            Second = second;
        }

        public int CompareTo(Pair<TK, TV> other)
        {
            if (First.CompareTo(other.First) == 0)
            {
                return Second.CompareTo(other.Second);
            }
            return First.CompareTo(other.First);
        }
    }
}
