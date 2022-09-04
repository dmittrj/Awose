using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awose
{
    enum FirstSpaceObject { None, Satellite, Planet, Star }
    public enum SpriteType { White, Yellow, Green, Sky, Red, Sign, Charge, Mass, Velocity }
    public enum TrajectoryType { None, Fade, Nonfade}
    public class AwoseAgent
    {
        //User information
        /// <summary>
        /// Name of object to identify it
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// X-coordinate of object
        /// </summary>
        [Obsolete]
        public double X { get; set; }
        public int X_screen;
        /// <summary>
        /// Y-coordinate of object
        /// </summary>
        [Obsolete]
        public double Y { get; set; }
        public int Y_screen;
        /// <summary>
        /// Object mass, kg
        /// </summary>
        public double Weight { get; set; }
        public PointParticle Location { get; set; }
        /// <summary>
        /// Electrical charge, C
        /// </summary>
        public double Charge { get; set; }
        /// <summary>
        /// X-axis velocity
        /// </summary>
        [Obsolete("Use Velocity.X", false)]
        public double VelocityX { get; set; }
        /// <summary>
        /// Y-axis velocity
        /// </summary>
        [Obsolete("Use Velocity.Y", false)]
        public double VelocityY { get; set; }
        /// <summary>
        /// Is this object pinned (pinned objects don't move)
        /// </summary>
        public bool IsPinned { get; set; }
        /// <summary>
        /// Velocity of the object
        /// </summary>
        public Vector Velocity { get; set; }
        /// <summary>
        /// List of satellites (objects that revolve
        /// around this object)
        /// </summary>
        public List<string> Satellites { get; set; }
        /// <summary>
        /// Star (object around which this object revolves)
        /// </summary>
        public string Star { get; set; }
        //public bool IsFirstSpace { get; set; }
        public bool ChangeAfterFSV = false;
        public Vector Force { get; set; }
        public double ForceGX;
        public double ForceGY;
        public double ForceEX;
        public double ForceEY;

        public bool IsSelected;

        public int MistakeType;
        public string MDescription;
        public Color Dye { get; set; }
        public SolidBrush DyeDim { get; set; }

        public Queue<PointParticle> Trajectory = new();
        public readonly Queue<Point> Spray = new();
        public SpriteType Sprite { get; set; }
        public TrajectoryType TrajectoryLine { get; set; }
        public int Ban { get; set; }

        //backups
        private double Backup_X;
        private double Backup_Y;
        private double Backup_VelocityX;
        private double Backup_VelocityY;

        public AwoseAgent(string name, float x, float y, double weight, double charge, float velocityX, float velocityY, bool isPinned)
        {
            Name = name;
            Location = new PointParticle(x, y);
            X_screen = (int)x;
            Y_screen = (int)y;
            Weight = weight;
            Charge = charge;
            Velocity = new Vector(new PointParticle(velocityX, velocityY));
            //VelocityX = velocityX;
            //VelocityY = velocityY;
            IsPinned = isPinned;
            MistakeType = 0;
            MDescription = "";
            //IsFirstSpace = false;
            IsSelected = false;
            Dye = Color.White;
            DyeDim = new SolidBrush(Color.Gray);
            Satellites = new List<string>();
            Star = "";
            Sprite = SpriteType.White;
            Force = new Vector(new PointParticle(0, 0));
            TrajectoryLine = TrajectoryType.None;
            Ban = 0;
        }

        public AwoseAgent(string name, float x, float y)
        {
            Name = name;
            Location = new PointParticle(x, y);
            Sprite = SpriteType.White;
        }

        public AwoseAgent()
        {
        }

        public void Backup()
        {
            Backup_X = Location.X;
            Backup_Y = Location.Y;
            Backup_VelocityX = Velocity.Tail.X;
            Backup_VelocityY = Velocity.Tail.Y;
        }

        public void Restore()
        {
            Location.Y = (float)Backup_Y;
            Location.X = (float)Backup_X;
            Velocity.Tail.X = (float)Backup_VelocityX;
            Velocity.Tail.Y = (float)Backup_VelocityY;
        }

        public void ForceCalc(AwoseAgent opposite)
        {
            //gravity
            double tmpForceGX = 0, tmpForceGY = 0;
            double tmpForceEX = 0, tmpForceEY = 0;
            double distance = Math.Pow(Location.X - opposite.Location.X, 2) + Math.Pow(Location.Y - opposite.Location.Y, 2);
            Calculations.Gravity(this, opposite, ref tmpForceGX, ref tmpForceGY, distance);
            ForceGX += tmpForceGX;
            ForceGY += tmpForceGY;
            Calculations.Electrical(this, opposite, ref tmpForceEX, ref tmpForceEY, distance);
            ForceEX += tmpForceEX;
            ForceEY += tmpForceEY;
        }

        public void ForceCalcG(AwoseAgent opposite)
        {
            //gravity
            double tmpForceGX = 0, tmpForceGY = 0;
            //double tmpForceEX = 0, tmpForceEY = 0;
            double distance = Math.Pow(Location.X - opposite.Location.X, 2) + Math.Pow(Location.Y - opposite.Location.Y, 2);
            Calculations.Gravity(this, opposite, ref tmpForceGX, ref tmpForceGY, distance);
            ForceGX += tmpForceGX;
            ForceGY += tmpForceGY;
            //Calculations.Electrical(this, opposite, ref tmpForceEX, ref tmpForceEY, distance);
            //ForceEX += tmpForceEX;
            //ForceEY += tmpForceEY;
        }

        public void ForceCalc(AwoseAgent opposite, float tmpX, float tmpY, double tmpVX, double tmpVY)
        {
            //gravity
            double tmpForceGX = 0, tmpForceGY = 0;
            double tmpForceEX = 0, tmpForceEY = 0;
            double distance = Math.Pow(tmpX - opposite.X, 2) + Math.Pow(tmpY - opposite.Y, 2);
            //Calculations.Gravity(new AwoseAgent("Temp", tmpX, tmpY, Weight, Charge, tmpVX, tmpVY, false), opposite, ref tmpForceGX, ref tmpForceGY, distance);
            ForceGX += tmpForceGX;
            ForceGY += tmpForceGY;
            //Calculations.Electrical(new AwoseAgent("Temp", tmpX, tmpY, Weight, Charge, tmpVX, tmpVY, false), opposite, ref tmpForceEX, ref tmpForceEY, distance);
            ForceEX += tmpForceEX;
            ForceEY += tmpForceEY;
        }

        public void AgentSprayUpdate()
        {
            if (MistakeType == 0)
            {
                if (Spray.Count > 0)
                    for (int i = 0; i < 10; i++)
                if (Spray.Count > 0) lock(Spray) Spray.Dequeue();
            }
            if (MistakeType > 0)
            {
                if (Spray.Count > 500) return;
                //Point tmp;
                Random rnd = new();
                int p = Spray.Count;
                for (int i = 0; i <= p * Awose.timeStep / 160; i++)
                    lock (Spray) 
                        Spray.Enqueue(new Point((int)X + rnd.Next(-Spray.Count / 10, Spray.Count / 8), (int)Y + rnd.Next(-Spray.Count / 10, Spray.Count / 8)));
            }
            
        }
    }
}
