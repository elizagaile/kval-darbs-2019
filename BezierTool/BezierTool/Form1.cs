using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MathNet.Numerics.LinearAlgebra;
using System.IO;


namespace BezierTool
{
    public partial class FormMain : Form //contains form, all its atributes and functions
    {
        public FormMain()
        {
            InitializeComponent();
            this.Width = Convert.ToInt32(0.5 * Screen.PrimaryScreen.Bounds.Width);
            this.Height = Convert.ToInt32(0.75 * Screen.PrimaryScreen.Bounds.Height);
        }

        //each line is representet by two lists: 
        //list of control points, list of known points on the line 
        //all lines have control points, but come lines (<4cPoints>) dont have known points on the line
        //also each line has a drawing type (saved in list allLines) and a parametrization type 
        //the i-th drawn list is represented by i-th position in all of the lists

        private List<Point> cPoints = null;//list of line's control points
        public static List<List<Point>> cPointsAll = new List<List<Point>>();//contains all lists of control points, public so Form2 can access

        private List<Point> pPoints = null;// list of points on line
        public static List<List<Point>> pPointsAll = new List<List<Point>>();// contains all lists of points on line
        
        public static Tuple<int, int> localPoint = null;//location of a selected point in the representitive lists

        private Point cPointNew; //new control point for <4 cPoints> line 

        private enum MoveType { LeftClick, RightClick, pPoints, Nothing };//ways to move <Composite> points by mouse
        private List<MoveType> MovedLine = new List<MoveType>();//saves the last way a line was modified
        
        enum ParamType { Uniform, Chord, Centripetal, Nothing }; // parametrization types
        private List<ParamType> parametrization = new List<ParamType>();//contains parametrization types for drawn lines

        public enum FormType { Add, Modify, Output };

        public enum BezierType { cPoints, pPoints, LeastSquares, Composite, Nothing }; //all posible line types, public for Form2

        public static List<BezierType> allLines = new List<BezierType>();// contains type of drawn lines

        public static BezierType addType = BezierType.Nothing;// type of line to be or being added, public static for Form2
        public static BezierType modifyLineType = BezierType.Nothing;// type of line to be or being modified
        public static BezierType modifyPointType = BezierType.Nothing;// type of line to be or being draged by mouse
        public static BezierType outputPointType = BezierType.Nothing;
        
        String imageLocation = ""; //for background image

        const int pointRadius = 2; //radius for control points and specific points on lines, chosen arbitrary
        const int localRadius = 7; //radius of neiborghood, used when selecting a point with mouse, chosen arbitrary
        const int maxPointCount = 25; //maximum count of points to choose for lines <Least Squares> and <Composite>, chosen arbitrary

        bool isCompositeDone = false;//indicates if the last line of type <Composite> needs to be finished;
        bool canChangeParam = false;//indicates if option to change parametrization is enabled
        bool isChangingParam = false;//indicates if parametrization is being changed
        bool canDeleteLine = false;//indicates if option to delete a line is enabled

        private void pbCanva_MouseDown(object sender, MouseEventArgs e)
            //Mouse has been pressed inside pictureBox1. This can be for adding control points or points on the line,
            //for moving points with mouse or for selecting a line to output point coordinates
        {
            if (addType != BezierType.Nothing && rbMouseInput.Checked == true)
            //if we want to add a new point by mouse            
            {
                if (addType == BezierType.cPoints)
                //if we want to add a new point for a <4 cPoints> type line
                {
                    AddcPoint(e.Location);
                }

                else if (addType == BezierType.pPoints || addType == BezierType.LeastSquares || addType == BezierType.Composite)
                //for <4 pPoints>, <Least Squares> or <Composite> type lines we can add only line points
                {
                    AddpPoint(e.Location);
                }

                pbCanva.Invalidate();
            }

            if (cPointsAll != null && modifyPointType == BezierType.cPoints)
            //if we want to drag a control point
            {
                FindLocalPoint(cPointsAll, e.Location);

                if (localPoint != null)
                {
                    int i = localPoint.Item1;
                    int j = localPoint.Item2;
                    modifyLineType = allLines[i];

                    if (modifyLineType == BezierType.pPoints || modifyLineType == BezierType.LeastSquares)
                    {
                        error.Text = "It's not allowed to move curve's " + modifyLineType + " control points!";
                        modifyLineType = BezierType.Nothing;
                        localPoint = null;
                    }

                    else if (modifyLineType == BezierType.Composite)
                    {
                        if (rbKeyboardModify.Checked == true)
                        {
                            error.Text = "It's not allowed to move curve's " + modifyLineType + " points by keyboard!!";
                            localPoint = null;
                            modifyLineType = BezierType.Nothing;
                        }

                        else if (j % 3 == 0)
                        // every third control point on a composite line is one of the line points (pPoint) 
                        {
                            localPoint = null;
                            modifyLineType = BezierType.Nothing;
                        }

                        else if (e.Button == MouseButtons.Left)
                        {
                            MovedLine[i] = MoveType.LeftClick;
                        }

                        else if (e.Button == MouseButtons.Right)
                        {
                            MovedLine[i] = MoveType.RightClick;
                        }
                    }

                    else if (rbKeyboardModify.Checked == true)
                    {
                        FormCoordinates form_KeyboardAdd = new FormCoordinates(FormType.Modify, modifyLineType);
                        form_KeyboardAdd.ShowDialog();
                        
                        MovedLine[i] = MoveType.pPoints;
                        modifyLineType = BezierType.Nothing;
                        localPoint = null;
                    }
                }

                pbCanva.Invalidate();
            }

            if (pPointsAll != null && modifyPointType == BezierType.pPoints)
            //if we want to drag a line point
            {
                FindLocalPoint(pPointsAll, e.Location);

                if (localPoint != null)
                {
                    int i = localPoint.Item1;
                    modifyLineType = allLines[i];

                    if (rbKeyboardModify.Checked == true)
                    {
                        FormCoordinates form_KeyboardAdd = new FormCoordinates(FormType.Modify, modifyLineType);
                        form_KeyboardAdd.ShowDialog();

                        if (modifyLineType == BezierType.pPoints || modifyLineType == BezierType.LeastSquares)
                        {
                            GetcPointsInterpolation(i);
                        }

                        modifyLineType = BezierType.Nothing;
                        localPoint = null;
                        pbCanva.Invalidate();
                    }
                }

                pbCanva.Invalidate();
            }

            if (cPointsAll != null && outputPointType == BezierType.cPoints && rbScreenOutput.Checked == true)
            // if we want to output line's control point coordinates on screen
            {
                FindLocalPoint(cPointsAll, e.Location);

                if (localPoint != null)
                {
                    int i = localPoint.Item1;

                    FormCoordinates form_KeyboardAdd = new FormCoordinates(FormType.Output, allLines[i]);
                    form_KeyboardAdd.ShowDialog();
                    
                    outputPointType = BezierType.Nothing;
                    modifyLineType = BezierType.Nothing;
                    localPoint = null;
                }

                pbCanva.Invalidate();
            }

            if (pPointsAll != null && outputPointType == BezierType.pPoints && rbScreenOutput.Checked == true)
            //if we want to output coordinates of known line points on screen
            {
                FindLocalPoint(pPointsAll, e.Location);

                if (localPoint != null)
                {
                    int i = localPoint.Item1;

                    FormCoordinates form_KeyboardAdd = new FormCoordinates(FormType.Output, allLines[i]);
                    form_KeyboardAdd.ShowDialog();

                    outputPointType = BezierType.Nothing;
                    modifyLineType = BezierType.Nothing;
                    localPoint = null;
                }

                pbCanva.Invalidate();
            }

            if (cPointsAll != null && outputPointType == BezierType.cPoints && rbFileOutput.Checked == true)
            // if we want to output line's control point coordinates to .txt file
            {
                FindLocalPoint(cPointsAll, e.Location);

                if (localPoint != null)
                {
                    int i = localPoint.Item1;

                    OutputcPointsFile(i);
                    outputPointType = BezierType.Nothing;
                    modifyLineType = BezierType.Nothing;
                    localPoint = null;
                }

                pbCanva.Invalidate();
            }

            if (pPointsAll != null && outputPointType == BezierType.pPoints && rbFileOutput.Checked == true)
            // if we want to output lines's line point coordinates to .txt file
            {
                FindLocalPoint(pPointsAll, e.Location);

                if (localPoint != null)
                {
                    int i = localPoint.Item1;

                    OutputpPointsFile(i);
                    outputPointType = BezierType.Nothing;
                    modifyLineType = BezierType.Nothing;
                    localPoint = null;
                }

                pbCanva.Invalidate();
            }

            if (cPointsAll != null && canChangeParam == true && isChangingParam == false)
            //if we want to change parametrization type
            {
                FindLocalPoint(cPointsAll, e.Location);

                if (localPoint == null)
                //mouse wasn't clicked near a control point
                {
                    return;
                }

                int i = localPoint.Item1;
                ParamType paramType = parametrization[i];

                if (allLines[i] == BezierType.cPoints || allLines[i] == BezierType.Composite)
                {
                    error.Text = "<" + allLines[i] + "> lines doesn't use parametrization!";
                    return;
                }

                //show the real parametrization type of the selected line
                if (paramType == ParamType.Uniform)
                {
                    rbUniform.Checked = true;
                }

                else if (paramType == ParamType.Chord)
                {
                    rbChord.Checked = true;
                }

                else if (paramType == ParamType.Centripetal)
                {
                    rbCentripetal.Checked = true;
                }

                isChangingParam = true;
            }

            if (cPointsAll != null && canDeleteLine == true)
            {
                FindLocalPoint(cPointsAll, e.Location);

                if (localPoint == null)
                //mouse wasn't clicked near a control point
                {
                    return;
                }

                int i = localPoint.Item1;

                string message = "Do you want to delete this line?";
                string title = "Delete line";
                MessageBoxButtons buttons = MessageBoxButtons.OKCancel;
                DialogResult result = MessageBox.Show(message, title, buttons);
                
                if (result == DialogResult.OK)
                {
                    DeleteLine(i);
                }

                pbCanva.Invalidate();
            }
        }

        private void pbCanva_MouseMove(object sender, MouseEventArgs e)
            //Mouse is moving inside pictureBox1. This is used to draw dashed line when adding new points for <4 cPoints> line
            // or it can be used for modifying a line by mouse.
        {
            if (addType == BezierType.cPoints)
            // if we are adding a new control point for <4 cPoints> line 
            {
                cPointNew = e.Location;
                pbCanva.Invalidate();
            }

            int i = 0;//need to set a value for code to work, chose 0 arbitrary
            int j = 0;

            if (localPoint != null)
            {
                i = localPoint.Item1;
                j = localPoint.Item2;
            }

            if (modifyLineType == BezierType.cPoints)
            // for type <4 cPoints> line we can just change coordinates
            {
                cPointsAll[i][j] = e.Location;
                pbCanva.Invalidate();
            }

            else if ((modifyLineType == BezierType.pPoints || modifyLineType == BezierType.LeastSquares) && modifyPointType == BezierType.pPoints)
            //if we change type <4 pPoints> or <Least Squares> line points, we need to re-calculate its control points
            {
                pPointsAll[i][j] = e.Location;
                GetcPointsInterpolation(i);
                pbCanva.Invalidate();
            }

            else if (modifyLineType == BezierType.Composite && modifyPointType == BezierType.cPoints)
            // if we change type <Composite> line's control points, we need to make sure the line stays C2 continuous
            {
                if (MovedLine[i] == MoveType.LeftClick)
                // using left click, we can drag the control point anywhere, but the 'opposite' control point moves aswell
                //to maintain continuity
                {
                    cPointsAll[i][j] = e.Location;

                    if (j % 3 == 1 && j != 1)
                    //starting from the fifth control point, every third point's opposite control point is two points before
                    {
                        ModifycPointComposite(cPointsAll[i][j], cPointsAll[i][j - 1], cPointsAll[i][j - 2], j - 2);
                    }

                    if (j % 3 == 2 && j != cPointsAll[i].Count - 2)
                    //starting from the third control point, every third point's opposite control point is two points after
                    {
                        ModifycPointComposite(cPointsAll[i][j], cPointsAll[i][j + 1], cPointsAll[i][j + 2], j + 2);
                    }
                }

                if (MovedLine[i] == MoveType.RightClick)
                // using right click no other control points will move, but to maintain continuity, we can only move the
                // control point in straight line away from its opposite point
                {
                    if (j % 3 == 1 && j != 1)
                    //starting from the fifth control point, every third point's opposite control point is two points before
                    {
                        ModifycPointCompositeStraight(e.Location, cPointsAll[i][j - 1], cPointsAll[i][j - 2]);
                    }

                    if (j % 3 == 2 && j != cPointsAll[i].Count - 2)
                    //starting from the third control point, every third point's opposite control point is two points after
                    {
                        ModifycPointCompositeStraight(e.Location, cPointsAll[i][j + 1], cPointsAll[i][j + 2]);
                    }
                }
                pbCanva.Invalidate();
            }

            else if (modifyLineType == BezierType.Composite && modifyPointType == BezierType.pPoints)
            {
                ModifypPointComposite(e.Location);
                
                MovedLine[i] = MoveType.pPoints;
                pbCanva.Invalidate();
            }
        }

        private void pbCanva_MouseUp(object sender, MouseEventArgs e)
            //Mouse button was released after pressing it in pictureBox1. If a point was being dragged by mouse, the dragging stops.
        {
            if (modifyPointType != BezierType.Nothing)
            {
                modifyLineType = BezierType.Nothing;
                localPoint = null;
            }
            pbCanva.Invalidate();
        }

        private void pbCanva_Paint(object sender, PaintEventArgs e)
            //Draws all graphics in this programm - all bezier functions, straight lines and points, and calls for functions to get needed control points
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;//makes lines look smoother

            if (cPointsAll == null || pPointsAll == null)
            {
                return;
            }

            if (cPoints != null)
            //if we are selecting points for <4 cPoints> line, draw a dashed line from mouse location to previous control point
            {
                if (cPoints.Count < 4 && addType == BezierType.cPoints)
                //<4 cPoints> line cant have more than 4 control points
                {
                    using (Pen dashedPen = new Pen(Color.LightGray))
                    {
                        dashedPen.DashPattern = new float[] { 5, 5 }; // 5 and 5 chosen arbitrary, describes length of dashed design
                        e.Graphics.DrawLine(dashedPen, cPoints[cPoints.Count - 1], cPointNew);
                    }
                }
            }

            for (int i = 0; i < pPointsAll.Count; i++)
            //go through every list of line points
            {
                if (pPointsAll[i] != null)
                {
                    foreach (Point pPoint in pPointsAll[i])
                    //draw a black point for every line point
                    {
                        e.Graphics.FillEllipse(Brushes.Black, pPoint.X - pointRadius, pPoint.Y - pointRadius, 2 * pointRadius, 2 * pointRadius);
                    }

                    if (allLines[i] == BezierType.pPoints && pPointsAll[i].Count == 4 && cPointsAll[i] == null)
                    //if there are four line points for <4 pPoints> line, but we haven't yet calculated control points, calculate them
                    {
                        GetcPointsInterpolation(i);
                    }

                    if (allLines[i] == BezierType.LeastSquares && pPointsAll[i].Count >= 4)
                    //if there are at least four line points for <Least Square> line, calculate control points
                    {
                        GetcPointsInterpolation(i);
                    }

                    if (allLines[i] == BezierType.Composite && MovedLine[i] == MoveType.Nothing)
                    // if we want to draw a <Composite> line which hasn't been moved
                    {
                        if (isCompositeDone == true && pPointsAll[i].Count == 2)
                        //finish and draw a <Composite> line with only two line points
                        {
                            cPointsAll[i] = new List<Point>();
                            AddOnlycPointsComposite(i);
                        }

                        else if (pPointsAll[i].Count >= 3)
                        //if a <Composite> line has more than 3 line points, we can calculate its control points
                        {
                            cPointsAll[i] = new List<Point>();
                            AddcPointsComposite(i);
                        }
                    }
                }
            }
            
            for (int i = 0; i < cPointsAll.Count; i++)
            // go through every list of control points
            {
                if (cPointsAll[i] != null)
                {
                    //Drawing control points:

                    if (allLines[i] == BezierType.cPoints || allLines[i] == BezierType.LeastSquares)
                    // for <4 cPoints> and <Least Squares> draw all control points
                    {
                        foreach (Point cPoint in cPointsAll[i])
                        {
                            e.Graphics.DrawEllipse(Pens.Red, cPoint.X - pointRadius, cPoint.Y - pointRadius, 2 * pointRadius, 2 * pointRadius);
                        }
                    }

                    else if (allLines[i] == BezierType.pPoints)
                    //for <4 pPoints> draw only middle control points, because end points ar both control points and line points
                    {
                        e.Graphics.DrawEllipse(Pens.Red, cPointsAll[i][1].X - pointRadius, cPointsAll[i][1].Y - pointRadius, 2 * pointRadius, 2 * pointRadius);
                        e.Graphics.DrawEllipse(Pens.Red, cPointsAll[i][2].X - pointRadius, cPointsAll[i][2].Y - pointRadius, 2 * pointRadius, 2 * pointRadius);
                    }

                    else if (allLines[i] == BezierType.Composite)
                    // for <Composite> draw only those control points, which are not line points - every third line point starting from the first is also a control point
                    {
                        for (int j = 0; j < cPointsAll[i].Count - 1; j++)
                        {
                            if (j % 3 != 2)
                            {
                                e.Graphics.DrawEllipse(Pens.Red, cPointsAll[i][j + 1].X - pointRadius, cPointsAll[i][j + 1].Y - pointRadius, 2 * pointRadius, 2 * pointRadius);
                            }
                        }
                    }


                    //Drawing control point polygons / "handle" lines

                    if (cPointsAll[i].Count > 1 && (allLines[i] == BezierType.cPoints || allLines[i] == BezierType.LeastSquares || allLines[i] == BezierType.pPoints))
                    {
                        e.Graphics.DrawLines(Pens.LightGray, cPointsAll[i].ToArray());
                    }

                    else if (allLines[i] == BezierType.Composite)
                    //for <Composite> lines, draw only handles
                    {
                        for (int j = 0; j < cPointsAll[i].Count - 1; j++)
                        {
                            if (j % 3 != 1)
                            {
                                e.Graphics.DrawLine(Pens.LightGray, cPointsAll[i][j], cPointsAll[i][j + 1]);
                            }
                        }
                    }


                    //Drawing all bezier lines:

                    if (cPointsAll[i].Count == 4 && (allLines[i] == BezierType.cPoints || allLines[i] == BezierType.LeastSquares || allLines[i] == BezierType.pPoints))
                    {
                        e.Graphics.DrawBezier(Pens.Black, cPointsAll[i][0], cPointsAll[i][1], cPointsAll[i][2], cPointsAll[i][3]);
                    }

                    else if (allLines[i] == BezierType.Composite)
                    {
                        for (int j = 0; j < cPointsAll[i].Count - 3; j += 3)
                        {
                            e.Graphics.DrawBezier(Pens.Black, cPointsAll[i][j], cPointsAll[i][j + 1], cPointsAll[i][j + 2], cPointsAll[i][j + 3]);
                        }
                    }
                }
            }
        }

        private void btnUploadBackground_Click(object sender, EventArgs e)
            //uploads background image
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog //types of files allowed ???
                {
                    Filter = "jpg files(.*jpg)|*.jpg| PNG files(.*png)|*.png| All Files(*.*)|*.*"
                };

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    imageLocation = dialog.FileName;
                    pbCanva.ImageLocation = imageLocation;
                    cbShowBackground.Checked = true;
                }
            }

            catch (Exception)
            {
                MessageBox.Show("Image upload error!");
            }
        }

        private void cbShowBackground_CheckStateChanged(object sender, EventArgs e)
            //make uploaded background picture visable or invisible
        {
            if (cbShowBackground.Checked == false)
            {
                pbCanva.ImageLocation = "";
            }

            else
            {
                pbCanva.ImageLocation = imageLocation;
            }
        }

        private void FormMain_Resize(object sender, EventArgs e)
            //make form responsive
        {
            int formWidth = this.Width;
            int formHeight = this.Height;

            panel_tools.Left = formWidth - panel_tools.Width - 20; // 20px, 50px, 35 px and 55px makes margins
            panel_bottom.Left = formWidth - panel_bottom.Width - 20;
            panel_bottom.Top = formHeight - panel_bottom.Height - 50;
            pbCanva.Width = formWidth - panel_tools.Width - 35;
            pbCanva.Height = formHeight - 55;
        }

        private void NewLine(BezierType lineType)
            //start a new line
        {
            allLines.Add(lineType);

            cPoints = null;
            pPoints = null;
            modifyPointType = BezierType.Nothing;
            modifyLineType = BezierType.Nothing;
            localPoint = null;
            isChangingParam = false;
            isCompositeDone = false;
            MovedLine.Add(MoveType.Nothing);
            cPointsAll.Add(null);
            pPointsAll.Add(null);

            if (lineType == BezierType.cPoints || lineType == BezierType.Composite)
            {
                parametrization.Add(ParamType.Nothing);
            }

            else if (lineType == BezierType.pPoints || lineType == BezierType.LeastSquares)
            {
                if (rbUniform.Checked == true)
                {
                    parametrization.Add(ParamType.Uniform);
                }

                else if (rbChord.Checked == true)
                {
                    parametrization.Add(ParamType.Chord);
                }

                else if (rbCentripetal.Checked == true)
                {
                    parametrization.Add(ParamType.Centripetal);
                }
            }

            return;
        }

        private void btnDeleteLine_Click(object sender, EventArgs e)
            //enable option to delete lines
        {
            canDeleteLine = true;
        }

        private void DeleteLine(int i)
            //remove everything function newLine() added; used when adding new line by keybord is canceled
        {
            addType = BezierType.Nothing;
            allLines.RemoveAt(i);
            MovedLine.RemoveAt(i);
            cPointsAll.RemoveAt(i);
            pPointsAll.RemoveAt(i);
            parametrization.RemoveAt(i);

            canDeleteLine = false;

            return;
        }

        private void btnAdd4cPoints_Click(object sender, EventArgs e)
            //start a new line of type <4 cPoints>
        {
            addType = BezierType.cPoints;

            if (rbMouseInput.Checked == true)
            //adding new line with mouse
            {
                cPoints = null;
            }

            if (rbKeyboardInput.Checked == true)
            //adding new line by keyboard
            {
                NewLine(BezierType.cPoints);

                FormCoordinates form_KeyboardAdd = new FormCoordinates(FormType.Add, addType);
                form_KeyboardAdd.ShowDialog();

                if (FormCoordinates.lineAdded == false) 
                    //an error or cancelation occured and no line was added
                {
                    DeleteLine(allLines.Count - 1);//delete what newLine() added
                    return;
                }

                pbCanva.Invalidate();
            }

            if (rbFileInput.Checked == true)
            //adding new line from a .txt file
            {
                cPoints = GetPointsfromFile();

                if (cPoints.Count != 4)
                {
                    error.Text = ".txt file was not correct!";
                    DeleteLine(allLines.Count - 1);
                    return;
                }

                cPointsAll[cPointsAll.Count - 1] = cPoints;
                pbCanva.Invalidate();
            }
        }

        private void btnAdd4pPoints_Click(object sender, EventArgs e)
            //start a new line of type <4 pPoints>
        {
            addType = BezierType.pPoints;

            if (rbMouseInput.Checked == true)
            {
                pPoints = null;
            }

            if (rbKeyboardInput.Checked == true)
                //if adding new line by keyboard
            {
                NewLine(BezierType.cPoints);

                FormCoordinates form_KeyboardAdd = new FormCoordinates(FormType.Add, addType);
                form_KeyboardAdd.ShowDialog();

                if (FormCoordinates.lineAdded == false)
                    //an error or cancelation occured and no line was added
                {
                    DeleteLine(allLines.Count - 1);//delete what newLine() added
                    return;
                }

                pbCanva.Invalidate();
            }

            if (rbFileInput.Checked == true)
            //if adding new line from a .txt file
            {
                pPoints = GetPointsfromFile();

                if (pPoints.Count != 4)
                {
                    error.Text = ".txt file was not correct!";
                    DeleteLine(allLines.Count - 1);
                    return;
                }

                pPointsAll[pPointsAll.Count - 1] = pPoints;
                pbCanva.Invalidate();
            }
        }

        private void btnAddLeastSquares_Click(object sender, EventArgs e)
            //start a new line of type <4 pPoints>
        {
            addType = BezierType.LeastSquares;

            if (rbMouseInput.Checked == true)
            {
                pPoints = null;
            }

            if (rbKeyboardInput.Checked == true)
            //if adding new line by keyboard
            {
                NewLine(BezierType.cPoints);

                FormCoordinates form_KeyboardAdd = new FormCoordinates(FormType.Add, addType);
                form_KeyboardAdd.ShowDialog();

                if (FormCoordinates.lineAdded == false)
                //an error or cancelation occured and no line was added
                {
                    DeleteLine(allLines.Count - 1);//delete what newLine() added
                    return;
                }

                pbCanva.Invalidate();
            }

            if (rbFileInput.Checked == true)
            //if adding new line from a .txt file
            {
                pPoints = GetPointsfromFile();

                if (pPoints.Count < 4 || pPoints.Count > maxPointCount)
                {
                    error.Text = ".txt file was not correct!";
                    DeleteLine(allLines.Count - 1);
                    return;
                }

                pPointsAll[pPointsAll.Count - 1] = pPoints;
                pbCanva.Invalidate();
            }
        }

        private void btnAddComposite_Click(object sender, EventArgs e)
            //start a new line of type <Composite>
        {
            addType = BezierType.Composite;

            if (rbMouseInput.Checked == true)
            {
                pPoints = null;
            }

            if (rbKeyboardInput.Checked == true)
            //if adding new line by keyboard
            {
                NewLine(BezierType.cPoints);

                FormCoordinates form_KeyboardAdd = new FormCoordinates(FormType.Add, addType);
                form_KeyboardAdd.ShowDialog();

                if (FormCoordinates.lineAdded == false)
                //an error or cancelation occured and no line was added
                {
                    DeleteLine(allLines.Count - 1);//delete what newLine() added
                    return;
                }

                isCompositeDone = true;

                pbCanva.Invalidate();
            }

            if (rbFileInput.Checked == true)
            //if adding new line from a .txt file
            {
                pPoints = GetPointsfromFile();

                if (pPoints.Count < 2 || pPoints.Count > maxPointCount)
                {
                    error.Text = ".txt file was not correct!";
                    DeleteLine(allLines.Count - 1);
                    return;
                }

                pPointsAll[pPointsAll.Count - 1] = pPoints;
                isCompositeDone = true;
                pbCanva.Invalidate();
            }
        }

        private void btnDoneComposite_Click(object sender, EventArgs e)
            //finish the <Composite> line being drawed - draw last segment of it
        {
            isCompositeDone = true;
            pbCanva.Invalidate();
        }

        private void AddcPoint(Point mouseLocation)
            //add new control point to the current line
        {
            if (cPoints == null)
            //if this is the first point of line
            {
                NewLine(addType);
                cPoints = new List<Point> { mouseLocation };
                cPointsAll[cPointsAll.Count - 1] = cPoints;
            }

            else if (cPoints.Count < 4 && cPoints[cPoints.Count - 1] != mouseLocation)
            //to avoid accidental double clicks
            {
                cPoints.Add(mouseLocation);
            }

            return;
        }

        private void AddpPoint(Point mouseLocation)
            //add new point on line to the current line
        {
            if (pPoints == null)
            //if this is the first point of line
            {
                NewLine(addType);
                pPoints = new List<Point> { mouseLocation };
                pPointsAll[pPointsAll.Count - 1] = pPoints;
                
                return;
            }

            if (pPoints[pPoints.Count - 1] == mouseLocation)
            //to avoid accidental double clicks
            {
                return;
            }

            if (addType == BezierType.pPoints && pPoints.Count >= 4)
            //type <4 pPoints can't have more than 4 chosen points on line)
            {
                return;
            }

            if (addType == BezierType.Composite && isCompositeDone == true)
            // can't add points to a <Composite> line that has been finished
            {
                return;
            }

            else if ((addType == BezierType.LeastSquares || addType == BezierType.Composite) && pPoints.Count > maxPointCount)
            // can't choose more points than the maximum allowed count
            {
                return;
            }

            pPoints.Add(mouseLocation);
            return;
        }

        private void AddcPointsComposite(int i)
            //calculate control points for a <Composite> line with three or more line points
        {
            cPointsAll[i].Add(pPointsAll[i][0]);//first control point is first line point
            AddFirstcPointComposite(i);//add first control point that isn't a line point

            for (int j = 2; j < pPointsAll[i].Count; j++)
            //we can add three new controlpoints for every line point starting with the third line point. 
            //Every line point is also a control point and for every but first andl last point, we get two control points - "handles"
            {
                cPointsAll[i].Add(GetFirstHandleComposite(pPointsAll[i][j - 2], pPointsAll[i][j - 1], pPointsAll[i][j]));
                cPointsAll[i].Add(pPointsAll[i][j - 1]);
                cPointsAll[i].Add(GetSecondHandleComposite(pPointsAll[i][j - 2], pPointsAll[i][j - 1], pPointsAll[i][j]));
            }

            if (i != allLines.Count - 1 && cPointsAll[i].Count < pPointsAll[i].Count * 3 - 2)
            //if a <Composite> line isn't the last drawn line, it must be finished. 
            //That means, it should have three times (every point is a control point and has two "handles") minus two (each end point dont have one handle) more control points than line points
            {
                AddLastcPointsComposite(i);
            }

            else if (i == allLines.Count - 1 && isCompositeDone == true)
            //if the last drawn <Composite> line is marked as done, calculate and add last control points
            {
                AddLastcPointsComposite(i);
            }

            return;
        }
        
        private void AddFirstcPointComposite(int i)
            //add first control point thats not a line point for <Composite> line with at least three line points
        {
            Point c1 = new Point();
            Point c2 = new Point();
            Point c3 = new Point();
            Point c4 = new Point();

            //control point location is calculated from first, third and fourth control points of the  <Composite> line

            c1 = pPointsAll[i][0];
            c3 = GetFirstHandleComposite(pPointsAll[i][0], pPointsAll[i][1], pPointsAll[i][2]);
            c4 = pPointsAll[i][1];

            //We can look at these calculations as vector operations. First we calculate dot product of vectors C4C3 and C1C4:
            double dot = (c3.X - c4.X) * (c1.X - c4.X) + (c3.Y - c4.Y) * (c1.Y - c4.Y);

            //We need to find how long the vector from C2 to C3 needs to be, so that the middle control points are symmetrical.
            //The symmetry can be achieved, if the C3C2 vector is parallel to C4C1 and has the length of 
            //length(C4C1) - 2*length(projection of vector C4C3 on to C4C1). One projection length for each side.
            //Using projection formula, we get: proportion = |C4C1| - 2 * dot / |C4C1| . We will multiply this proportion by unit vector
            //parallel to vector C4C1, which can be expressed as C4C1 / |C4C1|. If we devide our proportion with |C4C1| from the unit vector,
            //we get: proportion = 1 - 2 * dot / |C4C1|^2

            //That means, the length of the vector we will add equals 
            double prop = 1 - 2 * dot / (Math.Pow(GetLength(c1, c4), 2));

            //Lastly, to point C3 we add vector parallel to C1C4 scaled by the needed length - variable "prop":
            c2.X = Convert.ToInt32(c3.X + prop * (c1.X - c4.X));
            c2.Y = Convert.ToInt32(c3.Y + prop * (c1.Y - c4.Y));

            //We have achieved a "symmetrical" point to third control point, both of these points are on the same side of the bezier line.

            cPointsAll[i].Add(c2);

            return;
        }

        private Point GetFirstHandleComposite(Point prevpPoint, Point thispPoint, Point nextpPoint)
            //Calculate first handle (first middle control point) coordinates for <Composite> lines, to ensure C2 continuity.
        {
            Point res = new Point();
            double lengthPrevThis = GetLength(prevpPoint, thispPoint);
            double lengthThisNext = GetLength(thispPoint, nextpPoint);

            //Distance from first to second handle is half the distance from first given line point (a) to the last(c).
            //The proportions of each handle is the same as proportion ab/bc, where b is the middle line point.
            //Methods of calculation for distances can be different and there isn't one best method. 
            //I have discovered that this method works nice most of the time and isn't expesive.

            double proportion = 0.5 * lengthPrevThis / (lengthPrevThis + lengthThisNext);

            res.X = thispPoint.X + Convert.ToInt32(proportion * (prevpPoint.X - nextpPoint.X));
            res.Y = thispPoint.Y + Convert.ToInt32(proportion * (prevpPoint.Y - nextpPoint.Y));

            return res;
        }

        private Point GetSecondHandleComposite(Point prevpPoint, Point thispPoint, Point nextpPoint)
            //Calculate second handle (second middle control point) coordinates for <Composite> lines, to ensure C2 continuity.
        {
            Point res = new Point();
            double lengthPrevThis = GetLength(prevpPoint, thispPoint);
            double lengthThisNext = GetLength(thispPoint, nextpPoint);

            //Calculations are very similar to those in the function GetFirstHandle.

            double proportion = 0.5 * lengthThisNext / (lengthPrevThis + lengthThisNext);

            res.X = thispPoint.X + Convert.ToInt32(proportion * (nextpPoint.X - prevpPoint.X));
            res.Y = thispPoint.Y + Convert.ToInt32(proportion * (nextpPoint.Y - prevpPoint.Y));

            return res;
        }
        
        private void AddLastcPointsComposite(int i)
            //add two last control points to a <Composite> line that''
        {
            Point c1 = new Point();
            Point c2 = new Point();
            Point c3 = new Point();
            Point c4 = new Point();

            //control point location is calculated from first, second and fourth control point of the last segment in the <Comoposite> line

            c1 = pPointsAll[i][pPointsAll[i].Count - 2];
            c2 = cPointsAll[i][cPointsAll[i].Count - 1];
            c4 = pPointsAll[i][pPointsAll[i].Count - 1];

            //We can look at these calculations as vector operations. First we calculate dot product of vectors C4C2 and C1C4:
            double dot = (c2.X - c4.X) * (c1.X - c4.X) + (c2.Y - c4.Y) * (c1.Y - c4.Y);

            //To find how long the vector from C2 to C3 needs to be, we find the the proportion:
            //(for more information on the calculation, see function AddFirstcPointComposite();)
            double prop = 1 - 2 * dot / (Math.Pow(GetLength(c1, c4), 2));

            //Lastly, to point C2 we add vector parallel to C1C4 scaled by the needed length - variable "prop":
            c3.X = Convert.ToInt32(prop * (c1.X - c4.X) + c2.X);
            c3.Y = Convert.ToInt32(prop * (c1.Y - c4.Y) + c2.Y);

            //We have achieved a "symmetrical" point to second control point, both of these points are on the same side of the bezier line.

            //We add the calculated point as wall as the last control point - the last line point:
            cPointsAll[i].Add(c3);
            cPointsAll[i].Add(c4);

            pbCanva.Invalidate();
            return;
        }

        private void AddOnlycPointsComposite(int i)
            //add all control points to a <Composite> line thats marked as "done", but has only two line points
        {
            Point c1 = new Point();
            Point c2 = new Point();
            Point c3 = new Point();
            Point c4 = new Point();

            //the first and last control points are the line points:
            c1 = pPointsAll[i][0];
            c4 = pPointsAll[i][1];

            double sin60 = Math.Sin(Math.PI / 3);
            double cos60 = Math.Cos(Math.PI / 3);

            //Each control point will be line's C1C4 midpoint, rotated by 60 degrees. First we find the midpoint:
            double x03 = 0.5 * (c4.X - c1.X);
            double y03 = 0.5 * (c4.Y - c1.Y);

            //Then we rotate the midpoint by 60 degrees.
            c2.X = Convert.ToInt32(cos60 * x03 - sin60 * y03 + c1.X);
            c2.Y = Convert.ToInt32(sin60 * x03 + cos60 * y03 + c1.Y);

            //Change the signs for third control point, so the control points are on different sides of the bezier line:
            c3.X = Convert.ToInt32(cos60 * -x03 - sin60 * -y03 + c4.X);
            c3.Y = Convert.ToInt32(sin60 * -x03 + cos60 * -y03 + c4.Y);

            cPointsAll[i].Add(c1);
            cPointsAll[i].Add(c2);
            cPointsAll[i].Add(c3);
            cPointsAll[i].Add(c4);

            pbCanva.Invalidate();
            return;
        }

        private List<Point> GetPointsfromFile()
            //choose a .txt file and make a list of points from .it
        {
            List<Point> pointList = new List<Point>();
            Point point = new Point();
            
            string path = "";
            string textLine = "";

            OpenFileDialog dialog = new OpenFileDialog
            {
                Title = "Open Text File",
                Filter = "TXT files|*.txt",
                InitialDirectory = @"C:\"
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                path = dialog.FileName;
            }

            if (File.Exists(path))
            {
                NewLine(addType);
                using (StreamReader file = new StreamReader(path))
                {
                    while ((textLine = file.ReadLine()) != null)
                    {
                        int index = textLine.IndexOf(' ');
                        string xCoordinate = textLine.Substring(0, index);
                        string yCoordinate = textLine.Substring(index + 1);

                        point.X = Convert.ToInt32(xCoordinate);
                        point.Y = Convert.ToInt32(yCoordinate);

                        pointList.Add(point);
                    }
                }
            }

            return pointList;
        }

        private double GetLength(Point firstPoint, Point secondPoint)
            //get length between two points
        {
            return Math.Sqrt(Math.Pow(firstPoint.X - secondPoint.X, 2) + Math.Pow(firstPoint.Y - secondPoint.Y, 2));
        }

        private void FindLocalPoint(List<List<Point>> PointsAll, Point MouseLocation)
            //find if there is a control or line point near mouse location
        {
            for (int i = 0; i < PointsAll.Count; i++)
            // go through every list of points in the given list
            {
                if (PointsAll[i] != null)
                {
                    for (int j = 0; j < PointsAll[i].Count; j++)
                    // go through every point in the list
                    {
                        if (GetLength(MouseLocation, PointsAll[i][j]) < localRadius)
                        //mouse if in a neighborhood of some point, so we asume it this point was clicked
                        {
                            //modifyLineType = allLines[i];
                            localPoint = new Tuple<int, int>(i, j);
                        }
                    }
                }
            }

            return;
        }

        private void GetcPointsInterpolation(int i)
            //Calculate control points for lines, where only line points are know - <4 pPoints> and <Least Squares>.
        {
            List<Point> pList = pPointsAll[i];

            //This method of curve fitting uses least squares method, so that distance errors from given line points to the Bezier curve 
            // at respective t values is the smallest possible. For more calculation information see https://pomax.github.io/bezierinfo/#curvefitting.
            //or my documentation???? man ir uzrakstits latexa pieradijums sajai metodei, nezinu, cik vajadzigi tas seit ir.

            //We will represent Bezier curve in matrix form.

            //Matrix M contains coefficients in an expanded Bezier curve function. We will only use cubic Bezier curves, therefor M always is:
            var matrix = Matrix<double>.Build;
            double[,] arrayM4 = new double[4, 4]
                { { 1, 0, 0, 0 }, { -3, 3, 0, 0 }, { 3, -6, 3, 0 }, { -1, 3, -3, 1 } };

            //Matrix P contains coordinates of all line points:
            double[,] arrayP = new double[pList.Count, 2];
            for (int j = 0; j < pList.Count; j++)
            {
                arrayP[j, 0] = pList[j].X;
                arrayP[j, 1] = pList[j].Y;
            }

            var matrixP = matrix.DenseOfArray(arrayP);
            var matrixM4 = matrix.DenseOfArray(arrayM4);
            var matrixM4Inv = matrixM4.Inverse();

            //Bezier curves are parametric, so we need appropriate t values for each line point to tie together coordinates with points o curve B(t). 
            //This parametrization can be done in different ways; we will store the resulting t values in a list sPoints.
            List<double> sPoints = new List<double>();

            if (parametrization[i] == ParamType.Uniform)
            {
                sPoints = GetsPointsUniform(pList);
            }

            else if (parametrization[i] == ParamType.Chord)
            {
                sPoints = GetsPointsChord(pList);
            }

            else if (parametrization[i] == ParamType.Centripetal)
            {
                sPoints = GetsPointsCentripetal(pList);
            }

            var matrixS = matrix.DenseOfArray(GetArrayS(sPoints));
            var matrixSTranspose = matrixS.Transpose();
            var matrixSSTr = matrixSTranspose * matrixS;
            var matrixSSTrInv = matrixSSTr.Inverse();

            var matrixMul1 = matrixM4Inv * matrixSSTrInv;
            var matrixMul2 = matrixMul1 * matrixSTranspose;

            var matrixC = matrixMul2 * matrixP;

            if (cPointsAll[i] == null)
            //if we are not modifying a line, this is the first time calculating control points
            {
                cPoints = new List<Point>();

                for (int j = 0; j < 4; j++)
                {
                    Point tmp = new Point(Convert.ToInt32(matrixC[j, 0]), Convert.ToInt32(matrixC[j, 1]));
                    cPoints.Add(tmp);
                }
                cPointsAll[i] = cPoints;
            }

            else
            //else we need to replace the old control point coordinates by the new ones
            {
                for (int j = 0; j < 4; j++)
                {
                    Point tmp = new Point(Convert.ToInt32(matrixC[j, 0]), Convert.ToInt32(matrixC[j, 1]));
                    cPointsAll[i][j] = tmp;
                }
            }

            return;
        }

        private List<double> GetsPointsUniform(List<Point> pList)
            //a way of Bezier curve parametrization, where t values are equally spaced
        {
            List<double> sPoints = new List<double>();

            for (int i = 0; i < pList.Count; i++)
            {
                double s = (double)i / (pList.Count - 1);
                sPoints.Add(s);
            }
            return (sPoints);
        }

        private List<double> GetsPointsChord(List<Point> pList)
            //a way of Bezier curve parametrization, where t values are aligned with distance along the polygon
        {
            //At the first point, we're fixing t = 0, at the last point t = 1. Anywhere in between t value is equal to the distance
            //along the polygon (made from control points), scaled to the [0,1] domain.

            List<double> sPoints = new List<double>();

            //First we calculate distance along the polygon for each point:
            List<double> dPoints = new List<double> { 0 };
            for (int i = 1; i < pList.Count; i++)
            {
                double d = dPoints[i - 1] + GetLength(pList[i - 1], pList[i]);
                dPoints.Add(d);
            }

            //Then we scale these values to [0, 1] domain:
            for (int i = 0; i < pList.Count; i++)
            {
                double s = dPoints[i] / dPoints[pList.Count - 1];
                sPoints.Add(s);
            }

            return (sPoints);
        }

        private List<double> GetsPointsCentripetal(List<Point> pList)
            //a way of Bezier curve parametrization, where t values are aligned with square root of the distance along the polygon
        {
            //At the first point, we're fixing t = 0. Anywhere in between t value is equal to the square root of the 
            //distance along the polygon (made from control points), scaled to [0,1] domain.

            List<double> sPoints = new List<double>();

            //First we calculate the square root of distance along the polygon for each point:
            List<double> dPoints = new List<double> { 0 };
            for (int i = 1; i < pList.Count; i++)
            {
                double d = dPoints[i - 1] + Math.Sqrt(GetLength(pList[i - 1], pList[i]));
                dPoints.Add(d);
            }

            //Then we scale these values to [0, 1] domain:
            for (int i = 0; i < pList.Count; i++)
            {
                double s = dPoints[i] / dPoints[pList.Count - 1];
                sPoints.Add(s);
            }

            return (sPoints);
        }

        private double[,] GetArrayS(List<double> sPoints)
            //Get and fill matrix S with calculated sPoints
        {
            //In our error function (see references), we need to substitute symbolic t values in matrix T with the sPoint values we computed:
            double[,] arrayS = new double[sPoints.Count, 4];

            for (int i = 0; i < sPoints.Count; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    arrayS[i, j] = Math.Pow(sPoints[i], j);
                }
            }
            return arrayS;
        }
        
        private void btnModifycPoints_Click(object sender, EventArgs e)
            //allow to drag existing control points by mouse
        {
            canDeleteLine = false;
            addType = BezierType.Nothing; // to stop new point adding
            modifyPointType = BezierType.cPoints;
        }

        private void btnModifypPoints_Click(object sender, EventArgs e)
            //allow to drag existing line points by mouse
        {
            canDeleteLine = false;
            addType = BezierType.Nothing; // to stop new point adding
            modifyPointType = BezierType.pPoints;
        }

        public static void ModifypPointComposite(Point pointNew)
            //modify <Composite> line point
        {
            int i = localPoint.Item1;
            int j = localPoint.Item2;

            Point pointOld = new Point();
            pointOld = pPointsAll[i][j];

            //every <Composite> line point is also a control point; change both these point coordinates to the new point
            pPointsAll[i][j] = pointNew;
            cPointsAll[i][j * 3] = pointNew;


            //We can look at these calculations as vector operations. We want for the adjacent handles of the line point to 
            //stay in the same position relative to the line point. To do that, we take vectors from previous line point to control points 
            //and add those vectors to the new line point coordinates.

            Point newcPoint = new Point();
            if (j != 0)
            //first line point doesn't have first handle
            {
                newcPoint.X = pointNew.X - pointOld.X + cPointsAll[i][j * 3 - 1].X;
                newcPoint.Y = pointNew.Y - pointOld.Y + cPointsAll[i][j * 3 - 1].Y;
                cPointsAll[i][j * 3 - 1] = newcPoint;
            }

            if (j != pPointsAll[i].Count - 1)
            //last line point doesn't have second handle
            {
                newcPoint.X = pointNew.X - pointOld.X + cPointsAll[i][j * 3 + 1].X;
                newcPoint.Y = pointNew.Y - pointOld.Y + cPointsAll[i][j * 3 + 1].Y;
                cPointsAll[i][j * 3 + 1] = newcPoint;
            }

            return;
        }

        private void ModifycPointComposite(Point modifycPoint, Point middlecPoint, Point oppositecPoint, int opposite)
            //When moving a control point of a <Composite> line with left mouse button, 
            //the opposite handle needs to moved as well to ensure C2 continuity
        {
            if (middlecPoint == modifycPoint)
            //in segment no two control points should have the same location, it doesn't make mathematical sense and makes an error
            {
                modifycPoint.X++;
                modifycPoint.Y++;
            }

            //We can look at these calculations as vector operations. We want for vector middle-change to keep its length, 
            //but change its direction so it starts from middle point and is parallel to moving-middle vector.
            //To do that, we take unit vector from moving-middle (devide moving-middle with its length) and multiply that by 
            //middle-change length. Finally, we add that to middle point.

            double proportion = GetLength(middlecPoint, oppositecPoint) / GetLength(modifycPoint, middlecPoint);

            oppositecPoint.X = Convert.ToInt32(middlecPoint.X + proportion * (middlecPoint.X - modifycPoint.X));
            oppositecPoint.Y = Convert.ToInt32(middlecPoint.Y + proportion * (middlecPoint.Y - modifycPoint.Y));

            cPointsAll[localPoint.Item1][opposite] = oppositecPoint;

            return;
        }

        private void ModifycPointCompositeStraight(Point modifycPoint, Point middlecPoint, Point oppositecPoint)
            //When moving a control point of a <Composite> line with right mouse button, it can be moved only in straight line away from the
            //middle point. This ensures C2 continuity and that no other control point moves.
        {
            Point result = new Point();

            //To move the control point in straight line, we take unit vector from the middle line point ("middle") to 
            //the place control point was before moving ("prev") for which we know it was on the needed line. Than we multiply
            //this unit vector by the distance mouse (toMove) is from middle point and add this vector to the middle point.

            double prop = GetLength(middlecPoint, modifycPoint) / GetLength(oppositecPoint, middlecPoint);

            result.X = Convert.ToInt32(middlecPoint.X + prop * (middlecPoint.X - oppositecPoint.X));
            result.Y = Convert.ToInt32(middlecPoint.Y + prop * (middlecPoint.Y - oppositecPoint.Y));

            cPointsAll[localPoint.Item1][localPoint.Item2] = result;

            return;
        }
        
        private void btnChangeParam_Click(object sender, EventArgs e)
            //enable option to change parametrization type for already drawn <Least Squares> and <4 pPoints> lines
        {
            canChangeParam = true;
            isChangingParam = false;
            modifyPointType = BezierType.Nothing;
            modifyLineType = BezierType.Nothing;
        }

        private void rbtn_Uniform_CheckedChanged(object sender, EventArgs e)
            //redraw line when parametrization type has been changed
        {
            if (isChangingParam == false)
            {
                return;
            }

            int i = localPoint.Item1;
            //ParamType paramType = Parametrization[i];

            if (rbUniform.Checked == true)
            {
                parametrization[i] = ParamType.Uniform;
            }

            else if (rbChord.Checked == true)
            {
                parametrization[i] = ParamType.Chord;
            }

            else if (rbCentripetal.Checked == true)
            {
                parametrization[i] = ParamType.Centripetal;
            }

            GetcPointsInterpolation(i);
            pbCanva.Invalidate();
        }

        private void rbtn_Chord_CheckedChanged(object sender, EventArgs e)
            //redraw line when parametrization type has been changed
        {
            rbtn_Uniform_CheckedChanged(sender, e);
        }

        private void btnOutputcPoints_Click(object sender, EventArgs e)
            //enable option to output control point coordinates
        {
            outputPointType = BezierType.cPoints;
            addType = BezierType.Nothing;
            modifyPointType = BezierType.Nothing;
            modifyLineType = BezierType.Nothing;
        }

        private void btnOutputpPoints_Click(object sender, EventArgs e)
            //enable option to output line point coordinates
        {
            outputPointType = BezierType.pPoints;
            addType = BezierType.Nothing;
            modifyPointType = BezierType.Nothing;
            modifyLineType = BezierType.Nothing;
        }
        
        private void OutputcPointsFile(int i)
            //output control points to .txt file
        {
            string folderPath = "";

            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                folderPath = dialog.SelectedPath;
            }

            string path = Path.Combine(folderPath, "points.txt");
            using (var file = new StreamWriter(path, true)) //if "points.txt" exists add new lines to it, else create this file
            {
                file.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")); //write date and time in the first line
                file.WriteLine("<" + allLines[i] + "> line: \n");

                for (int j = 0; j < cPointsAll[i].Count; j++)
                {
                    string tmp = "C" + (j + 1) + ": (" + cPointsAll[i][j].X + "; " + cPointsAll[i][j].Y + ")"; //one control point coordinates in each line
                    file.WriteLine(tmp);
                }

                file.WriteLine("\n \n");
            }
        }

        private void OutputpPointsFile(int i)
            //output line points to .txt file
        {
            string folderPath = "";

            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                folderPath = dialog.SelectedPath;
            }

            string path = Path.Combine(folderPath, "points.txt");
            using (var file = new StreamWriter(path, true)) //if "points.txt" exists add new lines to it, else create this file
            {
                file.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")); //write date and time in the first line
                file.WriteLine("<" + allLines[i] + "> line: \n");

                for (int j = 0; j < pPointsAll[i].Count; j++)
                {
                    string tmp = "P" + (j + 1) + ": (" + pPointsAll[i][j].X + "; " + pPointsAll[i][j].Y + ")"; //one control point coordinates in each line
                    file.WriteLine(tmp);
                }

                file.WriteLine("\n \n");
            }
        }
        
        private void btnResetAll_Click(object sender, EventArgs e)
            //reset form to its inial state, clean pictureBox1 and reset all settings
        {
            addType = BezierType.Nothing;
            modifyLineType = BezierType.Nothing;
            modifyPointType = BezierType.Nothing;
            cPoints = null;
            cPointsAll = new List<List<Point>>();
            pPoints = null;
            pPointsAll = new List<List<Point>>();
            allLines = new List<BezierType>();
            MovedLine = new List<MoveType>();
            parametrization = new List<ParamType>();
            localPoint = null;
            rbMouseInput.Checked = true;
            rbMouseModify.Checked = true;
            imageLocation = "";
            error.Text = "";
            cbShowBackground.Checked = false;
            isCompositeDone = false;
            canChangeParam = false;
            isChangingParam = false;
            canDeleteLine = false;
            outputPointType = BezierType.Nothing;

            pbCanva.Invalidate();
        }
    }
}