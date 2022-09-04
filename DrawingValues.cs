using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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

        public static Bitmap DrawCircleVelocity(int width, int height, bool isSelected)
        {
            Bitmap picturebox_img = new(width, height);
            using Graphics pb_grfx = Graphics.FromImage(picturebox_img);
            //cb_grfx.Clear(Color.FromArgb(15, 15, 15));

            GraphicsPath path = new();
            //path->AddEllipse(0, 0, 140, 70);
            path.AddRectangle(new Rectangle(0, 0, width, height));
            PathGradientBrush pthGrBrush = new PathGradientBrush(path);
            PointF cur = new PointF(0, height);
            pthGrBrush.CenterColor = Color.FromArgb(0, 205, 240);
            pthGrBrush.CenterPoint = cur;
            Color[] colors = {
                Color.FromArgb(0, 0, 255)
            };
            pthGrBrush.SurroundColors = colors;

            pb_grfx.FillEllipse(pthGrBrush, 0, 0, width, height);
            if (isSelected)
            {
                pb_grfx.DrawLine(new Pen(Brushes.Black, 2), width * 0.25f, height * 0.45f, width * 0.5f, height * 0.7f);
                pb_grfx.DrawLine(new Pen(Brushes.Black, 2), width * 0.5f, height * 0.7f, width * 0.75f, height * 0.25f);
            } else
            {
                Point[] nav = {
                    new Point(width / 5, height / 2),
                    new Point(width * 4 / 5, height / 5),
                    new Point(width / 2, height * 4 / 5),
                    new Point(width / 2, height / 2)
                };
                pb_grfx.FillPolygon(Brushes.White, nav);
            }
            return picturebox_img;
        }

        public static Bitmap DrawCircleSign(int width, int height, bool isSelected)
        {
            Bitmap picturebox_img = new(width, height);
            using Graphics pb_grfx = Graphics.FromImage(picturebox_img);
            //cb_grfx.Clear(Color.FromArgb(15, 15, 15));

            GraphicsPath path = new();
            //path->AddEllipse(0, 0, 140, 70);
            path.AddRectangle(new Rectangle(0, 0, width, height));
            PathGradientBrush pthGrBrush = new PathGradientBrush(path);
            PointF cur = new PointF(0, height);
            pthGrBrush.CenterColor = Color.FromArgb(255, 255, 20, 30);
            pthGrBrush.CenterPoint = cur;
            Color[] colors = {
                Color.FromArgb(255, 0, 100, 255)

            };
            pthGrBrush.SurroundColors = colors;

            pb_grfx.FillEllipse(pthGrBrush, 0, 0, width, height);
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

        public static async void Unfold(Panel obj, int y_start, int y_finish, int h_start, int h_finish, int speed)
        {
            int y_speed = (y_finish - y_start) / speed;
            int h_speed = (h_finish - h_start) / speed;
            obj.Visible = false;
            obj.Height = h_start;
            obj.Location = new Point(obj.Location.X, y_start);
            obj.Visible = true;
            for (int ani_y = y_start, ani_h = h_start; (ani_y < y_finish || ani_h < h_finish); ani_y += y_speed, ani_h += h_speed, await Task.Delay(10))
            {
                obj.Height = ani_h;
                obj.Location = new Point(obj.Location.X, ani_y);
            }
            obj.Height = h_finish;
            obj.Location = new Point(obj.Location.X, y_finish);
        }
    }
}
