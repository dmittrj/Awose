using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awose
{
    class Vector
    {
        public PointParticle Head { get; set; }
        public PointParticle Tail { get; set; }
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
    }
}
