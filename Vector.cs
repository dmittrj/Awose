using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awose
{
    public class Vector
    {
        public PointParticle Head { get; set; }
        public PointParticle Tail { get; set; }
        public float Length { get
            {
                return MathF.Sqrt(MathF.Pow(Head.X - Tail.X, 2) + MathF.Pow(Head.Y - Tail.Y, 2));
            } 
        }

        public float Angle { get
            {
                float delta_x = Tail.X - Head.X;
                float delta_y = Tail.Y - Head.Y;
                return MathF.Round(MathF.Atan2(delta_y, delta_x) * 180 / MathF.PI, 2);
            } 
        }

        public Vector(PointParticle head, PointParticle tail)
        {
            Head = head;
            Tail = tail;
        }
        public Vector(PointParticle tail)
        {
            Head = new(0, 0);
            Tail = tail;
        }

        public Vector()
        {
            Head = new(0, 0);
            Tail = new(0, 0);
        }

        private float Sinus()
        {
            if (Length == 0) return 0;
            return ((Head.Y - Tail.Y) / Length);
        }

        private float Cosine()
        {
            if (Length == 0) return 0;
            return ((Head.X - Tail.X) / Length);
        }

        public Point[] CreateTriangle(float length, float width)
        {
            Point[] triangle = new Point[3];
            triangle[0] = new Point((int)Head.X, (int)Head.Y);
            triangle[1] = new Point(
                (int)(Head.X - length * Cosine() + width * Sinus() / 2),
                (int)(Head.Y - length * Sinus() - width * Cosine() / 2));
            triangle[2] = new Point(
                (int)(Head.X - length * Cosine() - width * Sinus() / 2),
                (int)(Head.Y - length * Sinus() + width * Cosine() / 2));
            return triangle;
        }
    }
}
