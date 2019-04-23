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
        int counter = 1; //count of input points
        string type = ""; //point type - "C" for control points, "P" for line points

        public Form_KeyboardAdd()
            //initialization
        {
            InitializeComponent();

            //for scrolling point list:
            tableLayoutPanel1.HorizontalScroll.Maximum = 0;
            tableLayoutPanel1.AutoScroll = false;
            tableLayoutPanel1.VerticalScroll.Visible = false;
            tableLayoutPanel1.AutoScroll = true;

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

            for (int i = 0; i < 4; i++)//for every line, start with 4 points
            {
                makeRow();
            }

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
            //add new row to form
        {
            Label newLabel = new Label();
            string tmp = type + counter;
            newLabel.Text = tmp;
            tableLayoutPanel1.RowCount = tableLayoutPanel1.RowCount + 1;
            tableLayoutPanel1.Controls.Add(newLabel);

            TextBox newxPoint = new TextBox();//new textbox for x coordinate
            newxPoint.Name = "input_x" + counter;
            tableLayoutPanel1.Controls.Add(newxPoint);

            TextBox newyPoint = new TextBox();//new textbox for y coordinate
            newyPoint.Name = "input_y" + counter;
            tableLayoutPanel1.Controls.Add(newyPoint);

            Label newEmpty = new Label();//table has an empty column where scroll bar goes on top
            newEmpty.Text = "";
            tableLayoutPanel1.Controls.Add(newEmpty);

            newLabel.Anchor = AnchorStyles.Bottom;

            counter++;

            InputValues.Add(newxPoint);
            InputValues.Add(newyPoint);

            return;
        }

        private void btn_ResetInput_Click(object sender, EventArgs e)
            //reset typed input values
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

            List<Point> pointList = new List<Point>();
            int x, y;

            for (int i = 0; i < InputValues.Count; i += 2)
            //put all values from text boxes to control point list
            {
                x = Convert.ToInt32(InputValues[i].Text);
                y = Convert.ToInt32(InputValues[i + 1].Text);
                Point tmp = new Point(x, y);

                pointList.Add(tmp);
            }

            if (addType == Form1.BezierType.cPoints)
            {
                Form1.cPointsAll[Form1.cPointsAll.Count - 1] = pointList;
                lineAdded = true; //line was added successfully
            }

            else if (addType == Form1.BezierType.pPoints || addType == Form1.BezierType.leastSquares || addType == Form1.BezierType.composite)
            {
                Form1.pPointsAll[Form1.pPointsAll.Count - 1] = pointList;
                lineAdded = true; //line was added successfully
            }
            
            this.Close();
        }

        private void btn_DeleteRow_Click(object sender, EventArgs e)
            //delete input row
        {
            if (addType == Form1.BezierType.leastSquares && tableLayoutPanel1.RowCount <= 9) //4 rows minimum plus 5 rows from design equals 9 rows
            {
                MessageBox.Show("<Least Squares> lines can't have less than 4 points!");
                return;
            }

            if (addType == Form1.BezierType.composite && tableLayoutPanel1.RowCount <= 7) //2 rows minimum plus 5 rows from design equals 7 rows
            {
                MessageBox.Show("<Composite> lines can't have less than 2 points!");
                return;
            }

            for (int i = 0; i < tableLayoutPanel1.ColumnCount; i ++)
            // remove all controls from row
            {
                int count = tableLayoutPanel1.Controls.Count;
                tableLayoutPanel1.Controls.RemoveAt(count - 1);
            }

            //remove textboxes from input list
            InputValues.RemoveAt(InputValues.Count - 1);
            InputValues.RemoveAt(InputValues.Count - 1);

            tableLayoutPanel1.RowCount --;
            counter --;
        }

        private void btn_AddRow_Click(object sender, EventArgs e)
            //add new row
        {
            makeRow();
        }
    }
}