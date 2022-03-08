using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awose
{
    enum MistakeType{ No, Green, Red }
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
        public MistakeType MType;
        public string MDescription;
        public SolidBrush Dye { get; set; }

        public Queue<Point> Trajectory = new();
        private readonly Queue<Point> Spray = new();

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
            MType = MistakeType.No;
            MDescription = "";
            Dye = new SolidBrush(Color.White);
        }

        private void AgentSprayUpdate()
        {
            if (Spray.Count > 100) Spray.Dequeue();
            Spray.Enqueue(new Point((int)X, (int)Y));
            Point tmp;
            Random rnd = new Random();
            for (int i = 0; i < Spray.Count; i++)
            {
                tmp = Spray.Dequeue();
                tmp.X += rnd.Next(-2, 2);
                tmp.Y += rnd.Next(-2, 2);
                Spray.Enqueue(tmp);
            }
        }
    }
}
