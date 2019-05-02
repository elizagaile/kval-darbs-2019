using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace BezierTool
{
    public partial class Form_KeyboardAdd : Form
    {

        private List<TextBox> coordinates = new List<TextBox>(); //list of textBoxes for point coordinates
        public static bool lineAdded = false; //to determine if a line was drawn successfully
        int counter = 1; //count of point coordinates, used for naming textboxes and labels
        string type = ""; //point type for labels - "C" for control points, "P" for line points
        Form1.BezierType lineType; // type of line being read by this form
        Form1.Form2Type form2Type; // reason for opening this form

        public Form_KeyboardAdd( Form1.Form2Type thisForm2Type, Form1.BezierType thisLineType)
            //initialization
        {
            InitializeComponent();

            //for scrolling point list:
            tableLayoutPanel1.HorizontalScroll.Maximum = 0;
            tableLayoutPanel1.AutoScroll = false;
            tableLayoutPanel1.VerticalScroll.Visible = false;
            tableLayoutPanel1.AutoScroll = true;

            lineType = thisLineType;
            form2Type = thisForm2Type;

            if (form2Type == Form1.Form2Type.add)
            {
                initializeAdd();
            }

            else if (form2Type == Form1.Form2Type.modify)
            {
                initializeModify();
            }

            else if (form2Type == Form1.Form2Type.output)
            {
                initializeOutput();
            }
        }

        private void initializeAdd()
            //initialize form for adding new line
        {
            this.Text = "New <" + lineType + "> line";

            if (lineType == Form1.BezierType.cPoints)
            {
                type = "C";
            }

            else if (lineType == Form1.BezierType.pPoints || lineType == Form1.BezierType.leastSquares || lineType == Form1.BezierType.composite)
            {
                type = "P";
            }

            for (int i = 0; i < 4; i++)
            //for every line, start with 4 points
            {
                makeRow();
            }

            if (lineType == Form1.BezierType.cPoints || lineType == Form1.BezierType.pPoints)
            //<4 cPoint> and <4 pPoint> lines have exactly 4 input points, no need to add or delete input lines 
            {
                btn_AddRow.Visible = false;
                btn_DeleteRow.Visible = false;
            }

            if (lineType == Form1.BezierType.leastSquares || lineType == Form1.BezierType.composite)
            //<Least Squares> and <Composite> line input point count can vary, its possible to add and delete input lines
            {
                btn_AddRow.Visible = true;
                btn_DeleteRow.Visible = true;
            }

            return;
        }

        private void initializeModify()
        {
            this.Text = "Modify <" + lineType + "> line";

            btn_AddRow.Visible = false; // can't add or delete input lines when modifying a line
            btn_DeleteRow.Visible = false;

            List<Point> pointList = new List<Point>();
            int i = Form1.LocalPoint.Item1;

            if (Form1.DragType == Form1.BezierType.cPoints)
            {
                groupBox1.Text = "Modify <" + lineType + "> control point coordinates:";
                type = "C";
                pointList = Form1.cPointsAll[i];
            }

            else if (Form1.DragType == Form1.BezierType.pPoints)
            {
                groupBox1.Text = "Modify <" + lineType + "> line point coordinates:";
                type = "P";
                pointList = Form1.pPointsAll[i];
            }

            if (lineType == Form1.BezierType.composite)
            {
                int j = Form1.LocalPoint.Item2;
                counter = j + 1;
                makeRow();

                coordinates[0].Text = "" + pointList[j].X;
                coordinates[1].Text = "" + pointList[j].Y;

                return;
            }

            for (int j = 0; j < pointList.Count; j++) 
            // make new input row for each point
            {
                makeRow();
            }

            for (int j = 0; j < pointList.Count; j++)
            // after making input rows, fill each textbox with appropriate coordinate
            {
                coordinates[2 * j].Text = "" + pointList[j].X;
                coordinates[2 * j + 1].Text = "" + pointList[j].Y;
            }

            return;
        }

        private void initializeOutput()
        {
            this.Text = "Output <" + lineType + "> line";

            btn_AddRow.Visible = false; // can't add or delete input lines when viewing point coordinates
            btn_DeleteRow.Visible = false;
            btn_ResetInput.Visible = false; // can't reset point coordinates when viewing
            btn_SubmitInput.Visible = false; // can't submit point coordinates when viewing

            List<Point> pointList = new List<Point>();
            int i = Form1.LocalPoint.Item1;

            if (Form1.OutputPointsType == Form1.BezierType.cPoints)
            {
                groupBox1.Text = "List of <" + lineType + "> control points:";
                type = "C";
                pointList = Form1.cPointsAll[i];
            }

            else if (Form1.OutputPointsType == Form1.BezierType.pPoints)
            {
                groupBox1.Text = "List of <" + lineType + "> line points:";
                type = "P";
                pointList = Form1.pPointsAll[i];
            }

            for (int j = 0; j < pointList.Count; j++)
            // make new input row for each point
            {
                makeRow();
            }

            for (int j = 0; j < pointList.Count; j++)
            // after making input rows, fill each textbox with appropriate coordinate
            {
                coordinates[2 * j].Text = "" + pointList[j].X;
                coordinates[2 * j + 1].Text = "" + pointList[j].Y;
            }

            return;
        }

        private void makeRow()
            //add new row of coordinates to form
        {
            tableLayoutPanel1.RowCount = tableLayoutPanel1.RowCount + 1;//add new empty row

            Label newLabel = new Label(); //new label for coordinates
            newLabel.Text = "" + type + counter;
            tableLayoutPanel1.Controls.Add(newLabel);

            TextBox newxPoint = new TextBox();//new textbox for x coordinate
            newxPoint.Name = "point_x" + counter;
            tableLayoutPanel1.Controls.Add(newxPoint);
            coordinates.Add(newxPoint);

            TextBox newyPoint = new TextBox();//new textbox for y coordinate
            newyPoint.Name = "point_y" + counter;
            tableLayoutPanel1.Controls.Add(newyPoint);
            coordinates.Add(newyPoint);
            
            Label newEmpty = new Label();//table has an empty column where scroll bar goes
            newEmpty.Text = "";
            tableLayoutPanel1.Controls.Add(newEmpty);

            newLabel.Anchor = AnchorStyles.Bottom; //need to fix anchors

            counter++;

            return;
        }

        private void btn_ResetInput_Click(object sender, EventArgs e)
            //clear all coordinates in textboxes
        {
            for (int i = 0; i < coordinates.Count; i++)
            {
                coordinates[i].Text = "";
            }
        }

        private void btn_SubmitInput_Click(object sender, EventArgs e)
            //submit input to Form1
        {
            foreach (TextBox coordinate in coordinates)
            //check if all textboxes are filled
            {
                if (coordinate.Text == "")
                {
                    MessageBox.Show("Must fill all values!");
                    return;
                }
            }

            List<Point> pointList = new List<Point>();
            int x, y;

            for (int k = 0; k < coordinates.Count; k += 2)
            //put all values from textboxes to list of control points
            {
                x = Convert.ToInt32(coordinates[k].Text);
                y = Convert.ToInt32(coordinates[k + 1].Text);
                Point tmp = new Point(x, y);

                pointList.Add(tmp);
            }

            int i = 0;//describes where to save new list of coordinates; need to set value for code to work; chosen arbitrary

            if (form2Type == Form1.Form2Type.add)
            // if adding new line, its the last line in representitive lists
            {
                i = Form1.AllLines.Count - 1;
            }

            else if (form2Type == Form1.Form2Type.modify)
            // if modifying a line, get its location in representitive lists
            {
                i = Form1.LocalPoint.Item1;
            }

            if (lineType == Form1.BezierType.cPoints)
            {
                Form1.cPointsAll[i] = pointList;
                lineAdded = true; //line was added successfully
            }

            else if (lineType == Form1.BezierType.pPoints || lineType == Form1.BezierType.leastSquares)
            {
                Form1.pPointsAll[i] = pointList;
                lineAdded = true; //line was added successfully
            }

            else if (lineType == Form1.BezierType.composite && form2Type == Form1.Form2Type.add)
            {
                Form1.pPointsAll[i] = pointList;
                lineAdded = true; //line was added successfully
            }

            else if (lineType == Form1.BezierType.composite && form2Type == Form1.Form2Type.modify)
            {
                Form1.pPointsModifyComposite(pointList[0]);
            }
            
            this.Close();//vajag?
        }

        private void btn_DeleteRow_Click(object sender, EventArgs e)
            //delete input row
        {
            if (lineType == Form1.BezierType.leastSquares && tableLayoutPanel1.RowCount <= 9) //4 rows minimum plus 5 rows from design equals 9 rows
            {
                MessageBox.Show("<Least Squares> lines can't have less than 4 points!");
                return;
            }

            if (lineType == Form1.BezierType.composite && tableLayoutPanel1.RowCount <= 7) //2 rows minimum plus 5 rows from design equals 7 rows
            {
                MessageBox.Show("<Composite> lines can't have less than 2 points!");
                return;
            }

            for (int i = 0; i < tableLayoutPanel1.ColumnCount; i ++)
            // remove all controls from last row
            {
                int count = tableLayoutPanel1.Controls.Count;
                tableLayoutPanel1.Controls.RemoveAt(count - 1);
            }

            //remove textboxes from input list
            coordinates.RemoveAt(coordinates.Count - 1);
            coordinates.RemoveAt(coordinates.Count - 1);

            tableLayoutPanel1.RowCount --; //remove last row
            counter --;
        }

        private void btn_AddRow_Click(object sender, EventArgs e)
            //add new row
        {
            makeRow();
        }

    }
}