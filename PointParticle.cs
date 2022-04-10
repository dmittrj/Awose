using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awose
{
    public class PointParticle
    {
        public float X { get; set; }
        public float Y { get; set; }
        public PointParticle()
        {
            X = 0;
            Y = 0;
        }
        public PointParticle(float x, float y)
        {
            X = x;
            Y = y;
        }
        public static PointParticle operator +(PointParticle left, PointParticle right)
        {
            return new PointParticle(left.X + right.X, left.Y + right.Y);
        }
        public static PointParticle operator -(PointParticle left, PointParticle right)
        {
            return new PointParticle(left.X - right.X, left.Y - right.Y);
        }
    }
}
