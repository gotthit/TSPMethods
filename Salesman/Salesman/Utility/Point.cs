using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salesman.Utility
{
    public class Point
    {
        Double X;
        Double Y;

        public Point(Int32 x, Int32 y)
        {
            X = x;
            Y = y;
        }

        public virtual Int64 DistanceTo(Point to)
        {
            return (Int64)Math.Sqrt((X - to.X) * (X - to.X) + (Y - to.Y) * (Y - to.Y));
        }
    }
}
