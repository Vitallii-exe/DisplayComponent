namespace TestCorrectingCoords
{
    public partial class DisplayUserControl : UserControl
    {
        float currentScale = 1F;
        Image origin;
        Point shift = new Point(0, 0);
        (float Width, float Height) oldSize;
        bool myRedraw = true;

        public DisplayUserControl()
        {
            InitializeComponent();
            origin = Properties.Resources.templateImage2;
        }

        private void ScaleChangedByWheel(bool isScaleUp)
        {
            float scaleBeforeChanging = currentScale;
            if (isScaleUp)
            {
                currentScale += 0.1F;
            }
            else
            {
                currentScale -= 0.1F;
            }
            oldSize = (Size.Width, Size.Height);
            Point relativeCursorPos = PointToClient(Cursor.Position);
            relativeCursorPos = GetRelativeCoord(relativeCursorPos, scaleBeforeChanging);

            shift = GetCoordToScaleWithCursorBinding(relativeCursorPos, oldSize, (Size.Width / currentScale, Size.Height / currentScale));
            myRedraw = true;
            Invalidate();
            System.Diagnostics.Debug.WriteLine("Now scale is: " + currentScale);
            return;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (myRedraw)
            {
                Rectangle destRect = new Rectangle(0, 0, Size.Width, Size.Height);
                GraphicsUnit units = GraphicsUnit.Pixel;

                e.Graphics.DrawImage(origin, destRect, shift.X, shift.Y, Size.Width / currentScale, Size.Height / currentScale, units);
                myRedraw = false;
            }
            //oldSize = (Size.Width, Size.Height);
            return;
        }

        private Point GetCoordToScaleWithCursorBinding(Point elementCursor, (float Width, float Height) oldSize, (float Width, float Height) newSize)
        {
            //(float X, float Y) cursorRatio = (elementCursor.X / oldSize.Width, elementCursor.Y / oldSize.Height);
            (float X, float Y) newCursorPosition = (elementCursor.X / currentScale, elementCursor.Y / currentScale);

            Point result = new Point((int)(elementCursor.X - newCursorPosition.X), (int)(elementCursor.Y - newCursorPosition.Y));
            return result;
        }

        private Point GetRelativeCoord(Point absPoint, float scale)
        {
            absPoint.X = (int)(absPoint.X / scale);
            absPoint.Y = (int)(absPoint.Y / scale);
            absPoint.X += shift.X;
            absPoint.Y += shift.Y;
            if (absPoint.X < 0)
            {
                absPoint.X = 0;
            }
            if (absPoint.Y < 0)
            {
                absPoint.Y = 0;
            }
            //if (absPoint.X > Size.Width / currentScale)
            //{
            //    absPoint.X = (int)(Size.Width / currentScale);
            //}
            //if (absPoint.Y > Size.Height / currentScale)
            //{
            //    absPoint.X = (int)(Size.Height / currentScale);
            //}
            //absPoint.X = (int)(absPoint.X * currentScale);
            //absPoint.Y = (int)(absPoint.Y * currentScale);
            return absPoint;
        }

        private void DisplayUserControl_MouseClick(object sender, MouseEventArgs e)
        {
            Point relativeCursorPos = PointToClient(Cursor.Position);
            TmpDrawRect(GetRelativeCoord(relativeCursorPos, currentScale));

        }

        public void TmpDrawRect(Point cursor)
        {
            //Temporary function to draw rect by coord
            Graphics g = Graphics.FromImage(origin);
            Rectangle rect = new Rectangle(cursor.X, cursor.Y, 10, 10);
            g.DrawRectangle(new Pen(Color.Red, .5f), rect);
            myRedraw = true;
            Refresh();
            return;
        }
    }
}
