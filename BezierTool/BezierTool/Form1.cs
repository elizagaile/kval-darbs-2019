/*
 * Programma "BezierTool"
 * Izveidota xx.xx.2019
 * Autors Elīza Gaile eg17035
 * Programma izstrādāta kvalifikāciojas darba ietvaros
*/ 

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MathNet.Numerics.LinearAlgebra;
using System.IO;


namespace BezierTool
{
    public partial class FormMain : Form // contains form, all its atributes and functions
    {
        // each Bezier curve is represented in several "representitive" lists: 
        // list of construction type of each curve; "allLines"
        // list of control points of each curve; "cPoints"
        // list of knot points of each curve (if the curve has knot points); "pPoints"
        // list of parametrization method of each curve (if the curve was interpolated); "parametrization"
        // list of type a composite curve was moved; "movedLine"
        // the i-th drawn curve is represented by the i-th position in all of the lists

        // all possible construction types of curves:
        public enum BezierType { cPoints, pPoints, LeastSquares, Composite, Nothing }; 
        public static List<BezierType> allLines = new List<BezierType>();

        public static List<List<Point>> cPointsAll = new List<List<Point>>();
        public static List<List<Point>> pPointsAll = new List<List<Point>>();

        // all possible parametrization types of interpolated curves:
        enum ParamType { Uniform, Chord, Centripetal, Nothing };
        private List<ParamType> parametrization = new List<ParamType>(); //contains parametrization types for drawn curves

        // all possible ways to move a composite line
        private enum MoveType { LeftClick, RightClick, pPoints, Nothing };
        private List<MoveType> movedLine = new List<MoveType>();

        // all possible reasons for initialization of coordination form
        public enum FormType { Add, Modify, Output };

        public static BezierType addType = BezierType.Nothing; // type of curve to be or being added
        public static BezierType modifyLineType = BezierType.Nothing; // type of curve to be or being modified
        public static BezierType modifyPointType = BezierType.Nothing; // type of point to be or being draged by mouse
        public static BezierType outputPointType = BezierType.Nothing; // type of points to output
        
        public static Tuple<int, int> localPoint = null; //position of a selected point in the representitive lists

        private List<Point> cPoints = null; // list of control points of a curve
        private List<Point> pPoints = null; // list of knot points of a curve

        private Point cPointNew; // location of a new control point for <4 cPoints> curve 

        public const int maxPointCount = 12; // maximum count of points for <Least Squares> and <Composite> curves; chosen arbitrary

        bool isCompositeDone = false; // indicates if the last line of type <Composite> needs to be finished
        bool canChangeParam = false; // indicates if option to change parametrization is enabled
        bool isChangingParam = false; // indicates if parametrization of a curve is being changed
        bool canDeleteLine = false; // indicates if option to delete a line is enabled

        String imageLocation = ""; //path of background image

        public FormMain()
        {
            InitializeComponent();
            this.Width = Convert.ToInt32(0.75 * Screen.PrimaryScreen.Bounds.Width);
            this.Height = Convert.ToInt32(0.75 * Screen.PrimaryScreen.Bounds.Height);
        }


        // Called, when mouse is pressed inside pbCanva. 
        // This function can be used for adding control and knot points,
        // for dragging points with mouse or for selecting a curve to output its point coordinates
        private void pbCanva_MouseDown(object sender, MouseEventArgs e)
        {
            // addding a new control point with mouse for <4 cPoints> curve
            if (addType == BezierType.cPoints && rbMouseInput.Checked == true)
            {
                AddcPoint(e.Location);
                pbCanva.Invalidate();
            }

            // adding a new point with mouse for  <4 pPoints>, <Least Squares> or <Composite> curve
            else if ((addType == BezierType.pPoints || addType == BezierType.LeastSquares || addType == BezierType.Composite) && rbMouseInput.Checked == true)
            {
                AddpPoint(e.Location);
                pbCanva.Invalidate();
            }

            // dragging a control point with mouse
            if (cPointsAll != null && modifyPointType == BezierType.cPoints)
            {
                FindLocalPoint(cPointsAll, e.Location);

                if (localPoint != null)
                {
                    ModifycPoint(e);
                    pbCanva.Invalidate();
                }
            }

            // dragging a knot point with mouse
            if (pPointsAll != null && modifyPointType == BezierType.pPoints)
            {
                FindLocalPoint(pPointsAll, e.Location);

                if (localPoint != null)
                {
                    ModifypPoint();
                    pbCanva.Invalidate();
                }
            }

            // changing parametrization method of a drawn curve
            if (cPointsAll != null && canChangeParam == true && isChangingParam == false)
            {
                FindLocalPoint(cPointsAll, e.Location);

                if (localPoint != null)
                {
                    ChangeParametrization();
                    pbCanva.Invalidate();
                }
            }

            // outputting control point coordinates
            if (cPointsAll != null && outputPointType == BezierType.cPoints)
            {
                FindLocalPoint(cPointsAll, e.Location);

                if (localPoint == null)
                {
                    return;
                }

                int i = localPoint.Item1;

                // output on screen, initialize the form of coordinates
                if (rbScreenOutput.Checked == true)
                {
                    FormCoordinates form_KeyboardAdd = new FormCoordinates(FormType.Output, allLines[i]);
                    form_KeyboardAdd.ShowDialog();
                }

                // output to .txt file
                if (rbFileOutput.Checked == true)
                {
                    OutputcPointsToFile();
                }

                outputPointType = BezierType.Nothing;
                modifyLineType = BezierType.Nothing; // why is this needed ???
                localPoint = null;

                pbCanva.Invalidate();
            }

            // outputting knot point coordinates
            if (pPointsAll != null && outputPointType == BezierType.pPoints)
            {
                FindLocalPoint(pPointsAll, e.Location);

                if (localPoint == null)
                {
                    return;
                }

                int i = localPoint.Item1;

                // output on screen, initialize the form of coordinates
                if (rbScreenOutput.Checked == true)
                {
                    FormCoordinates form_KeyboardAdd = new FormCoordinates(FormType.Output, allLines[i]);
                    form_KeyboardAdd.ShowDialog();
                }

                // output to .txt file
                else if (rbFileOutput.Checked == true)
                {
                    OutputpPointsToFile();
                }

                outputPointType = BezierType.Nothing;
                modifyLineType = BezierType.Nothing; // why is this needed ???
                localPoint = null;

                pbCanva.Invalidate();
            }

            // deleting a curve
            if (cPointsAll != null && canDeleteLine == true)
            {
                FindLocalPoint(cPointsAll, e.Location);

                if (localPoint != null)
                {
                    DeleteLine(localPoint.Item1);
                    pbCanva.Invalidate();
                }
            }
        }


        // Called, when mouse is moved inside pbCanva. 
        // This function can be used for drawing a dashed line when adding new control point for <4 cPoints> curve,
        // for for modifying points of a curve by mouse.
        private void pbCanva_MouseMove(object sender, MouseEventArgs e)
        {
            // get the new control point coordines for <4 cPoints> curve
            if (addType == BezierType.cPoints)
            {
                cPointNew = e.Location;
                pbCanva.Invalidate();
            }

            // need to intialize variables; chose 0s arbitrary
            int i = 0;
            int j = 0;

            if (localPoint != null)
            {
                i = localPoint.Item1;
                j = localPoint.Item2;
            }

            // when modifiying <4 cPoints> curve, we can just change point coordinets
            if (modifyLineType == BezierType.cPoints)
            {
                cPointsAll[i][j] = e.Location;
                pbCanva.Invalidate();
            }

            // when modifiying knot points of <4 pPoints> or <Least Squares> curves, we need to re-calculates control points of the curve
            else if ((modifyLineType == BezierType.pPoints || modifyLineType == BezierType.LeastSquares) && modifyPointType == BezierType.pPoints)
            {
                pPointsAll[i][j] = e.Location;
                AddcPointsInterpolation(i);
                pbCanva.Invalidate();
            }

            // when modifiying control points of <Composite> curves, we need to make sure the curve stays C2 continuous
            else if (modifyLineType == BezierType.Composite && modifyPointType == BezierType.cPoints)
            {
                // using left click, we can drag the control point anywhere, but the 'opposite' control point moves
                // aswell - to maintain continuity
                if (movedLine[i] == MoveType.LeftClick)
                {
                    cPointsAll[i][j] = e.Location;

                    //starting from the fifth control point, every third point's opposite control point is two points before
                    if (j % 3 == 1 && j != 1)
                    {
                        ModifyHandleComposite(cPointsAll[i][j], cPointsAll[i][j - 1], cPointsAll[i][j - 2], j - 2);
                    }


                    //starting from the third control point, every third point's opposite control point is two points after
                    if (j % 3 == 2 && j != cPointsAll[i].Count - 2)
                    {
                        ModifyHandleComposite(cPointsAll[i][j], cPointsAll[i][j + 1], cPointsAll[i][j + 2], j + 2);
                    }
                }

                // using right click to drag a control point, no other control points will move, but to maintain  C2 continuity
                // we can only move the control point in straight line away from its opposite point
                if (movedLine[i] == MoveType.RightClick)
                {
                    //starting from the fifth control point, every third point's opposite control point is two points before
                    if (j % 3 == 1 && j != 1)
                    {
                        ModifyHandleCompositeStraight(e.Location, cPointsAll[i][j - 1], cPointsAll[i][j - 2]);
                    }

                    //starting from the third control point, every third point's opposite control point is two points after
                    if (j % 3 == 2 && j != cPointsAll[i].Count - 2)
                    {
                        ModifyHandleCompositeStraight(e.Location, cPointsAll[i][j + 1], cPointsAll[i][j + 2]);
                    }
                }
                pbCanva.Invalidate();
            }

            // when modifiying knot points of <Composite> curves, we need to re-calculates control points of the curve 
            else if (modifyLineType == BezierType.Composite && modifyPointType == BezierType.pPoints)
            {
                ModifypPointComposite(e.Location);
                
                movedLine[i] = MoveType.pPoints;
                pbCanva.Invalidate();
            }
        }


        // Called, when mouse is released inside pbCanva after pressing it. 
        // This function can be used for stopping point dragging with mouse.
        private void pbCanva_MouseUp(object sender, MouseEventArgs e)
        {
            if (modifyPointType != BezierType.Nothing)
            {
                modifyLineType = BezierType.Nothing;
                localPoint = null;
            }
            pbCanva.Invalidate();
        }


        // Draws all graphics in this programm - points, control point polygons, all bezier curves,
        // as well as calling for functions to get needed control points.
        private void pbCanva_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;//makes lines look smoother

            const int pointRadius = 2; // radius for control points and knot points to be drawn; chosen arbitrary
            const int dashLength = 5; // describes length of dashes; chosen arbitrary


            if (cPointsAll == null || pPointsAll == null)
            {
                return;
            }


            // if we are selecting points for <4 cPoints> curve, draw a dashed line from mouse location to previous control point
            if (cPoints != null)
            {
                // <4 cPoints> curves can't have more than 4 control points
                if (cPoints.Count < 4 && addType == BezierType.cPoints)
                {
                    Pen dashedPen = new Pen(Color.LightGray)
                    {
                        DashPattern = new float[] { dashLength, dashLength }
                    };

                    e.Graphics.DrawLine(dashedPen, cPoints[cPoints.Count - 1], cPointNew);
                }
            }


            // go through all lists of knot points
            for (int i = 0; i < pPointsAll.Count; i++)
            {
                if (pPointsAll[i] != null)
                {
                    // draw a black point for every point
                    foreach (Point pPoint in pPointsAll[i])
                    {
                        e.Graphics.FillEllipse(Brushes.Black, pPoint.X - pointRadius, pPoint.Y - pointRadius, 2 * pointRadius, 2 * pointRadius);
                    }
                    
                    // if <4 pPoints> curve has 4 knot points, but control points haven't been calculated yet, calculate them
                    if (allLines[i] == BezierType.pPoints && pPointsAll[i].Count == 4 && cPointsAll[i] == null)
                    {
                        AddcPointsInterpolation(i);
                    }

                    // if <Least Squares> curve has atleast 4 knot points, calculate control points
                    if (allLines[i] == BezierType.LeastSquares && pPointsAll[i].Count >= 4)
                    {
                        AddcPointsInterpolation(i);
                    }

                    // draw <Composite> curve which hasn't been moved
                    if (allLines[i] == BezierType.Composite && movedLine[i] == MoveType.Nothing)
                    {
                        if (isCompositeDone == true && pPointsAll[i].Count == 2)
                        {
                            cPointsAll[i] = new List<Point>();
                            AddOnlycPointsComposite(i);
                        }

                        // if <Composite> curve has more than 3 knot points, calculate control points
                        else if (pPointsAll[i].Count >= 3)
                        {
                            cPointsAll[i] = new List<Point>();
                            AddcPointsComposite(i);
                        }
                    }
                }
            }


            // go through all lists of knot points
            for (int i = 0; i < cPointsAll.Count; i++)
            {
                if (cPointsAll[i] != null)
                {

                    // Drawing red circle for control points:

                    // for <4 cPoints> and <Least Squares> curves draw all control points
                    if (allLines[i] == BezierType.cPoints || allLines[i] == BezierType.LeastSquares)
                    {
                        foreach (Point cPoint in cPointsAll[i])
                        {
                            e.Graphics.DrawEllipse(Pens.Red, cPoint.X - pointRadius, cPoint.Y - pointRadius, 2 * pointRadius, 2 * pointRadius);
                        }
                    }
                    
                    //for <4 pPoints> curves draw only middle control points, because end are also points knot points
                    else if (allLines[i] == BezierType.pPoints)
                    {
                        e.Graphics.DrawEllipse(Pens.Red, cPointsAll[i][1].X - pointRadius, cPointsAll[i][1].Y - pointRadius, 2 * pointRadius, 2 * pointRadius);
                        e.Graphics.DrawEllipse(Pens.Red, cPointsAll[i][2].X - pointRadius, cPointsAll[i][2].Y - pointRadius, 2 * pointRadius, 2 * pointRadius);
                    }

                    // for <Composite> curves draw only those control points which are not line points -
                    // - every third line point starting from the first is also a control point
                    else if (allLines[i] == BezierType.Composite)
                    {
                        for (int j = 0; j < cPointsAll[i].Count - 1; j++)
                        {
                            if (j % 3 != 2)
                            {
                                e.Graphics.DrawEllipse(Pens.Red, cPointsAll[i][j + 1].X - pointRadius, cPointsAll[i][j + 1].Y - pointRadius, 2 * pointRadius, 2 * pointRadius);
                            }
                        }
                    }


                    //Drawing control point polygons / handle lines:

                    if (cPointsAll[i].Count > 1 && (allLines[i] == BezierType.cPoints || allLines[i] == BezierType.LeastSquares || allLines[i] == BezierType.pPoints))
                    {
                        e.Graphics.DrawLines(Pens.LightGray, cPointsAll[i].ToArray());
                    }

                    //for <Composite> lines, draw only handle lines
                    else if (allLines[i] == BezierType.Composite)
                    {
                        for (int j = 0; j < cPointsAll[i].Count - 1; j++)
                        {
                            // connect every control point to the next, 
                            //exept every third starting from first and the last
                            if (j % 3 != 1)
                            {
                                e.Graphics.DrawLine(Pens.LightGray, cPointsAll[i][j], cPointsAll[i][j + 1]);
                            }
                        }
                    }


                    //Drawing all bezier lines:

                    Pen bezierPen = new Pen(Brushes.Black);

                    // <4 cPoints>, <4 pPoints> and <Least Squares> curves have 4 control points
                    if (cPointsAll[i].Count == 4 && (allLines[i] == BezierType.cPoints || allLines[i] == BezierType.LeastSquares || allLines[i] == BezierType.pPoints))
                    {
                        e.Graphics.DrawBezier(bezierPen, cPointsAll[i][0], cPointsAll[i][1], cPointsAll[i][2], cPointsAll[i][3]);
                    }

                    // draw each segment of <Composite> curve
                    else if (allLines[i] == BezierType.Composite)
                    {
                        for (int j = 0; j < cPointsAll[i].Count - 3; j += 3)
                        {
                            e.Graphics.DrawBezier(bezierPen, cPointsAll[i][j], cPointsAll[i][j + 1], cPointsAll[i][j + 2], cPointsAll[i][j + 3]);
                        }
                    }
                }
            }
        }


        // Ensures main form is responsive.
        private void FormMain_Resize(object sender, EventArgs e)
        {
            int formWidth = this.Width;
            int formHeight = this.Height;

            // 20px, 50px, 35 px and 55px makes margins for panels and pbCanva
            panel_tools.Left = formWidth - panel_tools.Width - 20; 
            panel_bottom.Left = formWidth - panel_bottom.Width - 20;
            panel_bottom.Top = formHeight - panel_bottom.Height - 50;
            pbCanva.Width = formWidth - panel_tools.Width - 35;
            pbCanva.Height = formHeight - 55;
        }


        // Uploads background image for pbCanva.
        private void btnUploadBackground_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog 
                { 
                    Filter = "jpg files(.*jpg)|*.jpg| PNG files(.*png)|*.png| All Files(*.*)|*.*" // types of files allowed ???
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


        // Changes the visibility of uploaded background picture of pbCanva.
        private void cbShowBackground_CheckStateChanged(object sender, EventArgs e)
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


        // Start a new <4 cPoints> curve.
        private void btnNew4cPoints_Click(object sender, EventArgs e)
        {
            addType = BezierType.cPoints;

            if (rbMouseInput.Checked == true)
            {
                cPoints = null;
            }

            if (rbKeyboardInput.Checked == true)
            {
                NewLine(BezierType.cPoints);

                // when inputting points by keyboard, intialize the form of coordinates
                FormCoordinates form_KeyboardAdd = new FormCoordinates(FormType.Add, addType);
                form_KeyboardAdd.ShowDialog();


                // an error or cancelation occured in the form of coordinates and no curves were added
                if (FormCoordinates.lineAdded == false)
                {
                    DeleteLine(allLines.Count - 1); // reverse the actions of newLine() function
                    return;
                }

                pbCanva.Invalidate();
            }

            if (rbFileInput.Checked == true)
            {
                cPoints = GetPointsfromFile();

                if (cPoints.Count != 4)
                {
                    error.Text = ".txt file was not correct!";
                    DeleteLine(allLines.Count - 1); // reverse the actions of newLine() function
                    return;
                }

                cPointsAll[cPointsAll.Count - 1] = cPoints;
                pbCanva.Invalidate();
            }
        }


        // Start a new <4 pPoints> curve.
        private void btnNew4pPoints_Click(object sender, EventArgs e)
        {
            addType = BezierType.pPoints;

            if (rbMouseInput.Checked == true)
            {
                pPoints = null;
            }

            if (rbKeyboardInput.Checked == true)
            {
                NewLine(BezierType.pPoints);

                // when inputting points by keyboard, intialize the form of coordinates
                FormCoordinates form_KeyboardAdd = new FormCoordinates(FormType.Add, addType);
                form_KeyboardAdd.ShowDialog();

                // an error or cancelation occured in the form of coordinates and no curves were added
                if (FormCoordinates.lineAdded == false)
                {
                    DeleteLine(allLines.Count - 1); 
                    return;
                }

                pbCanva.Invalidate();
            }

            if (rbFileInput.Checked == true)
            {
                pPoints = GetPointsfromFile();

                if (pPoints.Count != 4)
                {
                    error.Text = ".txt file was not correct!";
                    DeleteLine(allLines.Count - 1);  // reverse the actions of newLine() function
                    return;
                }

                pPointsAll[pPointsAll.Count - 1] = pPoints;
                pbCanva.Invalidate();
            }
        }


        // Start a new <Least Squares> curve.
        private void btnNewLeastSquares_Click(object sender, EventArgs e)
        {
            addType = BezierType.LeastSquares;

            if (rbMouseInput.Checked == true)
            {
                pPoints = null;
            }

            if (rbKeyboardInput.Checked == true)
            {
                NewLine(BezierType.LeastSquares);

                // when inputting points by keyboard, intialize the form of coordinates
                FormCoordinates form_KeyboardAdd = new FormCoordinates(FormType.Add, addType);
                form_KeyboardAdd.ShowDialog();

                // an error or cancelation occured in the form of coordinates and no curves were added
                if (FormCoordinates.lineAdded == false)
                {
                    DeleteLine(allLines.Count - 1); // reverse the actions of newLine() function
                    return;
                }

                pbCanva.Invalidate();
            }

            if (rbFileInput.Checked == true)
            {
                pPoints = GetPointsfromFile();

                if (pPoints.Count < 4 || pPoints.Count > maxPointCount)
                {
                    error.Text = ".txt file was not correct!";
                    DeleteLine(allLines.Count - 1);  // reverse the actions of newLine() function
                    return;
                }

                pPointsAll[pPointsAll.Count - 1] = pPoints;
                pbCanva.Invalidate();
            }
        }


        // Start a new <Composite> curve.
        private void btnNewComposite_Click(object sender, EventArgs e)
        {
            addType = BezierType.Composite;

            if (rbMouseInput.Checked == true)
            {
                pPoints = null;
            }

            if (rbKeyboardInput.Checked == true)
            {
                NewLine(BezierType.Composite);

                // when inputting points by keyboard, intialize the form of coordinates
                FormCoordinates form_KeyboardAdd = new FormCoordinates(FormType.Add, addType);
                form_KeyboardAdd.ShowDialog();

                // an error or cancelation occured in the form of coordinates and no curves were added
                if (FormCoordinates.lineAdded == false)
                {
                    DeleteLine(allLines.Count - 1); // reverse the actions of newLine() function
                    return;
                }

                isCompositeDone = true;

                pbCanva.Invalidate();
            }

            if (rbFileInput.Checked == true)
            {
                pPoints = GetPointsfromFile();

                if (pPoints.Count < 2 || pPoints.Count > maxPointCount)
                {
                    error.Text = ".txt file was not correct!";
                    DeleteLine(allLines.Count - 1);  // reverse the actions of newLine() function
                    return;
                }

                pPointsAll[pPointsAll.Count - 1] = pPoints;
                isCompositeDone = true;
                pbCanva.Invalidate();
            }
        }


        // When a <Composite> curve is indicated as done, draw the last segment of it. 
        private void btnDoneComposite_Click(object sender, EventArgs e)
        {
            isCompositeDone = true;
            pbCanva.Invalidate();
        }


        // Allow to drag control points by mouse.
        private void btnModifycPoints_Click(object sender, EventArgs e)
        {
            canDeleteLine = false;
            canChangeParam = false;
            isChangingParam = false;
            addType = BezierType.Nothing;
            outputPointType = BezierType.Nothing;

            modifyPointType = BezierType.cPoints;
        }


        // Allow to drag knot points by mouse.
        private void btnModifypPoints_Click(object sender, EventArgs e)
        {
            canDeleteLine = false;
            canChangeParam = false;
            isChangingParam = false;
            addType = BezierType.Nothing;
            outputPointType = BezierType.Nothing;

            modifyPointType = BezierType.pPoints;
        }


        // Allow to change curve parametrization method.
        private void btnChangeParam_Click(object sender, EventArgs e)
        {
            isChangingParam = false;
            canDeleteLine = false;
            addType = BezierType.Nothing;
            modifyPointType = BezierType.Nothing;
            modifyLineType = BezierType.Nothing; // šo vajag ???
            outputPointType = BezierType.Nothing;

            canChangeParam = true;
        }


        // Change parametrization method and redraw the curve.
        private void rbUniform_CheckedChanged(object sender, EventArgs e)
        {
            if (isChangingParam == false)
            {
                return;
            }

            int i = localPoint.Item1;

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

            AddcPointsInterpolation(i);
            pbCanva.Invalidate();
        }


        // Change parametrization method and redraw the curve.
        private void rbChord_CheckedChanged(object sender, EventArgs e)
        {
            rbUniform_CheckedChanged(sender, e);
        }


        // Enable option to output control point coordinates.
        private void btnOutputcPoints_Click(object sender, EventArgs e)
        {
            canDeleteLine = false;
            canChangeParam = false;
            isChangingParam = false;
            addType = BezierType.Nothing;
            modifyPointType = BezierType.Nothing;
            modifyLineType = BezierType.Nothing;

            outputPointType = BezierType.cPoints;
        }


        // Enable option to output knot point coordinates.
        private void btnOutputpPoints_Click(object sender, EventArgs e)
        {
            canDeleteLine = false;
            canChangeParam = false;
            isChangingParam = false;
            addType = BezierType.Nothing;
            modifyPointType = BezierType.Nothing;
            modifyLineType = BezierType.Nothing;

            outputPointType = BezierType.pPoints;
        }


        // Enable option to delete a curve.
        private void btnDeleteLine_Click(object sender, EventArgs e)
        {
            canChangeParam = false;
            isChangingParam = false;
            addType = BezierType.Nothing;
            modifyPointType = BezierType.Nothing;
            modifyLineType = BezierType.Nothing;
            outputPointType = BezierType.Nothing;

            canDeleteLine = true;
        }


        // Reset main form to its inial state, clean pbCanva and reset all variables.
        private void btnResetAll_Click(object sender, EventArgs e)
        {
            string message = "Do you want to reset this form?";
            string title = "Reset all";
            MessageBoxButtons buttons = MessageBoxButtons.OKCancel;
            DialogResult result = MessageBox.Show(message, title, buttons);

            if (result != DialogResult.OK)
            {
                return;
            }

            allLines = new List<BezierType>();
            movedLine = new List<MoveType>();
            parametrization = new List<ParamType>();

            cPointsAll = new List<List<Point>>();
            pPointsAll = new List<List<Point>>();

            cPoints = null;
            pPoints = null;
            localPoint = null;

            addType = BezierType.Nothing;
            modifyLineType = BezierType.Nothing;
            modifyPointType = BezierType.Nothing;
            outputPointType = BezierType.Nothing;

            isCompositeDone = false;
            canChangeParam = false;
            isChangingParam = false;
            canDeleteLine = false;

            rbMouseInput.Checked = true;
            rbMouseModify.Checked = true;

            imageLocation = "";
            error.Text = "";
            cbShowBackground.Checked = false;

            pbCanva.Invalidate();
        }


        // Start a new line, add its parametrs to representitive lists.
        private void NewLine(BezierType lineType)

        {
            allLines.Add(lineType);

            cPoints = null;
            pPoints = null;

            localPoint = null;

            isChangingParam = false;
            isCompositeDone = false;
            canDeleteLine = false;

            modifyPointType = BezierType.Nothing;
            modifyLineType = BezierType.Nothing;
            outputPointType = BezierType.Nothing;

            cPointsAll.Add(null);
            pPointsAll.Add(null);
            movedLine.Add(MoveType.Nothing);

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


        // Reverse action of newLine() function.
        // Used when a curve was deleted or when adding a curve by keyboard had an error or cancelation.
        private void DeleteLine(int i)
        {
            addType = BezierType.Nothing;
            allLines.RemoveAt(i);
            movedLine.RemoveAt(i);
            cPointsAll.RemoveAt(i);
            pPointsAll.RemoveAt(i);
            parametrization.RemoveAt(i);

            canDeleteLine = false;

            return;
        }


        // Add new control point by mouse to the last curve.
        private void AddcPoint(Point mouseLocation)
        {
            // adding the first control point of curve
            if (cPoints == null)
            {
                NewLine(addType);
                cPoints = new List<Point> { mouseLocation };
                cPointsAll[cPointsAll.Count - 1] = cPoints;
            }

            // to avoid accidental double clicks
            else if (cPoints.Count < 4 && cPoints[cPoints.Count - 1] != mouseLocation)
            {
                cPoints.Add(mouseLocation);
            }

            return;
        }


        // Add new knot point by mouse to the last curve.
        private void AddpPoint(Point mouseLocation)
        {
            // adding the first control point of curve
            if (pPoints == null)
            {
                NewLine(addType);
                pPoints = new List<Point> { mouseLocation };
                pPointsAll[pPointsAll.Count - 1] = pPoints;
                
                return;
            }

            // to avoid accidental double clicks
            if (pPoints[pPoints.Count - 1] == mouseLocation)
            {
                return;
            }

            //<4 pPoints> curves can't have more than 4 knot points
            if (addType == BezierType.pPoints && pPoints.Count >= 4)
            {
                return;
            }

            // can't add any more knot points to a finished <Composite> curve
            if (addType == BezierType.Composite && isCompositeDone == true)
            {
                return;
            }

            else if ((addType == BezierType.LeastSquares || addType == BezierType.Composite) && pPoints.Count > maxPointCount)
            {
                return;
            }

            pPoints.Add(mouseLocation);

            return;
        }


        // Calculate and add control points for <Composite> curves with three or more knot points.
        private void AddcPointsComposite(int i)
        {
            int  pCount = pPointsAll[i].Count;

            // first control point is the first knot point:
            cPointsAll[i].Add(pPointsAll[i][0]);

            // add first handle:
            Point firstHandle = new Point();
            firstHandle = GetFirstHandle(pPointsAll[i][0], pPointsAll[i][1], pPointsAll[i][2]);
            cPointsAll[i].Add(GetVeryFirstHandle(pPointsAll[i][0], firstHandle, pPointsAll[i][1]));

            // add three new control points for every knot point starting with the third -
            // - every knot point is also a control point and for every but first and last knot point, 
            // we get two handles:
            for (int j = 2; j < pPointsAll[i].Count; j++)
            {
                cPointsAll[i].Add(GetFirstHandle(pPointsAll[i][j - 2], pPointsAll[i][j - 1], pPointsAll[i][j]));
                cPointsAll[i].Add(pPointsAll[i][j - 1]);
                cPointsAll[i].Add(GetSecondHandle(pPointsAll[i][j - 2], pPointsAll[i][j - 1], pPointsAll[i][j]));
            }

            // every <Composite> line except the last one always needs to be finished
            // that means, it should have three times (every point is a control point and has two handles) 
            // minus two (each end point doesn't have one handle) more control points than knot points
            if ((i != allLines.Count - 1 && cPointsAll[i].Count < pCount * 3 - 2) || (i == allLines.Count - 1 && isCompositeDone == true))
            {
                Point veryLastHandle;
                veryLastHandle = GetVeryLastHandle(pPointsAll[i][pCount - 2], cPointsAll[i][cPointsAll[i].Count - 1], pPointsAll[i][pCount - 1]);
                cPointsAll[i].Add(veryLastHandle);
                cPointsAll[i].Add(pPointsAll[i][pPointsAll[i].Count - 1]);
            }

            return;
        }


        // Finish a <Composite> curve, that has only two control points, but is indicated as finished.
        private void AddOnlycPointsComposite(int i)
        {
            Point firstcPoint = new Point();
            Point firstHandle = new Point();
            Point secondHandle = new Point();
            Point lastcPoint = new Point();

            // first and last control points are knot points of the curve:
            firstcPoint = pPointsAll[i][0];
            lastcPoint = pPointsAll[i][1];

            double sin60 = Math.Sin(Math.PI / 3);
            double cos60 = Math.Cos(Math.PI / 3);

            // each control point will be the midpoint of firstcPoint-lastcPoint line segment, rotated by 60 degrees
            // first we find oordinates of the midpoint:
            double xMidpoint = 0.5 * (lastcPoint.X - firstcPoint.X);
            double yMidpoint = 0.5 * (lastcPoint.Y - firstcPoint.Y);

            // then we rotate the midpoint by 60 degrees:
            firstHandle.X = Convert.ToInt32(cos60 * xMidpoint - sin60 * yMidpoint + firstcPoint.X);
            firstHandle.Y = Convert.ToInt32(sin60 * xMidpoint + cos60 * yMidpoint + firstcPoint.Y);

            // for control points of the curve to be on different sides, change the signs for second handle:
            secondHandle.X = Convert.ToInt32(cos60 * -xMidpoint - sin60 * -yMidpoint + lastcPoint.X);
            secondHandle.Y = Convert.ToInt32(sin60 * -xMidpoint + cos60 * -yMidpoint + lastcPoint.Y);

            cPointsAll[i].Add(firstcPoint);
            cPointsAll[i].Add(firstHandle);
            cPointsAll[i].Add(secondHandle);
            cPointsAll[i].Add(lastcPoint);

            pbCanva.Invalidate();

            return;
        }


        // Add the very first control point that's not a line point for <Composite> curve with at least three line points.
        private Point GetVeryFirstHandle(Point firstpPoint, Point nextHandle, Point secondpPoint)
        {
            Point veryFirstHandle = new Point();

            // coordinates of the very first handle is calculated from first, third and fourth control points of the  <Composite> curve

            // We can look at these calculations as vector operations. 
            // First, we calculate dot product of vectors secondpPoint-nextHandle and secondpPoint-firstpPOint:
            double dotProduct = (nextHandle.X - secondpPoint.X) * (firstpPoint.X - secondpPoint.X) +
                                (nextHandle.Y - secondpPoint.Y) * (firstpPoint.Y - secondpPoint.Y);

            //We need to find how long the vector v1 from veryFirstHandle to nextHandle needs to be, so that the middle control points are symmetrical.
            //The symmetry can be achieved if the vector v1 is parallel to vector v2 from secondpPoint to firstpPoint  
            //and has the length: length(v2) - 2*length(projection of vector secondpPoint-nextHandle on to v2). One projection length for each side.
            //Using projection formula, we get: proportion = |v2| - 2 * dot / |v2| . We will multiply this proportion by unit vector
            //parallel to vector v2, which can be expressed as v2 / |v2|. If we devide our proportion with |v2| from the unit vector,
            //we get: proportion = 1 - 2 * dot / |v2|^2

            //That means, the length of the vector we will add equals 
            double prop = 1 - 2 * dotProduct / (Math.Pow(GetLength(firstpPoint, secondpPoint), 2));

            //Lastly, to point nextHandle we add vector parallel to vector firstpPoint-secondpPoint scaled by the proportion:
            veryFirstHandle.X = Convert.ToInt32(nextHandle.X + prop * (firstpPoint.X - secondpPoint.X));
            veryFirstHandle.Y = Convert.ToInt32(nextHandle.Y + prop * (firstpPoint.Y - secondpPoint.Y));

            // We have achieved a "symmetrical" point to nextHandle; both of these points are on the same side of the bezier line.

            return veryFirstHandle;
        }


        // Calculate coordinates of first handle for <Composite> lines in a way to ensure C2 continuity.
        private Point GetFirstHandle(Point prevpPoint, Point thispPoint, Point nextpPoint)
        {
            Point firstHandle = new Point();
            double lengthPrevThis = GetLength(prevpPoint, thispPoint);
            double lengthThisNext = GetLength(thispPoint, nextpPoint);

            // Distance from first to second handle is half the distance from prevpPoint (a) to nextpPoint (b).
            // The proportions of the length of each handle are the same as proportion ab/bc, where b thispPoint.
            // Methods of calculations for distances and angles of handles can be different and there isn't one best method. 
            // I have discovered that this method works nice most of the time and isn't computationally expensive.

            double proportion = 0.5 * lengthPrevThis / (lengthPrevThis + lengthThisNext);

            firstHandle.X = thispPoint.X + Convert.ToInt32(proportion * (prevpPoint.X - nextpPoint.X));
            firstHandle.Y = thispPoint.Y + Convert.ToInt32(proportion * (prevpPoint.Y - nextpPoint.Y));

            return firstHandle;
        }


        // Calculate coordinates of second handle for <Composite> lines in a way to ensure C2 continuity.
        private Point GetSecondHandle(Point prevpPoint, Point thispPoint, Point nextpPoint)
        {
            Point secondHandle = new Point();
            double lengthPrevThis = GetLength(prevpPoint, thispPoint);
            double lengthThisNext = GetLength(thispPoint, nextpPoint);

            //Calculations are very similar to those in the function GetFirstHandle.

            double proportion = 0.5 * lengthThisNext / (lengthPrevThis + lengthThisNext);

            secondHandle.X = thispPoint.X + Convert.ToInt32(proportion * (nextpPoint.X - prevpPoint.X));
            secondHandle.Y = thispPoint.Y + Convert.ToInt32(proportion * (nextpPoint.Y - prevpPoint.Y));

            return secondHandle;
        }


        // Add two last control points of a <Composite> line that is indicated as finished and has at least three knot points
        private Point GetVeryLastHandle(Point prevpPoint, Point prevHandle, Point lastpPoint)
        {
            Point veryLastHandle = new Point();

            // Coordinates of the very last handle is calculated from first, second and fourth control points of the <Composite> curve's last segment

            // We can look at these calculations as vector operations. 
            // First we calculate dot product of vectors lastpPoint-prevHandle and lastpPoint-prevHandle:
            double dotProduct = (prevHandle.X - lastpPoint.X) * (prevpPoint.X - lastpPoint.X) + 
                                (prevHandle.Y - lastpPoint.Y) * (prevpPoint.Y - lastpPoint.Y);

            // Calculations are very similar to those in the function GetVeryFirstHandle.
            // To find how long the vector from prevHandle to veryLastHandle needs to be, we find the proportion:
            double proportion = 1 - 2 * dotProduct / (Math.Pow(GetLength(prevpPoint, lastpPoint), 2));

            // Lastly, to point prevHandle we add vector parallel to vector prevpPoint-lastpPoint scaled by the  proportion:
            veryLastHandle.X = Convert.ToInt32(proportion * (prevpPoint.X - lastpPoint.X) + prevHandle.X);
            veryLastHandle.Y = Convert.ToInt32(proportion * (prevpPoint.Y - lastpPoint.Y) + prevHandle.Y);

            // We have achieved a "symmetrical" point to prevHandle; both of these points are on the same side of the bezier line.

            return veryLastHandle;
        }
        

        // Modify coordinates of a chosen control point.
        private void ModifycPoint(MouseEventArgs e)
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

                // every third control point on a composite line is also a knot point therefore moved as a knot point
                else if (j % 3 == 0)
                {
                    localPoint = null;
                    modifyLineType = BezierType.Nothing;
                }

                else if (e.Button == MouseButtons.Left)
                {
                    movedLine[i] = MoveType.LeftClick;
                }

                else if (e.Button == MouseButtons.Right)
                {
                    movedLine[i] = MoveType.RightClick;
                }

                return;
            }

            else if (rbKeyboardModify.Checked == true)
            {
                // when modifying points by keyboard, intialize the form of coordinates
                FormCoordinates form_KeyboardAdd = new FormCoordinates(FormType.Modify, modifyLineType);
                form_KeyboardAdd.ShowDialog();

                movedLine[i] = MoveType.pPoints;
                modifyLineType = BezierType.Nothing;
                localPoint = null;
            }

            pbCanva.Invalidate();
            return;
        }


        // Modify coordinates of a chosen line point.
        private void ModifypPoint()
        {
            int i = localPoint.Item1;
            modifyLineType = allLines[i];

            if (rbKeyboardModify.Checked == true)
            {
                // when modifying points by keyboard, intialize the form of coordinates
                FormCoordinates form_KeyboardAdd = new FormCoordinates(FormType.Modify, modifyLineType);
                form_KeyboardAdd.ShowDialog();

                if (modifyLineType == BezierType.pPoints || modifyLineType == BezierType.LeastSquares)
                {
                    AddcPointsInterpolation(i);
                }

                modifyLineType = BezierType.Nothing;
                localPoint = null;
            }

            pbCanva.Invalidate();
            return;
        }


        // To ensure C2 continuity, when dragging a control point of a <Composite> curve with the left mouse button, 
        // the opposite handle needs to move as well.
        private void ModifyHandleComposite(Point modifyHandle, Point middlepPoint, Point oppositeHandle, int opposite)
        {

            // It doesn't make mathematical sense and makes an error for two control points in <Composite> curve segment to have the same location.
            if (middlepPoint == modifyHandle)
            {
                modifyHandle.X++;
                modifyHandle.Y++;
            }

            //We can look at these calculations as vector operations. We want for vector middle-change to keep its length, 
            //but change its direction so it starts from middle point and is parallel to moving-middle vector.
            //To do that, we take unit vector from moving-middle (devide moving-middle with its length) and multiply that by 
            //middle-change length. Finally, we add that to middle point.

            double proportion = GetLength(middlepPoint, oppositeHandle) / GetLength(modifyHandle, middlepPoint);

            oppositeHandle.X = Convert.ToInt32(middlepPoint.X + proportion * (middlepPoint.X - modifyHandle.X));
            oppositeHandle.Y = Convert.ToInt32(middlepPoint.Y + proportion * (middlepPoint.Y - modifyHandle.Y));

            cPointsAll[localPoint.Item1][opposite] = oppositeHandle;

            return;
        }


        // To ensure C2 continuity and make sure no other points move when dragging a control point of a <Composite> curve with the right mouse button, 
        //the control point can only be moved in a straight line away from the middle point. 
        private void ModifyHandleCompositeStraight(Point modifyHandle, Point middlepPoint, Point oppositeHandle)
        {
            const int maxDistanceToMouse = 100; // maximum distance between mouse location and control point being dragged; chosen arbitrary

            int i = localPoint.Item1;
            int j = localPoint.Item2;

            if (GetLength(modifyHandle, cPointsAll[i][j]) > maxDistanceToMouse)
            {
                return;
            }

            Point result = new Point();

            // To move the control point in straight line, we take unit vector from the middlepPoint to 
            // the place control point was before moving (modifyHandle). It's known that modifyHandle was on the needed line. 
            // Than we scale this unit vector by the distance mouse is from the middlepPoint and at last add this vector to the middlepPoint.

            double prop = GetLength(middlepPoint, modifyHandle) / GetLength(oppositeHandle, middlepPoint);

            result.X = Convert.ToInt32(middlepPoint.X + prop * (middlepPoint.X - oppositeHandle.X));
            result.Y = Convert.ToInt32(middlepPoint.Y + prop * (middlepPoint.Y - oppositeHandle.Y));

            cPointsAll[i][j] = result;

            return;
        }


        // Modify coordinates of a chosen knot point of <Composite> curve.
        public static void ModifypPointComposite(Point mouseLocation)
        {
            int i = localPoint.Item1;
            int j = localPoint.Item2;

            Point pointOld = new Point();
            pointOld = pPointsAll[i][j];

            // every knot point of <Composite> curve is also a control point; change both these point coordinates:
            pPointsAll[i][j] = mouseLocation;
            cPointsAll[i][j * 3] = mouseLocation;

            // We can look at these calculations as vector operations. 
            // We want for the adjacent handles of the knot point to stay in the same position relative to the knot point. 
            // To do that, we take vectors from knot point to control points and add those vectors to the new knot point coordinates.

            Point newcPoint = new Point();

            // first knot point doesn't have the first handle
            if (j != 0)
            {
                newcPoint.X = mouseLocation.X - pointOld.X + cPointsAll[i][j * 3 - 1].X;
                newcPoint.Y = mouseLocation.Y - pointOld.Y + cPointsAll[i][j * 3 - 1].Y;
                cPointsAll[i][j * 3 - 1] = newcPoint;
            }

            //last knot point doesn't have the second handle
            if (j != pPointsAll[i].Count - 1)
            {
                newcPoint.X = mouseLocation.X - pointOld.X + cPointsAll[i][j * 3 + 1].X;
                newcPoint.Y = mouseLocation.Y - pointOld.Y + cPointsAll[i][j * 3 + 1].Y;
                cPointsAll[i][j * 3 + 1] = newcPoint;
            }

            return;
        }


        // Change parametrization method and show the method being used now.
        private void ChangeParametrization()
        {
            int i = localPoint.Item1;
            ParamType paramType = parametrization[i];

            if (allLines[i] == BezierType.cPoints || allLines[i] == BezierType.Composite)
            {
                error.Text = "<" + allLines[i] + "> lines doesn't use parametrization!";
                return;
            }

            // Show the real parametrization type of the selected line:

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


        // Find if there is a control or knot point near mouse location
        private void FindLocalPoint(List<List<Point>> PointsAll, Point MouseLocation)
        {
            const int localRadius = 7; // radius of neiborghood, used when selecting a point with mouse; chosen arbitrary

            for (int i = 0; i < PointsAll.Count; i++)
            {
                if (PointsAll[i] != null)
                {
                    for (int j = 0; j < PointsAll[i].Count; j++)
                    {
                        if (GetLength(MouseLocation, PointsAll[i][j]) < localRadius)
                        {
                            localPoint = new Tuple<int, int>(i, j);
                        }
                    }
                }
            }

            return;
        }


        // Calculate control points for interpolated curves - <4 pPoints> and <Least Squares>.
        private void AddcPointsInterpolation(int i)
        {
            List<Point> pList = pPointsAll[i];

            // This method of curve fitting uses least squares method, so that distance errors from given line points to the Bezier curve 
            // at respective t values is the smallest possible. 
            // To get control point coordinates, we will use formula C = M^1 * ( T^T * T )^1 * T^T * P
            // For more calculation information see documentation.

            // We will represent Bezier curve in its matrix form.

            // Matrix M contains coefficients of an expanded Bezier curve function. We will use only cubic Bezier curves therefor M always is:
            var matrix = Matrix<double>.Build;
            double[,] arrayM4 = new double[4, 4]
                { 
                    { 1, 0, 0, 0 }, 
                    { -3, 3, 0, 0 }, 
                    { 3, -6, 3, 0 }, 
                    { -1, 3, -3, 1 }
                };

            // Matrix P contains coordinates of all knot points:
            double[,] arrayP = new double[pList.Count, 2];
            for (int j = 0; j < pList.Count; j++)
            {
                arrayP[j, 0] = pList[j].X;
                arrayP[j, 1] = pList[j].Y;
            }

            var matrixP = matrix.DenseOfArray(arrayP);
            var matrixM4 = matrix.DenseOfArray(arrayM4);
            var matrixM4Inv = matrixM4.Inverse();

            // Bezier curves are parametric, so we need appropriate t values to tie each knot point to coordinates of points on the curve.
            // This parametrization can be done in different ways; we will store the resulting t values in a list sValues.
            List<double> sValues = new List<double>();

            if (parametrization[i] == ParamType.Uniform)
            {
                sValues = GetsValuesUniform(pList);
            }

            else if (parametrization[i] == ParamType.Chord)
            {
                sValues = GetsValuesChord(pList);
            }

            else if (parametrization[i] == ParamType.Centripetal)
            {
                sValues = GetsValuesCentripetal(pList);
            }

            var matrixS = matrix.DenseOfArray(GetArrayS(sValues));
            var matrixSTranspose = matrixS.Transpose();
            var matrixSSTr = matrixSTranspose * matrixS;
            var matrixSSTrInv = matrixSSTr.Inverse();

            var matrixMul1 = matrixM4Inv * matrixSSTrInv;
            var matrixMul2 = matrixMul1 * matrixSTranspose;

            var matrixC = matrixMul2 * matrixP;

            // if this is the first time calculating control points
            if (cPointsAll[i] == null)
            {
                cPoints = new List<Point>();

                for (int j = 0; j < 4; j++)
                {
                    Point tmp = new Point(Convert.ToInt32(matrixC[j, 0]), Convert.ToInt32(matrixC[j, 1]));
                    cPoints.Add(tmp);
                }
                cPointsAll[i] = cPoints;
            }

            // if we are modifying a curve
            else
            {
                for (int j = 0; j < 4; j++)
                {
                    Point tmp = new Point(Convert.ToInt32(matrixC[j, 0]), Convert.ToInt32(matrixC[j, 1]));
                    cPointsAll[i][j] = tmp;
                }
            }

            return;
        }


        // Bezier curve parametrization method where t values are equally spaced.
        private List<double> GetsValuesUniform(List<Point> pList)
        {
            List<double> sValues = new List<double>();

            for (int i = 0; i < pList.Count; i++)
            {
                double s = (double)i / (pList.Count - 1);
                sValues.Add(s);
            }
            return (sValues);
        }


        // Bezier curve parametrization method where t values are aligned with distance along the polygon of control points.
        private List<double> GetsValuesChord(List<Point> pList)
        {
            // At the first point, we're fixing t = 0, at the last point t = 1. Anywhere in between t value is equal to the distance
            // along the polygon scaled to the [0,1] domain.

            List<double> sValues = new List<double>();

            // First we calculate distance along the polygon for each point:
            List<double> dPoints = new List<double> { 0 };
            for (int i = 1; i < pList.Count; i++)
            {
                double d = dPoints[i - 1] + GetLength(pList[i - 1], pList[i]);
                dPoints.Add(d);
            }

            // Then we scale these values to [0, 1] domain:
            for (int i = 0; i < pList.Count; i++)
            {
                double s = dPoints[i] / dPoints[pList.Count - 1];
                sValues.Add(s);
            }

            return (sValues);
        }


        // Bezier curve parametrization method where t values are aligned with square root of the distance along the polygon.
        private List<double> GetsValuesCentripetal(List<Point> pList)
        {
            // At the first point, we're fixing t = 0, at the last point t = 1. Anywhere in between t value is equal to the
            // square root of the distance along the polygon scaled to the [0,1] domain.

            List<double> sValues = new List<double>();

            // First we calculate the square root of distance along the polygon for each point:
            List<double> dPoints = new List<double> { 0 };
            for (int i = 1; i < pList.Count; i++)
            {
                double d = dPoints[i - 1] + Math.Sqrt(GetLength(pList[i - 1], pList[i]));
                dPoints.Add(d);
            }

            // Then we scale these values to [0, 1] domain:
            for (int i = 0; i < pList.Count; i++)
            {
                double s = dPoints[i] / dPoints[pList.Count - 1];
                sValues.Add(s);
            }

            return (sValues);
        }


        // Make matrix S and fill it using sValues from paramtetrization
        private double[,] GetArrayS(List<double> sValues)
        {
            //  see documentation to see why its done this way
            double[,] arrayS = new double[sValues.Count, 4];

            for (int i = 0; i < sValues.Count; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    arrayS[i, j] = Math.Pow(sValues[i], j);
                }
            }
            return arrayS;
        }


        // Get length between two points
        private double GetLength(Point firstPoint, Point secondPoint)
        {
            return Math.Sqrt(Math.Pow(firstPoint.X - secondPoint.X, 2) + Math.Pow(firstPoint.Y - secondPoint.Y, 2));
        }


        // Choose a .txt file and output a list of points from it.
        private List<Point> GetPointsfromFile()

        {
            List<Point> pointList = new List<Point>();
            Point point = new Point();

            string path = "";
            string textLine = "";

            OpenFileDialog dialog = new OpenFileDialog
            {
                Title = "Open Text File",
                Filter = "TXT files|*.txt", // only .txt files are supported
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


        // Output coordinates of control points to .txt file.
        private void OutputcPointsToFile()
        {
            int i = localPoint.Item1;

            string folderPath = "";

            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                folderPath = dialog.SelectedPath;
            }

            string path = Path.Combine(folderPath, "points.txt");
            using (var file = new StreamWriter(path, true))
            {
                file.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")); // write date and time in the first line
                file.WriteLine("<" + allLines[i] + "> line: \n");

                for (int j = 0; j < cPointsAll[i].Count; j++)
                {
                    string tmp = "C" + (j + 1) + ": (" + cPointsAll[i][j].X + "; " + cPointsAll[i][j].Y + ")"; // in each line write coordinates of one control point
                    file.WriteLine(tmp);
                }

                file.WriteLine("\n \n");
            }
        }


        // Output coordinates of knot points to .txt file.
        private void OutputpPointsToFile()
        {
            int i = localPoint.Item1;

            string folderPath = "";

            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                folderPath = dialog.SelectedPath;
            }

            string path = Path.Combine(folderPath, "points.txt");
            using (var file = new StreamWriter(path, true))
            {
                file.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")); // write date and time in the first line
                file.WriteLine("<" + allLines[i] + "> line: \n");

                for (int j = 0; j < pPointsAll[i].Count; j++)
                {
                    string tmp = "P" + (j + 1) + ": (" + pPointsAll[i][j].X + "; " + pPointsAll[i][j].Y + ")"; // in each line write coordinates of one knot point
                    file.WriteLine(tmp);
                }

                file.WriteLine("\n \n");
            }
        }

    }
}