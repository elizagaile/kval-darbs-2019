using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MathNet.Numerics.LinearAlgebra;
using System.IO;


namespace BezierTool
{
    public partial class Form1 : Form //contains form, all its atributes and functions
    {
        public Form1()
        {
            InitializeComponent();
            this.Width = Convert.ToInt32(0.5 * Screen.PrimaryScreen.Bounds.Width);
            this.Height = Convert.ToInt32(0.75 * Screen.PrimaryScreen.Bounds.Height);
        }

        //each line is representet by two lists: 
        //list of control points, list of known points on the line 
        //all lines have control points, but come lines (<4cPoints>) dont have known points on the line
        //also each line has a drawing type (saved in list AllLines) and a parametrization type 
        //the i-th drawn list is represented by i-th position in all of the lists

        private List<Point> cPoints = null;//list of line's control points
        public static List<List<Point>> cPointsAll = new List<List<Point>>();//contains all lists of control points, public so Form2 can access

        private List<Point> pPoints = null;// list of points on line
        public static List<List<Point>> pPointsAll = new List<List<Point>>();// contains all lists of points on line
        
        public static Tuple<int, int> LocalPoint = null;//location of a selected point in the representitive lists

        private Point NewcPoint; //new point for line <4 cPoints>
        
        private enum MoveType { leftClick, rightClick, pPoints, nothing };//ways to move <Composite> points by mouse
        private List<MoveType> MovedLine = new List<MoveType>();//contains a way a list has been moved
        
        enum ParamType { uniform, chord, centripental, nothing }; // parametrization types
        private List<ParamType> Parametrization = new List<ParamType>();//contains parametrization types for drawn lines

        public enum Form2Type { add, modify, output };

        public enum BezierType { cPoints, pPoints, leastSquares, composite, nothing }; //all posible line types, public for Form2

        public static List<BezierType> AllLines = new List<BezierType>();// contains type of drawn lines

        public static BezierType AddType = BezierType.nothing;// type of line to be or being added, public static for Form2
        public static BezierType ModifyType = BezierType.nothing;// type of line to be or being modified
        public static BezierType DragType = BezierType.nothing;// type of line to be or being modified
        public static BezierType OutputPointsType = BezierType.nothing;// type of line to be or being draged by mouse
        
        String imageLocation = ""; //for background image

        int PointRadius = 2; //radius for control points and specific points on lines, chosen arbitrary
        int LocalRadius = 7; //radius of neiborghood, used when selecting a point with mouse, chosen arbitrary
        int maxPointCount = 25; //maximum count of points to choose for lines <Least Squares> and <Composite>, chosen arbitrary

        bool CompositeDone = false;//indicates if the last line of type <Composite> needs to be finished;
        bool ChangeParam = false;//indicates if option to change parametrization is enabled
        bool ChangingMode = false;//indicates if parametrization is being changed
        bool DeleteMode = false;//indicates if option to delete a line is enabled

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
            //Mouse has been pressed inside pictureBox1. This can be for adding control points or points on the line,
            //for moving points with mouse or for selecting a line to output point coordinates
        {
            if (AddType != BezierType.nothing && rbtn_MouseAdd.Checked == true)
            //if we want to add a new point by mouse            
            {
                if (AddType == BezierType.cPoints)
                //if we want to add a new point for a <4 cPoints> type line
                {
                    AddcPoint(BezierType.cPoints, e.Location);
                }

                else if (AddType == BezierType.pPoints || AddType == BezierType.leastSquares || AddType == BezierType.composite)
                //for <4 pPoints>, <Least Squares> or <Composite> type lines we can add only line points
                {
                    AddpPoint(AddType, e.Location);
                }

                pictureBox1.Invalidate();
            }

            if (cPointsAll != null && DragType == BezierType.cPoints)
            //if we want to drag a control point
            {
                findLocalPoint(cPointsAll, e.Location);

                if (LocalPoint != null)
                {
                    int i = LocalPoint.Item1;
                    int j = LocalPoint.Item2;
                    ModifyType = AllLines[i];

                    if (ModifyType == BezierType.pPoints || ModifyType == BezierType.leastSquares)
                    {
                        error.Text = "It's not allowed to move curve's " + ModifyType + " control points!";
                        ModifyType = BezierType.nothing;
                        LocalPoint = null;
                    }

                    else if (ModifyType == BezierType.composite)
                    {
                        if (rbtn_KeyboardModify.Checked == true)
                        {
                            error.Text = "It's not allowed to move curve's " + ModifyType + " points by keyboard!!";
                            LocalPoint = null;
                            ModifyType = BezierType.nothing;
                        }

                        else if (j % 3 == 0)
                        // every third control point on a composite line is one of the line points (pPoint) 
                        {
                            LocalPoint = null;
                            ModifyType = BezierType.nothing;
                        }

                        else if (e.Button == MouseButtons.Left)
                        {
                            MovedLine[i] = MoveType.leftClick;
                        }

                        else if (e.Button == MouseButtons.Right)
                        {
                            MovedLine[i] = MoveType.rightClick;
                        }
                    }

                    else if (rbtn_KeyboardModify.Checked == true)
                    {
                        Form_KeyboardAdd form_KeyboardAdd = new Form_KeyboardAdd(Form2Type.modify, ModifyType);
                        form_KeyboardAdd.ShowDialog();
                        
                        MovedLine[i] = MoveType.pPoints;
                        ModifyType = BezierType.nothing;
                        LocalPoint = null;
                    }
                }

                pictureBox1.Invalidate();
            }

            if (pPointsAll != null && DragType == BezierType.pPoints)
            //if we want to drag a line point
            {
                findLocalPoint(pPointsAll, e.Location);

                if (LocalPoint != null)
                {
                    int i = LocalPoint.Item1;
                    ModifyType = AllLines[i];

                    if (rbtn_KeyboardModify.Checked == true)
                    {
                        Form_KeyboardAdd form_KeyboardAdd = new Form_KeyboardAdd(Form2Type.modify, ModifyType);
                        form_KeyboardAdd.ShowDialog();

                        if (ModifyType == BezierType.pPoints || ModifyType == BezierType.leastSquares)
                        {
                            getcPoints(i);
                        }

                        ModifyType = BezierType.nothing;
                        LocalPoint = null;
                        pictureBox1.Invalidate();
                    }
                }

                pictureBox1.Invalidate();
            }

            if (cPointsAll != null && OutputPointsType == BezierType.cPoints && rbtn_ScreenOutput.Checked == true)
            // if we want to output line's control point coordinates on screen
            {
                findLocalPoint(cPointsAll, e.Location);

                if (LocalPoint != null)
                {
                    int i = LocalPoint.Item1;

                    Form_KeyboardAdd form_KeyboardAdd = new Form_KeyboardAdd(Form2Type.output, AllLines[i]);
                    form_KeyboardAdd.ShowDialog();
                    
                    OutputPointsType = BezierType.nothing;
                    ModifyType = BezierType.nothing;
                    LocalPoint = null;
                }

                pictureBox1.Invalidate();
            }

            if (pPointsAll != null && OutputPointsType == BezierType.pPoints && rbtn_ScreenOutput.Checked == true)
            //if we want to output coordinates of known line points on screen
            {
                findLocalPoint(pPointsAll, e.Location);

                if (LocalPoint != null)
                {
                    int i = LocalPoint.Item1;

                    Form_KeyboardAdd form_KeyboardAdd = new Form_KeyboardAdd(Form2Type.output, AllLines[i]);
                    form_KeyboardAdd.ShowDialog();

                    OutputPointsType = BezierType.nothing;
                    ModifyType = BezierType.nothing;
                    LocalPoint = null;
                }

                pictureBox1.Invalidate();
            }

            if (cPointsAll != null && OutputPointsType == BezierType.cPoints && rbtn_FileOutput.Checked == true)
            // if we want to output line's control point coordinates to .txt file
            {
                findLocalPoint(cPointsAll, e.Location);

                if (LocalPoint != null)
                {
                    int i = LocalPoint.Item1;

                    OutputcPointsFile(i);
                    OutputPointsType = BezierType.nothing;
                    ModifyType = BezierType.nothing;
                    LocalPoint = null;
                }

                pictureBox1.Invalidate();
            }

            if (pPointsAll != null && OutputPointsType == BezierType.pPoints && rbtn_FileOutput.Checked == true)
            // if we want to output lines's line point coordinates to .txt file
            {
                findLocalPoint(pPointsAll, e.Location);

                if (LocalPoint != null)
                {
                    int i = LocalPoint.Item1;

                    OutputpPointsFile(i);
                    OutputPointsType = BezierType.nothing;
                    ModifyType = BezierType.nothing;
                    LocalPoint = null;
                }

                pictureBox1.Invalidate();
            }

            if (cPointsAll != null && ChangeParam == true && ChangingMode == false)
            //if we want to change parametrization type
            {
                findLocalPoint(cPointsAll, e.Location);

                if (LocalPoint == null)
                //mouse wasn't clicked near a control point
                {
                    return;
                }

                int i = LocalPoint.Item1;
                ParamType paramType = Parametrization[i];

                if (AllLines[i] == BezierType.cPoints || AllLines[i] == BezierType.composite)
                {
                    error.Text = "<" + AllLines[i] + "> lines doesn't use parametrization!";
                    return;
                }

                //show the real parametrization type of the selected line
                if (paramType == ParamType.uniform)
                {
                    rbtn_Uniform.Checked = true;
                }

                else if (paramType == ParamType.chord)
                {
                    rbtn_Chord.Checked = true;
                }

                else if (paramType == ParamType.centripental)
                {
                    rbtn_Centripental.Checked = true;
                }

                ChangingMode = true;
            }

            if (cPointsAll != null && DeleteMode == true)
            {
                findLocalPoint(cPointsAll, e.Location);

                if (LocalPoint == null)
                //mouse wasn't clicked near a control point
                {
                    return;
                }

                int i = LocalPoint.Item1;

                string message = "Do you want to delete this line?";
                string title = "Delete line";
                MessageBoxButtons buttons = MessageBoxButtons.OKCancel;
                DialogResult result = MessageBox.Show(message, title, buttons);
                
                if (result == DialogResult.OK)
                {
                    deleteLine(i);
                }

                pictureBox1.Invalidate();
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
            //Mouse is moving inside pictureBox1. This is used to draw dashed line when adding new points for <4 cPoints> line
            // or it can be used for modifying a line by mouse.
        {
            if (AddType == BezierType.cPoints)
            // if we are adding a new control point for <4 cPoints> line 
            {
                NewcPoint = e.Location;
                pictureBox1.Invalidate();
            }

            int i = 0;//need to set a value for code to work, chose 0 arbitrary
            int j = 0;

            if (LocalPoint != null)
            {
                i = LocalPoint.Item1;
                j = LocalPoint.Item2;
            }

            if (ModifyType == BezierType.cPoints)
            // for type <4 cPoints> line we can just change coordinates
            {
                cPointsAll[i][j] = e.Location;
                pictureBox1.Invalidate();
            }

            else if ((ModifyType == BezierType.pPoints || ModifyType == BezierType.leastSquares) && DragType == BezierType.pPoints)
            //if we change type <4 pPoints> or <Least Squares> line points, we need to re-calculate its control points
            {
                pPointsAll[i][j] = e.Location;
                getcPoints(i);
                pictureBox1.Invalidate();
            }

            else if (ModifyType == BezierType.composite && DragType == BezierType.cPoints)
            // if we change type <Composite> line's control points, we need to make sure the line stays C2 continuous
            {
                if (MovedLine[i] == MoveType.leftClick)
                // using left click, we can drag the control point anywhere, but the 'opposite' control point moves aswell
                //to maintain continuity
                {
                    cPointsAll[i][j] = e.Location;

                    if (j % 3 == 1 && j != 1)
                    //starting from the fifth control point, every third point's opposite control point is two points before
                    {
                        changecPoint(j, j - 1, j - 2);
                    }

                    if (j % 3 == 2 && j != cPointsAll[i].Count - 2)
                    //starting from the third control point, every third point's opposite control point is two points after
                    {
                        changecPoint(j, j + 1, j + 2);
                    }
                }

                if (MovedLine[i] == MoveType.rightClick)
                // using right click no other control points will move, but to maintain continuity, we can only move the
                // control point in straight line away from its opposite point
                {
                    if (j % 3 == 1 && j != 1)
                    //starting from the fifth control point, every third point's opposite control point is two points before
                    {
                        changeStraight(e.Location, cPointsAll[i][j - 1], cPointsAll[i][j - 2]);
                    }

                    if (j % 3 == 2 && j != cPointsAll[i].Count - 2)
                    //starting from the third control point, every third point's opposite control point is two points after
                    {
                        changeStraight(e.Location, cPointsAll[i][j + 1], cPointsAll[i][j + 2]);
                    }
                }
                pictureBox1.Invalidate();
            }

            else if (ModifyType == BezierType.composite && DragType == BezierType.pPoints)
            {
                pPointsModifyComposite(e.Location);
                
                MovedLine[i] = MoveType.pPoints;
                pictureBox1.Invalidate();
            }
        }

        public static void pPointsModifyComposite(Point pointNew)
        {
            int i = LocalPoint.Item1;
            int j = LocalPoint.Item2;

            Point pointOld = new Point();
            pointOld = pPointsAll[i][j];

            pPointsAll[i][j] = pointNew;
            cPointsAll[i][j * 3] = pointNew;

            Point tmp = new Point();
            if (j != 0)
            {
                tmp.X = pointNew.X - pointOld.X + cPointsAll[i][j * 3 - 1].X;
                tmp.Y = pointNew.Y - pointOld.Y + cPointsAll[i][j * 3 - 1].Y;
                cPointsAll[i][j * 3 - 1] = tmp;
            }

            if (j != pPointsAll[i].Count - 1)
            {
                tmp.X = pointNew.X - pointOld.X + cPointsAll[i][j * 3 + 1].X;
                tmp.Y = pointNew.Y - pointOld.Y + cPointsAll[i][j * 3 + 1].Y;
                cPointsAll[i][j * 3 + 1] = tmp;
            }

            return;
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
            //Mouse button was released after pressing it in pictureBox1. If a point was being dragged by mouse, the dragging stops.
        {
            if (DragType != BezierType.nothing)
            {
                ModifyType = BezierType.nothing;
                LocalPoint = null;
            }
            pictureBox1.Invalidate();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
            //Draws all graphics in this programm - all bezier functions, straight lines and points, and calls for functions to get needed control points
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;//makes lines look smoother

            if (cPointsAll == null || pPointsAll == null)
            {
                return;
            }

            if (cPoints != null)
            //if we are selecting points for <4 cPoints> line, draw a dashed line from mouse location to previous control point
            {
                if (cPoints.Count < 4 && AddType == BezierType.cPoints)
                {
                    using (Pen dashed_pen = new Pen(Color.LightGray))
                    {
                        dashed_pen.DashPattern = new float[] { 5, 5 };
                        e.Graphics.DrawLine(dashed_pen, cPoints[cPoints.Count - 1], NewcPoint);
                    }
                }
            }

            for (int i = 0; i < pPointsAll.Count; i++)
            //go through every list of line points
            {
                if (pPointsAll[i] != null)
                {
                    foreach (Point p in pPointsAll[i])
                    //draw a black point for every line point
                    {
                        e.Graphics.FillEllipse(Brushes.Black, p.X - PointRadius, p.Y - PointRadius, 2 * PointRadius, 2 * PointRadius);
                    }

                    if (AllLines[i] == BezierType.pPoints && pPointsAll[i].Count == 4 && cPointsAll[i] == null)
                    //if there are four line points for <4 pPoints> line, but we haven't yet calculated control points, calculate them
                    {
                        getcPoints(i);
                    }

                    if (AllLines[i] == BezierType.leastSquares && pPointsAll[i].Count >= 4)
                    //if there are at least four line points for <Least Square> line, calculate control points
                    {
                        getcPoints(i);
                    }

                    if (AllLines[i] == BezierType.composite && MovedLine[i] == MoveType.nothing)
                    // if we want to draw a <Composite> line which hasn't been moved
                    {
                        if (CompositeDone == true && pPointsAll[i].Count == 2)
                        //finish and draw a <Composite> line with only two line points
                        {
                            cPointsAll[i] = new List<Point>();
                            addOnlycPoints(i);
                        }

                        else if (pPointsAll[i].Count >= 3)
                        //if a <Composite> line has more than 3 line points, we can calculate its control points
                        {
                            cPointsAll[i] = new List<Point>();
                            addcPointsComposite(i);
                        }
                    }
                }
            }
            
            for (int i = 0; i < cPointsAll.Count; i++)
            // go through every list of control points
            {
                if (cPointsAll[i] != null)
                {
                    //Drawing control points:

                    if (AllLines[i] == BezierType.cPoints || AllLines[i] == BezierType.leastSquares)
                    // for <4 cPoints> and <Least Squares> draw all control points
                    {
                        foreach (Point c in cPointsAll[i])
                        {
                            e.Graphics.DrawEllipse(Pens.Red, c.X - PointRadius, c.Y - PointRadius, 2 * PointRadius, 2 * PointRadius);
                        }
                    }

                    else if (AllLines[i] == BezierType.pPoints)
                    //for <4 pPoints> draw only middle control points, because end points ar both control points and line points
                    {
                        e.Graphics.DrawEllipse(Pens.Red, cPointsAll[i][1].X - PointRadius, cPointsAll[i][1].Y - PointRadius, 2 * PointRadius, 2 * PointRadius);
                        e.Graphics.DrawEllipse(Pens.Red, cPointsAll[i][2].X - PointRadius, cPointsAll[i][2].Y - PointRadius, 2 * PointRadius, 2 * PointRadius);
                    }

                    else if (AllLines[i] == BezierType.composite)
                    // for <Composite> draw only those control points, which are not line points - every third line point starting from the first is also a control point
                    {
                        for (int j = 0; j < cPointsAll[i].Count - 1; j++)
                        {
                            if (j % 3 != 2)
                            {
                                e.Graphics.DrawEllipse(Pens.Red, cPointsAll[i][j + 1].X - PointRadius, cPointsAll[i][j + 1].Y - PointRadius, 2 * PointRadius, 2 * PointRadius);
                            }
                        }
                    }


                    //Drawing control point polygons / "handle" lines

                    if (cPointsAll[i].Count > 1 && (AllLines[i] == BezierType.cPoints || AllLines[i] == BezierType.leastSquares || AllLines[i] == BezierType.pPoints))
                    {
                        e.Graphics.DrawLines(Pens.LightGray, cPointsAll[i].ToArray());
                    }

                    else if (AllLines[i] == BezierType.composite)
                    //for <Composite> lines, draw only handles
                    {
                        for (int j = 0; j < cPointsAll[i].Count - 1; j++)
                        {
                            if (j % 3 != 1)
                            {
                                e.Graphics.DrawLine(Pens.LightGray, cPointsAll[i][j], cPointsAll[i][j + 1]);
                            }
                        }
                    }


                    //Drawing all bezier lines:

                    if (cPointsAll[i].Count == 4 && (AllLines[i] == BezierType.cPoints || AllLines[i] == BezierType.leastSquares || AllLines[i] == BezierType.pPoints))
                    {
                        e.Graphics.DrawBezier(Pens.Black, cPointsAll[i][0], cPointsAll[i][1], cPointsAll[i][2], cPointsAll[i][3]);
                    }

                    else if (AllLines[i] == BezierType.composite)
                    {
                        for (int j = 0; j < cPointsAll[i].Count - 3; j += 3)
                        {
                            e.Graphics.DrawBezier(Pens.Black, cPointsAll[i][j], cPointsAll[i][j + 1], cPointsAll[i][j + 2], cPointsAll[i][j + 3]);
                        }
                    }
                }
            }
        }

        private void btnBackground_Click(object sender, EventArgs e)
            //uploads background image
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "jpg files(.*jpg)|*.jpg| PNG files(.*png)|*.png| All Files(*.*)|*.*"; //types of files allowed ???

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    imageLocation = dialog.FileName;
                    pictureBox1.ImageLocation = imageLocation;
                    cbox_ShowBackground.Checked = true;
                }
            }

            catch (Exception)
            {
                MessageBox.Show("Image upload error!");
            }
        }

        private void cbox_ShowBackground_CheckStateChanged(object sender, EventArgs e)
            //make uploaded background picture visable or invisible
        {
            if (cbox_ShowBackground.Checked == false)
            {
                pictureBox1.ImageLocation = "";
            }

            else
            {
                pictureBox1.ImageLocation = imageLocation;
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
            //make form responsive
        {
            int width = this.Width;
            int height = this.Height;

            panel_tools.Left = width - panel_tools.Width - 20; // 20px, 50px, 35 px and 55px makes margins
            panel_bottom.Left = width - panel_bottom.Width - 20;
            panel_bottom.Top = height - panel_bottom.Height - 50;
            pictureBox1.Width = width - panel_tools.Width - 35;
            pictureBox1.Height = height - 55;
        }

        private void newLine(BezierType type)
            //start a new line
        {
            AllLines.Add(type);

            cPoints = null;
            pPoints = null;
            DragType = BezierType.nothing;
            ModifyType = BezierType.nothing;
            LocalPoint = null;
            ChangingMode = false;
            CompositeDone = false;
            MovedLine.Add(MoveType.nothing);
            cPointsAll.Add(null);
            pPointsAll.Add(null);

            if (type == BezierType.cPoints || type == BezierType.composite)
            {
                Parametrization.Add(ParamType.nothing);
            }

            else if (type == BezierType.pPoints || type == BezierType.leastSquares)
            {
                if (rbtn_Uniform.Checked == true)
                {
                    Parametrization.Add(ParamType.uniform);
                }

                else if (rbtn_Chord.Checked == true)
                {
                    Parametrization.Add(ParamType.chord);
                }

                else if (rbtn_Centripental.Checked == true)
                {
                    Parametrization.Add(ParamType.centripental);
                }
            }

            return;
        }

        private void btn_deleteLine_Click(object sender, EventArgs e)
            //enable option to delete lines
        {
            DeleteMode = true;
        }

        private void deleteLine(int i)
            //remove everything function newLine() added; used when adding new line by keybord is canceled
        {
            AddType = BezierType.nothing;
            AllLines.RemoveAt(i);
            MovedLine.RemoveAt(i);
            cPointsAll.RemoveAt(i);
            pPointsAll.RemoveAt(i);
            Parametrization.RemoveAt(i);

            DeleteMode = false;

            return;
        }

        private void btn_cPointsAdd_Click(object sender, EventArgs e)
            //start a new line of type <4 cPoints>
        {
            AddType = BezierType.cPoints;

            if (rbtn_MouseAdd.Checked == true)
            //adding new line with mouse
            {
                cPoints = null;
            }

            if (rbtn_KeyboardAdd.Checked == true)
            //adding new line by keyboard
            {
                newLine(BezierType.cPoints);

                Form_KeyboardAdd form_KeyboardAdd = new Form_KeyboardAdd(Form2Type.add, AddType);
                form_KeyboardAdd.ShowDialog();

                if (Form_KeyboardAdd.lineAdded == false) 
                    //an error or cancelation occured and no line was added
                {
                    deleteLine(AllLines.Count - 1);//delete what newLine() added
                    return;
                }

                pictureBox1.Invalidate();
            }

            if (rbtn_FileAdd.Checked == true)
            //adding new line from a .txt file
            {
                cPoints = getPointsfromFile();

                if (cPoints.Count != 4)
                {
                    error.Text = ".txt file was not correct!";
                    deleteLine(AllLines.Count - 1);
                    return;
                }

                cPointsAll[cPointsAll.Count - 1] = cPoints;
                pictureBox1.Invalidate();
            }
        }

        private void btn_pPointsAdd_Click(object sender, EventArgs e)
            //start a new line of type <4 pPoints>
        {
            AddType = BezierType.pPoints;

            if (rbtn_MouseAdd.Checked == true)
            {
                pPoints = null;
            }

            if (rbtn_KeyboardAdd.Checked == true)
                //if adding new line by keyboard
            {
                newLine(BezierType.cPoints);

                Form_KeyboardAdd form_KeyboardAdd = new Form_KeyboardAdd(Form2Type.add, AddType);
                form_KeyboardAdd.ShowDialog();

                if (Form_KeyboardAdd.lineAdded == false)
                    //an error or cancelation occured and no line was added
                {
                    deleteLine(AllLines.Count - 1);//delete what newLine() added
                    return;
                }

                pictureBox1.Invalidate();
            }

            if (rbtn_FileAdd.Checked == true)
            //if adding new line from a .txt file
            {
                pPoints = getPointsfromFile();

                if (pPoints.Count != 4)
                {
                    error.Text = ".txt file was not correct!";
                    deleteLine(AllLines.Count - 1);
                    return;
                }

                pPointsAll[pPointsAll.Count - 1] = pPoints;
                pictureBox1.Invalidate();
            }
        }

        private void btn_LeastSquaresAdd_Click(object sender, EventArgs e)
            //start a new line of type <4 pPoints>
        {
            AddType = BezierType.leastSquares;

            if (rbtn_MouseAdd.Checked == true)
            {
                pPoints = null;
            }

            if (rbtn_KeyboardAdd.Checked == true)
            //if adding new line by keyboard
            {
                newLine(BezierType.cPoints);

                Form_KeyboardAdd form_KeyboardAdd = new Form_KeyboardAdd(Form2Type.add, AddType);
                form_KeyboardAdd.ShowDialog();

                if (Form_KeyboardAdd.lineAdded == false)
                //an error or cancelation occured and no line was added
                {
                    deleteLine(AllLines.Count - 1);//delete what newLine() added
                    return;
                }

                pictureBox1.Invalidate();
            }

            if (rbtn_FileAdd.Checked == true)
            //if adding new line from a .txt file
            {
                pPoints = getPointsfromFile();

                if (pPoints.Count < 4 || pPoints.Count > maxPointCount)
                {
                    error.Text = ".txt file was not correct!";
                    deleteLine(AllLines.Count - 1);
                    return;
                }

                pPointsAll[pPointsAll.Count - 1] = pPoints;
                pictureBox1.Invalidate();
            }
        }

        private void btn_CompositeAdd_Click(object sender, EventArgs e)
            //start a new line of type <Composite>
        {
            AddType = BezierType.composite;

            if (rbtn_MouseAdd.Checked == true)
            {
                pPoints = null;
            }

            if (rbtn_KeyboardAdd.Checked == true)
            //if adding new line by keyboard
            {
                newLine(BezierType.cPoints);

                Form_KeyboardAdd form_KeyboardAdd = new Form_KeyboardAdd(Form2Type.add, AddType);
                form_KeyboardAdd.ShowDialog();

                if (Form_KeyboardAdd.lineAdded == false)
                //an error or cancelation occured and no line was added
                {
                    deleteLine(AllLines.Count - 1);//delete what newLine() added
                    return;
                }
                CompositeDone = true;

                pictureBox1.Invalidate();
            }

            if (rbtn_FileAdd.Checked == true)
            //if adding new line from a .txt file
            {
                pPoints = getPointsfromFile();

                if (pPoints.Count < 2 || pPoints.Count > maxPointCount)
                {
                    error.Text = ".txt file was not correct!";
                    deleteLine(AllLines.Count - 1);
                    return;
                }

                pPointsAll[pPointsAll.Count - 1] = pPoints;
                CompositeDone = true;
                pictureBox1.Invalidate();
            }
        }

        private void AddcPoint(BezierType type, Point MouseLocation)
            //add new control point to the current line
        {
            if (cPoints == null)
            //if this is the first point of line
            {
                newLine(AddType);
                cPoints = new List<Point>();
                cPoints.Add(MouseLocation);
                cPointsAll[cPointsAll.Count - 1] = cPoints;
            }

            else if (cPoints.Count < 4 && cPoints[cPoints.Count - 1] != MouseLocation)
            //to avoid accidental double clicks
            {
                cPoints.Add(MouseLocation);
            }

            return;
        }

        private void AddpPoint(BezierType type, Point MouseLocation)
            //add new point on line to the current line
        {
            if (pPoints == null)
            //if this is the first point of line
            {
                newLine(AddType);
                pPoints = new List<Point>();
                pPoints.Add(MouseLocation);
                pPointsAll[pPointsAll.Count - 1] = pPoints;
                
                return;
            }

            if (pPoints[pPoints.Count - 1] == MouseLocation)
            //to avoid accidental double clicks
            {
                return;
            }

            if (type == BezierType.pPoints && pPoints.Count >= 4)
            //type <4 pPoints can't have more than 4 chosen points on line)
            {
                return;
            }

            if (type == BezierType.composite && CompositeDone == true)
            // can't add points to a <Composite> line that has been finished
            {
                return;
            }

            else if ((type == BezierType.leastSquares || type == BezierType.composite) && pPoints.Count > maxPointCount)
            // can't choose more points than the maximum allowed count
            {
                return;
            }

            pPoints.Add(MouseLocation);
            return;
        }

        private void addcPointsComposite(int i)
            //calculate control points for a <Composite> line with three or more line points
        {
            cPointsAll[i].Add(pPointsAll[i][0]);//first control point is first line point
            addFirstcPoint(i);//add first control point that isn't a line point

            for (int j = 2; j < pPointsAll[i].Count; j++)
            //we can add three new controlpoints for every line point starting with the third line point. 
            //Every line point is also a control point and for every but first andl last point, we get two control points - "handles"
            {
                cPointsAll[i].Add(firstHandle(pPointsAll[i][j - 2], pPointsAll[i][j - 1], pPointsAll[i][j]));
                cPointsAll[i].Add(pPointsAll[i][j - 1]);
                cPointsAll[i].Add(secondHandle(pPointsAll[i][j - 2], pPointsAll[i][j - 1], pPointsAll[i][j]));
            }

            if (i != AllLines.Count - 1 && cPointsAll[i].Count < pPointsAll[i].Count * 3 - 2)
            //if a <Composite> line isn't the last drawn line, it must be finished. 
            //That means, it should have three times (every point is a control point and has two "handles") minus two (each end point dont have one handle) more control points than line points
            {
                addLastcPoints(i);
            }

            else if (i == AllLines.Count - 1 && CompositeDone == true)
            //if the last drawn <Composite> line is marked as done, calculate and add last control points
            {
                addLastcPoints(i);
            }

            return;
        }
        
        private void btn_DoneComposite_Click(object sender, EventArgs e)
            //finish the <Composite> line being drawed - draw last segment of it
        {
            CompositeDone = true;
            pictureBox1.Invalidate();
        }

        private void addFirstcPoint(int i)
            //add first control point thats not a line point for <Composite> line with at least three line points
        {
            Point c1 = new Point();
            Point c2 = new Point();
            Point c3 = new Point();
            Point c4 = new Point();

            //control point location is calculated from first, third and fourth control points of the  <Composite> line

            c1 = pPointsAll[i][0];
            c3 = firstHandle(pPointsAll[i][0], pPointsAll[i][1], pPointsAll[i][2]);
            c4 = pPointsAll[i][1];

            //We can look at these calculations as vector operations. First we calculate dot product of vectors C4C3 and C1C4:
            double dot = (c3.X - c4.X) * (c1.X - c4.X) + (c3.Y - c4.Y) * (c1.Y - c4.Y);

            //We need to find how long the vector from C2 to C3 needs to be, so that the middle control points are symmetrical.
            //The symmetry can be achieved, if the C3C2 vector is parallel to C4C1 and has the length of 
            //length(C4C1) - 2*length(projection of vector C4C3 on to C4C1). One projection length for each side.
            //Using projection formula, we get: proportion = |C4C1| - 2 * dot / |C4C1| . We will multiply this proportion by unit vector
            //parallel to vector C4C1, which can be expressed as C4C1 / |C4C1|. If we devide our proportion with |C4C1| from the unit vector,
            //we get: proportion = 1 - 2 * dot / |C4C1|^2

            //That means, the length of the vector we will add equals 
            double prop = 1 - 2 * dot / (Math.Pow(length(c1, c4), 2));

            //Lastly, to point C3 we add vector parallel to C1C4 scaled by the needed length - variable "prop":
            c2.X = Convert.ToInt32(c3.X + prop * (c1.X - c4.X));
            c2.Y = Convert.ToInt32(c3.Y + prop * (c1.Y - c4.Y));

            //We have achieved a "symmetrical" point to third control point, both of these points are on the same side of the bezier line.

            cPointsAll[i].Add(c2);

            return;
        }

        private Point firstHandle(Point a, Point b, Point c)
            //Calculate first handle (first middle control point) coordinates for <Composite> lines, to ensure C2 continuity.
        {
            Point res = new Point();
            double AB = length(a, b);
            double BC = length(b, c);

            //Distance from first to second handle is half the distance from first given line point (a) to the last(c).
            //The proportions of each handle is the same as proportion ab/bc, where b is the middle line point.
            //Methods of calculation for distances can be different and there isn't one best method. 
            //I have discovered that this method works nice most of the time and isn't expesive.

            res.X = b.X + Convert.ToInt32(0.5 * (a.X - c.X) * AB / (AB + BC));
            res.Y = b.Y + Convert.ToInt32(0.5 * (a.Y - c.Y) * AB / (AB + BC));

            return res;
        }

        private Point secondHandle(Point a, Point b, Point c)
            //Calculate fsecond handle (second middle control point) coordinates for <Composite> lines, to ensure C2 continuity.
        {
            Point res = new Point();
            double AB = length(a, b);
            double BC = length(b, c);

            //Calculations are very similar to those in function firstHandle.

            res.X = b.X + Convert.ToInt32(0.5 * (c.X - a.X) * BC / (AB + BC));
            res.Y = b.Y + Convert.ToInt32(0.5 * (c.Y - a.Y) * BC / (AB + BC));

            return res;
        }
        
        private void addLastcPoints(int i)
            //add two last control points to a <Composite> line that''
        {
            Point c1 = new Point();
            Point c2 = new Point();
            Point c3 = new Point();
            Point c4 = new Point();

            //control point location is calculated from first, second and fourth control point of the last segment in the <Comoposite> line

            c1 = pPointsAll[i][pPointsAll[i].Count - 2];
            c2 = cPointsAll[i][cPointsAll[i].Count - 1];
            c4 = pPointsAll[i][pPointsAll[i].Count - 1];

            //We can look at these calculations as vector operations. First we calculate dot product of vectors C4C2 and C1C4:
            double dot = (c2.X - c4.X) * (c1.X - c4.X) + (c2.Y - c4.Y) * (c1.Y - c4.Y);

            //To find how long the vector from C2 to C3 needs to be, we find the the proportion:
            //(for more information on the calculation, see function addFirstcPoint();)
            double prop = 1 - 2 * dot / (Math.Pow(length(c1, c4), 2));

            //Lastly, to point C2 we add vector parallel to C1C4 scaled by the needed length - variable "prop":
            c3.X = Convert.ToInt32(prop * (c1.X - c4.X) + c2.X);
            c3.Y = Convert.ToInt32(prop * (c1.Y - c4.Y) + c2.Y);

            //We have achieved a "symmetrical" point to second control point, both of these points are on the same side of the bezier line.

            //We add the calculated point as wall as the last control point - the last line point:
            cPointsAll[i].Add(c3);
            cPointsAll[i].Add(c4);

            pictureBox1.Invalidate();
            return;
        }

        private void addOnlycPoints(int i)
            //add all control points to a <Composite> line thats marked as "done", but has only two line points
        {
            Point c1 = new Point();
            Point c2 = new Point();
            Point c3 = new Point();
            Point c4 = new Point();

            //the first and last control points are the line points:
            c1 = pPointsAll[i][0];
            c4 = pPointsAll[i][1];

            double sin60 = Math.Sin(Math.PI / 3);
            double cos60 = Math.Cos(Math.PI / 3);

            //Each control point will be line's C1C4 midpoint, rotated by 60 degrees. First we find the midpoint:
            double x03 = 0.5 * (c4.X - c1.X);
            double y03 = 0.5 * (c4.Y - c1.Y);

            //Then we rotate the midpoint by 60 degrees.
            c2.X = Convert.ToInt32(cos60 * x03 - sin60 * y03 + c1.X);
            c2.Y = Convert.ToInt32(sin60 * x03 + cos60 * y03 + c1.Y);

            //Change the signs for third control point, so the control points are on different sides of the bezier line:
            c3.X = Convert.ToInt32(cos60 * -x03 - sin60 * -y03 + c4.X);
            c3.Y = Convert.ToInt32(sin60 * -x03 + cos60 * -y03 + c4.Y);

            cPointsAll[i].Add(c1);
            cPointsAll[i].Add(c2);
            cPointsAll[i].Add(c3);
            cPointsAll[i].Add(c4);

            pictureBox1.Invalidate();
            return;
        }

        private List<Point> getPointsfromFile()
            //choose a .txt file and make a list of points from .it
        {
            List<Point> pointList = new List<Point>();
            Point point = new Point();
            
            string path = "";
            string textLine = "";

            OpenFileDialog theDialog = new OpenFileDialog();
            theDialog.Title = "Open Text File";
            theDialog.Filter = "TXT files|*.txt";
            theDialog.InitialDirectory = @"C:\";
            if (theDialog.ShowDialog() == DialogResult.OK)
            {
                path = theDialog.FileName;
            }

            if (File.Exists(path))
            {
                newLine(AddType);
                using (StreamReader file = new StreamReader(path))
                {
                    while ((textLine = file.ReadLine()) != null)
                    {
                        int index = textLine.IndexOf(' ');
                        string stringX = textLine.Substring(0, index);
                        string stringY = textLine.Substring(index + 1);

                        point.X = Convert.ToInt32(stringX);
                        point.Y = Convert.ToInt32(stringY);

                        pointList.Add(point);
                    }
                }
            }

            return pointList;
        }

        private double length(Point a, Point b)
            //get length between two points
        {
            return Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
        }

        private void findLocalPoint(List<List<Point>> PointsAll, Point MouseLocation)
            //find if there is a control or line point near mouse location
        {
            for (int i = 0; i < PointsAll.Count; i++)
            // go through every list of points in the given list
            {
                if (PointsAll[i] != null)
                {
                    for (int j = 0; j < PointsAll[i].Count; j++)
                    // go through every point in the list
                    {
                        if (length(MouseLocation, PointsAll[i][j]) < LocalRadius)
                        //mouse if in a neighborhood of some point, so we asume it this point was clicked
                        {
                            //ModifyType = AllLines[i];
                            LocalPoint = new Tuple<int, int>(i, j);
                        }
                    }
                }
            }

            return;
        }

        private void getcPoints(int i)
            //Calculate control points for lines, where only line points are know - <4 pPoints> and <Least Squares>.
        {
            List<Point> pList = pPointsAll[i];

            //This method of curve fitting uses least squares method, so that distance errors from given line points to the Bezier curve 
            // at respective t values is the smallest possible. For more calculation information see https://pomax.github.io/bezierinfo/#curvefitting.
            //or my documentation???? man ir uzrakstits latexa pieradijums sajai metodei, nezinu, cik vajadzigi tas seit ir.

            //We will represent Bezier curve in matrix form.

            //Matrix M contains coefficients in an expanded Bezier curve function. We will only use cubic Bezier curves, therefor M always is:
            var M = Matrix<double>.Build;
            double[,] matrixM = new double[4, 4]
                { { 1, 0, 0, 0 }, { -3, 3, 0, 0 }, { 3, -6, 3, 0 }, { -1, 3, -3, 1 } };

            //Matrix P contains coordinates of all line points:
            double[,] matrixP = new double[pList.Count, 2];
            for (int j = 0; j < pList.Count; j++)
            {
                matrixP[j, 0] = pList[j].X;
                matrixP[j, 1] = pList[j].Y;
            }

            var p = M.DenseOfArray(matrixP);
            var m4 = M.DenseOfArray(matrixM);
            var m4_inv = m4.Inverse();

            //Bezier curves are parametric, so we need appropriate t values for each line point to tie together coordinates with points o curve B(t). 
            //This parametrization can be done in different ways; we will store the resulting t values in a list sPoints.
            List<double> sPoints = new List<double>();

            if (Parametrization[i] == ParamType.uniform)
            {
                sPoints = sPointsUniform(pList);
            }

            else if (Parametrization[i] == ParamType.chord)
            {
                sPoints = sPointsChord(pList);
            }

            else if (Parametrization[i] == ParamType.centripental)
            {
                sPoints = sPointsCentripental(pList);
            }

            var s = M.DenseOfArray(sMatrix(sPoints));
            var s_tr = s.Transpose();
            var s_reiz = s_tr * s;
            var s_reiz_inv = s_reiz.Inverse();

            var r1 = m4_inv * s_reiz_inv;
            var r2 = r1 * s_tr;

            var c = r2 * p;

            if (cPointsAll[i] == null)
            //if we are not modifying a line, this is the first time calculating control points
            {
                cPoints = new List<Point>();

                for (int j = 0; j < 4; j++)
                {
                    Point tmp = new Point(Convert.ToInt32(c[j, 0]), Convert.ToInt32(c[j, 1]));
                    cPoints.Add(tmp);
                }
                cPointsAll[i] = cPoints;
            }

            else
            //else we need to replace the old control point coordinates by the new ones
            {
                for (int j = 0; j < 4; j++)
                {
                    Point tmp = new Point(Convert.ToInt32(c[j, 0]), Convert.ToInt32(c[j, 1]));
                    cPointsAll[i][j] = tmp;
                }
            }

            return;
        }

        private List<double> sPointsUniform(List<Point> pList)
            //a way of Bezier curve parametrization, where t values are equally spaced
        {
            List<double> sPoints = new List<double>();

            for (int i = 0; i < pList.Count; i++)
            {
                double s = (double)i / (pList.Count - 1);
                sPoints.Add(s);
            }
            return (sPoints);
        }

        private List<double> sPointsChord(List<Point> pList)
            //a way of Bezier curve parametrization, where t values are aligned with distance along the polygon
        {
            //At the first point, we're fixing t = 0, at the last point t = 1. Anywhere in between t value is equal to the distance
            //along the polygon (made from control points), scaled to the [0,1] domain.

            List<double> sPoints = new List<double>();
            List<double> dPoints = new List<double>();

            //First we calculate distance along the polygon for each point:
            dPoints.Add(0);
            for (int i = 1; i < pList.Count; i++)
            {
                double d = dPoints[i - 1] + length(pList[i - 1], pList[i]);
                dPoints.Add(d);
            }

            //Then we scale these values to [0, 1] domain:
            for (int i = 0; i < pList.Count; i++)
            {
                double s = dPoints[i] / dPoints[pList.Count - 1];
                sPoints.Add(s);
            }

            return (sPoints);
        }

        private List<double> sPointsCentripental(List<Point> pList)
            //a way of Bezier curve parametrization, where t values are aligned with square root of the distance along the polygon
        {
            //At the first point, we're fixing t = 0. Anywhere in between t value is equal to the square root of the 
            //distance along the polygon (made from control points), scaled to [0,1] domain.
            error.Text = "centr";

            List<double> sPoints = new List<double>();
            List<double> dPoints = new List<double>();

            //First we calculate the square root of distance along the polygon for each point:
            dPoints.Add(0);
            for (int i = 1; i < pList.Count; i++)
            {
                double d = dPoints[i - 1] + Math.Sqrt(length(pList[i - 1], pList[i]));
                dPoints.Add(d);
            }

            //Then we scale these values to [0, 1] domain:
            for (int i = 0; i < pList.Count; i++)
            {
                double s = dPoints[i] / dPoints[pList.Count - 1];
                sPoints.Add(s);
            }

            return (sPoints);
        }

        private double[,] sMatrix(List<double> sPoints)
            //Get and fill matrix S with calculated sPoints
        {
            //In our error function (see references), we need to substitute symbolic t values in matrix T with the sPoint values we computed:
            double[,] sMatrix = new double[sPoints.Count, 4];

            for (int i = 0; i < sPoints.Count; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    sMatrix[i, j] = Math.Pow(sPoints[i], j);
                }
            }
            return sMatrix;
        }
        
        private void btn_cPointsModify_Click(object sender, EventArgs e)
            //allow to drag existing control points by mouse
        {
            DeleteMode = false;
            AddType = BezierType.nothing; // to stop new point adding
            DragType = BezierType.cPoints;
        }

        private void btn_pPointsModify_Click(object sender, EventArgs e)
            //allow to drag existing line points by mouse
        {
            DeleteMode = false;
            AddType = BezierType.nothing; // to stop new point adding
            DragType = BezierType.pPoints;
        }

        private void changecPoint(int a, int b, int c)
            //When moving a control point of a <Composite> line with left mouse button, 
            //the opposite handle needs to moved as well to ensure C2 continuity
        {
            Point moving = new Point();// the point being dragged
            Point middle = new Point();// the middle line point
            Point change = new Point();// the opposite handle that needs to be moved

            moving = cPointsAll[LocalPoint.Item1][a];
            middle = cPointsAll[LocalPoint.Item1][b];
            change = cPointsAll[LocalPoint.Item1][c];

            if (middle == moving)
            //in segment no two control points should have the same location, it doesn't make mathematical sense and makes an error
            {
                middle.X++;
                middle.Y++;
            }

            //We can look at these calculations as vector operations. We want for vector middle-change to keep its length, 
            //but change its direction so it starts from middle point and is parallel to moving-middle vector.
            //To do that, we take unit vector from moving-middle (devide moving-middle with its length) and multiply that by 
            //middle-change length. Finally, we add that to middle point.

            double prop = length(middle, change) / length(moving, middle);

            change.X = Convert.ToInt32(middle.X + prop * (middle.X - moving.X));
            change.Y = Convert.ToInt32(middle.Y + prop * (middle.Y - moving.Y));

            cPointsAll[LocalPoint.Item1][c] = change;

            return;
        }

        private void changeStraight(Point toMove, Point middle, Point prev)
            //When moving a control point of a <Composite> line with right mouse button, it can be moved only in straight line away from the
            //middle point. This ensures C2 continuity and that no other control point moves.
        {
            Point res = new Point();

            //To move the control point in straight line, we take unit vector from the middle line point ("middle") to 
            //the place control point was before moving ("prev") for which we know it was on the needed line. Than we multiply
            //this unit vector by the distance mouse (toMove) is from middle point and add this vector to the middle point.

            double prop = length(middle, toMove) / length(prev, middle);

            res.X = Convert.ToInt32(middle.X + prop * (middle.X - prev.X));
            res.Y = Convert.ToInt32(middle.Y + prop * (middle.Y - prev.Y));

            cPointsAll[LocalPoint.Item1][LocalPoint.Item2] = res;

            return;
        }
        
        private void btn_ChangeParam_Click(object sender, EventArgs e)
            //enable option to change parametrization type for already drawn <Least Squares> and <4 pPoints> lines
        {
            ChangeParam = true;
            ChangingMode = false;
            DragType = BezierType.nothing;
            ModifyType = BezierType.nothing;
        }

        private void rbtn_Uniform_CheckedChanged(object sender, EventArgs e)
            //redraw line when parametrization type has been changed
        {
            if (ChangingMode == false)
            {
                return;
            }

            int i = LocalPoint.Item1;
            //ParamType paramType = Parametrization[i];

            if (rbtn_Uniform.Checked == true)
            {
                Parametrization[i] = ParamType.uniform;
            }

            else if (rbtn_Chord.Checked == true)
            {
                Parametrization[i] = ParamType.chord;
            }

            else if (rbtn_Centripental.Checked == true)
            {
                Parametrization[i] = ParamType.centripental;
            }

            getcPoints(i);
            pictureBox1.Invalidate();
        }

        private void rbtn_Chord_CheckedChanged(object sender, EventArgs e)
            //redraw line when parametrization type has been changed
        {
            rbtn_Uniform_CheckedChanged(sender, e);
        }

        private void btn_cPointsOutput_Click(object sender, EventArgs e)
            //enable option to output control point coordinates
        {
            OutputPointsType = BezierType.cPoints;
            AddType = BezierType.nothing;
            DragType = BezierType.nothing;
            ModifyType = BezierType.nothing;
        }

        private void btn_pPointsOutput_Click(object sender, EventArgs e)
            //enable option to output line point coordinates
        {
            OutputPointsType = BezierType.pPoints;
            AddType = BezierType.nothing;
            DragType = BezierType.nothing;
            ModifyType = BezierType.nothing;
        }
        
        private void OutputcPointsFile(int i)
            //output control points to .txt file
        {
            string pathFolder="";

            FolderBrowserDialog folder = new FolderBrowserDialog();
            if (folder.ShowDialog() == DialogResult.OK)
            {
                pathFolder = folder.SelectedPath;
            }

            string path = Path.Combine(pathFolder, "points.txt");
            using (var file = new StreamWriter(path, true))
            {
                file.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
                file.WriteLine("<" + AllLines[i] + "> line: \n");

                for (int j = 0; j < cPointsAll[i].Count; j++)
                {
                    string tmp = "C" + (j + 1) + ": (" + cPointsAll[i][j].X + "; " + cPointsAll[i][j].Y + ")";
                    file.WriteLine(tmp);
                }

                file.WriteLine("\n \n");
            }
        }

        private void OutputpPointsFile(int i)
            //output line points to .txt file
        {
            string pathFolder = "";

            FolderBrowserDialog folder = new FolderBrowserDialog();
            if (folder.ShowDialog() == DialogResult.OK)
            {
                pathFolder = folder.SelectedPath;
            }

            string path = Path.Combine(pathFolder, "points.txt");
            using (var file = new StreamWriter(path, true))
            {
                file.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
                file.WriteLine("<" + AllLines[i] + "> line: \n");

                for (int j = 0; j < pPointsAll[i].Count; j++)
                {
                    string tmp = "P" + (j + 1) + ": (" + pPointsAll[i][j].X + "; " + pPointsAll[i][j].Y + ")";
                    file.WriteLine(tmp);
                }

                file.WriteLine("\n \n");
            }
        }
        
        private void btn_Reset_Click(object sender, EventArgs e)
            //reset form to its inial state, clean pictureBox1 and reset all settings
        {
            AddType = BezierType.nothing;
            ModifyType = BezierType.nothing;
            DragType = BezierType.nothing;
            cPoints = null;
            cPointsAll = new List<List<Point>>();
            pPoints = null;
            pPointsAll = new List<List<Point>>();
            AllLines = new List<BezierType>();
            MovedLine = new List<MoveType>();
            Parametrization = new List<ParamType>();
            LocalPoint = null;
            rbtn_MouseAdd.Checked = true;
            rbtn_MouseModify.Checked = true;
            imageLocation = "";
            error.Text = "";
            cbox_ShowBackground.Checked = false;
            CompositeDone = false;
            ChangeParam = false;
            ChangingMode = false;
            DeleteMode = false;
            OutputPointsType = BezierType.nothing;

            pictureBox1.Invalidate();
        }
    }
}