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
        public double Y { get; set; }
        public double Weight { get; set; }
        public double Charge { get; set; }
        public double VelocityX { get; set; }
        public double VelocityY { get; set; }
        public Point Trajectory { get; set; }
    }
}