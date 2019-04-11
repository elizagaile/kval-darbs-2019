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

        private Point NewcPoint;

        private List<Point> cPoints = null;
        private List<List<Point>> cPointsAll = new List<List<Point>>();

        private List<Point> pPoints = null;
        private List<List<Point>> pPointsAll = new List<List<Point>>();

        enum BezierType { cPoints, pPoints, LastSquares, Composite, nothing };
        BezierType AddType = BezierType.nothing;
        BezierType ModifyType = BezierType.nothing;
        BezierType DragType = BezierType.nothing;

        Tuple<int, int> MovingPoint = null;

        private List<BezierType> AllLines = new List<BezierType>();

        enum MoveType { leftClick, rightClick, nothing };
        private List<MoveType> MovedLine = new List<MoveType>();

        String imageLocation = "";
        int PointRadius = 2;
        int LocalRadius = 7;
        int maxPointCount = 15;

        bool CompositeDone = false;

        private void btnBackground_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "jpg files(.*jpg)|*.jpg| PNG files(.*png)|*.png| All Files(*.*)|*.*";

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
        {
            DragType = BezierType.nothing;
            AddType = BezierType.cPoints;
            ModifyType = BezierType.nothing;
            MovingPoint = null;
            cPoints = null;
        }

        private void btn_pPointsAdd_Click(object sender, EventArgs e)
        {
            DragType = BezierType.nothing;
            AddType = BezierType.pPoints;
            ModifyType = BezierType.nothing;
            MovingPoint = null;
            pPoints = null;//?
            cPoints = null;//?
        }

        private void btn_LeastSquaresAdd_Click(object sender, EventArgs e)
        {
            DragType = BezierType.nothing;
            AddType = BezierType.LastSquares;
            ModifyType = BezierType.nothing;
            MovingPoint = null;
            pPoints = null;
            cPoints = null;
        }

        private void btn_CompositeAdd_Click(object sender, EventArgs e)
        {
            DragType = BezierType.nothing;
            AddType = BezierType.Composite;
            ModifyType = BezierType.nothing;
            MovingPoint = null;
            pPoints = null;
            cPoints = null;
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (AddType == BezierType.cPoints && rbtn_MouseAdd.Checked == true)
            {
                if (cPoints == null)
                {
                    AllLines.Add(BezierType.cPoints);
                    cPoints = new List<Point>();
                    cPointsAll.Add(cPoints);
                    cPoints.Add(e.Location);

                    pPointsAll.Add(null);
                    MovedLine.Add(MoveType.nothing);
                }

                else if (cPoints.Count < 4 && cPoints[cPoints.Count - 1] != e.Location)
                {
                    cPoints.Add(e.Location);
                }

                pictureBox1.Invalidate();
            }

            if (AddType == BezierType.pPoints && rbtn_MouseAdd.Checked == true)
            {

                if (pPoints == null)
                {
                    AllLines.Add(BezierType.pPoints);
                    pPoints = new List<Point>();
                    pPointsAll.Add(pPoints);
                    pPoints.Add(e.Location);
                    cPointsAll.Add(null);
                    MovedLine.Add(MoveType.nothing);
                }

                else if (pPoints.Count < 4 && pPoints[pPoints.Count - 1] != e.Location)
                {
                    pPoints.Add(e.Location);
                }

                pictureBox1.Invalidate();
            }

            if (AddType == BezierType.LastSquares && rbtn_MouseAdd.Checked == true)
            {
                if (pPoints == null)
                {
                    AllLines.Add(BezierType.LastSquares);
                    pPoints = new List<Point>();
                    pPointsAll.Add(pPoints);
                    pPoints.Add(e.Location);
                    cPointsAll.Add(null);
                    MovedLine.Add(MoveType.nothing);
                }

                else if (pPoints.Count < maxPointCount && pPoints[pPoints.Count - 1] != e.Location)
                {
                    pPoints.Add(e.Location);
                }

                pictureBox1.Invalidate();
            }

            if (AddType == BezierType.Composite && rbtn_MouseAdd.Checked == true)
            {
                CompositeDone = false;

                if (pPoints == null)
                {
                    AllLines.Add(BezierType.Composite);
                    pPoints = new List<Point>();
                    pPointsAll.Add(pPoints);
                    pPoints.Add(e.Location);
                    cPointsAll.Add(null);
                    MovedLine.Add(MoveType.nothing);
                }

                else if (pPoints.Count < maxPointCount && pPoints[pPoints.Count - 1] != e.Location && pPoints.Count <= maxPointCount)
                {
                    pPoints.Add(e.Location);
                }

                pictureBox1.Invalidate();
            }

            if (cPointsAll != null && DragType == BezierType.cPoints)
            {
                for (int i = 0; i < cPointsAll.Count; i++)
                {
                    for (int j = 0; j < cPointsAll[i].Count; j++)
                    {
                        if (length(e.Location, cPointsAll[i][j]) < LocalRadius)
                        {
                            ModifyType = AllLines[i];
                            if (ModifyType == BezierType.pPoints)
                            {
                                MessageBox.Show("It's not allowed to move curve's <4 pPoints> control points!", "Error");
                                ModifyType = BezierType.nothing;
                            }

                            else if (ModifyType == BezierType.LastSquares)
                            {
                                MessageBox.Show("It's not allowed to move curve's <Least Squares> control points!", "Error");
                                ModifyType = BezierType.nothing;
                            }

                            else if (ModifyType == BezierType.Composite && j % 3 != 0 )
                            {
                                MovingPoint = new Tuple<int, int>(i, j);

                                if (e.Button == MouseButtons.Left)
                                {
                                    MovedLine[i] = MoveType.leftClick;
                                }

                                else if (e.Button == MouseButtons.Right)
                                {
                                    MovedLine[i] = MoveType.rightClick;
                                }
                            }

                            else if (ModifyType == BezierType.cPoints )
                            {
                                MovingPoint = new Tuple<int, int>(i, j);
                            }
                        }
                    }
                }

                pictureBox1.Invalidate();
            }

            if (pPointsAll != null && DragType == BezierType.pPoints)
            {
                for (int i = 0; i < pPointsAll.Count; i++)
                {
                    if (pPointsAll[i] != null)
                    {
                        for (int j = 0; j < pPointsAll[i].Count; j++)
                        {
                            if (length(e.Location, pPointsAll[i][j]) < LocalRadius)
                            {
                                ModifyType = AllLines[i];
                                if (ModifyType != BezierType.Composite)
                                {
                                    MovingPoint = new Tuple<int, int>(i, j);
                                }

                                error.Text = "ya";

                                if (ModifyType == BezierType.Composite)
                                {
                                    MessageBox.Show("It's not allowed to move curve's <Composite> points!", "Error");
                                    ModifyType = BezierType.nothing;
                                }
                            }
                        }
                    }
                }
                pictureBox1.Invalidate();
            }
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

            if (ModifyType == BezierType.Composite && DragType == BezierType.cPoints)
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

            if (ModifyType == BezierType.pPoints || ModifyType == BezierType.LastSquares)
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

                        if (AddType == BezierType.LastSquares && pPointsAll[i].Count >= 4 && pPointsAll[i].Count <= maxPointCount)
                        {
                            getcPoints(i);
                        }

                        if (AllLines[i] == BezierType.Composite && pPointsAll[i].Count >= 3 && MovedLine[i] == MoveType.nothing)
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

                        if (AllLines[i] == BezierType.Composite && CompositeDone == true && pPointsAll[i].Count == 2 && MovedLine[i] == MoveType.nothing)
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

                        else if (AllLines[i] != BezierType.Composite)
                        {
                            foreach (Point c in cPointsAll[i])
                            {
                                e.Graphics.DrawEllipse(Pens.Red, c.X - PointRadius, c.Y - PointRadius, 2 * PointRadius, 2 * PointRadius);
                            }
                        }

                        if (AllLines[i] != BezierType.Composite && cPointsAll[i].Count > 1)
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
            cbox_ShowBackground.Checked = false;
            CompositeDone = true;

            pictureBox1.Invalidate();
        }

        private void btn_DoneComposite_Click(object sender, EventArgs e)
        {
            CompositeDone = true;
            pictureBox1.Invalidate();
        }
    }
}
