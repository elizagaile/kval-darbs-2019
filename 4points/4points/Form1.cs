using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _4points
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

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            // See if we are already drawing a polygon.
            if (cPoints != null && cPoints.Count < 4)
            {
                // Add a point to this polygon.
                if (cPoints[cPoints.Count - 1] != e.Location)
                {
                    cPoints.Add(e.Location);
                }
            }

            else if (cPoints != null && cPoints.Count == 4 && jauns == false)
            {
                for (int i = 0; i < 4; i++)
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

            if (cPoints != null)
            {
                // Draw the new line.
                if (cPoints.Count > 1)
                {
                    e.Graphics.DrawLines(Pens.LightGray, cPoints.ToArray());
                }

                // Draw the unfinished line.
                if (cPoints.Count > 0 && cPoints.Count < 4)
                {
                    using (Pen dashed_pen = new Pen(Color.LightGray))
                    {
                        dashed_pen.DashPattern = new float[] { 3, 3 };
                        e.Graphics.DrawLine(dashed_pen, cPoints[cPoints.Count - 1], NewPoint);
                    }
                }

                if (cPoints.Count == 4)
                {
                    float r = 2;
                    e.Graphics.DrawBezier(Pens.Black, cPoints[0] , cPoints[1], cPoints[2], cPoints[3]);
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

        private void button1_Click(object sender, EventArgs e)
        {
            List<Label> lblListC = new List<Label> { lblC1, lblC2, lblC3, lblC4 };
            for (int i = 0; i < 4; i++)
            {
                lblListC[i].Text = "C" + (i + 1);
            }
            cPoints = null;
            pictureBox1.Invalidate();
        }

        private void cValues()
        {
            List<Label> lblListC = new List<Label> { lblC1, lblC2, lblC3, lblC4 };
            if (cPoints != null && cPoints.Count == 4)
            {
                for (int i = 0; i < 4; i++)
                {
                    lblListC[i].Text = "C" + (i + 1) + " = (" + cPoints[i].X + ";" + cPoints[i].Y + ")";
                }
            }
        }
    }
}
