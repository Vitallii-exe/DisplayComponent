namespace TestCorrectingCoords
{
    public partial class DisplayUserControl : UserControl
    {
        float currentScale = 1F;
        Image origin;
        Point shift = new Point(0, 0);
        (float X, float Y) Scroll;
        bool myRedraw = true;

        public DisplayUserControl()
        {
            InitializeComponent();
            origin = Properties.Resources.templateImage2;
        }

        private void ScaleChangedByWheel(bool isScaleUp)
        {
            Point controlCursor = PointToClient(Cursor.Position);
            (float X, float Y) imageCursor = GetImageCursor(controlCursor, Scroll, currentScale);

            if (isScaleUp)
            {
                currentScale += 0.1F;
            }
            else
            {
                currentScale -= 0.1F;
            }
            Scroll = GetScroll(controlCursor, imageCursor, currentScale);
            myRedraw = true;
            Invalidate();

            return;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (myRedraw)
            {
                Rectangle destRect = new Rectangle(0, 0, Size.Width, Size.Height);
                GraphicsUnit units = GraphicsUnit.Pixel;

                e.Graphics.DrawImage(origin, destRect, Scroll.X, Scroll.Y, Size.Width / currentScale, Size.Height / currentScale, units);
                myRedraw = false;
            }
            //oldSize = (Size.Width, Size.Height);
            return;
        }

        private (float, float) GetImageCursor(Point controlCursor, (float X, float Y) scroll, float scale)
        {
            float resultCursorX = controlCursor.X / scale + scroll.X;
            float resultCursorY = controlCursor.Y / scale + scroll.Y;
            return (resultCursorX, resultCursorY);
        }

        private (float, float) GetScroll(Point controlCursor, (float X, float Y) imageCursor, float scale)
        {
            float resultScrollX = imageCursor.X - controlCursor.X / scale;
            float resultScrollY = imageCursor.Y - controlCursor.Y / scale;
            return (resultScrollX, resultScrollY);
        }

        private void DisplayUserControl_MouseMove(object sender, MouseEventArgs e)
        {
            Point controlCursor = PointToClient(Cursor.Position);
            (float X, float Y) imageCursor = GetImageCursor(controlCursor, Scroll, currentScale);

            System.Diagnostics.Debug.WriteLine("_____________________________________________");
            System.Diagnostics.Debug.WriteLine("controlCursor is: X = " + controlCursor.X + " Y = " + controlCursor.Y);
            System.Diagnostics.Debug.WriteLine("imageCursor is: X = " + imageCursor.X + " Y = " + imageCursor.Y);
            System.Diagnostics.Debug.WriteLine("currentScale is: " + currentScale.ToString());
            System.Diagnostics.Debug.WriteLine("Scroll Value is: X = " + Scroll.X + " Y = " + Scroll.Y);
            return;

        }
    }
}
