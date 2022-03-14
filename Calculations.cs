using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awose
{
    class Calculations
    {
        public static int Normilize(int min, int max, int current)
        {
            if (current < min) return min;
            if (current > max) return max;
            return current;
        }
        public static bool IsInRadius(int CurX, int CurY, AwoseAgent obj, float radius)
        {
            return (Math.Sqrt(Math.Pow(CurX - obj.X, 2) + Math.Pow(CurY - obj.Y, 2)) > radius);
        }

        public static void Gravity(AwoseAgent agent1, AwoseAgent agent2, ref double forceX, ref double forceY)
        {
            double distance = Math.Pow(agent1.X - agent2.X, 2) + Math.Pow(agent1.Y - agent2.Y, 2);

        }
    }
}
