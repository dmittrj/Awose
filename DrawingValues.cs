using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awose
{
    public class DrawingValues
    {
        public int GridWidth { get; set; }
        public int GridHeight { get; set; }
        public DrawingValues()
        {
            GridWidth = 0;
            GridHeight = 0;
            
        }

        public static Bitmap DrawTick()
        {
            Bitmap checkbox_img = new(15, 15);
            using Graphics cb_grfx = Graphics.FromImage(checkbox_img);
            //cb_grfx.Clear(Color.FromArgb(15, 15, 15));
            
            cb_grfx.DrawLine(new Pen(Brushes.White, 2), 3, 7, 7, 11);
            cb_grfx.DrawLine(new Pen(Brushes.White, 2), 7, 11, 11, 3);
            return checkbox_img;
        }
    }
}
