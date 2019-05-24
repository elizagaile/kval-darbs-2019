using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace BezierTool
{
    public partial class FormCoordinates : Form
    {
        FormMain.FormType formType; // reason for opening this form
        FormMain.BezierType lineType; // type of line being used in this form

        private List<TextBox> coordinates = new List<TextBox>(); //list of textBoxes for point coordinates
        string labelType = ""; //point type for labels - "C" for control points, "P" for line points
        int namingCounter = 1; //count of point coordinates, used for naming textboxes and labels
        public static bool lineAdded = false; //to determine if a line was added successfully

        public FormCoordinates( FormMain.FormType thisFormType, FormMain.BezierType thisLineType)
            //initialization
        {
            InitializeComponent();

            //for scrolling point list:
            tlpCoordinates.HorizontalScroll.Maximum = 0;
            tlpCoordinates.AutoScroll = false;
            tlpCoordinates.VerticalScroll.Visible = false;
            tlpCoordinates.AutoScroll = true;

            lineType = thisLineType;
            formType = thisFormType;

            if (formType == FormMain.FormType.Add)
            {
                InitializeAdd();
            }

            else if (formType == FormMain.FormType.Modify)
            {
                InitializeModify();
            }

            else if (formType == FormMain.FormType.Output)
            {
                InitializeOutput();
            }
        }

        private void InitializeAdd()
            //initialize form for adding a new line
        {
            this.Text = "New <" + lineType + "> line";

            if (lineType == FormMain.BezierType.cPoints)
            {
                labelType = "C";
            }

            else if (lineType == FormMain.BezierType.pPoints || lineType == FormMain.BezierType.LeastSquares || lineType == FormMain.BezierType.Composite)
            {
                labelType = "P";
            }

            for (int i = 0; i < 4; i++)
            //start with 4 points for every line type
            {
                AddRow();
            }

            if (lineType == FormMain.BezierType.cPoints || lineType == FormMain.BezierType.pPoints)
            //<4 cPoint> and <4 pPoint> lines have exactly 4 input points; no need to add or delete input lines 
            {
                gbCoordinates.Text = "Input <" + lineType + "> control point coordinates:";
                btnAddRow.Visible = false;
                btnDeleteRow.Visible = false;
            }

            if (lineType == FormMain.BezierType.LeastSquares || lineType == FormMain.BezierType.Composite)
            //<Least Squares> and <Composite> line input point count can vary; its possible to add and delete input lines
            {
                gbCoordinates.Text = "Input <" + lineType + "> knot point coordinates:";
                btnAddRow.Visible = true;
                btnDeleteRow.Visible = true;
            }

            return;
        }

        private void InitializeModify()
            //initialize form for modifying a line
        {
            this.Text = "Modify <" + lineType + "> line";

            btnAddRow.Visible = false; // can't add or delete input lines when modifying a line
            btnDeleteRow.Visible = false;

            List<Point> pointList = new List<Point>();
            int i = FormMain.localPoint.Item1;

            if (FormMain.modifyPointType == FormMain.BezierType.cPoints)
            {
                gbCoordinates.Text = "Modify <" + lineType + "> control point coordinates:";
                labelType = "C";
                pointList = FormMain.cPointsAll[i];
            }

            else if (FormMain.modifyPointType == FormMain.BezierType.pPoints)
            {
                gbCoordinates.Text = "Modify <" + lineType + "> knot point coordinates:";
                labelType = "P";
                pointList = FormMain.pPointsAll[i];
            }

            if (lineType == FormMain.BezierType.Composite)
            // its possible to modify only one <Composite> line point at a time
            {
                int j = FormMain.localPoint.Item2; //get which line point is being modified
                namingCounter = j + 1; //labels start at 1, lists at 0
                AddRow();

                coordinates[0].Text = "" + pointList[j].X;
                coordinates[1].Text = "" + pointList[j].Y;

                return;
            }

            for (int j = 0; j < pointList.Count; j++) 
            // make new input row for each point
            {
                AddRow();
            }

            for (int j = 0; j < pointList.Count; j++)
            // after making input rows, fill each textbox with appropriate coordinate
            {
                coordinates[2 * j].Text = "" + pointList[j].X;
                coordinates[2 * j + 1].Text = "" + pointList[j].Y;
            }

            return;
        }

        private void InitializeOutput()
        //initialize form for outputting line coordinates
        {
            this.Text = "Output <" + lineType + "> line";

            btnAddRow.Visible = false; // can't add or delete input lines when viewing point coordinates
            btnDeleteRow.Visible = false;
            btnResetInput.Visible = false; // can't reset point coordinates when viewing
            btnSubmitInput.Visible = false; // can't submit point coordinates when viewing

            List<Point> pointList = new List<Point>();
            int i = FormMain.localPoint.Item1;

            if (FormMain.outputPointType == FormMain.BezierType.cPoints)
            {
                gbCoordinates.Text = "List of <" + lineType + "> control point coordinates:";
                labelType = "C";
                pointList = FormMain.cPointsAll[i];
            }

            else if (FormMain.outputPointType == FormMain.BezierType.pPoints)
            {
                gbCoordinates.Text = "List of <" + lineType + "> knot point coordinates:";
                labelType = "P";
                pointList = FormMain.pPointsAll[i];
            }

            for (int j = 0; j < pointList.Count; j++)
            // make new input row for each point
            {
                AddRow();
            }

            for (int j = 0; j < pointList.Count; j++)
            // after making input rows, fill each textbox with appropriate coordinate
            {
                coordinates[2 * j].Text = "" + pointList[j].X;
                coordinates[2 * j + 1].Text = "" + pointList[j].Y;
            }

            return;
        }

        private void AddRow()
            //add new row of coordinates to form
        {
            if (tlpCoordinates.RowCount > FormMain.maxPointCount + 4)
            {
                MessageBox.Show("Maximum count of input points is " + FormMain.maxPointCount + "!");
                return;
            }

            tlpCoordinates.RowCount = tlpCoordinates.RowCount + 1;//add new empty row

            Label newLabel = new Label //new label for coordinates
            {
                Text = "" + labelType + namingCounter
            };
            tlpCoordinates.Controls.Add(newLabel);

            TextBox xCoordinate = new TextBox //new textbox for x coordinate
            {
                Name = "x" + namingCounter
            };
            tlpCoordinates.Controls.Add(xCoordinate);
            coordinates.Add(xCoordinate);

            TextBox yCoordinate = new TextBox //new textbox for y coordinate
            {
                Name = "y" + namingCounter
            };
            tlpCoordinates.Controls.Add(yCoordinate);
            coordinates.Add(yCoordinate);
            
            Label newEmpty = new Label //table has an empty column where scroll bar goes
            {
                Text = ""
            };
            tlpCoordinates.Controls.Add(newEmpty);

            newLabel.Anchor = AnchorStyles.Bottom; //need to fix anchors, this doeasn work

            namingCounter++;

            return;
        }

        private void btnResetInput_Click(object sender, EventArgs e)
            //clear all coordinates in textboxes
        {
            for (int i = 0; i < coordinates.Count; i++)
            {
                coordinates[i].Text = "";
            }
        }

        private void btnSubmitInput_Click(object sender, EventArgs e)
            //submit input to FormMain
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

            for (int j = 0; j < coordinates.Count; j += 2)
            //put all values from textboxes to list of control points
            {
                x = Convert.ToInt32(coordinates[j].Text);
                y = Convert.ToInt32(coordinates[j + 1].Text);
                Point tmp = new Point(x, y);

                pointList.Add(tmp);
            }

            int i = 0;//describes where to save new list of coordinates; need to set value for code to work; chosen arbitrary

            if (formType == FormMain.FormType.Add)
            // if adding new line, its the last line in representitive lists
            {
                i = FormMain.allLines.Count - 1;
            }

            else if (formType == FormMain.FormType.Modify)
            // if modifying a line, get its location in representitive lists
            {
                i = FormMain.localPoint.Item1;
            }

            if (lineType == FormMain.BezierType.cPoints)
            {
                FormMain.cPointsAll[i] = pointList;
                lineAdded = true; //line was added successfully
            }

            else if (lineType == FormMain.BezierType.pPoints || lineType == FormMain.BezierType.LeastSquares)
            {
                FormMain.pPointsAll[i] = pointList;
                lineAdded = true; //line was added successfully
            }

            else if (lineType == FormMain.BezierType.Composite && formType == FormMain.FormType.Add)
            {
                FormMain.pPointsAll[i] = pointList;
                lineAdded = true; //line was added successfully
            }

            else if (lineType == FormMain.BezierType.Composite && formType == FormMain.FormType.Modify)
            {
                FormMain.ModifypPointComposite(pointList[0]);
            }
            
            this.Close();
        }

        private void btnDeleteRow_Click(object sender, EventArgs e)
            //delete input row
        {
            if (lineType == FormMain.BezierType.LeastSquares && tlpCoordinates.RowCount <= 9) //4 rows minimum plus 5 rows from table design equals 9 rows
            {
                MessageBox.Show("<Least Squares> lines can't have less than 4 points!");
                return;
            }

            if (lineType == FormMain.BezierType.Composite && tlpCoordinates.RowCount <= 7) //2 rows minimum plus 5 rows from design equals 7 rows
            {
                MessageBox.Show("<Composite> lines can't have less than 2 points!");
                return;
            }

            for (int i = 0; i < tlpCoordinates.ColumnCount; i ++)
            // remove all controls from last row
            {
                tlpCoordinates.Controls.RemoveAt(tlpCoordinates.Controls.Count - 1);
            }

            //remove textboxes from input list
            coordinates.RemoveAt(coordinates.Count - 1);
            coordinates.RemoveAt(coordinates.Count - 1);

            tlpCoordinates.RowCount --; //remove last row
            namingCounter--;
        }

        private void btnAddRow_Click(object sender, EventArgs e)
            //add new row
        {
            AddRow();
        }
    }
}