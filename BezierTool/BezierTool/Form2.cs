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

        private List<TextBox> coordinates = new List<TextBox>(); // list of textBoxes for point coordinates
        string labelType = ""; // point type for labels - "C" for control points, "P" for knot points
        int namingCounter = 1; // count of points, used for naming textboxes and labels
        public static bool curveAdded = false; // to determine if a curve was added successfully

        // Initialization
        public FormCoordinates( FormMain.FormType thisFormType, FormMain.BezierType thisCurveType)
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


        // Initialize form for adding a new curve
        private void InitializeAdd()
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

            // Start with 4 points for every curve type
            for (int i = 0; i < 4; i++)
            {
                AddRow();
            }

            // <4 cPoint> and <4 pPoint> curves have exactly 4 input points; no need to add or delete input lines
            if (curveType == FormMain.BezierType.cPoints || curveType == FormMain.BezierType.pPoints)
            {
                gbCoordinates.Text = "Input <" + curveType + "> control point coordinates:";
                btnAddRow.Visible = false;
                btnDeleteRow.Visible = false;
            }

            // Count of <Least Squares> and <Composite> input points can vary; its possible to add and delete input lines
            if (curveType == FormMain.BezierType.LeastSquares || curveType == FormMain.BezierType.Composite)
            {
                gbCoordinates.Text = "Input <" + curveType + "> knot point coordinates:";
                btnAddRow.Visible = true;
                btnDeleteRow.Visible = true;
            }

            return;
        }


        // Initialize form for modifying a curve
        private void InitializeModify()
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

            // It is possible to modify only one <Composite> knot point at a time
            if (curveType == FormMain.BezierType.Composite)
            {
                int j = FormMain.localPoint.Item2;
                namingCounter = j + 1; //labels start at 1, lists at 0
                AddRow();

                coordinates[0].Text = "" + pointList[j].X;
                coordinates[1].Text = "" + pointList[j].Y;

                return;
            }

            for (int j = 0; j < pointList.Count; j++) 
            {
                AddRow();
            }

            // After making input rows, fill each textBox with appropriate coordinates
            for (int j = 0; j < pointList.Count; j++)
            {
                coordinates[2 * j].Text = "" + pointList[j].X;
                coordinates[2 * j + 1].Text = "" + pointList[j].Y;
            }

            return;
        }


        // Initialize form for outputting points of a curve
        private void InitializeOutput()
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
            {
                AddRow();
            }

            // After making input rows, fill each textBox with appropriate coordinate
            for (int j = 0; j < pointList.Count; j++)
            {
                coordinates[2 * j].Text = "" + pointList[j].X;
                coordinates[2 * j + 1].Text = "" + pointList[j].Y;
            }

            return;
        }


        // Add new row to form for coordinates 
        private void AddRow()
        {
            if (formType == FormMain.FormType.Add && (tlpCoordinates.RowCount > FormMain.maxPointCount + 4))
            {
                MessageBox.Show("Maximum count of input points is " + FormMain.maxPointCount + "!");
                return;
            }

            tlpCoordinates.RowCount = tlpCoordinates.RowCount + 1;// add new empty row

            Label newLabel = new Label // new label for coordinates
            {
                Text = "" + labelType + namingCounter
            };
            tlpCoordinates.Controls.Add(newLabel);

            TextBox xCoordinate = new TextBox // new textbox for x coordinate
            {
                Name = "x" + namingCounter
            };
            tlpCoordinates.Controls.Add(xCoordinate);
            coordinates.Add(xCoordinate);

            TextBox yCoordinate = new TextBox // new textbox for y coordinate
            {
                Name = "y" + namingCounter
            };
            tlpCoordinates.Controls.Add(yCoordinate);
            coordinates.Add(yCoordinate);
            
            Label newEmpty = new Label // table has an empty column where scroll bar goes
            {
                Text = ""
            };
            tlpCoordinates.Controls.Add(newEmpty);

            namingCounter++;

            return;
        }


        // Clear all coordinates from textBoxes
        private void btnResetInput_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < coordinates.Count; i++)
            {
                coordinates[i].Text = "";
            }
        }


        // Submit input to FormMain
        private void btnSubmitInput_Click(object sender, EventArgs e)
        {
            // Check if all textboxes are filled
            foreach (TextBox coordinate in coordinates)
            {
                if (coordinate.Text == "")
                {
                    MessageBox.Show("Must fill all values!");
                    return;
                }
            }

            List<Point> pointList = new List<Point>();
            int x, y;

            // Put all values from textBoxes in a list of points
            for (int j = 0; j < coordinates.Count; j += 2)
            {
                x = Convert.ToInt32(coordinates[j].Text);
                y = Convert.ToInt32(coordinates[j + 1].Text);
                Point tmp = new Point(x, y);

                pointList.Add(tmp);
            }

            int i = 0;//describes where to save new list of coordinates; need to set value for code to work; chosen arbitrary

            // If adding a new curve, it is the last line in the representitive lists
            if (formType == FormMain.FormType.Add)
            {
                i = FormMain.allCurves.Count - 1;
            }

            // If modifying a curve, get its location in the representitive lists
            else if (formType == FormMain.FormType.Modify)
            {
                i = FormMain.localPoint.Item1;
            }

            if (curveType == FormMain.BezierType.cPoints)
            {
                FormMain.cPointsAll[i] = pointList;
                curveAdded = true; // curve was added successfully
            }

            else if (curveType == FormMain.BezierType.pPoints || curveType == FormMain.BezierType.LeastSquares)
            {
                FormMain.pPointsAll[i] = pointList;
                curveAdded = true; // curve was added successfully
            }

            else if (curveType == FormMain.BezierType.Composite && formType == FormMain.FormType.Add)
            {
                FormMain.pPointsAll[i] = pointList;
                curveAdded = true; //c urve was added successfully
            }

            else if (curveType == FormMain.BezierType.Composite && formType == FormMain.FormType.Modify)
            {
                FormMain.ModifypPointComposite(pointList[0]);
            }
            
            this.Close();
        }


        // Delete a row of input textBoxes
        private void btnDeleteRow_Click(object sender, EventArgs e)
        {
            if (curveType == FormMain.BezierType.LeastSquares && tlpCoordinates.RowCount <= 9) // 4 rows minimum plus 5 rows from table design
            {
                MessageBox.Show("<Least Squares> curves can't have less than 4 knot points!");
                return;
            }

            if (curveType == FormMain.BezierType.Composite && tlpCoordinates.RowCount <= 7) // 2 rows minimum plus 5 rows from table design
            {
                MessageBox.Show("<Composite> curves can't have less than 2 knot points!");
                return;
            }


            // Remove all controls from last row
            for (int i = 0; i < tlpCoordinates.ColumnCount; i ++)
            {
                tlpCoordinates.Controls.RemoveAt(tlpCoordinates.Controls.Count - 1);
            }

            // Remove both textBoxes from input list
            coordinates.RemoveAt(coordinates.Count - 1);
            coordinates.RemoveAt(coordinates.Count - 1);

            tlpCoordinates.RowCount --; // Remove last row
            namingCounter--;
        }


        // Add new row of coordinates
        private void btnAddRow_Click(object sender, EventArgs e)
        {
            AddRow();
        }
    }
}