﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Awose
{
    public partial class Awose : Form
    {
        List<AwoseAgent> agents = new();
        Point aw_cursor = new(0, 0);
        Point lu_corner = new(0, 0);
        int aw_scale = 1;
        int aw_agentsize = 15;
        public Awose()
        {
            InitializeComponent();
        }

        private void Aw_Refresh()
        {
            int diameter = aw_agentsize * aw_scale;
            Bitmap board = new Bitmap(ModelBoard_PB.Width, ModelBoard_PB.Height);
            using Graphics grfx = Graphics.FromImage(board);
            grfx.Clear(Color.FromArgb(35, 35, 35));
            foreach (AwoseAgent item in agents)
            {
                RectangleF circle = new((float)(item.X - diameter / 2), (float)(item.Y - diameter / 2), diameter, diameter);
                grfx.FillEllipse(item.Dye, circle);
            }
            ModelBoard_PB.BackgroundImage = board;
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

        private void Aw_ChechMistakes()
        {
            //Useless object
            if (agents.Count == 1)
            {
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
            aw_cursor.X = lu_corner.X + (Cursor.Position.X - Location.X - ModelBoard_PB.Location.X - 7) * aw_scale;
            aw_cursor.Y = lu_corner.Y + (Cursor.Position.Y - Location.Y - ModelBoard_PB.Location.Y - 29) * aw_scale;
        }

        private void CreateObject_CMItem_Click(object sender, EventArgs e)
        {
            agents.Add(new AwoseAgent("Object " + (agents.Count + 1).ToString(), aw_cursor.X, aw_cursor.Y, 1, 0, 0, 0, false));
            Aw_Refresh();
            Aw_ChechMistakes();
        }
    }
}
