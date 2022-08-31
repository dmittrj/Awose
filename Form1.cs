using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Awose
{
    enum EditingValue { None, Mass, Charge, Name, X, Y, VelocityLength }

    enum MovingEntity { None, Board, Agent }

    enum SpecialCondition { None, SetVelocity, SetFirstSpaceVelocity}

    enum BeautyPreviewMode { None, ObjectColor }
    public partial class Awose : Form
    {
        [Obsolete]
        readonly List<AwoseAgent> agents = new();
        public DrawingValues drawingValues = new();
        public static List<AwoseLayer> Layers { get; set; }
        private int CurrentLayer = 0;
        //private readonly int agentsNumeric = 1;
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
        private Vector CopiedVelocity = null;
        private Point objBeforeMoving = new(0, 0);
        private Point boardBeforeMoving = new(0, 0);
        private float aw_scale = 1;
        const int aw_agentsize = 15;
        private EditingValue editingValue = EditingValue.None;
        private MovingEntity movingEntity = MovingEntity.None;
        private SpecialCondition specialCondition = SpecialCondition.None;
        private BeautyPreviewMode beautyPreview = BeautyPreviewMode.None;
        //[Obsolete]
        //private bool isBoardMoving = false;
        //[Obsolete]
        //private bool isObjectMoving = false;
        private bool isLaunched = false;
        [Obsolete]
        private bool isFirstSpaceSetting = false;
        private AwoseAgent Phantom = null;
        private int AnimationCounter = 0;
        //int c = -1;
        //constants
        public static int timeStep = 20;
        public static float ConstG = 100000;
        public static float ConstE = 100000;

        public static bool IsActive = true;

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

        [Obsolete]
        private Point RealToScreen(double realX, double realY)
        {
            return new(
                (int)((realX + lu_corner.X) * aw_scale),
                (int)((realY + lu_corner.Y) * aw_scale));
        }

        private PointParticle RealToScreen(PointParticle realPoint)
        {
            return new(
                (int)((realPoint.X + lu_corner.X) * aw_scale),
                (int)((realPoint.Y + lu_corner.Y) * aw_scale));
        }
        
        private PointParticle GetCursorPosition()
        {
            return new PointParticle(
                Cursor.Position.X - Location.X - ModelBoard_PB.Location.X - 8,
                Cursor.Position.Y - Location.Y - ModelBoard_PB.Location.Y - 30);
        }

        private void Aw_Refresh()
        {
            AnimationCounter++;
            AnimationCounter %= 360;
            const int GRID_FREQUENCY = 100;
            float diameter = aw_agentsize * aw_scale;
            if (ModelBoard_PB.Width <= 0 || ModelBoard_PB.Height <= 0) return;
            Bitmap board = new(ModelBoard_PB.Width, ModelBoard_PB.Height);
            using Graphics grfx = Graphics.FromImage(board);
            //Drawing background
            grfx.Clear(Color.FromArgb(5, 5, 5));
            //Displaying grid
            int horLine = (int)(lu_corner.X * aw_scale) - Calculations.BruteRound(lu_corner.X * aw_scale, aw_scale * GRID_FREQUENCY);
            int verLine = (int)(lu_corner.Y * aw_scale) - Calculations.BruteRound(lu_corner.Y * aw_scale, aw_scale * GRID_FREQUENCY);
            for (int i = horLine + (int)(aw_scale * GRID_FREQUENCY / 2); i < ModelBoard_PB.Width; i += (int)(aw_scale * GRID_FREQUENCY))
            {
                grfx.DrawLine(new Pen(Layers[CurrentLayer].GridColorSub, 2),
                    new Point(i, 0),
                    new Point(i, drawingValues.GridHeight));
            }
            for (int i = verLine + (int)(aw_scale * GRID_FREQUENCY / 2); i < ModelBoard_PB.Height; i += (int)(aw_scale * GRID_FREQUENCY))
            {
                grfx.DrawLine(new Pen(Layers[CurrentLayer].GridColorSub, 2),
                    new Point(0, i),
                    new Point(drawingValues.GridWidth, i));
            }
            for (int i = horLine; i < ModelBoard_PB.Width; i += (int)(aw_scale * GRID_FREQUENCY))
            {
                grfx.DrawLine(new Pen(Layers[CurrentLayer].GridColorMain, 2),
                    new Point(i, 0),
                    new Point(i, drawingValues.GridHeight));
            }
            for (int i = verLine; i < ModelBoard_PB.Height; i += (int)(aw_scale * GRID_FREQUENCY))
            {
                grfx.DrawLine(new Pen(Layers[CurrentLayer].GridColorMain, 2),
                    new Point(0, i),
                    new Point(drawingValues.GridWidth, i));
            }
            //Hiding or drawing grid due to checkbox
            if (DispGrid_Editing_CMItem.Checked)
            {
                if (drawingValues.GridWidth < ModelBoard_PB.Width)
                {
                    drawingValues.GridWidth += 60;
                }
                if (drawingValues.GridHeight < ModelBoard_PB.Height)
                {
                    drawingValues.GridHeight += 60;
                }
            }
            else
            {
                if (drawingValues.GridWidth > 0)
                {
                    drawingValues.GridWidth -= 60;
                }
                if (drawingValues.GridHeight > 0)
                {
                    drawingValues.GridHeight -= 60;
                }
            }

            //Drawing preview
            switch (beautyPreview)
            {
                case BeautyPreviewMode.None:
                    break;
                case BeautyPreviewMode.ObjectColor:
                    BeautyPreview_PB.Location = new Point(306 - drawingValues.PreviewPosition, 334);
                    BeautyPreview_PB.Image = DrawingValues.DrawObjectPreview(drawingValues.HoveredColor, (int)(255 - drawingValues.PreviewPosition * 5.1));
                    if (drawingValues.PreviewPosition < 50)
                        drawingValues.PreviewPosition += 4;
                    break;
                default:
                    break;
            }

            //Drawing trajectories
            lock (Layers)
            {
                foreach (AwoseLayer layer in Layers)
                {
                    lock (layer.Agents)
                    {
                        foreach (AwoseAgent agent in layer.Agents)
                        {
                            lock (agent.Trajectory)
                            {
                                if (agent.TrajectoryLine == TrajectoryType.None) continue;
                                if (agent.Trajectory.Count < 3) continue;
                                PointParticle point1 = agent.Trajectory.Dequeue();
                                agent.Trajectory.Enqueue(point1);
                                for (int i = 1; i < agent.Trajectory.Count; i++)
                                {
                                    PointParticle point2 = agent.Trajectory.Dequeue();
                                    agent.Trajectory.Enqueue(point2);
                                    grfx.DrawLine(new Pen(agent.Dye, 2), RealToScreen(point1).ToPoint(), RealToScreen(point2).ToPoint());
                                    point1 = point2;
                                }
                                if (agent.TrajectoryLine == TrajectoryType.Fade && agent.Trajectory.Count > 100)
                                {
                                    agent.Trajectory.Dequeue();
                                }
                            }
                        }
                    }
                }
            }

            //Drawing arrows, lines, ...
            Brush mainVelocityArrow;
            if (editingValue == EditingValue.VelocityLength)
            {
                mainVelocityArrow = Brushes.RosyBrown;
            } else
            {
                mainVelocityArrow = Brushes.Tomato;
            }

            switch (specialCondition)
            {
                case SpecialCondition.None:
                    if (Layers[CurrentLayer].IsThereSelections())
                    {
                        if (Layers[CurrentLayer].Agents[Layers[CurrentLayer].Selected].Velocity.Length > 0)
                        {
                            PointParticle vel_arrow1 = RealToScreen(Layers[CurrentLayer].Agents[Layers[CurrentLayer].Selected].Location + Layers[CurrentLayer].Agents[Layers[CurrentLayer].Selected].Velocity.Tail);
                            PointParticle vel_arrow2 = RealToScreen(Layers[CurrentLayer].Agents[Layers[CurrentLayer].Selected].Location);
                            grfx.DrawLine(new Pen(mainVelocityArrow, 1.5f),
                                new PointF(vel_arrow2.X, vel_arrow2.Y),
                                new PointF(vel_arrow1.X, vel_arrow1.Y));
                            Vector arrow1 = new(vel_arrow1, vel_arrow2);
                            grfx.FillPolygon(mainVelocityArrow, arrow1.CreateTriangle(12, 10));
                            if (editingValue == EditingValue.VelocityLength)
                            {
                                float dx = Layers[CurrentLayer].Agents[Layers[CurrentLayer].Selected].Velocity.Tail.X;
                                float dy = Layers[CurrentLayer].Agents[Layers[CurrentLayer].Selected].Velocity.Tail.Y;
                                double distance = Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));
                                double fsvelocity = double.Parse(NewValue_TB.Text);
                                double fsv_y = Math.Sqrt(fsvelocity * fsvelocity / (dx * dx / (dy * dy) + 1));
                                double fsv_x = fsv_y * dx / dy;
                                PointParticle new_vel_arrow1 = RealToScreen(Layers[CurrentLayer].Agents[Layers[CurrentLayer].Selected].Location + new PointParticle((float)fsv_x, (float)fsv_y));
                                PointParticle new_vel_arrow2 = RealToScreen(Layers[CurrentLayer].Agents[Layers[CurrentLayer].Selected].Location);

                                grfx.DrawLine(new Pen(Brushes.Tomato, 1.5f),
                                new PointF(new_vel_arrow2.X, new_vel_arrow2.Y),
                                new PointF(new_vel_arrow1.X, new_vel_arrow1.Y));
                                Vector arrow2 = new(new_vel_arrow1, new_vel_arrow2);
                                grfx.FillPolygon(Brushes.Tomato, arrow2.CreateTriangle(12, 10));
                            }
                        }
                    }
                    break;
                case SpecialCondition.SetVelocity:
                    PointParticle cursor = GetCursorPosition();
                    PointParticle objCenter = RealToScreen(Phantom.Location);
                    AwoseAgent tempAgentVelocity = new("TempVelocityObject",
                        Phantom.Location.X, Phantom.Location.Y, Phantom.Weight, Phantom.Charge,
                        0, 0, Phantom.IsPinned);
                    tempAgentVelocity.ForceEX = 0;
                    tempAgentVelocity.ForceGX = 0;
                    tempAgentVelocity.ForceEY = 0;
                    tempAgentVelocity.ForceGY = 0;
                    tempAgentVelocity.Velocity = new(-ScreenToReal(objCenter) + ScreenToReal(cursor));
                    List<Point> tempTrajectory = new();
                    for (int i = 0; i < 1500; i++)
                    {
                        tempAgentVelocity.ForceEX = 0;
                        tempAgentVelocity.ForceGX = 0;
                        tempAgentVelocity.ForceEY = 0;
                        tempAgentVelocity.ForceGY = 0;
                        foreach (AwoseAgent agent in Layers[CurrentLayer].Agents)
                        {
                            if (!agent.IsSelected)
                            {
                                tempAgentVelocity.ForceCalc(agent);
                            }
                        }
                        if (tempAgentVelocity.IsPinned) continue;
                        tempAgentVelocity.Force = new Vector(new PointParticle((float)(tempAgentVelocity.ForceGX + tempAgentVelocity.ForceEX), (float)(tempAgentVelocity.ForceGY + tempAgentVelocity.ForceEY)));
                        tempAgentVelocity.Velocity.Tail.X += (float)((tempAgentVelocity.ForceGX + tempAgentVelocity.ForceEX) * timeStep / tempAgentVelocity.Weight / 1000);
                        tempAgentVelocity.Velocity.Tail.Y += (float)((tempAgentVelocity.ForceGY + tempAgentVelocity.ForceEY) * timeStep / tempAgentVelocity.Weight / 1000);
                        tempAgentVelocity.Location.X += (float)(tempAgentVelocity.Velocity.Tail.X * timeStep / 1000);
                        tempAgentVelocity.Location.Y += (float)(tempAgentVelocity.Velocity.Tail.Y * timeStep / 1000);
                        tempTrajectory.Add(RealToScreen(tempAgentVelocity.Location).ToPoint());
                    }
                    grfx.DrawLines(new Pen(Brushes.White, 1), tempTrajectory.ToArray());
                    grfx.DrawLine(new Pen(mainVelocityArrow, 2),
                        new PointF(objCenter.X, objCenter.Y),
                        new PointF(cursor.X, cursor.Y));
                    Vector arrow = new(cursor, objCenter);
                    grfx.FillPolygon(mainVelocityArrow, arrow.CreateTriangle(17, 15));
                    break;
                case SpecialCondition.SetFirstSpaceVelocity:
                    PointParticle sfsv_cursor = GetCursorPosition();
                    PointParticle pointCursor = ScreenToReal(sfsv_cursor);
                    PointParticle sfsv_objCenter = RealToScreen(Layers[CurrentLayer].Agents[Layers[CurrentLayer].Selected].Location);
                    float pip_x = sfsv_cursor.X;
                    float pip_y = sfsv_cursor.Y;
                    RectangleF sfsv_spearhead = new(sfsv_cursor.X - 4, sfsv_cursor.Y - 4, 8, 8);

                    foreach (AwoseAgent agent in Layers[CurrentLayer].Agents)
                    {
                        if (Calculations.IsInRadius(pointCursor.X, pointCursor.Y, agent, aw_agentsize * aw_scale))
                        {
                            pip_x = RealToScreen(agent.Location).X;
                            pip_y = RealToScreen(agent.Location).Y;
                            float sfsv_diam = aw_agentsize * aw_scale + 7 * aw_scale;
                            float sfsv_length = MathF.Sqrt(MathF.Pow(sfsv_objCenter.X - pip_x, 2) + MathF.Pow(sfsv_objCenter.Y - pip_y, 2));
                            sfsv_spearhead = new(RealToScreen(agent.Location).X - sfsv_diam / 2, RealToScreen(agent.Location).Y - sfsv_diam / 2, sfsv_diam, sfsv_diam);
                            grfx.DrawEllipse(new Pen(Brushes.White, 1.5f), new Rectangle((int)RealToScreen(agent.Location).X - (int)sfsv_length, (int)RealToScreen(agent.Location).Y - (int)sfsv_length, (int)sfsv_length * 2, (int)sfsv_length * 2));
                            break;
                        }
                    }
                    grfx.DrawLine(new Pen(Brushes.DodgerBlue, 2),
                        new PointF(sfsv_objCenter.X, sfsv_objCenter.Y),
                        new PointF(pip_x, sfsv_objCenter.Y));
                    grfx.DrawLine(new Pen(Brushes.DodgerBlue, 2),
                        new PointF(pip_x, sfsv_objCenter.Y),
                        new PointF(pip_x, pip_y));
                    grfx.DrawEllipse(new Pen(Brushes.DodgerBlue, 2), sfsv_spearhead);
                    break;
                default:
                    break;
            }

            if (editingValue == EditingValue.VelocityLength)
            {

            }


            //Drawing agents
            lock (Layers)
            {
                foreach (AwoseLayer layer in Layers)
                {
                    lock (layer.Agents)
                    {
                        foreach (AwoseAgent agent in layer.Agents)
                        {
                            Point point = RealToScreen(agent.Location).ToPoint();
                            RectangleF circle = new((float)(point.X - diameter / 2), (float)(point.Y - diameter / 2), diameter, diameter);
                            grfx.FillEllipse(new SolidBrush(agent.Dye), circle);
                            if (agent.IsSelected)
                            {
                                RectangleF ring = new((float)(point.X - diameter), (float)(point.Y - diameter), diameter * 2, diameter * 2);
                                RectangleF circle_left = new((float)(point.X - (diameter * 1.25)), (float)(point.Y - (diameter / 4)), diameter / 2, diameter / 2);
                                RectangleF circle_right = new((float)(point.X + (diameter * 0.75)), (float)(point.Y - (diameter / 4)), diameter / 2, diameter / 2);
                                RectangleF circle_up = new((float)(point.X - (diameter / 4)), (float)(point.Y - (diameter * 1.25)), diameter / 2, diameter / 2);
                                RectangleF circle_down = new((float)(point.X - (diameter / 4)), (float)(point.Y + (diameter * 0.75)), diameter / 2, diameter / 2);
                                grfx.DrawEllipse(new Pen(Brushes.DimGray, 2), ring);
                                grfx.FillEllipse(Brushes.Black, circle_left);
                                grfx.DrawEllipse(new Pen(Brushes.DimGray, 2), circle_left);
                                grfx.FillEllipse(Brushes.Black, circle_right);
                                grfx.DrawEllipse(new Pen(Brushes.DimGray, 2), circle_right);
                                grfx.FillEllipse(Brushes.Black, circle_up);
                                grfx.DrawEllipse(new Pen(Brushes.DimGray, 2), circle_up);
                                grfx.FillEllipse(Brushes.Black, circle_down);
                                grfx.DrawEllipse(new Pen(Brushes.DimGray, 2), circle_down);
                            }
                        }
                    }
                }
            }
            if (Phantom != null)
            {
                Point point = RealToScreen(Phantom.Location).ToPoint();
                RectangleF circle = new((float)(point.X - diameter / 2), (float)(point.Y - diameter / 2), diameter, diameter);
                for (int i = AnimationCounter; i < 360 + AnimationCounter; i+= 60)
                {
                    grfx.DrawArc(new Pen(Brushes.LightGray, aw_scale * 2), circle, i, 40);
                }
                //grfx.DrawArc(new Pen(Brushes.Gray, aw_scale * 2), circle, 10, 70);
                //grfx.DrawArc(new Pen(Brushes.Gray, aw_scale * 2), circle, 100, 70);
                //grfx.DrawArc(new Pen(Brushes.Gray, aw_scale * 2), circle, 190, 70);
                //grfx.DrawArc(new Pen(Brushes.Gray, aw_scale * 2), circle, 280, 70);
            }

            //Drawing arrows, lines etc
            
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
                            grfx.FillEllipse(new SolidBrush(item.Dye), circle);
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
            foreach (AwoseLayer layer in Layers)
            {
                foreach (AwoseAgent agent in layer.Agents)
                {
                    agent.ForceGX = agent.ForceGY = agent.ForceEX = agent.ForceEY = 0;
                }
                for (int i = 0; i < layer.Agents.Count; i++)
                {
                    for (int j = 0; j < layer.Agents.Count; j++)
                    {
                        if (i != j) layer.Agents[i].ForceCalc(layer.Agents[j]);
                    }
                }
                foreach (AwoseAgent agent in layer.Agents)
                {
                    if (agent.IsPinned) continue;
                    agent.Force = new Vector(new PointParticle((float)(agent.ForceGX + agent.ForceEX), (float)(agent.ForceGY + agent.ForceEY)));
                    agent.Velocity.Tail.X += (float)((agent.ForceGX + agent.ForceEX) * timeStep / agent.Weight / 1000);
                    agent.Velocity.Tail.Y += (float)((agent.ForceGY + agent.ForceEY) * timeStep / agent.Weight / 1000);
                    agent.Location.X += (float)(agent.Velocity.Tail.X * timeStep / 1000);
                    agent.Location.Y += (float)(agent.Velocity.Tail.Y * timeStep / 1000);
                    agent.Trajectory.Enqueue(new PointParticle(agent.Location.X, (int)agent.Location.Y));
                }
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
            Bitmap button_img = new(23, 23);
            using Graphics grfx = Graphics.FromImage(button_img);
            grfx.Clear(Color.FromArgb(15, 15, 15));
            grfx.FillRectangle(Brushes.White, 11, 6, 2, 11);
            grfx.FillRectangle(Brushes.White, 6, 11, 11, 2);
            CreateNewLayer_Button.BackgroundImage = button_img;

            


            Layers = new List<AwoseLayer>();
            Layers.Add(new AwoseLayer("Layer 1", Layers.Count + 1));
            animation = new(AnimationEditor);
            animation.Start();
        }

        public async void AnimationEditor()
        {
            //Thread.Sleep(5000);
            while (IsActive)
            {
                foreach (AwoseAgent item in Layers[CurrentLayer].Agents)
                {
                    item.AgentSprayUpdate();
                }
                if (isLaunched) Aw_Step(timeStep);
                Invoke((Action)Aw_Refresh);
                await Task.Delay(timeStep);
                //Thread.Sleep(timeStep);
            }
        }

        private void Aw_CheckMistakes()
        {
            foreach (AwoseLayer layer in Layers)
            {
                foreach (AwoseAgent agent in layer.Agents)
                {
                    agent.MistakeType = 0;
                }
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
            if (Layers[CurrentLayer].IsThereSelections())
            {
                AwoseAgent agent = Layers[CurrentLayer].Agents[Layers[CurrentLayer].Selected];
                CurrentObjectName_Label.Text = agent.Name;
                CurrentObjectName_Label.ForeColor = Color.LightSkyBlue;
                CurrentObjectName_Label.Cursor = Cursors.IBeam;
                ObjectMass_Label.Text = Math.Round(agent.Weight, 5).ToString() + " kg";
                ObjectCharge_Label.Text = Math.Round(agent.Charge, 5).ToString() + " C";
                ObjectPositionX_Label.Text = agent.Location.X.ToString();
                ObjectPositionY_Label.Text = agent.Location.Y.ToString();
                ObjectSettings_Panel.Visible = true;
                ObjectBeauty_Panel.Visible = true;
                if (agent.IsPinned)
                {
                    Pinned_CB.BackgroundImage = DrawingValues.DrawTick();
                } else
                {
                    Pinned_CB.BackgroundImage = null;
                }
                ObjectSprite_White_PB.Image = DrawingValues.DrawCircle(ObjectSprite_White_PB.Width,
                    ObjectSprite_White_PB.Height, Color.White, agent.Sprite == SpriteType.White);
                ObjectSprite_Yellow_PB.Image = DrawingValues.DrawCircle(ObjectSprite_Yellow_PB.Width,
                    ObjectSprite_Yellow_PB.Height, Color.Yellow, agent.Sprite == SpriteType.Yellow);
                ObjectSprite_Green_PB.Image = DrawingValues.DrawCircle(ObjectSprite_White_PB.Width,
                    ObjectSprite_White_PB.Height, Color.Green, agent.Sprite == SpriteType.Green);
                ObjectSprite_Sky_PB.Image = DrawingValues.DrawCircle(ObjectSprite_Yellow_PB.Width,
                    ObjectSprite_Yellow_PB.Height, Color.SkyBlue, agent.Sprite == SpriteType.Sky);
                ObjectSprite_Red_PB.Image = DrawingValues.DrawCircle(ObjectSprite_Red_PB.Width,
                    ObjectSprite_Red_PB.Height, Color.PaleVioletRed, agent.Sprite == SpriteType.Red);

                ObjectForceCircle_PB.BackgroundImage = DrawingValues.DrawCircleWithArrow(ObjectForceCircle_PB.Width,
                    ObjectForceCircle_PB.Height, Color.CadetBlue, agent.Force.Tail.X - agent.Force.Head.X, agent.Force.Tail.Y - agent.Force.Head.Y);
                ObjectVelocityCircle_PB.BackgroundImage = DrawingValues.DrawCircleWithArrow(ObjectVelocityCircle_PB.Width,
                    ObjectVelocityCircle_PB.Height, Color.IndianRed, agent.Velocity.Tail.X - agent.Velocity.Head.X, agent.Velocity.Tail.Y - agent.Velocity.Head.Y);
                ObjectVelocity_Label.Text = Math.Round(agent.Velocity.Length, 2).ToString() + " px/s";
                switch (agent.TrajectoryLine)
                {
                    case TrajectoryType.None:
                        TrajNo_Button.BackColor = Color.FromArgb(15, 15, 15);
                        TrajFade_Button.BackColor = Color.FromArgb(64, 64, 64);
                        TrajNonfade_Button.BackColor = Color.FromArgb(64, 64, 64);
                        break;
                    case TrajectoryType.Fade:
                        TrajNo_Button.BackColor = Color.FromArgb(64, 64, 64);
                        TrajFade_Button.BackColor = Color.FromArgb(15, 15, 15);
                        TrajNonfade_Button.BackColor = Color.FromArgb(64, 64, 64);
                        break;
                    case TrajectoryType.Nonfade:
                        TrajNo_Button.BackColor = Color.FromArgb(64, 64, 64);
                        TrajFade_Button.BackColor = Color.FromArgb(64, 64, 64);
                        TrajNonfade_Button.BackColor = Color.FromArgb(15, 15, 15);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                ControlLayer_Panel.Visible = true;
                ControlAgents_Panel.Visible = false;
                ObjectBeauty_Panel.Visible = false;
            }
            return;



            if (aw_selected != - 1)
            {
                
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
            if (Layers[CurrentLayer].IsThereSelections())
            {
                AwoseAgent agent = Layers[CurrentLayer].Agents[Layers[CurrentLayer].Selected];
                CurrentObjectName_Label.Text = agent.Name;
                CurrentObjectName_Label.ForeColor = Color.LightSkyBlue;
                CurrentObjectName_Label.Cursor = Cursors.IBeam;
                ObjectMass_Label.Text = Math.Round(agent.Weight, 5).ToString() + " kg";
                ObjectCharge_Label.Text = Math.Round(agent.Charge, 5).ToString() + " C";
                ObjectPositionX_Label.Text = agent.Location.X.ToString();
                ObjectPositionY_Label.Text = agent.Location.Y.ToString();
                ObjectSettings_Panel.Visible = true;
                if (agent.IsPinned)
                {
                    Pinned_CB.BackgroundImage = DrawingValues.DrawTick();
                }
                else
                {
                    Pinned_CB.BackgroundImage = null;
                }
                ObjectSprite_White_PB.Image = DrawingValues.DrawCircle(ObjectSprite_White_PB.Width,
                    ObjectSprite_White_PB.Height, Color.White, agent.Sprite == SpriteType.White);
                ObjectSprite_Yellow_PB.Image = DrawingValues.DrawCircle(ObjectSprite_Yellow_PB.Width,
                    ObjectSprite_Yellow_PB.Height, Color.Yellow, agent.Sprite == SpriteType.Yellow);
                ObjectSprite_Green_PB.Image = DrawingValues.DrawCircle(ObjectSprite_White_PB.Width,
                    ObjectSprite_White_PB.Height, Color.Green, agent.Sprite == SpriteType.Green);
                ObjectSprite_Sky_PB.Image = DrawingValues.DrawCircle(ObjectSprite_Yellow_PB.Width,
                    ObjectSprite_Yellow_PB.Height, Color.SkyBlue, agent.Sprite == SpriteType.Sky);
                ObjectSprite_Red_PB.Image = DrawingValues.DrawCircle(ObjectSprite_Red_PB.Width,
                    ObjectSprite_Red_PB.Height, Color.PaleVioletRed, agent.Sprite == SpriteType.Red);

                ObjectForceCircle_PB.BackgroundImage = DrawingValues.DrawCircleWithArrow(ObjectForceCircle_PB.Width,
                    ObjectForceCircle_PB.Height, Color.CadetBlue, agent.Force.Tail.X - agent.Force.Head.X, agent.Force.Tail.Y - agent.Force.Head.Y);
                ObjectVelocityCircle_PB.BackgroundImage = DrawingValues.DrawCircleWithArrow(ObjectVelocityCircle_PB.Width,
                    ObjectVelocityCircle_PB.Height, Color.IndianRed, agent.Velocity.Tail.X - agent.Velocity.Head.X, agent.Velocity.Tail.Y - agent.Velocity.Head.Y);
                ObjectForce_Label.Text = Math.Round(agent.Force.Length, 2).ToString() + " N";
                ObjectVelocity_Label.Text = Math.Round(agent.Velocity.Length, 2).ToString() + " px/s";
            }
            else
            {
                ControlLayer_Panel.Visible = true;
                ControlAgents_Panel.Visible = false;
            }
            return;
        }

        private void SaveModel_MSItem_Click(object sender, EventArgs e)
        {
            if (SaveModel_SFD.ShowDialog() == DialogResult.OK)
            {
                FileInfo fileInfo = new(SaveModel_SFD.FileName);
                FileStream file = fileInfo.Create();
                string serializedobject = JsonConvert.SerializeObject(Layers);
                byte[] array = System.Text.Encoding.Default.GetBytes(serializedobject);
                file.Write(array);
                file.Close();
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
            //List<AwoseAgent> selects = new();
            //int possibleSelection = 0;
            aw_selected = 0;
            aw_cursor = GetCursorPosition();
            PointParticle pointCursor = ScreenToReal(aw_cursor);
            bool hoverAgent = false;
            foreach (AwoseAgent agent in Layers[CurrentLayer].Agents)
            {
                if (Calculations.IsInRadius(pointCursor.X, pointCursor.Y, agent, aw_agentsize * aw_scale))
                {
                    hoverAgent = true;
                    agent.IsSelected = true;
                }
                else
                {
                    agent.IsSelected = false;
                }
            }
            if (hoverAgent)
            {
                Layers[CurrentLayer].IsThereSelections();
                DeleteObject_CMItem.Visible = true;
                ObjectEditSep_CMSepar.Visible = true;
                SetVelocity_CMItem.Visible = true;
                ChangeSign_CMItem.Visible = true;
                PinUp_CMItem.Visible = true;
                PinUp_CMItem.Checked = Layers[CurrentLayer].Agents[Layers[CurrentLayer].Selected].IsPinned;
                ControlAgents_Panel.Visible = true;
                if (CopiedVelocity == null)
                {
                    ApplyVelocity_CMItem.Enabled = false;
                } else
                {
                    ApplyVelocity_CMItem.Enabled = true;
                }
                ControlLayer_Panel.Visible = false;
            }
            else
            {
                DeleteObject_CMItem.Visible = false;
                ObjectEditSep_CMSepar.Visible = false;
                SetVelocity_CMItem.Visible = false;
                ChangeSign_CMItem.Visible = false;
                PinUp_CMItem.Visible = false;
                ControlAgents_Panel.Visible = false;
                ControlLayer_Panel.Visible = true;
            }
            Aw_DrawControl();
            //aw_cursor.X = (int)((-lu_corner.X + Cursor.Position.X - Location.X - ModelBoard_PB.Location.X - 7) / aw_scale);
            //aw_cursor.Y = (int)((-lu_corner.Y + Cursor.Position.Y - Location.Y - ModelBoard_PB.Location.Y - 29) / aw_scale);
        }

        private void CreateObject_CMItem_Click(object sender, EventArgs e)
        {
            int agentsNum = 1;
            aaw_loopNames:
            foreach (AwoseAgent item in Layers[CurrentLayer].Agents)
            {
                if (item.Name == "Object " + agentsNum.ToString())
                {
                    agentsNum++;
                    goto aaw_loopNames;
                }
            }
            PointParticle newAgentPoint = ScreenToReal(aw_cursor);
            //PointF newAgentPoint = ScreenToReal(aw_cursor.X, aw_cursor.Y);
            //Text = newAgentPoint.X.ToString() + ", " + newAgentPoint.Y.ToString();
            Layers[CurrentLayer].Agents.Add(new AwoseAgent("Object " + agentsNum.ToString(), newAgentPoint.X, newAgentPoint.Y, 1, 0, 0, 0, false));
            //agents.Add(new AwoseAgent("Object " + (agentsNumeric++).ToString(), newAgentPoint.X, newAgentPoint.Y, 1, 0, 0, 0, false));
            aw_undo.Push(new AwoseChange(Layers[CurrentLayer].Agents[^1], Layers[CurrentLayer].Name, ChangeType.Creating));
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
                lu_corner.X = (int)((beforeScaling.X / aw_scale) - (beforeScaling.X / (aw_scale + .5f)) + lu_corner.X);
                lu_corner.Y = (int)((beforeScaling.Y / aw_scale) - (beforeScaling.Y / (aw_scale + .5f)) + lu_corner.Y);

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
            //aw_undo.Push(new AwoseChange(agents[aw_selected], ChangeType.Deleting));
            //foreach (AwoseAgent item in agents)
            //{
            //    if (item.Satellites.Contains(agents[aw_selected].Name))
            //    {
            //        item.Satellites.Remove(agents[aw_selected].Name);
            //    }
            //    if (item.Star == agents[aw_selected].Name)
            //    {
            //        item.Star = "";
            //    }
            //}
            Layers[CurrentLayer].Agents.RemoveAt(Layers[CurrentLayer].Selected);
            if (Layers[CurrentLayer].Agents.Count > 0)
            {
                Layers[CurrentLayer].Selected = Layers[CurrentLayer].Agents.Count - 1;
            } else
            {
                Layers[CurrentLayer].Selected = -1;
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
                    foreach (AwoseLayer item in Layers)
                    {
                        if (item.Name == ch_undo.EventSpace)
                        {
                            for (int i = 0; i < item.Agents.Count; i++)
                                if (item.Agents[i].Name == ch_undo.Subject.Name)
                                    item.Agents.RemoveAt(i);
                            break;
                        }
                    }
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
            if (isLaunched) return;
            if (NewValue_TB.Visible) NewValue_TB_PreviewKeyDown(sender, new PreviewKeyDownEventArgs(Keys.Enter));
            NewValue_TB.Location = new Point(ControlAgents_Panel.Location.X + ObjectSettings_Panel.Location.X + ObjectMass_Label.Location.X + 1,
                ControlAgents_Panel.Location.Y + ObjectSettings_Panel.Location.Y + ObjectMass_Label.Location.Y - 26);
            NewValue_TB.Text = Layers[CurrentLayer].Agents[Layers[CurrentLayer].Selected].Weight.ToString();
            editingValue = EditingValue.Mass;
            NewValue_TB.SelectAll();
            NewValue_TB.Visible = true;
            NewValue_TB.BringToFront();
            NewValue_TB.Focus();
        }

        private void NewValue_TB_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
            {
                NewValue_TB_PreviewKeyDown(sender, new(Keys.Enter));
                NewValue_TB.Visible = false;
                switch (editingValue)
                {
                    case EditingValue.None:
                        break;
                    case EditingValue.Mass:
                        ObjectCharge_Label_Click(sender, e);
                        NewValue_TB.Focus();
                        return;
                    case EditingValue.Charge:
                        ObjectPositionX_Label_Click(sender, e);
                        NewValue_TB.Focus();
                        return;
                    case EditingValue.Name:
                        break;
                    case EditingValue.X:
                        ObjectPositionY_Label_Click(sender, e);
                        NewValue_TB.Focus();
                        return;
                    case EditingValue.Y:
                        break;
                    default:
                        break;
                }
            }

            if (e.KeyCode == Keys.Enter && NewValue_TB.Visible)
            {
                float newValue;
                NewValue_TB.Text = NewValue_TB.Text.Replace('.', ',');
                switch (editingValue)
                {
                    case EditingValue.None:
                        break;
                    case EditingValue.Mass:
                        try
                        {
                            newValue = float.Parse(NewValue_TB.Text);
                            if (newValue > 99999800)
                            {
                                NewValue_TB.Visible = false;
                                break;
                            }
                            //aw_undo.Push(new AwoseChange(Layers[CurrentLayer].Agents[Layers[CurrentLayer].Selected], ChangeType.ChangingMass, Layers[CurrentLayer].Agents[Layers[CurrentLayer].Selected].Weight, newValue));
                            Layers[CurrentLayer].Agents[Layers[CurrentLayer].Selected].Weight = newValue;
                            if (Layers[CurrentLayer].Agents[Layers[CurrentLayer].Selected].Star != "")
                            {
                                Layers[CurrentLayer].Agents[Layers[CurrentLayer].Selected].ChangeAfterFSV = true;
                            }
                            else if (Layers[CurrentLayer].Agents[Layers[CurrentLayer].Selected].Satellites.Count > 0)
                            {
                                foreach (AwoseAgent item in Layers[CurrentLayer].Agents)
                                {
                                    if (Layers[CurrentLayer].Agents[Layers[CurrentLayer].Selected].Satellites.Contains(item.Name))
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
                            if (newValue > 99999800)
                            {
                                NewValue_TB.Visible = false;
                                break;
                            }
                            //aw_undo.Push(new AwoseChange(agents[aw_selected], ChangeType.ChangingCharge, agents[aw_selected].Charge, newValue));
                            Layers[CurrentLayer].Agents[Layers[CurrentLayer].Selected].Charge = newValue;
                        }
                        catch { }
                        finally
                        {
                            NewValue_TB.Visible = false;
                        }
                        break;
                    case EditingValue.Name:
                        //aw_undo.Push(new AwoseChange(Layers[CurrentLayer].Agents[Layers[CurrentLayer].Selected], ChangeType.ChangingName, agents[aw_selected].Name, NewValue_TB.Text));
                        foreach (AwoseAgent item in Layers[CurrentLayer].Agents)
                        {
                            if (item.Name == NewValue_TB.Text) { NewValue_TB.Visible = false; return; }
                        }
                        Layers[CurrentLayer].Agents[Layers[CurrentLayer].Selected].Name = NewValue_TB.Text;
                        NewValue_TB.Visible = false;
                        break;
                    case EditingValue.X:
                        try
                        {
                            newValue = float.Parse(NewValue_TB.Text);
                            if (float.IsNaN(newValue) || float.IsInfinity(newValue)) throw new Exception("Invalid coordinate");
                            //aw_undo.Push(new AwoseChange(agents[aw_selected], ChangeType.ChangingX, agents[aw_selected].X, newValue));
                            Layers[CurrentLayer].Agents[Layers[CurrentLayer].Selected].Location.X = newValue;
                            //if (Layers[CurrentLayer].Agents[Layers[CurrentLayer].Selected].Star != "")
                            //{
                            //    Layers[CurrentLayer].Agents[Layers[CurrentLayer].Selected].ChangeAfterFSV = true;
                            //}
                            //else if (agents[aw_selected].Satellites.Count > 0)
                            //{
                            //    foreach (AwoseAgent item in agents)
                            //    {
                            //        if (agents[aw_selected].Satellites.Contains(item.Name))
                            //            item.ChangeAfterFSV = true;
                            //    }
                            //}
                        }
                        catch { }
                        finally
                        {
                            NewValue_TB.Visible = false;
                            //lock (agents[aw_selected].Spray)
                            //    agents[aw_selected].Spray.Clear();
                        }
                        break;
                    case EditingValue.Y:
                        try
                        {
                            newValue = float.Parse(NewValue_TB.Text);
                            if (float.IsNaN(newValue) || float.IsInfinity(newValue)) throw new Exception("Invalid coordinate");
                            //aw_undo.Push(new AwoseChange(agents[aw_selected], ChangeType.ChangingY, agents[aw_selected].Y, newValue));
                            Layers[CurrentLayer].Agents[Layers[CurrentLayer].Selected].Location.Y = newValue;
                            //if (agents[aw_selected].Star != "")
                            //{
                            //    agents[aw_selected].ChangeAfterFSV = true;
                            //}
                            //else if (agents[aw_selected].Satellites.Count > 0)
                            //{
                            //    foreach (AwoseAgent item in agents)
                            //    {
                            //        if (agents[aw_selected].Satellites.Contains(item.Name))
                            //            item.ChangeAfterFSV = true;
                            //    }
                            //}
                        }
                        catch { }
                        finally
                        {
                            NewValue_TB.Visible = false;
                            //lock (agents[aw_selected].Spray)
                            //    agents[aw_selected].Spray.Clear();
                        }
                        break;
                    case EditingValue.VelocityLength:
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
                    foreach (AwoseLayer layer in Layers)
                    {
                        if (layer.Name == ch_redo.EventSpace)
                        {
                            foreach (AwoseAgent agent in layer.Agents)
                            {
                                if (agent.Name == ch_redo.Subject.Name) return;
                            }
                            layer.Agents.Add(ch_redo.Subject);
                            break;
                        }
                    }
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
            NewValue_TB.Text = Layers[CurrentLayer].Agents[Layers[CurrentLayer].Selected].Name.ToString();
            editingValue = EditingValue.Name;
            NewValue_TB.SelectAll();
            NewValue_TB.Visible = true;
            NewValue_TB.BringToFront();
            NewValue_TB.Focus();
        }

        private void ModelBoard_PB_MouseDown(object sender, MouseEventArgs e)
        {
            if (NewValue_TB.Visible) NewValue_TB_PreviewKeyDown(sender, new PreviewKeyDownEventArgs(Keys.Enter));
            //aw_cursor.X = Cursor.Position.X - Location.X - ModelBoard_PB.Location.X - 7;
            //aw_cursor.Y = Cursor.Position.Y - Location.Y - ModelBoard_PB.Location.Y - 29;
            aw_cursor = GetCursorPosition();
            PointParticle pointCursor = ScreenToReal(aw_cursor);
            bool hoverAgent = false;
            //Text = aw_cursor.X.ToString() + ", " + aw_cursor.Y.ToString();
            List<AwoseAgent> selects = new();
            PossibleSelections_LB.Visible = false;
            //isObjectMoving = false;
            //isBoardMoving = false;
            movingEntity = MovingEntity.None;
            switch (e.Button)
            {
                case MouseButtons.Left:
                    if (specialCondition == SpecialCondition.SetVelocity)
                    {
                        PointParticle cursor = GetCursorPosition();
                        PointParticle objCenter = RealToScreen(Phantom.Location);
                        Layers[CurrentLayer].Agents[Layers[CurrentLayer].Selected].Velocity = new(-ScreenToReal(objCenter) + ScreenToReal(cursor));
                        specialCondition = SpecialCondition.None;
                        return;
                    }
                    if (specialCondition == SpecialCondition.SetFirstSpaceVelocity)
                    {
                        specialCondition = SpecialCondition.None;
                        foreach (AwoseAgent agent in Layers[CurrentLayer].Agents)
                        {
                            if (agent == Layers[CurrentLayer].Agents[Layers[CurrentLayer].Selected])
                            {
                                return;
                            }
                            if (Calculations.IsInRadius(pointCursor.X, pointCursor.Y, agent, aw_agentsize * aw_scale))
                            {
                                PointParticle point1 = Layers[CurrentLayer].Agents[Layers[CurrentLayer].Selected].Location;
                                float dx = point1.X - agent.Location.X;
                                float dy = point1.Y - agent.Location.Y;
                                double distance = Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));
                                double fsvelocity = Math.Sqrt(ConstG * agent.Weight / distance);
                                double fsv_y; 
                                if (dx != 0)
                                {
                                    fsv_y = Math.Sqrt(fsvelocity * fsvelocity / (dy * dy / (dx * dx) + 1));
                                }
                                else
                                {
                                    fsv_y = 0;
                                }
                                double fsv_x;
                                if (dx != 0)
                                {
                                    fsv_x = -fsv_y * dy / dx;
                                }
                                else
                                {
                                    fsv_x = fsvelocity;
                                }
                                Layers[CurrentLayer].Agents[Layers[CurrentLayer].Selected].Velocity = new(

                                    new((float)fsv_x, (float)fsv_y));
                                return;
                            }
                        }

                        //PointParticle cursor = GetCursorPosition();
                        //PointParticle objCenter = RealToScreen(Phantom.Location);
                        //Layers[CurrentLayer].Agents[Layers[CurrentLayer].Selected].Velocity = new(-ScreenToReal(objCenter) + ScreenToReal(cursor));
                        specialCondition = SpecialCondition.None;
                        return;
                    }

                    foreach (AwoseAgent agent in Layers[CurrentLayer].Agents)
                    {
                        if (Calculations.IsInRadius(pointCursor.X, pointCursor.Y, agent, aw_agentsize * aw_scale) && !hoverAgent)
                        {
                            if (Layers[CurrentLayer].IsThereSelections())
                            {
                                if (Layers[CurrentLayer].Agents[Layers[CurrentLayer].Selected] == agent)
                                {
                                    aw_remember = GetCursorPosition();
                                    Phantom = new AwoseAgent("Phantom", ScreenToReal(aw_remember).X, ScreenToReal(aw_remember).Y);
                                    if (!isLaunched) movingEntity = MovingEntity.Agent;
                                }
                            }
                            hoverAgent = true;
                            agent.IsSelected = true;
                        } 
                        else
                        {
                            agent.IsSelected = false;
                        }
                    }
                    if (hoverAgent)
                    {
                        ControlAgents_Panel.Visible = true;
                        ControlLayer_Panel.Visible = false;
                    } 
                    else
                    {
                        ControlAgents_Panel.Visible = false;
                        ControlLayer_Panel.Visible = true;
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
                            //isObjectMoving = true;
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
                case MouseButtons.Right:
                    movingEntity = MovingEntity.None;
                    Phantom = null;
                    Aw_DrawControl();
                    break;
                case MouseButtons.Middle:
                    boardBeforeMoving = Cursor.Position;
                    //isBoardMoving = true;
                    movingEntity = MovingEntity.Board;
                    ModelBoard_PB.Cursor = Cursors.NoMove2D;
                    //aw_cursor = Cursor.Position;
                    //aw_remember = GetCursorPosition();
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
                    movingEntity = MovingEntity.None;
                    Phantom = null;
                    //aw_undo.Push(new AwoseChange(Layers[CurrentLayer].Agents[Layers[CurrentLayer].Selected], ChangeType.ChangingXY, lu_remember, new Point((int)agents[aw_selected].X, (int)agents[aw_selected].Y)));
                    Aw_DrawControl();
                    //if (isObjectMoving && aw_selected < agents.Count && (agents[aw_selected].X != lu_remember.X || agents[aw_selected].Y != lu_remember.Y))
                    //{
                    //    agents[aw_selected].Spray.Clear();
                    //    //agents[aw_selected].X_screen = aw_cursor.X - Cursor.Position.X;
                    //    //agents[aw_selected].Y_screen = aw_cursor.Y - Cursor.Position.Y;
                    //    //aw_undo.Push(new AwoseChange(agents[aw_selected], ChangeType.ChangingXY, lu_remember, new Point((int)agents[aw_selected].X, (int)agents[aw_selected].Y)));
                    //    if (agents[aw_selected].Star != "")
                    //    {
                    //        agents[aw_selected].ChangeAfterFSV = true;
                    //    } else if (agents[aw_selected].Satellites.Count > 0)
                    //    {
                    //        foreach (AwoseAgent item in agents)
                    //        {
                    //            if (agents[aw_selected].Satellites.Contains(item.Name))
                    //                item.ChangeAfterFSV = true;
                    //        }
                    //    }
                    //}
                    //isObjectMoving = false;
                    break;
                case MouseButtons.Right:
                    break;
                case MouseButtons.Middle:
                    movingEntity = MovingEntity.None;
                    //isBoardMoving = false;
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
            PointParticle pointCursor = ScreenToReal(aw_cursor);
            RT_X_Label.Text = Math.Round(pointCursor.X, 2).ToString();
            RT_Y_Label.Text = Math.Round(pointCursor.Y, 2).ToString();

            double force_gx = 0;
            double force_gy = 0;
            double force_ex = 0;
            double force_ey = 0;
            foreach (AwoseAgent agent in Layers[CurrentLayer].Agents)
            {
                float delta_x = pointCursor.X - agent.Location.X;
                float delta_y = pointCursor.Y - agent.Location.Y;
                float delta = MathF.Pow(MathF.Sqrt(delta_x * delta_x + delta_y * delta_y), 2);
                float coeff_x = delta_x / MathF.Sqrt(delta);
                float coeff_y = delta_y / MathF.Sqrt(delta);
                force_gx += agent.Weight * ConstG / delta * coeff_x;
                force_gy += agent.Weight * ConstG / delta * coeff_y;
                if (agent.Charge != 0)
                {
                    force_ex += agent.Charge * ConstE / delta * coeff_x;
                    force_ey += agent.Charge * ConstE / delta * coeff_y;
                }
            }
            if (double.IsNaN(force_ex))
            {
                RT_E_Label.Text = double.PositiveInfinity.ToString();
            }
            else 
            {
                RT_E_Label.Text = Math.Round(Math.Sqrt(force_ex * force_ex + force_ey * force_ey), 1).ToString();
            }
            if (double.IsNaN(force_gx))
            {
                RT_g_Label.Text = double.PositiveInfinity.ToString();
            }
            else
            {
                RT_g_Label.Text = Math.Round(Math.Sqrt(force_gx * force_gx + force_gy * force_gy), 1).ToString();
            }
            bool hoverAgent = false;
            switch (movingEntity)
            {
                case MovingEntity.None:
                    if (specialCondition == SpecialCondition.SetVelocity)
                    {
                        ModelBoard_PB.Cursor = Cursors.Cross;
                        break;
                    }
                    foreach (AwoseAgent agent in Layers[CurrentLayer].Agents)
                    {
                        if (Calculations.IsInRadius(pointCursor.X, pointCursor.Y, agent, aw_agentsize * aw_scale))
                        {
                            hoverAgent = true;
                            break;
                        }
                    }
                    if (hoverAgent)
                    {
                       ModelBoard_PB.Cursor = Cursors.Hand;
                    }
                    else
                    {
                       ModelBoard_PB.Cursor = Cursors.Cross;
                    }
                    break;
                case MovingEntity.Board:
                    //Cursor = Cursors.NoMove2D;
                    //lu_corner = lu_remember - ((aw_remember - GetCursorPosition()) / aw_scale);
                    lu_corner = lu_remember - (PointParticle.ToPointParticle(boardBeforeMoving) - PointParticle.ToPointParticle(Cursor.Position)) / aw_scale;
                    break;
                case MovingEntity.Agent:
                    if (Layers[CurrentLayer].IsThereSelections())
                    {
                        Layers[CurrentLayer].Agents[Layers[CurrentLayer].Selected].Location = -lu_corner + GetCursorPosition() / aw_scale;

                    }
                    break;
                default:
                    break;
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
            if (isLaunched) return;
            if (NewValue_TB.Visible) NewValue_TB_PreviewKeyDown(sender, new PreviewKeyDownEventArgs(Keys.Enter));
            NewValue_TB.Location = new Point(ControlAgents_Panel.Location.X + ObjectSettings_Panel.Location.X + ObjectCharge_Label.Location.X + 1,
                ControlAgents_Panel.Location.Y + ObjectSettings_Panel.Location.Y + ObjectCharge_Label.Location.Y - 26);
            NewValue_TB.Text = Layers[CurrentLayer].Agents[Layers[CurrentLayer].Selected].Charge.ToString();
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
            foreach (AwoseLayer layer in Layers)
            {
                foreach (AwoseAgent agent in layer.Agents)
                {
                    agent.Backup();
                }
            }
            isLaunched = true;
            LaunchSimulation_MSItem.Enabled = false;
            PauseSimulation_MSItem.Enabled = true;
            StopSimulation_MSItem.Enabled = true;
            ResetSimulation_MSItem.Enabled = true;
            ObjectMass_Label.Cursor = Cursors.Default;
            ObjectCharge_Label.Cursor = Cursors.Default;
            ObjectPositionX_Label.Cursor = Cursors.Default;
            ObjectPositionY_Label.Cursor = Cursors.Default;
            ObjectVelocity_Label.Cursor = Cursors.Default;
            Pinned_CB.Enabled = false;
            Aw_CheckMistakes();
        }

        private void StopSimulation_MSItem_Click(object sender, EventArgs e)
        {
            isLaunched = false;
            LaunchSimulation_MSItem.Enabled = true;
            PauseSimulation_MSItem.Enabled = false;
            StopSimulation_MSItem.Enabled = false;
            ResetSimulation_MSItem.Enabled = true;
            ObjectMass_Label.Cursor = Cursors.IBeam;
            ObjectCharge_Label.Cursor = Cursors.IBeam;
            ObjectPositionX_Label.Cursor = Cursors.IBeam;
            ObjectPositionY_Label.Cursor = Cursors.IBeam;
            ObjectVelocity_Label.Cursor = Cursors.IBeam;
            Pinned_CB.Enabled = true;
            Aw_CheckMistakes();
        }
        private void ObjectPositionX_Label_Click(object sender, EventArgs e)
        {
            if (isLaunched) return;
            if (NewValue_TB.Visible) NewValue_TB_PreviewKeyDown(sender, new PreviewKeyDownEventArgs(Keys.Enter));
            NewValue_TB.Location = new Point(ControlAgents_Panel.Location.X + ObjectSettings_Panel.Location.X + ObjectPositionX_Label.Location.X + 1,
                ControlAgents_Panel.Location.Y + ObjectSettings_Panel.Location.Y + ObjectPositionX_Label.Location.Y - 26);
            NewValue_TB.Text = Layers[CurrentLayer].Agents[Layers[CurrentLayer].Selected].Location.X.ToString();
            editingValue = EditingValue.X;
            NewValue_TB.SelectAll();
            NewValue_TB.Visible = true;
            NewValue_TB.BringToFront();
            NewValue_TB.Focus();
        }

        private void ObjectPositionY_Label_Click(object sender, EventArgs e)
        {
            if (isLaunched) return;
            if (NewValue_TB.Visible) NewValue_TB_PreviewKeyDown(sender, new PreviewKeyDownEventArgs(Keys.Enter));
            NewValue_TB.Location = new Point(ControlAgents_Panel.Location.X + ObjectSettings_Panel.Location.X + ObjectPositionY_Label.Location.X + 1,
                ControlAgents_Panel.Location.Y + ObjectSettings_Panel.Location.Y + ObjectPositionY_Label.Location.Y - 26);
            NewValue_TB.Text = Layers[CurrentLayer].Agents[Layers[CurrentLayer].Selected].Location.Y.ToString();
            editingValue = EditingValue.Y;
            NewValue_TB.SelectAll();
            NewValue_TB.Visible = true;
            NewValue_TB.BringToFront();
            NewValue_TB.Focus();
        }

        private void ChangeSign_CMItem_Click(object sender, EventArgs e)
        {
            if (Layers[CurrentLayer].Agents[Layers[CurrentLayer].Selected].Charge != 0)
            {
                Layers[CurrentLayer].Agents[Layers[CurrentLayer].Selected].Charge = -Layers[CurrentLayer].Agents[Layers[CurrentLayer].Selected].Charge;

            }
            Aw_DrawControl();
        }

        private void Awose_FormClosing(object sender, FormClosingEventArgs e)
        {
            IsActive = false;
            if (MessageBox.Show("Do you really want to close Awose?", "Closing app", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                e.Cancel = true;
                IsActive = true;
                animation = new(AnimationEditor);
                animation.Start();
            }
            //animation.Join();
        }

        private void PinUp_CMItem_Click(object sender, EventArgs e)
        {
            //agents[aw_selected].IsPinned = !agents[aw_selected].IsPinned;
            Layers[CurrentLayer].Agents[Layers[CurrentLayer].Selected].IsPinned =
                !Layers[CurrentLayer].Agents[Layers[CurrentLayer].Selected].IsPinned;
            Aw_CheckMistakes();
            Aw_DrawControl();
        }

        private void SetVelocity_CMItem_Click(object sender, EventArgs e)
        {
            //Space_CMStr.Close();
            ////SettingVelocity = aw_selected;
            //aw_remember = GetCursorPosition();
            //specialCondition = SpecialCondition.SetVelocity;
            ////aw_remember = new Point((int)agents[aw_selected].X, (int)agents[aw_selected].Y);
        }

        private void ResetVelocity_CMItem_Click(object sender, EventArgs e)
        {
            Layers[CurrentLayer].Agents[Layers[CurrentLayer].Selected].Velocity = new();
            //agents[aw_selected].VelocityX = 0;
            //agents[aw_selected].VelocityY = 0;
        }

        private void SetFirstSpace_CMItem_Click(object sender, EventArgs e)
        {
            //isFirstSpaceSetting = true;
            specialCondition = SpecialCondition.SetFirstSpaceVelocity;
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
            //aw_undo.Push(new AwoseChange(agents[^1], ChangeType.Creating));
            Aw_CheckMistakes();
            Aw_DrawControl();
        }

        private void ResetSimulation_MSItem_Click(object sender, EventArgs e)
        {
            foreach (AwoseLayer layer in Layers)
            {
                foreach (AwoseAgent agent in layer.Agents)
                {
                    agent.Trajectory.Clear();
                    agent.Restore();
                }
            }
            Aw_CheckMistakes();
        }

        private void ModelBoard_PB_Click(object sender, EventArgs e)
        {

        }

        private void DispGrid_Editing_CMItem_Click(object sender, EventArgs e)
        {
            if (DispGrid_Editing_CMItem.Checked)
            {
                DispGrid_Editing_CMItem.Checked = false;
            } 
            else
            {
                DispGrid_Editing_CMItem.Checked = true;
            }
        }

        private void CreateNewLayer_Button_Click(object sender, EventArgs e)
        {
            int layersNum = 1;
        alw_loopNames:
            foreach (AwoseAgent item in Layers[CurrentLayer].Agents)
            {
                if (item.Name == "Layer" + layersNum.ToString())
                {
                    layersNum++;
                    goto alw_loopNames;
                }
            }

            Layers.Add(new AwoseLayer("Layer" + layersNum.ToString(), Layers.Count));
        }

        private void Pinned_CB_Click(object sender, EventArgs e)
        {
            Layers[CurrentLayer].Agents[Layers[CurrentLayer].Selected].IsPinned =
                !Layers[CurrentLayer].Agents[Layers[CurrentLayer].Selected].IsPinned;
            Aw_DrawControl();
        }

        private void ModelBoard_PB_Click_1(object sender, EventArgs e)
        {

        }

        private void ControlAgents_Panel_Click(object sender, EventArgs e)
        {
            if (NewValue_TB.Visible) NewValue_TB_PreviewKeyDown(sender, new PreviewKeyDownEventArgs(Keys.Enter));
        }

        private void ObjectSettings_Panel_Click(object sender, EventArgs e)
        {
            if (NewValue_TB.Visible) NewValue_TB_PreviewKeyDown(sender, new PreviewKeyDownEventArgs(Keys.Enter));
        }

        private void OpenModel_MSItem_Click(object sender, EventArgs e)
        {
            if (OpenModel_OFD.ShowDialog() == DialogResult.OK)
            {
                FileInfo fileInfo = new(OpenModel_OFD.FileName);
                FileStream file = fileInfo.OpenRead();
                StreamReader reader = new(file);
                string serializedobject = reader.ReadToEnd();
                Layers = JsonConvert.DeserializeObject<List<AwoseLayer>>(serializedobject);
                reader.Close();
                file.Close();
            }
        }

        private void ObjectVelocityCircle_PB_MouseHover(object sender, EventArgs e)
        {
            DegreeHint_Label.Text = Layers[CurrentLayer].Agents[Layers[CurrentLayer].Selected].Velocity.Angle.ToString() + "°";
            DegreeHint_Label.Location = new(ObjectVelocityCircle_PB.Location.X + (int)(ObjectVelocityCircle_PB.Width * 0.8),
                ObjectVelocityCircle_PB.Location.Y + (int)(ObjectVelocityCircle_PB.Height * 0.8));
            DegreeHint_Label.Visible = true;
        }

        private void ObjectVelocityCircle_PB_MouseLeave(object sender, EventArgs e)
        {
            DegreeHint_Label.Visible = false;
        }

        private void SetCustomVelocity_CMItem_Click(object sender, EventArgs e)
        {
            Space_CMStr.Close();
            //SettingVelocity = aw_selected;
            aw_remember = Layers[CurrentLayer].Agents[Layers[CurrentLayer].Selected].Location;
            Phantom = Layers[CurrentLayer].Agents[Layers[CurrentLayer].Selected];
            specialCondition = SpecialCondition.SetVelocity;
            //aw_remember = new Point((int)agents[aw_selected].X, (int)agents[aw_selected].Y);
        }

        private void CopyVelocity_CMItem_Click(object sender, EventArgs e)
        {
            CopiedVelocity = new(
                Layers[CurrentLayer].Agents[Layers[CurrentLayer].Selected].Velocity.Tail
            );
        }

        private void ApplyVelocity_CMItem_Click(object sender, EventArgs e)
        {
            Layers[CurrentLayer].Agents[Layers[CurrentLayer].Selected].Velocity = new(CopiedVelocity.Tail);
        }

        private void ObjectVelocity_Label_Click(object sender, EventArgs e)
        {
            return;
            if (isLaunched) return;
            if (NewValue_TB.Visible) NewValue_TB_PreviewKeyDown(sender, new PreviewKeyDownEventArgs(Keys.Enter));
            NewValue_TB.Location = new Point(ControlAgents_Panel.Location.X + ObjectSettings_Panel.Location.X + ObjectVelocity_Label.Location.X + 10,
                ControlAgents_Panel.Location.Y + ObjectSettings_Panel.Location.Y + ObjectVelocity_Label.Location.Y - 26);
            NewValue_TB.Text = Layers[CurrentLayer].Agents[Layers[CurrentLayer].Selected].Velocity.Length.ToString();
            editingValue = EditingValue.VelocityLength;
            NewValue_TB.SelectAll();
            NewValue_TB.Visible = true;
            NewValue_TB.BringToFront();
            NewValue_TB.Focus();
        }

        private void ReverseVelocity_CMItem_Click(object sender, EventArgs e)
        {
            Layers[CurrentLayer].Agents[Layers[CurrentLayer].Selected].Velocity.Tail.X = -Layers[CurrentLayer].Agents[Layers[CurrentLayer].Selected].Velocity.Tail.X;
            Layers[CurrentLayer].Agents[Layers[CurrentLayer].Selected].Velocity.Tail.Y = -Layers[CurrentLayer].Agents[Layers[CurrentLayer].Selected].Velocity.Tail.Y;
        }

        private void ObjectSprite_White_PB_MouseHover(object sender, EventArgs e)
        {
            ObjectColorHint_Label.Text = "White";
            ObjectColorHint_Label.Location = new Point(
                ObjectSprite_White_PB.Location.X + 4,
                ObjectSprite_White_PB.Location.Y + ObjectSprite_White_PB.Height + 7
                );
            ObjectColorHint_Label.Visible = true;
            //beautyPreview = BeautyPreviewMode.ObjectColor;
        }

        private void ObjectSprite_White_PB_MouseLeave(object sender, EventArgs e)
        {
            ObjectColorHint_Label.Visible = false;
        }

        private void ObjectSprite_Yellow_PB_MouseHover(object sender, EventArgs e)
        {
            ObjectColorHint_Label.Text = "Yellow";
            ObjectColorHint_Label.Location = new Point(
                ObjectSprite_Yellow_PB.Location.X + 4,
                ObjectSprite_Yellow_PB.Location.Y + ObjectSprite_Yellow_PB.Height + 7
                );
            ObjectColorHint_Label.Visible = true;
        }

        private void ObjectSprite_Yellow_PB_MouseLeave(object sender, EventArgs e)
        {
            ObjectColorHint_Label.Visible = false;
        }

        private void ObjectSprite_Green_PB_MouseHover(object sender, EventArgs e)
        {
            ObjectColorHint_Label.Text = "Green";
            ObjectColorHint_Label.Location = new Point(
                ObjectSprite_Green_PB.Location.X + 4,
                ObjectSprite_Green_PB.Location.Y + ObjectSprite_Green_PB.Height + 7
                );
            ObjectColorHint_Label.Visible = true;
        }

        private void ObjectSprite_Green_PB_MouseLeave(object sender, EventArgs e)
        {
            ObjectColorHint_Label.Visible = false;
        }

        private void ObjectSprite_Sky_PB_MouseHover(object sender, EventArgs e)
        {
            ObjectColorHint_Label.Text = "Sky";
            ObjectColorHint_Label.Location = new Point(
                ObjectSprite_Sky_PB.Location.X + 4,
                ObjectSprite_Sky_PB.Location.Y + ObjectSprite_Sky_PB.Height + 7
                );
            ObjectColorHint_Label.Visible = true;
        }

        private void ObjectSprite_Sky_PB_MouseLeave(object sender, EventArgs e)
        {
            ObjectColorHint_Label.Visible = false;
        }

        private void ObjectSprite_Yellow_PB_Click(object sender, EventArgs e)
        {
            Layers[CurrentLayer].Agents[Layers[CurrentLayer].Selected].Dye = Color.Yellow;
            Layers[CurrentLayer].Agents[Layers[CurrentLayer].Selected].Sprite = SpriteType.Yellow;
            Aw_DrawControl();
        }

        private void ObjectSprite_White_PB_Click(object sender, EventArgs e)
        {
            Layers[CurrentLayer].Agents[Layers[CurrentLayer].Selected].Dye = Color.White;
            Layers[CurrentLayer].Agents[Layers[CurrentLayer].Selected].Sprite = SpriteType.White;
            Aw_DrawControl();
        }

        private void ObjectSprite_Green_PB_Click(object sender, EventArgs e)
        {
            Layers[CurrentLayer].Agents[Layers[CurrentLayer].Selected].Dye = Color.Green;
            Layers[CurrentLayer].Agents[Layers[CurrentLayer].Selected].Sprite = SpriteType.Green;
            Aw_DrawControl();
        }

        private void ObjectSprite_Sky_PB_Click(object sender, EventArgs e)
        {
            Layers[CurrentLayer].Agents[Layers[CurrentLayer].Selected].Dye = Color.SkyBlue;
            Layers[CurrentLayer].Agents[Layers[CurrentLayer].Selected].Sprite = SpriteType.Sky;
            Aw_DrawControl();
        }

        private void DeleteAllObjects_CMI_Click(object sender, EventArgs e)
        {
            Layers[CurrentLayer].Agents.Clear();

            Aw_DrawControl();
        }

        private void NewValue_TB_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Down)
            {
                NewValue_TB.SelectAll();
            }
        }

        private void TrajNonfade_Button_Click(object sender, EventArgs e)
        {
            Layers[CurrentLayer].Agents[Layers[CurrentLayer].Selected].TrajectoryLine = TrajectoryType.Nonfade;
            Aw_DrawControl();
            Aw_Refresh();
        }

        private void TrajFade_Button_Click(object sender, EventArgs e)
        {
            Layers[CurrentLayer].Agents[Layers[CurrentLayer].Selected].TrajectoryLine = TrajectoryType.Fade;
            Aw_DrawControl();
            Aw_Refresh();
        }

        private void TrajNo_Button_Click(object sender, EventArgs e)
        {
            Layers[CurrentLayer].Agents[Layers[CurrentLayer].Selected].TrajectoryLine = TrajectoryType.None;
            Aw_DrawControl();
            Aw_Refresh();
        }

        private void ObjectSprite_Red_PB_MouseHover(object sender, EventArgs e)
        {
            ObjectColorHint_Label.Text = "Pink";
            ObjectColorHint_Label.Location = new Point(
                ObjectSprite_Red_PB.Location.X + 4,
                ObjectSprite_Red_PB.Location.Y + ObjectSprite_Red_PB.Height + 7
                );
            ObjectColorHint_Label.Visible = true;
        }

        private void ObjectSprite_Red_PB_MouseLeave(object sender, EventArgs e)
        {
            ObjectColorHint_Label.Visible = false;
        }

        private void ObjectSprite_Red_PB_Click(object sender, EventArgs e)
        {
            Layers[CurrentLayer].Agents[Layers[CurrentLayer].Selected].Dye = Color.PaleVioletRed;
            Layers[CurrentLayer].Agents[Layers[CurrentLayer].Selected].Sprite = SpriteType.Red;
            Aw_DrawControl();
        }
    }
}
