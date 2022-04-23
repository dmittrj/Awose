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

        public static Bitmap DrawCircle(int width, int height, Color color, bool isSelected)
        {
            Bitmap picturebox_img = new(width, height);
            using Graphics pb_grfx = Graphics.FromImage(picturebox_img);
            //cb_grfx.Clear(Color.FromArgb(15, 15, 15));

            pb_grfx.FillEllipse(new SolidBrush(color), 0, 0, width, height);
            if (isSelected)
            {
                pb_grfx.DrawLine(new Pen(Brushes.Black, 2), width * 0.25f, height * 0.45f, width * 0.5f, height * 0.7f);
                pb_grfx.DrawLine(new Pen(Brushes.Black, 2), width * 0.5f, height * 0.7f, width * 0.75f, height * 0.25f);
            }
            return picturebox_img;
        }
    }
}
