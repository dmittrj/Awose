using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Awose
{
    public partial class Awose : Form
    {
        readonly List<AwoseAgent> agents = new();
        int aw_selected = 0;
        Point aw_cursor = new(0, 0);
        Point lu_corner = new(0, 0);
        float aw_scale = 1;
        const int aw_agentsize = 15;
        //Thread animation;

        private void Aw_Refresh()
        {
            float diameter = aw_agentsize * aw_scale;
            Bitmap board = new Bitmap(ModelBoard_PB.Width, ModelBoard_PB.Height);
            using Graphics grfx = Graphics.FromImage(board);
            grfx.Clear(Color.FromArgb(35, 35, 35));
            foreach (AwoseAgent item in agents)
            {
                int dotNumber = 0;
                foreach (Point dot in item.Spray)
                {
                    RectangleF spraydot = new(lu_corner.X + dot.X * aw_scale, lu_corner.Y + dot.Y * aw_scale, aw_scale, aw_scale);
                    if (item.MistakeType == 0)
                        grfx.FillRectangle(new SolidBrush(Color.FromArgb(100, 100, 100)), spraydot);
                    if (item.MistakeType == 1)
                        grfx.FillRectangle(new SolidBrush(Color.FromArgb(Calculations.Normilize(0, 255, (int)(-0.28 * (dotNumber) + 175)), Calculations.Normilize(0, 255, (int)(-0.44 * (dotNumber) + 255)), Calculations.Normilize(0, 255, (int)(-0.024 * (dotNumber++) + 47)))), spraydot);
                }
                RectangleF circle = new((float)(lu_corner.X + item.X * aw_scale - diameter / 2), (float)(lu_corner.Y + item.Y * aw_scale - diameter / 2), diameter, diameter);
                grfx.FillEllipse(item.Dye, circle);
            }
            ModelBoard_PB.BackgroundImage = board;
        }

        public Awose()
        {
            InitializeComponent();
        }

        private void Awose_Load(object sender, EventArgs e)
        {
            Thread animation = new(AnimationEditor);
            animation.Start();
        }

        public async void AnimationEditor()
        {
            //Thread.Sleep(5000);
            while (true)
            {
                foreach (AwoseAgent item in agents)
                {
                    item.AgentSprayUpdate();
                }
                Aw_Refresh();
                await Task.Delay(10);
            }
        }


        private void Aw_DrawMistake(bool isError, string text)
        {
            Bitmap icon = new Bitmap(31, 31);
            using Graphics grfx = Graphics.FromImage(icon);
            grfx.Clear(Color.FromArgb(15, 15, 15));
            Point[] triangle = {
                new Point(16, 0),
                new Point(30, 30),
                new Point(0, 30)
            };
            RectangleF exclmark1 = new(15, 9, 3, 13);
            RectangleF exclmark2 = new(15, 24, 3, 4);
            grfx.FillPolygon(Brushes.Khaki, triangle);
            grfx.FillRectangle(new SolidBrush(Color.FromArgb(15, 15, 15)), exclmark1);
            grfx.FillRectangle(new SolidBrush(Color.FromArgb(15, 15, 15)), exclmark2);
            MistakeIcon_PB.Image = icon;
        }

        private void Aw_CheckMistakes()
        {
            foreach (AwoseAgent item in agents)
            {
                item.MistakeType = 0;
            }
            //Useless object
            if (agents.Count == 1)
            {
                agents[0].MistakeType = 1;
                agents[0].MDescription = "Useless object";
                Aw_DrawMistake(true, "hhh");
            }
        }

        private void SaveModel_MSItem_Click(object sender, EventArgs e)
        {
            if (SaveModel_SFD.ShowDialog() == DialogResult.OK)
            {

            }
        }

        private void Space_CMStr_Opening(object sender, CancelEventArgs e)
        {
            aw_cursor.X = (int)((-lu_corner.X + Cursor.Position.X - Location.X - ModelBoard_PB.Location.X - 7) / aw_scale);
            aw_cursor.Y = (int)((-lu_corner.Y + Cursor.Position.Y - Location.Y - ModelBoard_PB.Location.Y - 29) / aw_scale);
            aw_selected = 0;
            Mistake_CMItem.Visible = false;
            SepMistake_CMSepar.Visible = false;
            foreach (AwoseAgent item in agents)
            {
                if (Calculations.IsInRadius(aw_cursor.X, aw_cursor.Y, item, aw_agentsize * aw_scale))
                    aw_selected++;
                else
                {
                    if (item.MistakeType > 0)
                    {
                        Mistake_CMItem.Text = item.MDescription;
                        Mistake_CMItem.Visible = true;
                        SepMistake_CMSepar.Visible = true;
                    }
                }
            }
        }

        private void CreateObject_CMItem_Click(object sender, EventArgs e)
        {
            agents.Add(new AwoseAgent("Object " + (agents.Count + 1).ToString(), aw_cursor.X, aw_cursor.Y, 1, 0, 0, 0, false));
            //Aw_Refresh();
            Aw_CheckMistakes();
        }

        private void ModelBoard_PB_Click(object sender, EventArgs e)
        {

        }

        private void ModelBoard_PB_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Point beforeScaling = new();
            //Point afterScaling = new();
            if (e.Delta > 0)
            {
                beforeScaling.X = Cursor.Position.X - Location.X - ModelBoard_PB.Location.X - 7;
                beforeScaling.Y = Cursor.Position.Y - Location.Y - ModelBoard_PB.Location.Y - 29;
                aw_cursor.X = (int)((-lu_corner.X + Cursor.Position.X - Location.X - ModelBoard_PB.Location.X - 7) / aw_scale);
                aw_cursor.Y = (int)((-lu_corner.Y + Cursor.Position.Y - Location.Y - ModelBoard_PB.Location.Y - 29) / aw_scale);
                aw_scale += .5f;
                lu_corner.X = (int)(-aw_cursor.X * aw_scale + beforeScaling.X);
                lu_corner.Y = (int)(-aw_cursor.Y * aw_scale + beforeScaling.Y);
            }
            else if (aw_scale > 1)
            {
                beforeScaling.X = Cursor.Position.X - Location.X - ModelBoard_PB.Location.X - 7;
                beforeScaling.Y = Cursor.Position.Y - Location.Y - ModelBoard_PB.Location.Y - 29;
                aw_cursor.X = (int)((-lu_corner.X + Cursor.Position.X - Location.X - ModelBoard_PB.Location.X - 7) / aw_scale);
                aw_cursor.Y = (int)((-lu_corner.Y + Cursor.Position.Y - Location.Y - ModelBoard_PB.Location.Y - 29) / aw_scale);
                aw_scale -= .5f;
                lu_corner.X = (int)(-aw_cursor.X * aw_scale + beforeScaling.X);
                lu_corner.Y = (int)(-aw_cursor.Y * aw_scale + beforeScaling.Y);
            }
        }

        private void toolStripTextBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
