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
        public static bool IsInRadius(float CurX, float CurY, AwoseAgent obj, float radius)
        {
            return (Math.Sqrt(Math.Pow(CurX - obj.X, 2) + Math.Pow(CurY - obj.Y, 2)) <= radius);
        }

        public static float FirstSpace(AwoseAgent planet, AwoseAgent satellite)
        {
            double distance = Math.Pow(planet.X - satellite.X, 2) + Math.Pow(planet.Y - satellite.Y, 2);
            float FSV = (float)Math.Sqrt(Awose.ConstG * planet.Weight / Math.Sqrt(distance));
            return FSV;
        }

        public static void Gravity(AwoseAgent agent1, AwoseAgent agent2, ref double forceX, ref double forceY, double distance)
        {
            //double distance = Math.Pow(agent1.X - agent2.X, 2) + Math.Pow(agent1.Y - agent2.Y, 2);
            double force = -agent1.Weight * agent2.Weight * Awose.ConstG / distance;
            forceX = force * (agent1.X - agent2.X) / Math.Sqrt(distance);
            forceY = force * (agent1.Y - agent2.Y) / Math.Sqrt(distance);
        }
        public static void Electrical(AwoseAgent agent1, AwoseAgent agent2, ref double forceX, ref double forceY, double distance)
        {
            double force = agent1.Charge * agent2.Charge * Awose.ConstE / distance;
            forceX = force * (agent1.X - agent2.X) / Math.Sqrt(distance);
            forceY = force * (agent1.Y - agent2.Y) / Math.Sqrt(distance);
        }

        public static int BruteRound(float number, float divider)
        {
            if (divider == 0) return 0;
            return (int)Math.Round(number) / (int)divider * (int)divider;
        }
    }
}
