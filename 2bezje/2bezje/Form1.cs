using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _2bezje
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private List<Point> cPoints = null;

        private Point NewPoint;

        bool move = false;
        bool jauns = false;
        int moving;

        int thickBezier = 0;

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            // See if we are already drawing a polygon.
            if (cPoints != null && cPoints.Count < 8)
            {
                // Add a point to this polygon.
                if (cPoints[cPoints.Count - 1] != e.Location)
                {
                    cPoints.Add(e.Location);
                }
            }

            else if (cPoints != null && jauns == false)
            {
                for (int i = 0; i < 8; i++)
                {
                    if (cPoints != null &&
                        (Math.Pow(e.X - cPoints[i].X, 2) + Math.Pow(e.Y - cPoints[i].Y, 2)) < 50)
                    {
                        move = true;
                        moving = i;
                    }
                }
            }

            else
            {
                // Start a new polygon.
                cPoints = new List<Point>();
                NewPoint = e.Location;
                cPoints.Add(e.Location);
            }

            pictureBox1.Invalidate();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (move == true)
            {
                cPoints[moving] = e.Location;
                cValues();
            }

            else if (cPoints == null) return;

            NewPoint = e.Location;
            pictureBox1.Invalidate();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode =
        System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            Pen bezierPen = new Pen(Brushes.Black);

            if (cPoints != null)
            {
                if (cPoints.Count > 1 && cPoints.Count < 5)
                {
                    e.Graphics.DrawLines(Pens.LightGray, cPoints.ToArray());
                }

                else if (cPoints.Count > 5)
                {
                    Point[] first = new Point[4];
                    Point[] last = new Point[cPoints.Count - 4];
                    for (int i = 0; i < 4; i ++)
                    {
                        first[i] = cPoints[i];
                    }

                    for (int i = 4; i < cPoints.Count; i++)
                    {
                        last[i-4] = cPoints[i];
                    }

                    e.Graphics.DrawLines(Pens.LightGray, first);
                    e.Graphics.DrawLines(Pens.LightGray, last);
                }
                
                if (cPoints.Count > 0 && cPoints.Count != 4 && cPoints.Count < 8)
                {
                    using (Pen dashed_pen = new Pen(Color.LightGray))
                    {
                        dashed_pen.DashPattern = new float[] { 3, 3 };
                        e.Graphics.DrawLine(dashed_pen, cPoints[cPoints.Count - 1], NewPoint);
                    }
                }

                if (cPoints.Count >= 4)
                {
                    if (thickBezier == 1)
                    {
                        bezierPen.Width = 2;
                    }

                    else
                    {
                        bezierPen.Width = 1;
                    }

                    float r = 2;
                    e.Graphics.DrawBezier(bezierPen, cPoints[0], cPoints[1], cPoints[2], cPoints[3]);
                    foreach (Point pi in cPoints)
                    {
                        e.Graphics.DrawEllipse(Pens.Red, pi.X - r, pi.Y - r, 2 * r, 2 * r);
                    }
                    cValues();
                }

                if (cPoints.Count == 8)
                {
                    if (thickBezier == 2)
                    {
                        bezierPen.Width = 2;
                    }

                    else
                    {
                        bezierPen.Width = 1;
                    }

                    float r = 2;
                    e.Graphics.DrawBezier(bezierPen, cPoints[4], cPoints[5], cPoints[6], cPoints[7]);
                    foreach (Point pi in cPoints)
                    {
                        e.Graphics.DrawEllipse(Pens.Red, pi.X - r, pi.Y - r, 2 * r, 2 * r);
                    }
                    cValues();
                }
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {

            if (move == true)
            {
                cPoints[moving] = e.Location;
                move = false;
            }

            pictureBox1.Invalidate();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            List<Label> lblListC = new List<Label>
                { lbl1C1, lbl1C2, lbl1C3, lbl1C4, lbl2C1, lbl2C2, lbl2C3, lbl2C4 };
            for (int i = 0; i < 4; i++)
            {
                lblListC[i].Text = "C" + (i + 1);
                lblListC[i+4].Text = "C" + (i + 1);
            }
            cPoints = null;
            pictureBox1.Invalidate();
        }

        private void cValues()
        {
            List<Label> lblListC = new List<Label>
                { lbl1C1, lbl1C2, lbl1C3, lbl1C4, lbl2C1, lbl2C2, lbl2C3, lbl2C4 };

            if (cPoints != null && cPoints.Count == 4)
            {
                for (int i = 0; i < 4; i++)
                {
                    lblListC[i].Text = "C" + (i + 1) + " = (" + cPoints[i].X + ";" + cPoints[i].Y + ")";
                    //lblListC[i+4].Text = "C" + (i + 1 + 4) + " = (" + cPoints[i+4].X + ";" + cPoints[i+4].Y + ")";
                }
            }

            if (cPoints != null && cPoints.Count == 8)
            {
                for (int i = 0; i < 4; i++)
                {
                    lblListC[i].Text = "C" + (i + 1) + " = (" + cPoints[i].X + ";" + cPoints[i].Y + ")";
                    lblListC[i+4].Text = "C" + (i + 1 + 4) + " = (" + cPoints[i+4].X + ";" + cPoints[i+4].Y + ")";
                }
            }
        }

        private void label1_MouseEnter(object sender, EventArgs e)
        {
            thickBezier = 1;
            pictureBox1.Invalidate();
        }

        private void label1_MouseLeave(object sender, EventArgs e)
        {
            thickBezier = 0;
            pictureBox1.Invalidate();
        }

        private void label2_MouseEnter(object sender, EventArgs e)
        {
            thickBezier = 2;
            pictureBox1.Invalidate();
        }

        private void label2_MouseLeave(object sender, EventArgs e)
        {
            thickBezier = 0;
            pictureBox1.Invalidate();
        }
    }
}
