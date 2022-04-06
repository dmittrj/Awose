using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awose
{
    enum FirstSpaceObject { None, Satellite, Planet, Star }
    class AwoseAgent
    {
        //User information
        /// <summary>
        /// Name of object to identify it
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// X-coordinate of object
        /// </summary>
        public double X { get; set; }
        public int X_screen;
        /// <summary>
        /// Y-coordinate of object
        /// </summary>
        public double Y { get; set; }
        public int Y_screen;
        /// <summary>
        /// Object mass, kg
        /// </summary>
        public double Weight { get; set; }
        /// <summary>
        /// Electrical charge
        /// </summary>
        public double Charge { get; set; }
        /// <summary>
        /// X-axis velocity
        /// </summary>
        public double VelocityX { get; set; }
        /// <summary>
        /// Y-axis velocity
        /// </summary>
        public double VelocityY { get; set; }
        /// <summary>
        /// Is this object pinned (pinned objects don't move)
        /// </summary>
        public bool IsPinned { get; set; }
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
        public double ForceGX;
        public double ForceGY;
        public double ForceEX;
        public double ForceEY;

        public bool IsSelected;

        public int MistakeType;
        public string MDescription;
        public SolidBrush Dye { get; set; }

        public Queue<Point> Trajectory = new();
        public readonly Queue<Point> Spray = new();

        //backups
        private double Backup_X;
        private double Backup_Y;
        private double Backup_VelocityX;
        private double Backup_VelocityY;

        public AwoseAgent(string name, double x, double y, double weight, double charge, double velocityX, double velocityY, bool isPinned)
        {
            Name = name;
            X = x;
            X_screen = (int)x;
            Y_screen = (int)y;
            Y = y;
            Weight = weight;
            Charge = charge;
            VelocityX = velocityX;
            VelocityY = velocityY;
            IsPinned = isPinned;
            MistakeType = 0;
            MDescription = "";
            //IsFirstSpace = false;
            IsSelected = false;
            Dye = new SolidBrush(Color.White);
            Satellites = new List<string>();
            Star = "";
        }

        public void Backup()
        {
            Backup_X = X;
            Backup_Y = Y;
            Backup_VelocityX = VelocityX;
            Backup_VelocityY = VelocityY;
        }

        public void Restore()
        {
            X = Backup_X;
            Y = Backup_Y;
            VelocityX = Backup_VelocityX;
            VelocityY = Backup_VelocityY;
        }

        public void ForceCalc(AwoseAgent opposite)
        {
            //gravity
            double tmpForceGX = 0, tmpForceGY = 0;
            double tmpForceEX = 0, tmpForceEY = 0;
            double distance = Math.Pow(X - opposite.X, 2) + Math.Pow(Y - opposite.Y, 2);
            Calculations.Gravity(this, opposite, ref tmpForceGX, ref tmpForceGY, distance);
            ForceGX += tmpForceGX;
            ForceGY += tmpForceGY;
            Calculations.Electrical(this, opposite, ref tmpForceEX, ref tmpForceEY, distance);
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
