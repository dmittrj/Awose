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
        public int PreviewPosition { get; set; }
        public Color HoveredColor { get; set; }
        public Color[] ObjectSprite_Last { get; set; }
        public DrawingValues()
        {
            GridWidth = 0;
            GridHeight = 0;
            PreviewPosition = 0;
            HoveredColor = Color.White;
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

        public static Bitmap DrawCircleWithArrow(int width, int height, Color color, float x, float y)
        {
            Bitmap picturebox_img = new(width, height);
            using Graphics pb_grfx = Graphics.FromImage(picturebox_img);
            //cb_grfx.Clear(Color.FromArgb(15, 15, 15));
            float length = MathF.Sqrt(x * x + y * y);
            float radius = width / 2;
            float x_circle = radius * x / length;
            float y_circle = radius * y / length;

            pb_grfx.FillEllipse(new SolidBrush(color), width / 2 - 2, height / 2 - 2, 4, 4);
            pb_grfx.DrawLine(new Pen(color, 2), width / 2, height / 2, width / 2 + x_circle, height / 2 + y_circle);
            pb_grfx.DrawEllipse(new Pen(Color.White, 2), 0, 0, width-1, height-1);
            return picturebox_img;
        }

        public static Bitmap DrawObjectPreview(Color obj_color, int alpha)
        {
            if (alpha < 0) alpha = 0;
            Bitmap picturebox_img = new(100, 100);
            using Graphics pb_grfx = Graphics.FromImage(picturebox_img);
            pb_grfx.Clear(Color.FromArgb(10, 10, 10));

            pb_grfx.FillEllipse(new SolidBrush(obj_color), 15, 15, 70, 70);
            pb_grfx.FillRectangle(new SolidBrush(Color.FromArgb(alpha, 10, 10, 10)), 0, 0, 100, 100);
            return picturebox_img;
        }
    }
}
