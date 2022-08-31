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
        public AwoseParticle(PointParticle location)
        {
            Location = location;
            LocationBackup = location;
            Lifetime = new Random().Next(100);
            Trajectory = new();
            Velocity = new Vector(new PointParticle(0, 0));
            Force = new Vector(new PointParticle(0, 0));
        }
        public double ForceGX;
        public double ForceGY;
        public double ForceEX;
        public double ForceEY;

        public void ForceCalc(AwoseAgent opposite)
        {
            //gravity
            double tmpForceGX = 0, tmpForceGY = 0;
            double tmpForceEX = 0, tmpForceEY = 0;
            double distance = Math.Pow(Location.X - opposite.Location.X, 2) + Math.Pow(Location.Y - opposite.Location.Y, 2);
            Calculations.Gravity(new AwoseAgent("", Location.X, Location.Y, 1, 0, 0, 0, false), opposite, ref tmpForceGX, ref tmpForceGY, distance);
            ForceGX += tmpForceGX;
            ForceGY += tmpForceGY;
            Calculations.Electrical(new AwoseAgent("", Location.X, Location.Y, 0, 1, 0, 0, false), opposite, ref tmpForceEX, ref tmpForceEY, distance);
            ForceEX += tmpForceEX;
            ForceEY += tmpForceEY;
        }

        public void Reborn()
        {
            Lifetime = new Random().Next(100);
            Location = LocationBackup;
            Trajectory.Clear();
            Velocity = new Vector(new PointParticle(0, 0));
            Force = new Vector(new PointParticle(0, 0));
        }
    }
}
