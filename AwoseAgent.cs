using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awose
{
    class AwoseAgent
    {
        public string Name { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Weight { get; set; }
        public double Charge { get; set; }
        public double VelocityX { get; set; }
        public double VelocityY { get; set; }
        public bool IsPinned { get; set; }

        public SolidBrush MistakeType;
        public string MDescription;
        public SolidBrush Dye { get; set; }

        public Queue<Point> Trajectory = new();
        public readonly Queue<Point> Spray = new();

        public AwoseAgent(string name, double x, double y, double weight, double charge, double velocityX, double velocityY, bool isPinned)
        {
            Name = name;
            X = x;
            Y = y;
            Weight = weight;
            Charge = charge;
            VelocityX = velocityX;
            VelocityY = velocityY;
            IsPinned = isPinned;
            MistakeType = new SolidBrush(Color.GreenYellow);
            MDescription = "";
            Dye = new SolidBrush(Color.White);
        }

        public void AgentSprayUpdate()
        {
            if (Spray.Count > 500) Spray.Dequeue();
            //Point tmp;
            Random rnd = new();
            Spray.Enqueue(new Point((int)X + rnd.Next(-Spray.Count, Spray.Count), (int)Y + rnd.Next(-Spray.Count, Spray.Count)));
            //int dotsCount = Spray.Count;
            //for (int i = 0; i < dotsCount; i++)
            //{
            //    tmp = Spray.Dequeue();
            //    tmp.X += rnd.Next(-2, 2);
            //    tmp.Y += rnd.Next(-2, 2);
            //    Spray.Enqueue(tmp);
            //}
        }
    }
}
