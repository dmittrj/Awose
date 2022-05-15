using System;
using System.Collections.Generic;
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

        public float Angel { get
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
        }
    }
}
