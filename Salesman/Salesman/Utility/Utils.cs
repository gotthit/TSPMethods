using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salesman.Utility
{
    public static class Utils
    {
        public static void Swap<T>(ref T a, ref T b)
        {
            T c = a;
            a = b;
            b = c;
        }

        public static void Shufle<T>(List<T> permutation)
        {
            Random random = new Random(DateTime.Now.Millisecond ^ DateTime.Now.DayOfYear + DateTime.Now.Minute);
            for (Int32 i = permutation.Count - 1; i >= 0; --i)
            {
                Int32 j = random.Next(0, i);
                T temp = permutation[i];
                permutation[i] = permutation[j];
                permutation[j] = temp;
            }
        }

        public static Int64 SafeAdd(Int64 a, Int64 b)
        {
            return Math.Min(Graph.INF_EDGE, a + b);
        }

        public static Int64 SafeAdd(params Int64[] arr)
        {
            Int64 result = 0;
            foreach (Int64 numb in arr)
            {
                result = Math.Min(Graph.INF_EDGE, result + numb);
            }
            return result;
        }

        public static Boolean AllDifferent(params Int32[] arr)
        {
            ISet<Int32> set = new HashSet<Int32>();
            foreach (Int32 numb in arr)
            {
                if (!set.Add(numb))
                {
                    return false;
                }
            }
            return true;
        }

        public static T GetLast<T>(this IList<T> list)
        {
            return list[list.Count - 1];
        }
    }
}
