using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace BezierTool
{
    public partial class Form_KeyboardAdd : Form
    {

        private List<TextBox> InputValues = new List<TextBox>();
        private List<Point> cPoints = null;
        private List<Point> pPoints = null;
        Form1.BezierType addType = Form1.AddType;
        public static bool lineAdded = false; //to determine if a line was drawn successfully
        int counter = 1;
        string type = "";

        public Form_KeyboardAdd()
            //initialization
        {
            InitializeComponent();

            //make labels according to input type:

            this.Text = "New <" + addType + "> line";
     
            if (addType == Form1.BezierType.cPoints)
            {
                this.groupBox1.Text = "Set control point coordinates:";
                type = "C";
            }

            else if (addType == Form1.BezierType.pPoints || addType == Form1.BezierType.leastSquares || addType == Form1.BezierType.composite)
            {
                this.groupBox1.Text = "Set line point coordinates:";
                type = "P";
            }

            for (int i = 0; i < 4; i++)
            {
                makeRow();
            }

            tableLayoutPanel1.HorizontalScroll.Maximum = 0;
            tableLayoutPanel1.AutoScroll = false;
            tableLayoutPanel1.VerticalScroll.Visible = false;
            tableLayoutPanel1.AutoScroll = true;

            if (addType == Form1.BezierType.cPoints || addType == Form1.BezierType.pPoints)
            {
                btn_AddRow.Visible = false;
                btn_DeleteRow.Visible = false;
            }

            if (addType == Form1.BezierType.leastSquares || addType == Form1.BezierType.composite)
            {
                btn_AddRow.Visible = true;
                btn_DeleteRow.Visible = true;
            }

        }

        private void makeRow()
        {
            Label newLabel = new Label();
            string tmp = type + counter;
            newLabel.Text = tmp;
            tableLayoutPanel1.RowCount = tableLayoutPanel1.RowCount + 1;
            tableLayoutPanel1.Controls.Add(newLabel);

            TextBox newxPoint = new TextBox();
            newxPoint.Name = "input_x" + counter;
            tableLayoutPanel1.Controls.Add(newxPoint);

            TextBox newyPoint = new TextBox();
            newyPoint.Name = "input_y" + counter;
            tableLayoutPanel1.Controls.Add(newyPoint);

            Label newEmpty = new Label();
            newEmpty.Text = "";
            tableLayoutPanel1.Controls.Add(newEmpty);

            newLabel.Anchor = AnchorStyles.Bottom;

            counter++;

            InputValues.Add(newxPoint);
            InputValues.Add(newyPoint);

            return;
        }

        private void btn_ResetInput_Click(object sender, EventArgs e)
            //clean input values
        {
            for (int i = 0; i < InputValues.Count; i++)
            {
                InputValues[i].Text = "";
            }
        }

        private void btn_SubmitInput_Click(object sender, EventArgs e)
            //submit input to Form1
        {
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

                for (int i = 0; i < InputValues.Count ; i += 2)
                //put all values from text boxes to control point list
                {
                    x = Convert.ToInt32(InputValues[i].Text);
                    y = Convert.ToInt32(InputValues[i + 1].Text);
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

                for (int i = 0; i < InputValues.Count; i += 2)
                //put all values from text boxes to line point list
                {
                    x = Convert.ToInt32(InputValues[i].Text);
                    y = Convert.ToInt32(InputValues[i + 1].Text);
                    Point tmp = new Point(x, y);

                    pPoints.Add(tmp);
                }

                Form1.pPointsAll.Add(pPoints);
                lineAdded = true;//line was added successfully
            }

            if (addType == Form1.BezierType.leastSquares)
            {
                pPoints = new List<Point>();
                int x, y;

                for (int i = 0; i < InputValues.Count; i += 2)
                //put all values from text boxes to line point list
                {
                    x = Convert.ToInt32(InputValues[i].Text);
                    y = Convert.ToInt32(InputValues[i + 1].Text);
                    Point tmp = new Point(x, y);

                    pPoints.Add(tmp);
                }

                Form1.pPointsAll.Add(pPoints);
                lineAdded = true;//line was added successfully

            }

            if (addType == Form1.BezierType.composite)
            {
                pPoints = new List<Point>();
                int x, y;

                for (int i = 0; i < InputValues.Count; i += 2)
                //put all values from text boxes to line point list
                {
                    x = Convert.ToInt32(InputValues[i].Text);
                    y = Convert.ToInt32(InputValues[i + 1].Text);
                    Point tmp = new Point(x, y);

                    pPoints.Add(tmp);
                }

                Form1.pPointsAll.Add(pPoints);
                lineAdded = true;//line was added successfully
            }

                this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            makeRow();
        }

        private void btn_DeleteRow_Click(object sender, EventArgs e)
        {
            if (addType == Form1.BezierType.leastSquares && tableLayoutPanel1.RowCount <= 9)
            {
                MessageBox.Show("<Least Squares> lines can't have less than 4 points!");
                return;
            }

            if (addType == Form1.BezierType.composite && tableLayoutPanel1.RowCount <= 7)
            {
                MessageBox.Show("<Composite> lines can't have less than 2 points!");
                return;
            }

            for (int i = 0; i < 4; i ++)
            {
                int count = tableLayoutPanel1.Controls.Count;
                tableLayoutPanel1.Controls.RemoveAt(count - 1);
            }

            InputValues.RemoveAt(InputValues.Count - 1);
            InputValues.RemoveAt(InputValues.Count - 1);

            tableLayoutPanel1.RowCount --;
            counter --;
        }
    }
}