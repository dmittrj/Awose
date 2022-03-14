﻿using System;
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
        public int X_screen;
        public double Y { get; set; }
        public int Y_screen;
        public double Weight { get; set; }
        public double Charge { get; set; }
        public double VelocityX { get; set; }
        public double VelocityY { get; set; }
        public bool IsPinned { get; set; }
        public double ForceGX;
        public double ForceGY;
        public double ForceEX;
        public double ForceEY;

        public int MistakeType;
        public string MDescription;
        public SolidBrush Dye { get; set; }

        public Queue<Point> Trajectory = new();
        public readonly Queue<Point> Spray = new();

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
            Dye = new SolidBrush(Color.White);
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
