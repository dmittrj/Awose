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
    enum EditingValue { None, Mass, Charge, Name }
    public partial class Awose : Form
    {
        readonly List<AwoseAgent> agents = new();
        int agentsNumeric = 1;
        readonly Stack<AwoseChange> aw_undo = new();
        readonly Stack<AwoseChange> aw_redo = new();
        int aw_selected = 0;
        Point aw_cursor = new(0, 0);
        Point lu_corner = new(0, 0);
        Point lu_remember = new(0, 0);
        float aw_scale = 1;
        const int aw_agentsize = 15;
        EditingValue editingValue = EditingValue.None;
        bool isBoardMoving = false;

        private void Aw_Refresh()
        {
            float diameter = aw_agentsize * aw_scale;
            Bitmap board = new(ModelBoard_PB.Width, ModelBoard_PB.Height);
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
                    if (item.MistakeType == 2)
                        grfx.FillRectangle(new SolidBrush(Color.FromArgb(Calculations.Normilize(0, 255, (int)(-0.44 * (dotNumber) + 255)), Calculations.Normilize(0, 255, (int)(-0.28 * (dotNumber) + 175)), Calculations.Normilize(0, 255, (int)(-0.024 * (dotNumber++) + 47)))), spraydot);
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

        private void Aw_CheckMistakes()
        {
            LaunchSimulation_MSItem.Enabled = true;
            foreach (AwoseAgent item in agents)
            {
                item.MistakeType = 0;
            }
            //Useless object
            if (agents.Count == 1)
            {
                agents[0].MistakeType = 1;
                agents[0].MDescription = "Useless object";
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
            foreach (AwoseAgent item in agents)
            {
                if (Calculations.IsInRadius(aw_cursor.X, aw_cursor.Y, item, aw_agentsize * aw_scale))
                    aw_selected++;
                else
                {
                    DeleteObject_CMItem.Visible = true;
                    ObjectEditSep_CMSepar.Visible = true;
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

        private void ModelBoard_PB_Click(object sender, EventArgs e)
        {
            aw_cursor.X = (int)((-lu_corner.X + Cursor.Position.X - Location.X - ModelBoard_PB.Location.X - 7) / aw_scale);
            aw_cursor.Y = (int)((-lu_corner.Y + Cursor.Position.Y - Location.Y - ModelBoard_PB.Location.Y - 29) / aw_scale);
            aw_selected = 0;
            foreach (AwoseAgent item in agents)
            {
                if (Calculations.IsInRadius(aw_cursor.X, aw_cursor.Y, item, aw_agentsize * aw_scale))
                    aw_selected++;
                else break;
            }
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
                case ChangeType.ChangingName:
                    foreach (AwoseAgent item in agents)
                    {
                        if (item.Name == ch_undo.NewStringValue)
                            item.Name = ch_undo.OldStringValue;
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
                case ChangeType.ChangingName:
                    foreach (AwoseAgent item in agents)
                    {
                        if (item.Name == ch_redo.OldStringValue)
                            item.Name = ch_redo.NewStringValue;
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
            NewValue_TB.Visible = true;
            NewValue_TB.BringToFront();
            NewValue_TB.Focus();
        }

        private void ModelBoard_PB_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                isBoardMoving = true;
                aw_cursor = Cursor.Position;
                lu_remember = lu_corner;
            }
        }

        private void ModelBoard_PB_MouseUp(object sender, MouseEventArgs e)
        {
            isBoardMoving = false;
        }

        private void ModelBoard_PB_MouseMove(object sender, MouseEventArgs e)
        {
            if (isBoardMoving)
            lu_corner = new Point(lu_remember.X - (aw_cursor.X - Cursor.Position.X),
                lu_remember.Y - (aw_cursor.Y - Cursor.Position.Y));
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
            NewValue_TB.Visible = true;
            NewValue_TB.BringToFront();
            NewValue_TB.Focus();
        }

        private void NewValue_TB_TextChanged_1(object sender, EventArgs e)
        {

        }
    }
}
