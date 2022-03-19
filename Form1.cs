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
    public partial class Awose : Form
    {
        readonly List<AwoseAgent> agents = new();
        int agentsNumeric = 1;
        readonly Stack<AwoseChange> aw_undo = new();
        readonly Stack<AwoseChange> aw_redo = new();
        int aw_selected = 0;
        //represents click-point in in-simulation coordinates
        Point aw_cursor = new(0, 0);
        //represents coordinate of up-left corner in in-simulation coordinates
        Point lu_corner = new(0, 0);
        //represents remembered point in screen coordinates
        Point lu_remember = new(0, 0);
        Point objBeforeMoving = new(0, 0);
        float aw_scale = 1;
        const int aw_agentsize = 15;
        EditingValue editingValue = EditingValue.None;
        bool isBoardMoving = false;
        bool isObjectMoving = false;
        bool isLaunched = false;
        bool isFirstSpaceSetting = false;
        int SettingVelocity = -1;
        //int c = -1;
        //constants
        public static int timeStep = 20;
        public static float ConstG = 100000;
        public static float ConstE = 100000;

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
                float dx = aw_cursor.X - (Cursor.Position.X - Location.X - ModelBoard_PB.Location.X - 7);
                float dy = aw_cursor.Y - (Cursor.Position.Y - Location.Y - ModelBoard_PB.Location.Y - 29);
                float l = (float)Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));
                float ax = 15 * dx / l;
                float ay = 15 * dy / l;
                float bx = 7 * dy / l;
                float by = 7 * dx / l;
                Point[] arrow =
                {
                    new Point(x, y),
                    new Point(x + (int)ax - (int)bx, y + (int)ay + (int)by),
                    new Point(x + (int)ax + (int)bx, y + (int)ay - (int)by)
                };
                grfx.DrawLine(new Pen(Brushes.Tomato, 2),
                    new Point(aw_cursor.X, aw_cursor.Y),
                    new Point(Cursor.Position.X - Location.X - ModelBoard_PB.Location.X - 7,
                    Cursor.Position.Y - Location.Y - ModelBoard_PB.Location.Y - 29));
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

                    RectangleF circle = new((float)(lu_corner.X + item.X * aw_scale - diameter / 2), (float)(lu_corner.Y + item.Y * aw_scale - diameter / 2), diameter, diameter);
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
                            grfx.FillEllipse(item.Dye, circle);
                            if (item.IsSelected)
                            {
                                RectangleF bigcircle = new((float)(lu_corner.X + item.X * aw_scale - (diameter * 2) / 2), (float)(lu_corner.Y + item.Y * aw_scale - (diameter * 2) / 2), (int)(diameter * 2), (int)(diameter * 2));
                                grfx.DrawEllipse(new Pen(Brushes.White, 2 * aw_scale), bigcircle);
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
            //Moved after setting first space velocity
            foreach (AwoseAgent item in agents)
                if (item.IsFirstSpace && item.MovedAfterSetting)
                {
                    item.MistakeType = 1;
                    item.MDescription = "The 1st space velocity should be set again";
                }
        }

        private void Aw_DrawControl()
        {
            if (aw_selected < agents.Count)
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
            if (aw_selected < agents.Count)
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
            aw_cursor.X = (int)((-lu_corner.X + Cursor.Position.X - Location.X - ModelBoard_PB.Location.X - 7) / aw_scale);
            aw_cursor.Y = (int)((-lu_corner.Y + Cursor.Position.Y - Location.Y - ModelBoard_PB.Location.Y - 29) / aw_scale);
            aw_selected = 0;
            Mistake_CMItem.Visible = false;
            SepMistake_CMSepar.Visible = false;
            DeleteObject_CMItem.Visible = false;
            ObjectEditSep_CMSepar.Visible = false;
            SetVelocity_CMItem.Visible = false;
            ChangeSign_CMItem.Visible = false;
            PinUp_CMItem.Visible = false;
            foreach (AwoseAgent item in agents)
            {
                if (Calculations.IsInRadius(aw_cursor.X, aw_cursor.Y, item, aw_agentsize * aw_scale))
                    aw_selected++;
                else
                {
                    DeleteObject_CMItem.Visible = true;
                    ObjectEditSep_CMSepar.Visible = true;
                    SetVelocity_CMItem.Visible = true;
                    ChangeSign_CMItem.Visible = true;
                    PinUp_CMItem.Visible = true;
                    PinUp_CMItem.Checked = agents[aw_selected].IsPinned;
                    if (item.MistakeType > 0)
                    {
                        Mistake_CMItem.Text = item.MDescription;
                        Mistake_CMItem.Visible = true;
                        SepMistake_CMSepar.Visible = true;
                    }
                    break;
                }
            }
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
            agents.Add(new AwoseAgent("Object " + (agentsNumeric++).ToString(), aw_cursor.X, aw_cursor.Y, 1, 0, 0, 0, false));
            aw_undo.Push(new AwoseChange(agents[^1], ChangeType.Creating));
            Aw_CheckMistakes();
            Aw_DrawControl();
        }

        private void ModelBoard_PB_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Point beforeScaling = new();
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

        private void DeleteObject_CMItem_Click(object sender, EventArgs e)
        {
            NewValue_TB.Visible = false;
            aw_undo.Push(new AwoseChange(agents[aw_selected], ChangeType.Deleting));
            agents.RemoveAt(aw_selected);
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
            NewValue_TB.Location = new Point(Control_Panel.Location.X + ObjectSettings_Panel.Location.X + ObjectMass_Label.Location.X + 1,
                Control_Panel.Location.Y + ObjectSettings_Panel.Location.Y + ObjectMass_Label.Location.Y - 26);
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
            NewValue_TB.Location = new Point(Control_Panel.Location.X + CurrentObjectName_Label.Location.X + 1,
                Control_Panel.Location.Y + CurrentObjectName_Label.Location.Y - 26);
            NewValue_TB.Text = agents[aw_selected].Name;
            editingValue = EditingValue.Name;
            NewValue_TB.SelectAll();
            NewValue_TB.Visible = true;
            NewValue_TB.BringToFront();
            NewValue_TB.Focus();
        }

        private void ModelBoard_PB_MouseDown(object sender, MouseEventArgs e)
        {
            aw_cursor.X = (int)((-lu_corner.X + Cursor.Position.X - Location.X - ModelBoard_PB.Location.X - 7) / aw_scale);
            aw_cursor.Y = (int)((-lu_corner.Y + Cursor.Position.Y - Location.Y - ModelBoard_PB.Location.Y - 29) / aw_scale);
            Text = aw_cursor.X.ToString() + " " + aw_cursor.Y.ToString();
            List<AwoseAgent> selects = new();
            PossibleSelections_LB.Visible = false;
            isObjectMoving = false;
            switch (e.Button)
            {
                case MouseButtons.Left:
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
                            isObjectMoving = true;
                            aw_cursor = Cursor.Position;
                            lu_remember = new Point((int)agents[aw_selected].X,
                                (int)agents[aw_selected].Y);
                            agents[aw_selected].IsSelected = true;
                            break;
                        default:
                            //Text = selects.Count.ToString();
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
                    break;
                case MouseButtons.Right:
                    break;
                case MouseButtons.Middle:
                    break;
                default:
                    break;
            }



            //if (SettingVelocity != -1) {
            //    lu_remember = new Point((int)agents[aw_selected].X,
            //            (int)agents[aw_selected].Y);
            //    return; 
            //}
            //switch (e.Button)
            //{
            //    case MouseButtons.Left:
            //        if (aw_selected >= agents.Count) return;
            //        isObjectMoving = true;
            //        aw_cursor = Cursor.Position;
            //        lu_remember = new Point((int)agents[aw_selected].X,
            //            (int)agents[aw_selected].Y);
            //        break;
            //    case MouseButtons.Middle:
            //        isBoardMoving = true;
            //        aw_cursor = Cursor.Position;
            //        lu_remember = lu_corner;
            //        break;
            //}
        }

        private void ModelBoard_PB_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (SettingVelocity != -1)
                {
                    int x = Cursor.Position.X - Location.X - ModelBoard_PB.Location.X - 7;
                    int y = Cursor.Position.Y - Location.Y - ModelBoard_PB.Location.Y - 29;
                    SettingVelocity = -1;
                    aw_undo.Push(new AwoseChange(agents[aw_selected], ChangeType.SettingVelocity, new Point((int)agents[aw_selected].VelocityX, (int)agents[aw_selected].VelocityY), new Point(x - aw_cursor.X, y - aw_cursor.Y)));
                    agents[aw_selected].VelocityX = x - aw_cursor.X;
                    agents[aw_selected].VelocityY = y - aw_cursor.Y;
                }
                aw_cursor.X = (int)((-lu_corner.X + Cursor.Position.X - Location.X - ModelBoard_PB.Location.X - 7) / aw_scale);
                aw_cursor.Y = (int)((-lu_corner.Y + Cursor.Position.Y - Location.Y - ModelBoard_PB.Location.Y - 29) / aw_scale);
                if (isFirstSpaceSetting)
                {
                    int rel = -1;
                    foreach (AwoseAgent item in agents)
                    {
                        if (Calculations.IsInRadius(aw_cursor.X, aw_cursor.Y, item, aw_agentsize * aw_scale))
                            rel++;
                        else break;
                    }
                    rel++;
                    if (rel != -1)
                    {
                        isFirstSpaceSetting = false;
                        double distance = Math.Sqrt(Math.Pow(agents[rel].X - agents[aw_selected].X, 2) + Math.Pow(agents[rel].Y - agents[aw_selected].Y, 2));
                        float tempFV = Calculations.FirstSpace(agents[rel], agents[aw_selected]);
                        agents[aw_selected].VelocityY = (agents[rel].X - agents[aw_selected].X) * tempFV / distance;
                        agents[aw_selected].VelocityX = (agents[aw_selected].Y - agents[rel].Y) * tempFV / distance;
                        agents[aw_selected].IsFirstSpace = true;
                        agents[aw_selected].MovedAfterSetting = false;
                        agents[rel].IsFirstSpace = true;
                        agents[rel].MovedAfterSetting = false;
                    }
                }
                aw_selected = 0;
                foreach (AwoseAgent item in agents)
                {
                    if (Calculations.IsInRadius(aw_cursor.X, aw_cursor.Y, item, aw_agentsize * aw_scale))
                        aw_selected++;
                    else break;
                }
                Aw_DrawControl();
            }
            isBoardMoving = false;
            if (isLaunched) return;
            if (isObjectMoving && aw_selected < agents.Count && (agents[aw_selected].X != lu_remember.X || agents[aw_selected].Y != lu_remember.Y)) { 
                agents[aw_selected].Spray.Clear();
                aw_undo.Push(new AwoseChange(agents[aw_selected], ChangeType.ChangingXY, lu_remember, new Point((int)agents[aw_selected].X, (int)agents[aw_selected].Y)));
                if (agents[aw_selected].IsFirstSpace)
                {
                    agents[aw_selected].MovedAfterSetting = true;
                }
            }
            isObjectMoving = false;
            Aw_CheckMistakes();
        }

        private void ModelBoard_PB_MouseMove(object sender, MouseEventArgs e)
        {
            if (isBoardMoving)
            lu_corner = new Point(lu_remember.X - (aw_cursor.X - Cursor.Position.X),
                lu_remember.Y - (aw_cursor.Y - Cursor.Position.Y));
            if (isLaunched) return;
            if (isObjectMoving)
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
            NewValue_TB.Location = new Point(Control_Panel.Location.X + ObjectSettings_Panel.Location.X + ObjectCharge_Label.Location.X + 1,
                Control_Panel.Location.Y + ObjectSettings_Panel.Location.Y + ObjectCharge_Label.Location.Y - 26);
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
            ResetSimulation_MSItem.Enabled = false;
            Aw_CheckMistakes();
        }
        private void ObjectPositionX_Label_Click(object sender, EventArgs e)
        {
            NewValue_TB.Location = new Point(Control_Panel.Location.X + ObjectSettings_Panel.Location.X + ObjectPositionX_Label.Location.X + 1,
                Control_Panel.Location.Y + ObjectSettings_Panel.Location.Y + ObjectPositionX_Label.Location.Y - 26);
            NewValue_TB.Text = agents[aw_selected].X.ToString();
            editingValue = EditingValue.X;
            NewValue_TB.SelectAll();
            NewValue_TB.Visible = true;
            NewValue_TB.BringToFront();
            NewValue_TB.Focus();
        }

        private void ObjectPositionY_Label_Click(object sender, EventArgs e)
        {
            NewValue_TB.Location = new Point(Control_Panel.Location.X + ObjectSettings_Panel.Location.X + ObjectPositionY_Label.Location.X + 1,
                Control_Panel.Location.Y + ObjectSettings_Panel.Location.Y + ObjectPositionY_Label.Location.Y - 26);
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
    }
}
