using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace BezierTool
{
    public partial class FormCoordinates : Form
    {
        FormMain.FormType formType; // reason for opening this form
        FormMain.BezierType curveType; // type of curve being used in this form

        private List<TextBox> coordinates = new List<TextBox>(); //list of textBoxes for point coordinates
        string labelType = ""; //point type for labels - "C" for control points, "P" for knot points
        int namingCounter = 1; //count of point coordinates, used for naming textboxes and labels
        public static bool curveAdded = false; //to determine if a curve was added successfully

        public FormCoordinates( FormMain.FormType thisFormType, FormMain.BezierType thisCurveType)
            //initialization
        {
            InitializeComponent();

            //for scrolling point list:
            tlpCoordinates.HorizontalScroll.Maximum = 0;
            tlpCoordinates.AutoScroll = false;
            tlpCoordinates.VerticalScroll.Visible = false;
            tlpCoordinates.AutoScroll = true;

            curveType = thisCurveType;
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
            //initialize form for adding a new curve
        {
            this.Text = "New <" + curveType + "> curve";

            if (curveType == FormMain.BezierType.cPoints)
            {
                labelType = "C";
            }

            else if (curveType == FormMain.BezierType.pPoints || curveType == FormMain.BezierType.LeastSquares || curveType == FormMain.BezierType.Composite)
            {
                labelType = "P";
            }

            for (int i = 0; i < 4; i++)
            //start with 4 points for every curve type
            {
                AddRow();
            }

            if (curveType == FormMain.BezierType.cPoints || curveType == FormMain.BezierType.pPoints)
            //<4 cPoint> and <4 pPoint> curves have exactly 4 input points; no need to add or delete input lines
            {
                gbCoordinates.Text = "Input <" + curveType + "> control point coordinates:";
                btnAddRow.Visible = false;
                btnDeleteRow.Visible = false;
            }

            if (curveType == FormMain.BezierType.LeastSquares || curveType == FormMain.BezierType.Composite)
            //Count of <Least Squares> and <Composite> input point count can vary; its possible to add and delete input lines
            {
                gbCoordinates.Text = "Input <" + curveType + "> knot point coordinates:";
                btnAddRow.Visible = true;
                btnDeleteRow.Visible = true;
            }

            return;
        }

        private void InitializeModify()
            //initialize form for modifying a curve
        {
            this.Text = "Modify <" + curveType + "> curve";

            btnAddRow.Visible = false; // can't add or delete input lines when modifying a curve
            btnDeleteRow.Visible = false;

            List<Point> pointList = new List<Point>();
            int i = FormMain.localPoint.Item1;

            if (FormMain.modifyPointType == FormMain.BezierType.cPoints)
            {
                gbCoordinates.Text = "Modify <" + curveType + "> control point coordinates:";
                labelType = "C";
                pointList = FormMain.cPointsAll[i];
            }

            else if (FormMain.modifyPointType == FormMain.BezierType.pPoints)
            {
                gbCoordinates.Text = "Modify <" + curveType + "> knot point coordinates:";
                labelType = "P";
                pointList = FormMain.pPointsAll[i];
            }

            if (curveType == FormMain.BezierType.Composite)
            // its possible to modify only one <Composite> knot point at a time
            {
                int j = FormMain.localPoint.Item2; //get which knot point is being modified
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
            this.Text = "Output <" + curveType + "> curve";

            btnAddRow.Visible = false; // can't add or delete input lines when viewing point coordinates
            btnDeleteRow.Visible = false;
            btnResetInput.Visible = false; // can't reset point coordinates when viewing
            btnSubmitInput.Visible = false; // can't submit point coordinates when viewing

            List<Point> pointList = new List<Point>();
            int i = FormMain.localPoint.Item1;

            if (FormMain.outputPointType == FormMain.BezierType.cPoints)
            {
                gbCoordinates.Text = "List of <" + curveType + "> control point coordinates:";
                labelType = "C";
                pointList = FormMain.cPointsAll[i];
            }

            else if (FormMain.outputPointType == FormMain.BezierType.pPoints)
            {
                gbCoordinates.Text = "List of <" + curveType + "> knot point coordinates:";
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
            if (formType == FormMain.FormType.Add && (tlpCoordinates.RowCount > FormMain.maxPointCount + 4))
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
                i = FormMain.allCurves.Count - 1;
            }

            else if (formType == FormMain.FormType.Modify)
            // if modifying a line, get its location in representitive lists
            {
                i = FormMain.localPoint.Item1;
            }

            if (curveType == FormMain.BezierType.cPoints)
            {
                FormMain.cPointsAll[i] = pointList;
                curveAdded = true; //line was added successfully
            }

            else if (curveType == FormMain.BezierType.pPoints || curveType == FormMain.BezierType.LeastSquares)
            {
                FormMain.pPointsAll[i] = pointList;
                curveAdded = true; //curve was added successfully
            }

            else if (curveType == FormMain.BezierType.Composite && formType == FormMain.FormType.Add)
            {
                FormMain.pPointsAll[i] = pointList;
                curveAdded = true; //curve was added successfully
            }

            else if (curveType == FormMain.BezierType.Composite && formType == FormMain.FormType.Modify)
            {
                FormMain.ModifypPointComposite(pointList[0]);
            }
            
            this.Close();
        }

        private void btnDeleteRow_Click(object sender, EventArgs e)
            //delete input row
        {
            if (curveType == FormMain.BezierType.LeastSquares && tlpCoordinates.RowCount <= 9) //4 rows minimum plus 5 rows from table design equals 9 rows
            {
                MessageBox.Show("<Least Squares> curves can't have less than 4 knot points!");
                return;
            }

            if (curveType == FormMain.BezierType.Composite && tlpCoordinates.RowCount <= 7) //2 rows minimum plus 5 rows from design equals 7 rows
            {
                MessageBox.Show("<Composite> curves can't have less than 2 knot points!");
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