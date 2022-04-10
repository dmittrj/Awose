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
    enum EditingValue { None, Mass, Charge, Name, X, Y }

    enum MovingEntity { None, Board, Agent }
    public partial class Awose : Form
    {
        readonly List<AwoseAgent> agents = new();
        private int agentsNumeric = 1;
        readonly Stack<AwoseChange> aw_undo = new();
        readonly Stack<AwoseChange> aw_redo = new();
        private int aw_selected = -1;
        //represents click-point in screen coordinates
        private PointParticle aw_cursor = new(0, 0);
        //represents coordinate of up-left corner in real coordinates
        private PointParticle lu_corner = new(0, 0);
        //represents remembered point in screen coordinates
        private PointParticle aw_remember = new(0, 0);
        //represents remembered up-left corner in real coordinates
        private PointParticle lu_remember = new(0, 0);
        private Point objBeforeMoving = new(0, 0);
        private float aw_scale = 1;
        const int aw_agentsize = 15;
        private EditingValue editingValue = EditingValue.None;
        private MovingEntity movingEntity = MovingEntity.None;
        [Obsolete]
        private bool isBoardMoving = false;
        [Obsolete]
        private bool isObjectMoving = false;
        private bool isLaunched = false;
        private bool isFirstSpaceSetting = false;
        private int SettingVelocity = -1;
        //int c = -1;
        //constants
        public static int timeStep = 20;
        public static float ConstG = 100000;
        public static float ConstE = 100000;

        [Obsolete]
        private PointF ScreenToReal(float screenX, float screenY)
        {
            return new(
                screenX / aw_scale - lu_corner.X,
                screenY / aw_scale - lu_corner.Y);
        }

        private PointParticle ScreenToReal(PointParticle screenPoint)
        {
            return new(
                screenPoint.X / aw_scale - lu_corner.X,
                screenPoint.Y / aw_scale - lu_corner.Y);
        }

        private Point RealToScreen(double realX, double realY)
        {
            return new(
                (int)((realX + lu_corner.X) * aw_scale),
                (int)((realY + lu_corner.Y) * aw_scale));
        }

        private PointParticle GetCursorPosition()
        {
            return new PointParticle(
                Cursor.Position.X - Location.X - ModelBoard_PB.Location.X - 7,
                Cursor.Position.Y - Location.Y - ModelBoard_PB.Location.Y - 29);
        }

        private void Aw_Refresh()
        {
            float diameter = aw_agentsize * aw_scale;
            Bitmap board = new(ModelBoard_PB.Width, ModelBoard_PB.Height);
            using Graphics grfx = Graphics.FromImage(board);
            grfx.Clear(Color.FromArgb(35, 35, 35));
            if (SettingVelocity != -1)
            {
                int x = Cursor.Position.X - Location.X - ModelBoard_PB.Location.X - 7;
                int y = Cursor.Position.Y - Location.Y - ModelBoard_PB.Location.Y - 29;
                //float dx = aw_cursor.X - (Cursor.Position.X - Location.X - ModelBoard_PB.Location.X - 7);
                //float dy = aw_cursor.Y - (Cursor.Position.Y - Location.Y - ModelBoard_PB.Location.Y - 29);
                float dx = agents[aw_selected].X_screen;
                float dy = agents[aw_selected].Y_screen;
                float l = (float)Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));
                float ax = 15 * dx / l;
                float ay = 15 * dy / l;
                float bx = 7 * dy / l;
                float by = 7 * dx / l;
                agents[aw_selected].ForceEX = 0;
                agents[aw_selected].ForceEY = 0;
                agents[aw_selected].ForceGX = 0; 
                agents[aw_selected].ForceGY = 0;
                List<PointF> tmpTraj = new();
                double tmpVelocityX = x - dx;
                double tmpVelocityY = y - dy;
                double tmpX = agents[aw_selected].X;
                double tmpY = agents[aw_selected].Y;
                for (int i = 0; i < 200; i++)
                {
                    foreach (AwoseAgent item in agents)
                    {
                        if (agents[aw_selected].Name != item.Name)
                            agents[aw_selected].ForceCalc(item, tmpX, tmpY, tmpVelocityX, tmpVelocityY);
                    }
                    tmpVelocityX += (agents[aw_selected].ForceGX + agents[aw_selected].ForceEX) * timeStep / agents[aw_selected].Weight / 1000;
                    tmpVelocityY += (agents[aw_selected].ForceGY + agents[aw_selected].ForceEY) * timeStep / agents[aw_selected].Weight / 1000;
                    tmpX += tmpVelocityX * timeStep / 1000;
                    tmpY += tmpVelocityY * timeStep / 1000;
                    tmpTraj.Add(new PointF((float)(lu_corner.X + tmpX * aw_scale), (float)(lu_corner.Y + tmpY * aw_scale)));
                }
                for (int i = 0; i < 199; i++)
                {
                   grfx.DrawLine(new Pen(Brushes.White, 1),
                   new Point((int)tmpTraj[i].X, (int)tmpTraj[i].Y),
                   new Point((int)tmpTraj[i+1].X, (int)tmpTraj[i+1].Y));
                }
                Point[] arrow =
                {
                    new Point(x, y),
                    new Point(x + (int)ax - (int)bx, y + (int)ay + (int)by),
                    new Point(x + (int)ax + (int)bx, y + (int)ay - (int)by)
                };
                grfx.DrawLine(new Pen(Brushes.Tomato, 2),
                    new Point((int)dx, (int)dy),
                    new Point(x,
                    y));
                grfx.FillPolygon(Brushes.Tomato, arrow);
            }
            if (isFirstSpaceSetting)
            {
                int x = Cursor.Position.X - Location.X - ModelBoard_PB.Location.X - 7;
                int y = Cursor.Position.Y - Location.Y - ModelBoard_PB.Location.Y - 29;
                grfx.DrawLine(new Pen(Brushes.DodgerBlue, 2),
                    new Point((int)agents[aw_selected].X, (int)agents[aw_selected].Y),
                    new Point(x, (int)agents[aw_selected].Y));
                grfx.DrawLine(new Pen(Brushes.DodgerBlue, 2),
                    new Point(x, (int)agents[aw_selected].Y),
                    new Point(x, y));
            }
            lock (agents) {
                foreach (AwoseAgent item in agents)
                {
                    int dotNumber = 0;
                    lock (item.Spray)
                    {
                        foreach (Point dot in item.Spray)
                        {
                            RectangleF spraydot = new(lu_corner.X + dot.X * aw_scale, lu_corner.Y + dot.Y * aw_scale, aw_scale, aw_scale);
                            if (item.MistakeType == 0)
                                grfx.FillRectangle(new SolidBrush(Color.FromArgb(100, 100, 100)), spraydot);
                            if (item.MistakeType == 1)
                                grfx.FillRectangle(new SolidBrush(Color.FromArgb(Calculations.Normilize(0, 255, (int)(-0.28 * (dotNumber) + 175)), Calculations.Normilize(0, 255, (int)(-0.44 * (dotNumber) + 255)), Calculations.Normilize(0, 255, (int)(-0.024 * (dotNumber++) + 47)))), spraydot);
                            if (item.MistakeType == 2)
                                grfx.FillRectangle(new SolidBrush(Color.FromArgb(Calculations.Normilize(0, 255, (int)(-0.44 * (dotNumber) + 255)), Calculations.Normilize(0, 255, (int)(-0.28 * (dotNumber) + 175)), Calculations.Normilize(0, 255, (int)(-0.024 * (dotNumber++) + 47)))), spraydot);
                        }
                    }

                    Point point = RealToScreen(item.X, item.Y);
                    RectangleF circle = new((float)(point.X - diameter / 2), (float)(point.Y - diameter / 2), diameter, diameter);
                    if (circle.X + diameter < 0) {
                        if (circle.Y + 0.5 * diameter < 15)
                        {
                            grfx.FillRectangle(Brushes.WhiteSmoke, new Rectangle(15, 15, item.Name.Length * 7, 20));
                            grfx.DrawString(item.Name, DefaultFont, Brushes.Black, new Point(19, 16));
                        } 
                        else if (circle.Y > ModelBoard_PB.Height)
                        {
                            grfx.FillRectangle(Brushes.WhiteSmoke, new Rectangle(15, ModelBoard_PB.Height - 35, item.Name.Length * 7, 20));
                            grfx.DrawString(item.Name, DefaultFont, Brushes.Black, new Point(19, ModelBoard_PB.Height - 34));
                        }
                        else
                        {
                            grfx.FillRectangle(Brushes.WhiteSmoke, new Rectangle(15, (int)circle.Y, item.Name.Length * 7, 20));
                            grfx.DrawString(item.Name, DefaultFont, Brushes.Black, new Point(19, (int)circle.Y + 1));
                        }
                    } else if (circle.X > ModelBoard_PB.Width)
                    {
                        if (circle.Y + 0.5 * diameter < 15)
                        {
                            grfx.FillRectangle(Brushes.WhiteSmoke, new Rectangle(ModelBoard_PB.Width - item.Name.Length * 7 - 15, 15, item.Name.Length * 7, 20));
                            grfx.DrawString(item.Name, DefaultFont, Brushes.Black, new Point(ModelBoard_PB.Width - item.Name.Length * 7 - 11, 16));
                        }
                        else if (circle.Y > ModelBoard_PB.Height)
                        {
                            grfx.FillRectangle(Brushes.WhiteSmoke, new Rectangle(ModelBoard_PB.Width - item.Name.Length * 7 - 15, ModelBoard_PB.Height - 35, item.Name.Length * 7, 20));
                            grfx.DrawString(item.Name, DefaultFont, Brushes.Black, new Point(ModelBoard_PB.Width - item.Name.Length * 7 - 11, ModelBoard_PB.Height - 34));
                        }
                        else
                        {
                            grfx.FillRectangle(Brushes.WhiteSmoke, new Rectangle(ModelBoard_PB.Width - item.Name.Length * 7 - 15, (int)circle.Y, item.Name.Length * 7, 20));
                            grfx.DrawString(item.Name, DefaultFont, Brushes.Black, new Point(ModelBoard_PB.Width - item.Name.Length * 7 - 11, (int)circle.Y + 1));
                        }
                    } else
                    {
                        if (circle.Y + 0.5 * diameter < 0)
                        {
                            Point[] triangle = {
                                new Point((int)circle.X, 10),
                                new Point((int)circle.X - 8, 20),
                                new Point((int)circle.X + 8, 20)
                            };
                            grfx.FillRectangle(Brushes.WhiteSmoke, new Rectangle((int)circle.X - 13, 20, item.Name.Length * 7, 20));
                            grfx.FillPolygon(Brushes.WhiteSmoke, triangle);
                            grfx.DrawString(item.Name, DefaultFont, Brushes.Black, new Point((int)circle.X - 9, 21));
                        }
                        else if (circle.Y > ModelBoard_PB.Height)
                        {
                            Point[] triangle = {
                                new Point((int)circle.X, ModelBoard_PB.Height - 10),
                                new Point((int)circle.X - 8, ModelBoard_PB.Height - 20),
                                new Point((int)circle.X + 8, ModelBoard_PB.Height - 20)
                            };
                            grfx.FillRectangle(Brushes.WhiteSmoke, new Rectangle((int)circle.X - 13, ModelBoard_PB.Height - 40, item.Name.Length * 7, 20));
                            grfx.FillPolygon(Brushes.WhiteSmoke, triangle);
                            grfx.DrawString(item.Name, DefaultFont, Brushes.Black, new Point((int)circle.X - 12, ModelBoard_PB.Height - 39));
                        }
                        else
                        {
                            if (!isLaunched)
                                if (Math.Abs(item.VelocityX) + Math.Abs(item.VelocityY) > 10)
                                {
                                    int x = (int)item.X + (int)item.VelocityX;
                                    int y = (int)item.Y + (int)item.VelocityY;
                                    float dx = (float)item.VelocityX;
                                    float dy = (float)item.VelocityY;
                                    float l = (float)Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));
                                    float ax = 15 * dx / l;
                                    float ay = 15 * dy / l;
                                    float bx = 7 * dy / l;
                                    float by = 7 * dx / l;
                                    Point[] arrow =
                                    {
                                        new Point(x, y),
                                        new Point(x - (int)ax + (int)bx, y - (int)ay - (int)by),
                                        new Point(x - (int)ax - (int)bx, y - (int)ay + (int)by)
                                    };
                                    grfx.DrawLine(new Pen(Brushes.DimGray, 2),
                                        new Point((int)item.X, (int)item.Y),
                                        new Point((int)item.X + (int)item.VelocityX,
                                        (int)item.Y + (int)item.VelocityY));
                                    grfx.FillPolygon(Brushes.DimGray, arrow);
                                }
                            grfx.FillEllipse(item.Dye, circle);
                            if (item.IsSelected)
                            {
                                RectangleF bigcircle = new((float)(lu_corner.X + item.X * aw_scale - (diameter * 1.7) / 2), (float)(lu_corner.Y + item.Y * aw_scale - (diameter * 1.7) / 2), (int)(diameter * 1.7), (int)(diameter * 1.7));
                                grfx.DrawEllipse(new Pen(Brushes.White, (int)(1.5 * aw_scale)), bigcircle);
                            }
                        }
                    }
                    
                }
        }
            ModelBoard_PB.BackgroundImage = board;
        }

        private void Aw_Step(int time)
        {
            foreach (AwoseAgent item in agents)
            {
                item.ForceGX = item.ForceGY = item.ForceEX = item.ForceEY = 0;
            }
            for (int i = 0; i < agents.Count; i++)
            {
                for (int j = 0; j < agents.Count; j++)
                {
                    if (i != j) agents[i].ForceCalc(agents[j]);
                }
            }
            foreach (AwoseAgent item in agents)
            {
                if (item.IsPinned) continue;
                item.VelocityX += (item.ForceGX + item.ForceEX) * timeStep / item.Weight / 1000;
                item.VelocityY += (item.ForceGY + item.ForceEY) * timeStep / item.Weight / 1000;
                item.X += item.VelocityX * timeStep / 1000;
                item.Y += item.VelocityY * timeStep / 1000;
            }
            try
            {
                Invoke((Action)Aw_DrawControlLite);
            }
            catch { }
            //Aw_DrawControlLite();
        }

        public Awose()
        {
            InitializeComponent();
        }
        Thread animation;
        private void Awose_Load(object sender, EventArgs e)
        {
            animation = new(AnimationEditor);
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
                if (isLaunched) Aw_Step(timeStep);
                Aw_Refresh();
                await Task.Delay(timeStep);
            }
        }

        private void Aw_CheckMistakes()
        {
            foreach (AwoseAgent item in agents)
            {
                item.MistakeType = 0;
            }
            if (isLaunched) return;
            LaunchSimulation_MSItem.Enabled = true;
            //Useless object
            if (agents.Count == 1)
            {
                agents[0].MistakeType = 1;
                agents[0].MDescription = "Useless object";
            }
            int pinned = 0;
            foreach (AwoseAgent item in agents)
            {
                if (item.IsPinned) pinned++;
            }
            if (pinned == agents.Count)
                foreach (AwoseAgent item in agents)
                {
                    item.MistakeType = 1;
                    item.MDescription = "Useless object";
                }
            //Same name
            for (int i = 0; i < agents.Count; i++)
                for (int j = i + 1; j < agents.Count; j++)
                    if (agents[i].Name == agents[j].Name)
                    {
                        agents[i].MistakeType = 1;
                        agents[i].MDescription = "Multiple object with same name";
                        agents[j].MistakeType = 1;
                        agents[j].MDescription = "Multiple object with same name";
                        
                    }
            //No name
            foreach (AwoseAgent item in agents)
                if (item.Name == "")
                {
                    item.MistakeType = 1;
                    item.MDescription = "No name";
                }
            //Moved after setting first space velocity
            foreach (AwoseAgent item in agents)
                if (item.Star != "" && item.ChangeAfterFSV)
                {
                    item.MistakeType = 1;
                    item.MDescription = "The 1st space velocity should be set again";
                }
            //Satellite for itself
            foreach (AwoseAgent item in agents)
            {
                if (item.Satellites.Contains(item.Star))
                {
                    item.MistakeType = 1;
                    item.MDescription = "This object is a satellite for itself";
                }
            }
            //Pinned satellites
            foreach (AwoseAgent item in agents)
            {
                if (item.Star != "" && item.IsPinned)
                {
                    item.MistakeType = 1;
                    item.MDescription = "Pinned satellite";
                }
            }

            //Errors

            //Zero mass
            foreach (AwoseAgent item in agents)
            {
                if (item.Weight == 0)
                {
                    item.MistakeType = 2;
                    item.MDescription = "Zero mass";
                    LaunchSimulation_MSItem.Enabled = false;
                }
            }
        }

        private void Aw_DrawControl()
        {
            if (aw_selected != - 1)
            {
                CurrentObjectName_Label.Text = agents[aw_selected].Name;
                CurrentObjectName_Label.ForeColor = Color.LightSkyBlue;
                CurrentObjectName_Label.Cursor = Cursors.IBeam;
                ObjectMass_Label.Text = agents[aw_selected].Weight.ToString() + " kg";
                ObjectCharge_Label.Text = agents[aw_selected].Charge.ToString() + " C";
                ObjectPositionX_Label.Text = agents[aw_selected].X.ToString();
                ObjectPositionY_Label.Text = agents[aw_selected].Y.ToString();
                ObjectSettings_Panel.Visible = true;
                Bitmap btm_icon = new(34, 29);
                using Graphics grfx = Graphics.FromImage(btm_icon);
                grfx.Clear(Color.FromArgb(15, 15, 15));
                if (agents[aw_selected].MistakeType == 1)
                {
                    Point[] triangle =
                    {
                        new Point(17, 3),
                        new Point(4, 26),
                        new Point(30, 26)
                    };
                    grfx.FillPolygon(Brushes.Khaki, triangle);
                    grfx.FillRectangle(new SolidBrush(Color.FromArgb(15, 15, 15)), 16, 9, 3, 10);
                    grfx.FillRectangle(new SolidBrush(Color.FromArgb(15, 15, 15)), 16, 21, 3, 3);
                } else if (agents[aw_selected].MistakeType == 2)
                {
                    Point[] hexagon =
                    {
                        new Point(10, 3),
                        new Point(24, 3),
                        new Point(30, 14),
                        new Point(24, 25),
                        new Point(10, 25),
                        new Point(5, 14)
                    };
                    grfx.FillPolygon(Brushes.IndianRed, hexagon);
                    grfx.FillRectangle(new SolidBrush(Color.FromArgb(15, 15, 15)), 10, 13, 15, 3);
                }
                if (agents[aw_selected].MistakeType == 0)
                    MistakeIcon_PB.Visible = false;
                else MistakeIcon_PB.Visible = true;
                MistakeIcon_PB.Image = btm_icon;
                MistakeHint_Label.Text = agents[aw_selected].MDescription;
                if (agents[aw_selected].Star != "" || agents[aw_selected].Satellites.Count > 0)
                {
                    if (agents[aw_selected].Star == "")
                    {
                        Space_Star_Label.Text = "None";
                    } else
                    {
                        Space_Star_Label.Text = agents[aw_selected].Star;
                    }
                    Space_Satellites_LB.Items.Clear();
                    foreach (string item in agents[aw_selected].Satellites)
                    {
                        Space_Satellites_LB.Items.Add(item);
                    }
                    ObjectSpace_Panel.Visible = true;
                } else
                {
                    ObjectSpace_Panel.Visible = false;
                }
            }
            else
            {
                CurrentObjectName_Label.Text = "No object selected";
                CurrentObjectName_Label.ForeColor = Color.DarkGray;
                CurrentObjectName_Label.Cursor = Cursors.Default;
                ObjectSettings_Panel.Visible = false;
            }
        }

        private void Aw_DrawControlLite()
        {
            if (aw_selected != -1)
            {
                ObjectPositionX_Label.Text = ((int)agents[aw_selected].X).ToString();
                ObjectPositionY_Label.Text = ((int)agents[aw_selected].Y).ToString();
                ObjectSettings_Panel.Visible = true;
            }
            else
            {
                CurrentObjectName_Label.Text = "No object selected";
                CurrentObjectName_Label.ForeColor = Color.DarkGray;
                //CurrentObjectName_Label.Cursor = Cursors.Default;
                ObjectSettings_Panel.Visible = false;
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
            Mistake_CMItem.Visible = false;
            SepMistake_CMSepar.Visible = false;
            DeleteObject_CMItem.Visible = false;
            ObjectEditSep_CMSepar.Visible = false;
            SetVelocity_CMItem.Visible = false;
            ChangeSign_CMItem.Visible = false;
            PinUp_CMItem.Visible = false;
            List<AwoseAgent> selects = new();
            int possibleSelection = 0;
            aw_selected = 0;
            foreach (AwoseAgent item in agents)
            {
                item.IsSelected = false;
                if (Calculations.IsInRadius(aw_cursor.X, aw_cursor.Y, item, aw_agentsize * aw_scale))
                {
                    selects.Add(item);
                    possibleSelection = aw_selected++;
                }
                else
                {
                    aw_selected++;
                }
            }
            switch (selects.Count)
            {
                case 0:
                    aw_selected = -1;
                    break;
                case 1:
                    aw_selected = possibleSelection;
                    DeleteObject_CMItem.Visible = true;
                    ObjectEditSep_CMSepar.Visible = true;
                    SetVelocity_CMItem.Visible = true;
                    ChangeSign_CMItem.Visible = true;
                    PinUp_CMItem.Visible = true;
                    PinUp_CMItem.Checked = agents[aw_selected].IsPinned;
                    if (agents[aw_selected].MistakeType > 0)
                    {
                        Mistake_CMItem.Text = agents[aw_selected].MDescription;
                        Mistake_CMItem.Visible = true;
                        SepMistake_CMSepar.Visible = true;
                    }
                    agents[aw_selected].IsSelected = true;
                    break;
                default:
                    aw_selected = -1;
                    break;
            }
            Aw_DrawControl();
            //aw_cursor.X = (int)((-lu_corner.X + Cursor.Position.X - Location.X - ModelBoard_PB.Location.X - 7) / aw_scale);
            //aw_cursor.Y = (int)((-lu_corner.Y + Cursor.Position.Y - Location.Y - ModelBoard_PB.Location.Y - 29) / aw_scale);
        }

        private void CreateObject_CMItem_Click(object sender, EventArgs e)
        {
            aaw_loopNames:
            foreach (AwoseAgent item in agents)
            {
                if (item.Name == "Object " + agentsNumeric.ToString())
                {
                    agentsNumeric++;
                    goto aaw_loopNames;
                }
            }
            PointF newAgentPoint = ScreenToReal(aw_cursor.X, aw_cursor.Y);
            Text = newAgentPoint.X.ToString() + ", " + newAgentPoint.Y.ToString();
            agents.Add(new AwoseAgent("Object " + (agentsNumeric++).ToString(), newAgentPoint.X, newAgentPoint.Y, 1, 0, 0, 0, false));
            aw_undo.Push(new AwoseChange(agents[^1], ChangeType.Creating));
            Aw_CheckMistakes();
            Aw_DrawControl();
        }

        private void ModelBoard_PB_MouseWheel(object sender, MouseEventArgs e)
        {
            PointParticle beforeScaling = GetCursorPosition();
            if (e.Delta > 0)
            {
                aw_scale += .5f;
                lu_corner.X = (int)((beforeScaling.X / aw_scale) - (beforeScaling.X / (aw_scale - .5f)) + lu_corner.X);
                lu_corner.Y = (int)((beforeScaling.Y / aw_scale) - (beforeScaling.Y / (aw_scale - .5f)) + lu_corner.Y);
            }
            else if (aw_scale > 1)
            {
                aw_scale -= .5f;
                lu_corner.X = (int)(-aw_cursor.X * aw_scale + beforeScaling.X);
                lu_corner.Y = (int)(-aw_cursor.Y * aw_scale + beforeScaling.Y);
                //aw_cursor.Y = (int)((-lu_corner.Y + Cursor.Position.Y - Location.Y - ModelBoard_PB.Location.Y - 29) / aw_scale);
            }
            foreach (AwoseAgent item in agents)
            {
                item.X_screen = (int)(lu_corner.X + item.X * aw_scale);
                item.Y_screen = (int)(lu_corner.Y + item.Y * aw_scale);
            }
            float tmpScale = aw_scale;
            while (Math.Round(tmpScale) != tmpScale)
            {
                tmpScale *= 2;
            }
            RT_Scale_Label.Text = tmpScale.ToString() + ":" + (tmpScale / aw_scale).ToString();
        }

        private void DeleteObject_CMItem_Click(object sender, EventArgs e)
        {
            NewValue_TB.Visible = false;
            aw_undo.Push(new AwoseChange(agents[aw_selected], ChangeType.Deleting));
            foreach (AwoseAgent item in agents)
            {
                if (item.Satellites.Contains(agents[aw_selected].Name))
                {
                    item.Satellites.Remove(agents[aw_selected].Name);
                }
                if (item.Star == agents[aw_selected].Name)
                {
                    item.Star = "";
                }
            }
            agents.RemoveAt(aw_selected);
            if (agents.Count > 0)
            {
                aw_selected = agents.Count - 1;
            } else
            {
                aw_selected = -1;
            }
            Aw_CheckMistakes();
            Aw_DrawControl();
        }

        private void Simulation_MSItem_DropDownOpening(object sender, EventArgs e)
        {
            if (aw_undo.Count > 0)
            {
                Undo_MSItem.Text = "Undo " + aw_undo.Peek().ToString();
                Undo_MSItem.Enabled = true;
            } 
            else
            {
                Undo_MSItem.Text = "Undo";
                Undo_MSItem.Enabled = false;
            }
            if (aw_redo.Count > 0)
            {
                Redo_MSItem.Text = "Redo " + aw_redo.Peek().ToString();
                Redo_MSItem.Enabled = true;
            }
            else
            {
                Redo_MSItem.Text = "Redo";
                Redo_MSItem.Enabled = false;
            }

        }

        private void Undo_MSItem_Click(object sender, EventArgs e)
        {
            if (aw_undo.Count == 0) return;
            AwoseChange ch_undo = aw_undo.Pop();
            switch (ch_undo.Type)
            {
                case ChangeType.Creating:
                    for (int i = 0; i < agents.Count; i++)
                        if (agents[i].Name == ch_undo.Subject.Name)
                            agents.RemoveAt(i);
                    break;
                case ChangeType.Deleting:
                    foreach (AwoseAgent item in agents)
                    {
                        if (item.Name == ch_undo.Subject.Name) return;
                    }
                    agents.Add(ch_undo.Subject);
                    break;
                case ChangeType.ChangingMass:
                    foreach (AwoseAgent item in agents)
                    {
                        if (item.Name == ch_undo.Subject.Name) 
                            item.Weight = ch_undo.OldValue;
                    }
                    break;
                case ChangeType.ChangingCharge:
                    foreach (AwoseAgent item in agents)
                    {
                        if (item.Name == ch_undo.Subject.Name)
                            item.Charge = ch_undo.OldValue;
                    }
                    break;
                case ChangeType.ChangingName:
                    foreach (AwoseAgent item in agents)
                    {
                        if (item.Name == ch_undo.NewStringValue)
                            item.Name = ch_undo.OldStringValue;
                    }
                    break;
                case ChangeType.ChangingX:
                    foreach (AwoseAgent item in agents)
                    {
                        if (item.Name == ch_undo.Subject.Name)
                        {
                            item.X = ch_undo.OldValue;
                            lock (item.Spray) item.Spray.Clear();
                        }
                    }
                    break;
                case ChangeType.ChangingY:
                    foreach (AwoseAgent item in agents)
                    {
                        if (item.Name == ch_undo.Subject.Name)
                        {
                            item.Y = ch_undo.OldValue;
                            lock (item.Spray) item.Spray.Clear();
                        }
                    }
                    break;
                case ChangeType.ChangingXY:
                    foreach (AwoseAgent item in agents)
                    {
                        if (item.Name == ch_undo.Subject.Name)
                        {
                            item.X = ch_undo.OldPointValue.X;
                            item.Y = ch_undo.OldPointValue.Y;
                            lock (item.Spray) item.Spray.Clear();
                        }
                    }
                    break;
                case ChangeType.SettingVelocity:
                    foreach (AwoseAgent item in agents)
                    {
                        if (item.Name == ch_undo.Subject.Name)
                        {
                            item.VelocityX = ch_undo.OldPointValue.X;
                            item.VelocityY = ch_undo.OldPointValue.Y;
                            //item.Spray.Clear();
                        }
                    }
                    break;
                default:
                    break;
            }
            aw_redo.Push(ch_undo);
            Simulation_MSItem_DropDownOpening(sender, null);
            Aw_CheckMistakes();
            Aw_DrawControl();
        }

        private void ObjectMass_Label_Click(object sender, EventArgs e)
        {
            NewValue_TB.Location = new Point(ControlAgents_Panel.Location.X + ObjectSettings_Panel.Location.X + ObjectMass_Label.Location.X + 1,
                ControlAgents_Panel.Location.Y + ObjectSettings_Panel.Location.Y + ObjectMass_Label.Location.Y - 26);
            NewValue_TB.Text = agents[aw_selected].Weight.ToString();
            editingValue = EditingValue.Mass;
            NewValue_TB.SelectAll();
            NewValue_TB.Visible = true;
            NewValue_TB.BringToFront();
            NewValue_TB.Focus();
        }

        private void NewValue_TB_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && NewValue_TB.Visible)
            {
                float newValue;
                switch (editingValue)
                {
                    case EditingValue.None:
                        break;
                    case EditingValue.Mass:
                        try
                        {
                            newValue = float.Parse(NewValue_TB.Text);
                            aw_undo.Push(new AwoseChange(agents[aw_selected], ChangeType.ChangingMass, agents[aw_selected].Weight, newValue));
                            agents[aw_selected].Weight = newValue;
                            if (agents[aw_selected].Star != "")
                            {
                                agents[aw_selected].ChangeAfterFSV = true;
                            }
                            else if (agents[aw_selected].Satellites.Count > 0)
                            {
                                foreach (AwoseAgent item in agents)
                                {
                                    if (agents[aw_selected].Satellites.Contains(item.Name))
                                        item.ChangeAfterFSV = true;
                                }
                            }
                        }
                        catch { }
                        finally
                        {
                            NewValue_TB.Visible = false;
                        }
                        break;
                    case EditingValue.Charge:
                        try
                        {
                            newValue = float.Parse(NewValue_TB.Text);
                            aw_undo.Push(new AwoseChange(agents[aw_selected], ChangeType.ChangingCharge, agents[aw_selected].Charge, newValue));
                            agents[aw_selected].Charge = newValue;
                        }
                        catch { }
                        finally
                        {
                            NewValue_TB.Visible = false;
                        }
                        break;
                    case EditingValue.Name:
                        aw_undo.Push(new AwoseChange(agents[aw_selected], ChangeType.ChangingName, agents[aw_selected].Name, NewValue_TB.Text));
                        agents[aw_selected].Name = NewValue_TB.Text;
                        NewValue_TB.Visible = false;
                        break;
                    case EditingValue.X:
                        try
                        {
                            newValue = float.Parse(NewValue_TB.Text);
                            aw_undo.Push(new AwoseChange(agents[aw_selected], ChangeType.ChangingX, agents[aw_selected].X, newValue));
                            agents[aw_selected].X = newValue;
                            if (agents[aw_selected].Star != "")
                            {
                                agents[aw_selected].ChangeAfterFSV = true;
                            }
                            else if (agents[aw_selected].Satellites.Count > 0)
                            {
                                foreach (AwoseAgent item in agents)
                                {
                                    if (agents[aw_selected].Satellites.Contains(item.Name))
                                        item.ChangeAfterFSV = true;
                                }
                            }
                        }
                        catch { }
                        finally
                        {
                            NewValue_TB.Visible = false;
                            lock (agents[aw_selected].Spray)
                                agents[aw_selected].Spray.Clear();
                        }
                        break;
                    case EditingValue.Y:
                        try
                        {
                            newValue = float.Parse(NewValue_TB.Text);
                            aw_undo.Push(new AwoseChange(agents[aw_selected], ChangeType.ChangingY, agents[aw_selected].Y, newValue));
                            agents[aw_selected].Y = newValue;
                            if (agents[aw_selected].Star != "")
                            {
                                agents[aw_selected].ChangeAfterFSV = true;
                            }
                            else if (agents[aw_selected].Satellites.Count > 0)
                            {
                                foreach (AwoseAgent item in agents)
                                {
                                    if (agents[aw_selected].Satellites.Contains(item.Name))
                                        item.ChangeAfterFSV = true;
                                }
                            }
                        }
                        catch { }
                        finally
                        {
                            NewValue_TB.Visible = false;
                            lock (agents[aw_selected].Spray)
                                agents[aw_selected].Spray.Clear();
                        }
                        break;
                    default:
                        break;
                }
            }
            Aw_CheckMistakes();
            Aw_DrawControl();
        }

        private void NewValue_TB_KeyDown(object sender, KeyEventArgs e)
        {
            //e.KeyCode
        }

        private void Redo_MSItem_Click(object sender, EventArgs e)
        {
            if (aw_redo.Count == 0) return;
            AwoseChange ch_redo = aw_redo.Pop();
            switch (ch_redo.Type)
            {
                case ChangeType.Deleting:
                    for (int i = 0; i < agents.Count; i++)
                        if (agents[i].Name == ch_redo.Subject.Name)
                            agents.RemoveAt(i);
                    break;
                case ChangeType.Creating:
                    foreach (AwoseAgent item in agents)
                    {
                        if (item.Name == ch_redo.Subject.Name) return;
                    }
                    agents.Add(ch_redo.Subject);
                    break;
                case ChangeType.ChangingMass:
                    foreach (AwoseAgent item in agents)
                    {
                        if (item.Name == ch_redo.Subject.Name)
                            item.Weight = ch_redo.NewValue;
                    }
                    break;
                case ChangeType.ChangingCharge:
                    foreach (AwoseAgent item in agents)
                    {
                        if (item.Name == ch_redo.Subject.Name)
                            item.Charge = ch_redo.NewValue;
                    }
                    break;
                case ChangeType.ChangingName:
                    foreach (AwoseAgent item in agents)
                    {
                        if (item.Name == ch_redo.OldStringValue)
                            item.Name = ch_redo.NewStringValue;
                    }
                    break;
                case ChangeType.ChangingX:
                    foreach (AwoseAgent item in agents)
                    {
                        if (item.Name == ch_redo.Subject.Name)
                        {
                            item.X = ch_redo.NewValue;
                            item.Spray.Clear();
                        }
                    }
                    break;
                case ChangeType.ChangingY:
                    foreach (AwoseAgent item in agents)
                    {
                        if (item.Name == ch_redo.Subject.Name)
                        {
                            item.Y = ch_redo.NewValue;
                            item.Spray.Clear();
                        }
                    }
                    break;
                case ChangeType.ChangingXY:
                    foreach (AwoseAgent item in agents)
                    {
                        if (item.Name == ch_redo.Subject.Name)
                        {
                            item.X = ch_redo.NewPointValue.X;
                            item.Y = ch_redo.NewPointValue.Y;
                            item.Spray.Clear();
                        }
                    }
                    break;
                case ChangeType.SettingVelocity:
                    foreach (AwoseAgent item in agents)
                    {
                        if (item.Name == ch_redo.Subject.Name)
                        {
                            item.VelocityX = ch_redo.NewPointValue.X;
                            item.VelocityY = ch_redo.NewPointValue.Y;
                            //item.Spray.Clear();
                        }
                    }
                    break;
                default:
                    break;
            }
            aw_undo.Push(ch_redo);
            Simulation_MSItem_DropDownOpening(sender, null);
            Aw_CheckMistakes();
            Aw_DrawControl();
        }

        private void CurrentObjectName_Label_Click(object sender, EventArgs e)
        {
            NewValue_TB.Location = new Point(ControlAgents_Panel.Location.X + CurrentObjectName_Label.Location.X + 1,
                ControlAgents_Panel.Location.Y + CurrentObjectName_Label.Location.Y - 26);
            NewValue_TB.Text = agents[aw_selected].Name;
            editingValue = EditingValue.Name;
            NewValue_TB.SelectAll();
            NewValue_TB.Visible = true;
            NewValue_TB.BringToFront();
            NewValue_TB.Focus();
        }

        private void ModelBoard_PB_MouseDown(object sender, MouseEventArgs e)
        {
            //aw_cursor.X = Cursor.Position.X - Location.X - ModelBoard_PB.Location.X - 7;
            //aw_cursor.Y = Cursor.Position.Y - Location.Y - ModelBoard_PB.Location.Y - 29;
            aw_cursor = GetCursorPosition();
            //Text = aw_cursor.X.ToString() + ", " + aw_cursor.Y.ToString();
            List<AwoseAgent> selects = new();
            PossibleSelections_LB.Visible = false;
            //isObjectMoving = false;
            //isBoardMoving = false;
            movingEntity = MovingEntity.None;
            switch (e.Button)
            {
                case MouseButtons.Left:
                    if (SettingVelocity != -1)
                    {
                        int x = Cursor.Position.X - Location.X - ModelBoard_PB.Location.X - 7;
                        int y = Cursor.Position.Y - Location.Y - ModelBoard_PB.Location.Y - 29;
                        SettingVelocity = -1;
                        //aw_undo.Push(new AwoseChange(agents[aw_selected], ChangeType.SettingVelocity, new Point((int)agents[aw_selected].VelocityX, (int)agents[aw_selected].VelocityY), new Point(x - aw_cursor.X, y - aw_cursor.Y)));
                        agents[aw_selected].VelocityX = aw_cursor.X - aw_remember.X;
                        agents[aw_selected].VelocityY = aw_cursor.Y - aw_remember.Y;
                        return;
                    }
                    if (isFirstSpaceSetting)
                    {
                        int rel = 0;
                        foreach (AwoseAgent item in agents)
                        {
                            if (Calculations.IsInRadius(aw_cursor.X, aw_cursor.Y, item, aw_agentsize * aw_scale))
                                break;
                            else rel++;
                        }
                        if (rel < agents.Count)
                        {
                            isFirstSpaceSetting = false;
                            double distance = Math.Sqrt(Math.Pow(agents[rel].X - agents[aw_selected].X, 2) + Math.Pow(agents[rel].Y - agents[aw_selected].Y, 2));
                            float tempFV = Calculations.FirstSpace(agents[rel], agents[aw_selected]);
                            agents[aw_selected].VelocityY = (agents[rel].X - agents[aw_selected].X) * tempFV / distance;
                            agents[aw_selected].VelocityX = (agents[aw_selected].Y - agents[rel].Y) * tempFV / distance;
                            agents[aw_selected].ChangeAfterFSV = false;
                            agents[rel].ChangeAfterFSV = false;
                            agents[aw_selected].Star = agents[rel].Name;
                            if (!agents[rel].Satellites.Contains(agents[aw_selected].Name))
                                agents[rel].Satellites.Add(agents[aw_selected].Name);
                        } else
                        {
                            isFirstSpaceSetting = false;
                        }
                    }
                    int possibleSelection = 0;
                    aw_selected = 0;
                    PointF point = ScreenToReal(aw_cursor.X, aw_cursor.Y);
                    foreach (AwoseAgent item in agents)
                    {
                        item.IsSelected = false;
                        if (Calculations.IsInRadius((int)point.X, (int)point.Y, item, aw_agentsize * aw_scale))
                        {
                            selects.Add(item);
                            possibleSelection = aw_selected++;
                        } 
                        else
                        {
                            aw_selected++;
                        }
                    }
                    switch (selects.Count)
                    {
                        case 0:
                            aw_selected = -1;
                            break;
                        case 1:
                            aw_selected = possibleSelection;
                            isObjectMoving = true;
                            //aw_cursor = Cursor.Position;
                            //lu_remember = new Point((int)agents[aw_selected].X,
                            //    (int)agents[aw_selected].Y);
                            agents[aw_selected].IsSelected = true;
                            break;
                        default:
                            //Text = selects.Count.ToString();
                            aw_selected = -1;
                            PossibleSelections_LB.Items.Clear();
                            foreach (AwoseAgent item in selects)
                            {
                                PossibleSelections_LB.Items.Add(item.Name);
                            }
                            PossibleSelections_LB.Location = new Point(Cursor.Position.X,
                                Cursor.Position.Y - 10);
                            PossibleSelections_LB.Height = (PossibleSelections_LB.Items.Count + 1) * PossibleSelections_LB.ItemHeight;
                            PossibleSelections_LB.Visible = true;
                            break;
                    }
                    Aw_DrawControl();
                    break;
                case MouseButtons.Middle:
                    //isBoardMoving = true;
                    movingEntity = MovingEntity.Board;
                    Cursor = Cursors.NoMove2D;
                    //aw_cursor = Cursor.Position;
                    aw_remember = GetCursorPosition();
                    lu_remember = lu_corner;
                    break;
                default:
                    break;
            }
        }

        private void ModelBoard_PB_MouseUp(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    if (isObjectMoving && aw_selected < agents.Count && (agents[aw_selected].X != lu_remember.X || agents[aw_selected].Y != lu_remember.Y))
                    {
                        agents[aw_selected].Spray.Clear();
                        //agents[aw_selected].X_screen = aw_cursor.X - Cursor.Position.X;
                        //agents[aw_selected].Y_screen = aw_cursor.Y - Cursor.Position.Y;
                        //aw_undo.Push(new AwoseChange(agents[aw_selected], ChangeType.ChangingXY, lu_remember, new Point((int)agents[aw_selected].X, (int)agents[aw_selected].Y)));
                        if (agents[aw_selected].Star != "")
                        {
                            agents[aw_selected].ChangeAfterFSV = true;
                        } else if (agents[aw_selected].Satellites.Count > 0)
                        {
                            foreach (AwoseAgent item in agents)
                            {
                                if (agents[aw_selected].Satellites.Contains(item.Name))
                                    item.ChangeAfterFSV = true;
                            }
                        }
                    }
                    isObjectMoving = false;
                    break;
                case MouseButtons.Right:
                    break;
                case MouseButtons.Middle:
                    isBoardMoving = false;
                    foreach (AwoseAgent item in agents)
                    {
                        item.X_screen = (int)(lu_corner.X + item.X * aw_scale);
                        item.Y_screen = (int)(lu_corner.Y + item.Y * aw_scale);
                    }
                    break;
                default:
                    break;
            }
            Aw_CheckMistakes();
        }

        private void ModelBoard_PB_MouseMove(object sender, MouseEventArgs e)
        {
            aw_cursor = GetCursorPosition();
            PointF pointCursor = ScreenToReal(aw_cursor.X, aw_cursor.Y);
            RT_X_Label.Text = Math.Round(pointCursor.X, 2).ToString();
            RT_Y_Label.Text = Math.Round(pointCursor.Y, 2).ToString();
            bool hoverAgent = false;
            switch (movingEntity)
            {
                case MovingEntity.None:
                    foreach (AwoseAgent item in agents)
                    {
                        if (Calculations.IsInRadius(aw_cursor.X, aw_cursor.Y, item, aw_agentsize * aw_scale))
                        {
                            hoverAgent = true;
                            break;
                        }
                    }
                    if (hoverAgent)
                    {
                        Cursor = Cursors.Default;
                    }
                    else
                    {
                        Cursor = Cursors.Cross;
                    }
                    break;
                case MovingEntity.Board:
                    Cursor = Cursors.NoMove2D;
                    //lu_corner = lu_remember - ((aw_remember - GetCursorPosition()) / aw_scale);
                    break;
                case MovingEntity.Agent:
                    break;
                default:
                    break;
            }
            
            
            if (isBoardMoving)
            {
                //lu_corner = new Point(lu_remember.X - (aw_cursor.X - Cursor.Position.X),
                //lu_remember.Y - (aw_cursor.Y - Cursor.Position.Y));
            }
            if (isObjectMoving && !isLaunched)
            {
                agents[aw_selected].X = lu_remember.X - (aw_cursor.X - Cursor.Position.X) / aw_scale;
                agents[aw_selected].Y = lu_remember.Y - (aw_cursor.Y - Cursor.Position.Y) / aw_scale;
            }
        }

        private void MistakeIcon_PB_MouseHover(object sender, EventArgs e)
        {
            MistakeHint_Label.Visible = true;
        }

        private void MistakeIcon_PB_MouseLeave(object sender, EventArgs e)
        {
            MistakeHint_Label.Visible = false;
        }

        private void NewValue_TB_TextChanged(object sender, EventArgs e)
        {

        }

        private void ObjectCharge_Label_Click(object sender, EventArgs e)
        {
            NewValue_TB.Location = new Point(ControlAgents_Panel.Location.X + ObjectSettings_Panel.Location.X + ObjectCharge_Label.Location.X + 1,
                ControlAgents_Panel.Location.Y + ObjectSettings_Panel.Location.Y + ObjectCharge_Label.Location.Y - 26);
            NewValue_TB.Text = agents[aw_selected].Charge.ToString();
            editingValue = EditingValue.Charge;
            NewValue_TB.SelectAll();
            NewValue_TB.Visible = true;
            NewValue_TB.BringToFront();
            NewValue_TB.Focus();
        }

        private void NewValue_TB_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void LaunchSimulation_MSItem_Click(object sender, EventArgs e)
        {
            foreach (AwoseAgent item in agents)
            {
                item.Backup();
            }
            isLaunched = true;
            LaunchSimulation_MSItem.Enabled = false;
            PauseSimulation_MSItem.Enabled = true;
            StopSimulation_MSItem.Enabled = true;
            ResetSimulation_MSItem.Enabled = true;
            Aw_CheckMistakes();
        }

        private void StopSimulation_MSItem_Click(object sender, EventArgs e)
        {
            isLaunched = false;
            LaunchSimulation_MSItem.Enabled = true;
            PauseSimulation_MSItem.Enabled = false;
            StopSimulation_MSItem.Enabled = false;
            ResetSimulation_MSItem.Enabled = true;
            Aw_CheckMistakes();
        }
        private void ObjectPositionX_Label_Click(object sender, EventArgs e)
        {
            NewValue_TB.Location = new Point(ControlAgents_Panel.Location.X + ObjectSettings_Panel.Location.X + ObjectPositionX_Label.Location.X + 1,
                ControlAgents_Panel.Location.Y + ObjectSettings_Panel.Location.Y + ObjectPositionX_Label.Location.Y - 26);
            NewValue_TB.Text = agents[aw_selected].X.ToString();
            editingValue = EditingValue.X;
            NewValue_TB.SelectAll();
            NewValue_TB.Visible = true;
            NewValue_TB.BringToFront();
            NewValue_TB.Focus();
        }

        private void ObjectPositionY_Label_Click(object sender, EventArgs e)
        {
            NewValue_TB.Location = new Point(ControlAgents_Panel.Location.X + ObjectSettings_Panel.Location.X + ObjectPositionY_Label.Location.X + 1,
                ControlAgents_Panel.Location.Y + ObjectSettings_Panel.Location.Y + ObjectPositionY_Label.Location.Y - 26);
            NewValue_TB.Text = agents[aw_selected].Y.ToString();
            editingValue = EditingValue.Y;
            NewValue_TB.SelectAll();
            NewValue_TB.Visible = true;
            NewValue_TB.BringToFront();
            NewValue_TB.Focus();
        }

        private void ChangeSign_CMItem_Click(object sender, EventArgs e)
        {
            agents[aw_selected].Charge = -agents[aw_selected].Charge;
            Aw_DrawControl();
        }

        private void Awose_FormClosing(object sender, FormClosingEventArgs e)
        {
            animation.Interrupt();
        }

        private void PinUp_CMItem_Click(object sender, EventArgs e)
        {
            agents[aw_selected].IsPinned = !agents[aw_selected].IsPinned;
            Aw_CheckMistakes();
        }

        private void SetVelocity_CMItem_Click(object sender, EventArgs e)
        {
            Space_CMStr.Close();
            SettingVelocity = aw_selected;
            //aw_remember = new Point((int)agents[aw_selected].X, (int)agents[aw_selected].Y);
        }

        private void ResetVelocity_CMItem_Click(object sender, EventArgs e)
        {
            agents[aw_selected].VelocityX = 0;
            agents[aw_selected].VelocityY = 0;
        }

        private void SetFirstSpace_CMItem_Click(object sender, EventArgs e)
        {
            isFirstSpaceSetting = true;
        }

        private void PossibleSelections_LB_SelectedIndexChanged(object sender, EventArgs e)
        {
            int possibleSelect = 0;
            foreach (AwoseAgent item in agents)
            {
                if (item.Name == PossibleSelections_LB.SelectedItem.ToString())
                    break;
                else possibleSelect++;
            }
            aw_selected = possibleSelect;
            PossibleSelections_LB.Visible = false;
            Aw_DrawControl();
        }

        private void CreateStar_CMItem_Click(object sender, EventArgs e)
        {
            int starsNumeric = 1;
        aaw_loopStarNames:
            foreach (AwoseAgent item in agents)
            {
                if (item.Name == "Star " + starsNumeric.ToString())
                {
                    starsNumeric++;
                    goto aaw_loopStarNames;
                }
            }
            agents.Add(new AwoseAgent("Star " + starsNumeric.ToString(), aw_cursor.X, aw_cursor.Y, 150, 0, 0, 0, true));
            aw_undo.Push(new AwoseChange(agents[^1], ChangeType.Creating));
            Aw_CheckMistakes();
            Aw_DrawControl();
        }

        private void ResetSimulation_MSItem_Click(object sender, EventArgs e)
        {
            foreach (AwoseAgent item in agents)
            {
                item.Restore();
            }
            Aw_CheckMistakes();
        }

        private void ModelBoard_PB_Click(object sender, EventArgs e)
        {

        }
    }
}
