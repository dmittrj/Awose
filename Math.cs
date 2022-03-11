using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awose
{
    class Math
    {
        public static int Normilize(int min, int max, int current)
        {
            if (current < min) return min;
            if (current > max) return max;
            return current;
        }
    }
}
