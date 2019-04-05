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

        Tuple<int, int> MovingPoint = new Tuple<int, int>(0,0);

        private List<BezierType> AllLines = new List<BezierType>();

        String imageLocation = "";
        int PointRadius = 2;
        int LocalRadius = 7;


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

            pictureBox1.Invalidate();
        }

        private void btn_pPointsAdd_Click(object sender, EventArgs e)
        {
            DragType = BezierType.nothing;
            AddType = BezierType.pPoints;

            pictureBox1.Invalidate();
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
                }

                else if (cPoints.Count < 4 && cPoints[cPoints.Count - 1] != e.Location)
                {
                    cPoints.Add(e.Location);
                }

                if (cPoints.Count == 4)
                {
                    cPoints = null;
                    AddType = BezierType.nothing;
                }
            }

            if (AddType == BezierType.pPoints && rbtn_MouseAdd.Checked == true)
            {
                if (pPoints == null)
                {
                    AllLines.Add(BezierType.pPoints);
                    pPoints = new List<Point>();
                    pPointsAll.Add(pPoints);
                    pPoints.Add(e.Location);
                }

                else if (pPoints.Count < 4 && pPoints[pPoints.Count - 1] != e.Location)
                {
                    pPoints.Add(e.Location);
                }

                if (pPoints.Count == 4)
                {
                    getcPoints();
                    pPoints = null;
                    cPoints = null;
                    AddType = BezierType.nothing;
                }
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
                            MovingPoint = new Tuple<int, int>(i, j);
                        }
                    }
                }
            }

            if (pPointsAll != null && DragType == BezierType.pPoints)
            {
                for (int i = 0; i < pPointsAll.Count; i++)
                {
                    for (int j = 0; j < pPointsAll[i].Count; j++)
                    {
                        if (length(e.Location, pPointsAll[i][j]) < LocalRadius)
                        {
                            ModifyType = AllLines[i];
                            MovingPoint = new Tuple<int, int>(i, j);
                        }
                    }
                }
            }

            //pictureBox1.Invalidate();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (ModifyType == BezierType.cPoints)
            {
                cPointsAll[MovingPoint.Item1][MovingPoint.Item2] = e.Location;
            }

            if (ModifyType == BezierType.pPoints)
            {
                pPointsAll[MovingPoint.Item1][MovingPoint.Item2] = e.Location;
            }

            NewcPoint = e.Location;
            pictureBox1.Invalidate();
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (DragType == BezierType.cPoints)
            {
                cPointsAll[MovingPoint.Item1][MovingPoint.Item2] = e.Location;
                ModifyType = BezierType.nothing;
            }

            if (DragType == BezierType.pPoints)
            {
                pPointsAll[MovingPoint.Item1][MovingPoint.Item2] = e.Location;
                ModifyType = BezierType.nothing;
            }

            pictureBox1.Invalidate();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            if (cPointsAll != null)
            {
                if (cPointsAll.Count > 0)
                {
error.Text = "" + cPointsAll[0][0];
                }
                
                foreach (List<Point> cList in cPointsAll)
                {
                    foreach (Point c in cList)
                    {
                        e.Graphics.DrawEllipse(Pens.Red, c.X - PointRadius, c.Y - PointRadius, 2 * PointRadius, 2 * PointRadius);
                    }

                    if (cList.Count < 4)
                    {
                        using (Pen dashed_pen = new Pen(Color.LightGray))
                        {
                            dashed_pen.DashPattern = new float[] { 5, 5 };
                            e.Graphics.DrawLine(dashed_pen, cPoints[cPoints.Count - 1], NewcPoint);
                        }
                    }

                    if (cList.Count > 1)
                    {
                        e.Graphics.DrawLines(Pens.LightGray, cList.ToArray());
                    }

                    if (cList.Count == 4)
                    {
                        e.Graphics.DrawBezier(Pens.Black, cList[0], cList[1], cList[2], cList[3]);
                    }
                }
            }

            if (pPointsAll != null)
            {
                foreach (List<Point> pList in pPointsAll)
                {
                    foreach (Point p in pList)
                    {
                        e.Graphics.FillEllipse(Brushes.Black, p.X - PointRadius, p.Y - PointRadius, 2 * PointRadius, 2 * PointRadius);
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

        private void getcPoints()
        {
            var M = Matrix<double>.Build;

            double[,] matrixM = new double[4, 4]
                { { 1, 0, 0, 0 }, { -3, 3, 0, 0 }, { 3, -6, 3, 0 }, { -1, 3, -3, 1 } };

            double[,] matrixP = new double[pPoints.Count, 2];
            for (int i = 0; i < pPoints.Count; i++)
            {
                matrixP[i, 0] = pPoints[i].X;
                matrixP[i, 1] = pPoints[i].Y;
            }

            var p = M.DenseOfArray(matrixP);
            var m4 = M.DenseOfArray(matrixM);
            var m4_inv = m4.Inverse();

            List<double> sPoints = new List<double>();

            if (rbtn_Uniform.Checked == true)
            {
                sPoints = sPointsUniform();
            }

            else if (rbtn_Chord.Checked == true)
            {
                sPoints = sPointsChord();
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
                //error.Text = "mm";
                cPoints = new List<Point>();

                for (int i = 0; i < 4; i++)
                {
                    Point tmp = new Point(Convert.ToInt32(c[i, 0]), Convert.ToInt32(c[i, 1]));
                    cPoints.Add(tmp);
                }

                cPointsAll.Add(cPoints);
                AllLines.Add(BezierType.pPoints);
            }

            else
            {
                error.Text = "gg";
                for (int i = 0; i < 4; i++)
                {
                    Point tmp = new Point(Convert.ToInt32(c[i, 0]), Convert.ToInt32(c[i, 1]));
                    cPointsAll[MovingPoint.Item1][i] = tmp;
                }
            }
            
        }

        private List<double> sPointsUniform()
        {
            List<double> sPoints = new List<double>();

            for (int i = 0; i < pPoints.Count; i++)
            {
                double s = (double)i / (pPoints.Count - 1);
                sPoints.Add(s);
            }
            return (sPoints);
        }

        private List<double> sPointsChord()
        {
            List<double> sPoints = new List<double>();
            List<double> dPoints = new List<double>();
            dPoints.Add(0);

            for (int i = 1; i < pPoints.Count; i++)
            {
                double d = dPoints[i - 1] + length(pPoints[i - 1], pPoints[i]);
                dPoints.Add(d);
            }

            for (int i = 0; i < pPoints.Count; i++)
            {
                double s = dPoints[i] / dPoints[pPoints.Count - 1];
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
            if (AddType != BezierType.nothing)
            {
                return;
            }

            DragType = BezierType.cPoints;
            pictureBox1.Invalidate();
        }

        private void btn_pPointsModify_Click(object sender, EventArgs e)
        {
            if (AddType != BezierType.nothing)
            {
                return;
            }

            DragType = BezierType.pPoints;
            //pictureBox1.Invalidate();
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
            rbtn_MouseAdd.Checked = true;
            rbtn_MouseModify.Checked = true;
            imageLocation = "";
            cbox_ShowBackground.Checked = false;

            pictureBox1.Invalidate();
        }
    }
}
