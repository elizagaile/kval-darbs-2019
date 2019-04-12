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

namespace BezierTool
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private Point NewcPoint; //new point for line <4 cPoints>

        //each line is representet by two lists: 
        //list of control points, list of known points on the line 
        //all lines have control points, but come lines (<4cPoints>) dont have known points on the line
        //also each line has a drawing type (saved in list AllLines) and a parametrization type 
        //the i-th drawn list is represented by i-th position in all of the lists

        private List<Point> cPoints = null;//list of line's control points
        private List<List<Point>> cPointsAll = new List<List<Point>>();//contains all lists of control points

        private List<Point> pPoints = null;// list of points on line
        private List<List<Point>> pPointsAll = new List<List<Point>>();// contains all lists of points on line

        enum BezierType { cPoints, pPoints, leastSquares, composite, nothing }; //all posible line types

        private List<BezierType> AllLines = new List<BezierType>();// contains type of drawn lines

        BezierType AddType = BezierType.nothing;// type of line to be or being added
        BezierType ModifyType = BezierType.nothing;// type of line to be or being modified
        BezierType DragType = BezierType.nothing;// type of line to be or being modified
        BezierType OutputPointsType = BezierType.nothing;// type of line to be or being draged by mouse

        Tuple<int, int> MovingPoint = null;// the moving point's location in the lines representitive lists (cPointsAll; pPointsAll)
        
        enum MoveType { leftClick, rightClick, nothing };//ways to move points by mouse
        private List<MoveType> MovedLine = new List<MoveType>();//contains a way a list has been moved

        enum ParamType { uniform, chord }; // parametrization ways
        private List<ParamType> Parametrization = new List<ParamType>();//contains parametrization types for drawn lines

        String imageLocation = ""; //for background image
        int PointRadius = 2; //radius for control points and specific points on lines, chosen arbitrary
        int LocalRadius = 7; //radius of neiborghood, used when selecting a point with mouse, chosen arbitrary
        int maxPointCount = 15; //maximum count of points to choose for lines <Least Squares> and <Composite>, chosen arbitrary

        bool CompositeDone = false;//indicates, if the last line of type <Composite> needs to be finished;

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

        private void btn_cPointsAdd_Click(object sender, EventArgs e)
            //start a new line of type <4 cPoints>
        {
            newLine(BezierType.cPoints);
        }

        private void btn_pPointsAdd_Click(object sender, EventArgs e)
            //start a new line of type <4 pPoints>
        {
            newLine(BezierType.pPoints);
        }

        private void btn_LeastSquaresAdd_Click(object sender, EventArgs e)
            //start a new line of type <4 pPoints>
        {
            newLine(BezierType.leastSquares);
        }

        private void btn_CompositeAdd_Click(object sender, EventArgs e)
            //start a new line of type <Composite>
        {
            newLine(BezierType.composite);
        }

        private void newLine(BezierType type)
            //start a new line
        {
            AddType = type;

            cPoints = null; //deletes previous list of control points
            pPoints = null; //deletes previous list of known points on line
            DragType = BezierType.nothing;//stops point's dragging by mouse if it was active
            ModifyType = BezierType.nothing;//indicates a point will not be moved
            MovingPoint = null;// indicates no point is selected for moving
        }

        private void AddcPoint(BezierType type, Point MouseLocation)
            //add new control point to the current line
        {
            if (cPoints == null)
            //if this is the first point of line
            {
                AllLines.Add(type);
                cPoints = new List<Point>();
                cPointsAll.Add(cPoints);
                cPoints.Add(MouseLocation);

                pPointsAll.Add(null);//adding empty list of pPoints, as <4 cPoints> won't have any, to keep correct counting
                MovedLine.Add(MoveType.nothing);
            }

            else if (cPoints.Count < 4 && cPoints[cPoints.Count - 1] != MouseLocation)
            //to avoid accidental double clicks
            {
                cPoints.Add(MouseLocation);
            }
        }

        private void AddpPoint(BezierType type, Point MouseLocation)
            //add new point on line to the current line
        {
            if (pPoints == null)
            //if this is the first point of line
            {
                AllLines.Add(type);
                pPoints = new List<Point>();
                pPointsAll.Add(pPoints);
                pPoints.Add(MouseLocation);
                cPointsAll.Add(null);
                MovedLine.Add(MoveType.nothing);

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

            else if ( (type == BezierType.leastSquares || type == BezierType.composite) && pPoints.Count > maxPointCount )
            // can't choose more points than the maximum allowed count
            {
                return;
            }

            pPoints.Add(MouseLocation);

        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
            //Mouse has been pressed inside picturebox. This can be for adding control points or points on the line, for moving points with mouse
            //or for selecting a line to output its points coordinates
        {

            if (AddType != BezierType.nothing && rbtn_MouseAdd.Checked == true)
            //if we want to add a new point by mouse            
            {
                if (AddType == BezierType.cPoints)
                //if we want to add a new point for a <4 cPoints> type line
                {
                    AddcPoint(BezierType.cPoints, e.Location);
                }

                else
                //if we want to add a new point by mouse for a <4 pPoints>, <Least Squares> or <Composite> type line
                {
                    AddpPoint(AddType, e.Location);
                }

                pictureBox1.Invalidate();
            }

            

            if (cPointsAll != null && DragType == BezierType.cPoints && rbtn_MouseModify.Checked == true )
            //if we want to drag a control point
            {
                findLocalPoint(cPointsAll, e.Location);

                if (MovingPoint != null)
                {
                    int i = MovingPoint.Item1;
                    int j = MovingPoint.Item2;

                    if (ModifyType == BezierType.pPoints || ModifyType == BezierType.leastSquares)
                    {
                        error.Text = "It's not allowed to move curve's " + ModifyType + " points!";
                        ModifyType = BezierType.nothing;
                        MovingPoint = null;
                    }

                    else if (ModifyType == BezierType.composite)
                    // every third control point on a composite line is one of the line points (pPoint) 
                    {
                        if (j % 3 == 0)
                        {
                            error.Text = "It's not allowed to move curve's " + ModifyType + " points!";
                            MovingPoint = null;
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
                }

                pictureBox1.Invalidate();

            }


            if (pPointsAll != null && DragType == BezierType.pPoints && rbtn_MouseModify.Checked == true)
            //if we want to drag a line point
            {
                findLocalPoint(pPointsAll, e.Location);
                
                if (MovingPoint != null)
                {
                    if (ModifyType == BezierType.composite)
                    {
                        error.Text = "It's not allowed to move curve's <Composite> line points!";
                        ModifyType = BezierType.nothing;
                        MovingPoint = null;
                    }
                }

                pictureBox1.Invalidate();
            }


            if (cPointsAll != null && OutputPointsType == BezierType.cPoints && rbtn_ScreenOutput.Checked == true)
            {
                findLocalPoint(cPointsAll, e.Location);

                if (MovingPoint != null)
                {
                    int i = MovingPoint.Item1;

                    OutputcPointsScreen(i);
                    OutputPointsType = BezierType.nothing;
                    ModifyType = BezierType.nothing;
                    MovingPoint = null;

                }

                pictureBox1.Invalidate();
            }

            if (pPointsAll != null && OutputPointsType == BezierType.pPoints && rbtn_ScreenOutput.Checked == true)
            {
                findLocalPoint(cPointsAll, e.Location);
                
                if (MovingPoint != null)
                {
                    int i = MovingPoint.Item1;

                    OutputpPointsScreen(i);
                    OutputPointsType = BezierType.nothing;
                    ModifyType = BezierType.nothing;
                    MovingPoint = null;

                }

                pictureBox1.Invalidate();
            }
        }

        private void findLocalPoint(List<List<Point>> PointsAll, Point MouseLocation)
        {
            for (int i = 0; i < PointsAll.Count; i++)
            {
                if (PointsAll[i] != null)
                {
                    for (int j = 0; j < PointsAll[i].Count; j++)
                    {
                        if ( length(MouseLocation, PointsAll[i][j]) < LocalRadius )
                        {
                            ModifyType = AllLines[i];
                            MovingPoint = new Tuple<int, int>(i, j);
                        }
                    }
                }
            }

            return;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            int i = 0;
            int j = 0;

            if (MovingPoint != null)
            {
                i = MovingPoint.Item1;
                j = MovingPoint.Item2;
            }

            if (ModifyType == BezierType.cPoints)
            {
                cPointsAll[i][j] = e.Location;
                pictureBox1.Invalidate();
            }

            if (ModifyType == BezierType.composite && DragType == BezierType.cPoints)
            {
                if (MovedLine[i] == MoveType.leftClick)
                {
                    cPointsAll[i][j] = e.Location;;

                    if (j % 3 == 1 && j != 1)
                    {
                        changecPoint(j, j - 1, j - 2);
                    }

                    if (j % 3 == 2 && j != cPointsAll[i].Count - 2)
                    {
                        changecPoint(j, j + 1, j + 2);
                    }
                }

                if (MovedLine[i] == MoveType.rightClick)
                {
                    if (j % 3 == 1 && j != 1)
                    {
                        changeStraight(e.Location, cPointsAll[i][j - 1], cPointsAll[i][j - 2]);
                    }

                    if (j % 3 == 2 && j != cPointsAll[i].Count - 2)
                    {
                        changeStraight(e.Location, cPointsAll[i][j + 1], cPointsAll[i][j + 2]);
                    }
                }
                pictureBox1.Invalidate();
            }

            if (ModifyType == BezierType.pPoints || ModifyType == BezierType.leastSquares)
            {
                pPointsAll[i][j] = e.Location;
                getcPoints(i);
                pictureBox1.Invalidate();
            }

            if (AddType == BezierType.cPoints)
            {
                NewcPoint = e.Location;
                pictureBox1.Invalidate();
            }
            
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (DragType != BezierType.nothing)
            {
                ModifyType = BezierType.nothing;
            }
            pictureBox1.Invalidate();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            if (pPointsAll != null)
            {
                for (int i = 0; i< pPointsAll.Count; i++)
                {
                    if (pPointsAll[i] != null)
                    {
                        foreach (Point p in pPointsAll[i])
                        {
                            e.Graphics.FillEllipse(Brushes.Black, p.X - PointRadius, p.Y - PointRadius, 2 * PointRadius, 2 * PointRadius);
                        }

                        if (AddType == BezierType.pPoints && pPointsAll[i].Count == 4 && cPointsAll[i] == null)
                        {
                            getcPoints(i);
                        }

                        if (AddType == BezierType.leastSquares && pPointsAll[i].Count >= 4 && pPointsAll[i].Count <= maxPointCount)
                        {
                            getcPoints(i);
                        }

                        if (AllLines[i] == BezierType.composite && pPointsAll[i].Count >= 3 && MovedLine[i] == MoveType.nothing)
                        {
                            cPointsAll[i] = new List<Point>();
                            cPointsAll[i].Add(pPointsAll[i][0]);
                            addFirstcPoint(i);

                            for (int j = 0; j < pPointsAll[i].Count - 2; j++)
                            {
                                cPointsAll[i].Add(firstHandle(pPointsAll[i][j], pPointsAll[i][j + 1], pPointsAll[i][j + 2]));
                                cPointsAll[i].Add(pPointsAll[i][j + 1]);
                                cPointsAll[i].Add(secondHandle(pPointsAll[i][j], pPointsAll[i][j + 1], pPointsAll[i][j + 2]));
                            }

                            if (cPointsAll[i].Count < pPointsAll[i].Count * 3 - 2 && i != AllLines.Count - 1)
                            {
                                addLastcPoints(i);
                            }

                            if (CompositeDone == true && i == AllLines.Count - 1)
                            {
                                addLastcPoints(i);
                            }
                        }

                        if (AllLines[i] == BezierType.composite && CompositeDone == true && pPointsAll[i].Count == 2 && MovedLine[i] == MoveType.nothing)
                        {
                            cPointsAll[i] = new List<Point>();
                            addOnlycPoints(i);
                        }
                    }
                }
            }

            if (cPointsAll != null)
            {
                if (cPoints != null)
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
                
                for (int i = 0; i < cPointsAll.Count; i++)
                {
                    if (cPointsAll[i] != null)
                    {
                        if (AllLines[i] == BezierType.pPoints)
                        {
                            e.Graphics.DrawEllipse(Pens.Red, cPointsAll[i][1].X - PointRadius, cPointsAll[i][1].Y - PointRadius, 2 * PointRadius, 2 * PointRadius);
                            e.Graphics.DrawEllipse(Pens.Red, cPointsAll[i][2].X - PointRadius, cPointsAll[i][2].Y - PointRadius, 2 * PointRadius, 2 * PointRadius);
                        }

                        else if (AllLines[i] != BezierType.composite)
                        {
                            foreach (Point c in cPointsAll[i])
                            {
                                e.Graphics.DrawEllipse(Pens.Red, c.X - PointRadius, c.Y - PointRadius, 2 * PointRadius, 2 * PointRadius);
                            }
                        }

                        if (AllLines[i] != BezierType.composite && cPointsAll[i].Count > 1)
                        {
                            e.Graphics.DrawLines(Pens.LightGray, cPointsAll[i].ToArray());

                            if (cPointsAll[i].Count == 4)
                            {
                                e.Graphics.DrawBezier(Pens.Black, cPointsAll[i][0], cPointsAll[i][1], cPointsAll[i][2], cPointsAll[i][3]);
                            }
                        }

                        else
                        {
                            for (int j = 0; j < cPointsAll[i].Count - 1; j++)
                            {
                                if (j % 3 != 2)
                                {
                                    e.Graphics.DrawEllipse(Pens.Red, cPointsAll[i][j + 1].X - PointRadius, cPointsAll[i][j + 1].Y - PointRadius, 2 * PointRadius, 2 * PointRadius);
                                }

                                e.Graphics.DrawLine(Pens.LightGray, cPointsAll[i][j], cPointsAll[i][j + 1]);
                            }

                            for (int j = 0; j < cPointsAll[i].Count - 3; j += 3)
                            {
                                e.Graphics.DrawBezier(Pens.Black, cPointsAll[i][j], cPointsAll[i][j + 1], cPointsAll[i][j + 2], cPointsAll[i][j + 3]);
                            }
                        }
                    }
                }
            }

            
        }

        private void cbox_ShowBackground_CheckStateChanged(object sender, EventArgs e)
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

        private double length(Point a, Point b)
        {
            return Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
        }

        private void addFirstcPoint(int i)
        {
            Point c0 = new Point();
            Point c1 = new Point();
            Point c2 = new Point();
            Point c3 = new Point();

            c0 = pPointsAll[i][0];
            c2 = firstHandle(pPointsAll[i][0], pPointsAll[i][1], pPointsAll[i][2]);
            c3 = pPointsAll[i][1];

            double tmp = (c2.X - c3.X) * (c0.X - c3.X) + (c2.Y - c3.Y) * (c0.Y - c3.Y);
            double prop = 1 - 2 * tmp / (Math.Pow(length(c0, c3), 2));

            c1.X = Convert.ToInt32(prop * (c0.X - c3.X) + c2.X);
            c1.Y = Convert.ToInt32(prop * (c0.Y - c3.Y) + c2.Y);

            cPointsAll[i].Add(c1);

            return;
        }

        private void addLastcPoints(int i)
        {
            Point c0 = new Point();
            Point c1 = new Point();
            Point c2 = new Point();
            Point c3 = new Point();

            c0 = pPointsAll[i][pPointsAll[i].Count - 2];
            c1 = cPointsAll[i][cPointsAll[i].Count - 1];
            c3 = pPointsAll[i][pPointsAll[i].Count - 1];

            double tmp = (c1.X - c3.X) * (c0.X - c3.X) + (c1.Y - c3.Y) * (c0.Y - c3.Y);
            double prop = 1 - 2 * tmp / (Math.Pow(length(c0, c3), 2));

            c2.X = Convert.ToInt32(prop * (c0.X - c3.X) + c1.X);
            c2.Y = Convert.ToInt32(prop * (c0.Y - c3.Y) + c1.Y);

            cPointsAll[i].Add(c2);
            cPointsAll[i].Add(c3);

            pictureBox1.Invalidate();
        }

        private void addOnlycPoints(int i)
        {
            Point c0 = new Point();
            Point c1 = new Point();
            Point c2 = new Point();
            Point c3 = new Point();

            c0 = pPointsAll[i][0];
            c3 = pPointsAll[i][1];

            double sin60 = Math.Sin(Math.PI / 3);
            double cos60 = Math.Cos(Math.PI / 3);

            double x03 = 0.5 * (c3.X - c0.X);
            double y03 = 0.5 * (c3.Y - c0.Y);

            c1.X = Convert.ToInt32(cos60 * x03 - sin60 * y03 + c0.X);
            c1.Y = Convert.ToInt32(sin60 * x03 + cos60 * y03 + c0.Y);

            c2.X = Convert.ToInt32(cos60 * -x03 - sin60 * -y03 + c3.X);
            c2.Y = Convert.ToInt32(sin60 * -x03 + cos60 * -y03 + c3.Y);

            cPointsAll[i].Add(c0);
            cPointsAll[i].Add(c1);
            cPointsAll[i].Add(c2);
            cPointsAll[i].Add(c3);

            pictureBox1.Invalidate();
        }

        private void changecPoint(int a, int b, int c)
        {
            Point moving = new Point();
            Point middle = new Point();
            Point change = new Point();

            moving = cPointsAll[MovingPoint.Item1][a];
            middle = cPointsAll[MovingPoint.Item1][b];
            change = cPointsAll[MovingPoint.Item1][c];

            if (middle == moving)
            {
                middle.X++;
                middle.Y++;
            }

            double prop = length(middle, change) / length(moving, middle);

            change.X = Convert.ToInt32(middle.X + prop * (middle.X - moving.X));
            change.Y = Convert.ToInt32(middle.Y + prop * (middle.Y - moving.Y));

            cPointsAll[MovingPoint.Item1][c] = change;
        }

        private void changeStraight(Point toMove, Point middle, Point prev)
        {
            Point res = new Point();

            double prop = length(middle, toMove) / length(prev, middle);

            res.X = Convert.ToInt32(middle.X + prop * (middle.X - prev.X));
            res.Y = Convert.ToInt32(middle.Y + prop * (middle.Y - prev.Y));

            cPointsAll[MovingPoint.Item1][MovingPoint.Item2] = res;
        }

        private Point firstHandle(Point a, Point b, Point c)
        {
            Point res = new Point();
            double AB = length(a, b);
            double BC = length(b, c);

            res.X = b.X + Convert.ToInt32(0.5 * (a.X - c.X) * AB / (AB + BC));
            res.Y = b.Y + Convert.ToInt32(0.5 * (a.Y - c.Y) * AB / (AB + BC));

            return res;
        }

        private Point secondHandle(Point a, Point b, Point c)
        {
            Point res = new Point();
            double AB = length(a, b);
            double BC = length(b, c);

            res.X = b.X + Convert.ToInt32(0.5 * (c.X - a.X) * BC / (AB + BC));
            res.Y = b.Y + Convert.ToInt32(0.5 * (c.Y - a.Y) * BC / (AB + BC));

            return res;
        }

        private void getcPoints(int i)
        {
            List<Point> pList = pPointsAll[i];

            var M = Matrix<double>.Build;

            double[,] matrixM = new double[4, 4]
                { { 1, 0, 0, 0 }, { -3, 3, 0, 0 }, { 3, -6, 3, 0 }, { -1, 3, -3, 1 } };

            double[,] matrixP = new double[pList.Count, 2];
            for (int j = 0; j < pList.Count; j++)
            {
                matrixP[j, 0] = pList[j].X;
                matrixP[j, 1] = pList[j].Y;
            }

            var p = M.DenseOfArray(matrixP);
            var m4 = M.DenseOfArray(matrixM);
            var m4_inv = m4.Inverse();

            List<double> sPoints = new List<double>();

            if (rbtn_Uniform.Checked == true)
            {
                sPoints = sPointsUniform(pList);
            }

            else if (rbtn_Chord.Checked == true)
            {
                sPoints = sPointsChord(pList);
            }

            var s = M.DenseOfArray(sMatrix(sPoints));
            var s_tr = s.Transpose();
            var s_reiz = s_tr * s;
            var s_reiz_inv = s_reiz.Inverse();

            var r1 = m4_inv * s_reiz_inv;
            var r2 = r1 * s_tr;

            var c = r2 * p;
            
            if (MovingPoint == null)
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
            {
                for (int j = 0; j < 4; j++)
                {
                    Point tmp = new Point(Convert.ToInt32(c[j, 0]), Convert.ToInt32(c[j, 1]));
                    cPointsAll[i][j] = tmp;
                }
            }
        }

        private List<double> sPointsUniform(List<Point> pList)
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
        {
            List<double> sPoints = new List<double>();
            List<double> dPoints = new List<double>();
            dPoints.Add(0);

            for (int i = 1; i < pList.Count; i++)
            {
                double d = dPoints[i - 1] + length(pList[i - 1], pList[i]);
                dPoints.Add(d);
            }

            for (int i = 0; i < pList.Count; i++)
            {
                double s = dPoints[i] / dPoints[pList.Count - 1];
                sPoints.Add(s);
            }

            return (sPoints);
        }

        private double[,] sMatrix(List<double> sPoints)
        {
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
        {
            DragType = BezierType.cPoints;
            AddType = BezierType.nothing;

        }

        private void btn_pPointsModify_Click(object sender, EventArgs e)
        {
            DragType = BezierType.pPoints;
            AddType = BezierType.nothing;
        }

        private void btn_DoneModify_Click(object sender, EventArgs e)
        {
            ModifyType = BezierType.nothing;
            DragType = BezierType.nothing;
        }

        private void btn_Reset_Click(object sender, EventArgs e)
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
            MovingPoint = null;
            rbtn_MouseAdd.Checked = true;
            rbtn_MouseModify.Checked = true;
            imageLocation = "";
            error.Text = "";
            listBox_ScreenOutput.DataSource = null;
            cbox_ShowBackground.Checked = false;
            CompositeDone = true;
            OutputPointsType = BezierType.nothing;

            pictureBox1.Invalidate();
        }

        private void btn_DoneComposite_Click(object sender, EventArgs e)
        {
            CompositeDone = true;
            pictureBox1.Invalidate();
        }

        private void btn_cPointsOutput_Click(object sender, EventArgs e)
        {
            OutputPointsType = BezierType.cPoints;
            AddType = BezierType.nothing;
            DragType = BezierType.nothing;
            ModifyType = BezierType.nothing;
        }

        private void btn_pPointsOutput_Click(object sender, EventArgs e)
        {
            OutputPointsType = BezierType.pPoints;
            AddType = BezierType.nothing;
            DragType = BezierType.nothing;
            ModifyType = BezierType.nothing;
        }

        private void OutputcPointsScreen( int i)
        {
            List <string> points = new List<string>();
            for (int j = 0; j < cPointsAll[i].Count; j++)
            {
                string tmp = "C" + (j + 1) + " : " + cPointsAll[i][j] + "\n";
                points.Add(tmp);
            }
            listBox_ScreenOutput.DataSource = points;
        }

        private void OutputpPointsScreen(int i)
        {
            List<string> points = new List<string>();
            for (int j = 0; j < pPointsAll[i].Count; j++)
            {
                string tmp = "P" + (j + 1) + " : " + pPointsAll[i][j] + "\n";
                points.Add(tmp);
            }
            listBox_ScreenOutput.DataSource = points;
        }

        private void btn_ResetScreenOutput_Click(object sender, EventArgs e)
        {
            listBox_ScreenOutput.DataSource = null;
            pictureBox1.Invalidate();
        }
    }
}
