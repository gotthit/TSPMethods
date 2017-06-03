using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salesman.Utility
{
    public class DSU
    {
        private List<Int32> parent;

        private List<Int32> rang;

        public DSU(Int32 size)
        {
            parent = new List<Int32>();
            rang = new List<Int32>();
            for (Int32 i = 0; i < size; ++i)
            {
                parent.Add(i);
                rang.Add(0);
            }
        }

        public Int32 Get(Int32 x)
        {
            if (parent[x] != x)
            {
                parent[x] = Get(parent[x]);
            }
            return parent[x];
        }

        public void Merge(Int32 x, Int32 y)
        {
            x = Get(x);
            y = Get(y);

            if (x != y)
            {
                if (rang[x] == rang[y])
                {
                    ++rang[x]; 
                }
                if (rang[x] > rang[y])
                {
                    parent[y] = x;
                }
                else
                {
                    parent[x] = y;
                }
            }
        }

        public Boolean IsInSame(Int32 x, Int32 y)
        {
            return Get(x) == Get(y);
        }
    }
}
