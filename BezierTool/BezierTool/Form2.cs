using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace BezierTool
{
    public partial class Form_KeyboardAdd : Form
    {
        private List<Point> cPoints = null;
        private List<Point> pPoints = null;
        Form1.BezierType addType = Form1.AddType;
        public static bool lineAdded = false; //to determine if a line was drawn successfully

        public Form_KeyboardAdd()
            //initialization
        {
            InitializeComponent();

            List<Label> PointLabels = new List<Label>
                { lbl_1, lbl_2, lbl_3, lbl_4 };
            string type = "";

            //make labels according to input type:

            if (addType == Form1.BezierType.cPoints)
            {
                type = "C";
            }

            else if (addType == Form1.BezierType.pPoints)
            {
                type = "P";
            }

            for (int i = 0; i < PointLabels.Count; i++)
            {
                PointLabels[i].Text = type + (i + 1);
            }
        }

        private void btn_ResetInput_Click(object sender, EventArgs e)
            //clean input values
        {
            List<TextBox> InputValues = new List<TextBox>
                { input_x1, input_x2, input_x3, input_x4, input_y1, input_y2, input_y3, input_y4};

            for (int i = 0; i < InputValues.Count; i++)
            {
                InputValues[i].Text = "";
            }
        }

        private void btn_SubmitInput_Click(object sender, EventArgs e)
            //submit input to Form1
        {
            List<TextBox> InputValues = new List<TextBox>
                { input_x1, input_x2, input_x3, input_x4, input_y1, input_y2, input_y3, input_y4};

            foreach (TextBox input in InputValues)
            {
                if (input.Text == "")
                {
                    MessageBox.Show("Must fill all values!");
                    return;
                }
            }

            if (addType == Form1.BezierType.cPoints)
            {
                cPoints = new List<Point>();
                int x, y;

                for (int i = 0; i < 4; i++)
                //put all values from text boxes to control point list
                {
                    x = Convert.ToInt32(InputValues[i].Text);
                    y = Convert.ToInt32(InputValues[i + 4].Text);
                    Point tmp = new Point(x, y);

                    cPoints.Add(tmp);
                }

                Form1.cPointsAll.Add(cPoints);
                lineAdded = true;//line was added successfully
            }

            if (addType == Form1.BezierType.pPoints)
            {
                pPoints = new List<Point>();
                int x, y;

                for (int i = 0; i < 4; i++)
                //put all values from text boxes to line point list
                {
                    x = Convert.ToInt32(InputValues[i].Text);
                    y = Convert.ToInt32(InputValues[i + 4].Text);
                    Point tmp = new Point(x, y);

                    pPoints.Add(tmp);
                }

                Form1.pPointsAll.Add(pPoints);
                lineAdded = true;//line was added successfully
            }

            this.Close();
        }
    }
}