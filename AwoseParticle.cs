using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awose
{
    public class AwoseParticle
    {
        public PointParticle Location { get; set; }
        public PointParticle LocationBackup { get; set; }
        public int Lifetime { get; set; }
        public Queue<PointParticle> Trajectory { get; set; }
        public Vector Velocity { get; set; }
        public Vector Force { get; set; }
        public bool Reborning { get; set; }
        public AwoseParticle(PointParticle location)
        {
            Location = location;
            LocationBackup = (PointParticle)Location.Clone();
            Lifetime = new Random().Next(100);
            Trajectory = new();
            Velocity = new Vector(new PointParticle(0, 0));
            Force = new Vector(new PointParticle(0, 0));
            Reborning = false;
        }
        public double ForceGX;
        public double ForceGY;
        public double ForceEX;
        public double ForceEY;

        public void ForceCalc(AwoseAgent opposite, StreamMode streamMode)
        {
            //gravity
            double tmpForceGX = 0, tmpForceGY = 0;
            double tmpForceEX = 0, tmpForceEY = 0;
            double distance = Math.Pow(Location.X - opposite.Location.X, 2) + Math.Pow(Location.Y - opposite.Location.Y, 2);
            switch (streamMode)
            {
                case StreamMode.Gravity:
                    Calculations.Gravity(new AwoseAgent("", Location.X, Location.Y, 1, 0, 0, 0, false), opposite, ref tmpForceGX, ref tmpForceGY, distance);
                    ForceGX += tmpForceGX;
                    ForceGY += tmpForceGY;
                    break;
                case StreamMode.Electric:
                    Calculations.Electrical(new AwoseAgent("", Location.X, Location.Y, 0, 1, 0, 0, false), opposite, ref tmpForceEX, ref tmpForceEY, distance);
                    ForceEX += tmpForceEX;
                    ForceEY += tmpForceEY;
                    break;
                default:
                    break;
            }
        }

        public void Reborn()
        {
            //Lifetime = new Random().Next(100);
            Location = (PointParticle)LocationBackup.Clone();
            //Trajectory.Clear();
            Reborning = true;
            Velocity = new Vector(new PointParticle(0, 0));
            Force = new Vector(new PointParticle(0, 0));
        }
    }
}
