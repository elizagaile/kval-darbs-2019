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

namespace compositeBezier
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private List<Point> pPoints = null;
        private List<Point> cPoints = null;
        private List<double> sPoints = null;

        private Point NewPoint;

        private int param = 1;

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (pPoints != null && pPoints.Count < 7)
            {
                // Add a point to this polygon.
                if (pPoints[pPoints.Count - 1] != e.Location)
                {
                    pPoints.Add(e.Location);
                }
            }

            else
            {
                // Start a new polygon.
                pPoints = new List<Point>();
                NewPoint = e.Location;
                pPoints.Add(e.Location);
            }

            pictureBox1.Invalidate();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            if (pPoints != null)
            {
                float r = 2;
                foreach (Point pi in pPoints)
                {
                    e.Graphics.FillEllipse(Brushes.Black, pi.X - r, pi.Y - r, 2 * r, 2 * r);
                }

                if (pPoints.Count == 4 || pPoints.Count == 7)
                {

                    cPoints = new List<Point>();
                    for ( int i = 0; i < pPoints.Count - 3; i += 3)
                    {
                        getControlPoints(i);
                        e.Graphics.DrawLines(Pens.LightGray, cPoints.ToArray());
                        e.Graphics.DrawBezier(Pens.Black, cPoints[i], cPoints[i+1], cPoints[i+2], cPoints[i+3]);
                    }

                    foreach (Point pi in cPoints)
                    {
                        e.Graphics.DrawEllipse(Pens.Red, pi.X - r, pi.Y - r, 2 * r, 2 * r);
                    }
                }
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            cPoints = null;
            pPoints = null;
            sPoints = null;
            pictureBox1.Invalidate();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (cPoints == null) return;

            NewPoint = e.Location;
            pictureBox1.Invalidate();
        }

        private void getSPointsUniform(int x)
        {
            sPoints = new List<double>();
            for (int i = 0; i < 4; i++)
            {
                double s = (double)i / 3;
                sPoints.Add(s);
            }
            return;
        }

        private void getSPointsChord(int x)
        {
            sPoints = new List<double>();

            List<double> dPoints = null;
            dPoints = new List<double>();

            dPoints.Add(0);

            for (int i = x+1; i < x + 4; i++)
            {
                double d = dPoints[i - 1] + length(pPoints[i - 1], pPoints[i]);
                dPoints.Add(d);
            }

            for (int i = x; i < x + 4; i++)
            {
                double s = dPoints[i] / dPoints[3];
                sPoints.Add(s);
            }

            return;
        }

        private double length(Point a, Point b)
        {
            return Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
        }

        private void getControlPoints(int x)
        {
            var M = Matrix<double>.Build;

            double[,] matrixM = new double[4, 4]
                { { 1, 0, 0, 0 }, { -3, 3, 0, 0 }, { 3, -6, 3, 0 }, { -1, 3, -3, 1 } };

            double[,] matrixP = new double[4, 2];
            for (int i = 0; i < 4; i++)
            {
                matrixP[i, 0] = pPoints[i + x].X;
                matrixP[i, 1] = pPoints[i + x].Y;
            }

            var p = M.DenseOfArray(matrixP);
            var m4 = M.DenseOfArray(matrixM);
            var m4_inv = m4.Inverse();

            if (param == 1)
            {
                getSPointsUniform(x);
            }

            else if (param == 2)
            {
                getSPointsChord(x);
            }

            var s = M.DenseOfArray(MakeMatS());
            var s_tr = s.Transpose();
            var s_reiz = s_tr * s;
            var s_reiz_inv = s_reiz.Inverse();

            var r1 = m4_inv * s_reiz_inv;
            var r2 = r1 * s_tr;

            var c = r2 * p;

            for (int i = 0; i < 4; i++)
            {
                Point tmp = new Point(Convert.ToInt32(c[i, 0]), Convert.ToInt32(c[i, 1]));
                cPoints.Add(tmp);
            }
        }

        private double[,] MakeMatS()
        {
            double[,] res = new double[sPoints.Count, 4];

            for (int i = 0; i < sPoints.Count; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    res[i, j] = Math.Pow(sPoints[i], j);
                }
            }
            return res;
        }
    }
}
