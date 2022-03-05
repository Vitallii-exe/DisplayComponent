namespace ImageDisplayComponent
{
    public interface IControlDrawable
    {
        void DrawOver(Graphics graphics);

    }
    public partial class DisplayUserControl : UserControl
    {
        IControlDrawable draw;

        public float currentScale = 1F;
        public Image origin;
        public (float X, float Y) originZoneShift;
        public Rectangle currentControlRect;

        float[] scaleSteps = { 0.25F, 0.5F, 1F, 1.5F, 2F, 3F, 4F, 5F };
        float scaleFixedStep = 0.5F;
        int scrollCoefficient = 20;
        int currentScaleStepIndex = 2;

        bool isBlockScrollValueChangedEvent = false;
        bool isMiddleMouseButtonHolding = false;
        (float X, float Y) initialCursorPosition;

        public DisplayUserControl()
        {
            InitializeComponent();
            origin = Properties.Resources.templateImage;
            draw = new DrawUserRectangles(this);
        }

        private void SynchronizeScrollBarWithShift(ScrollBar scrollBar, int imageSize, float shift, float scale)
        {
            scrollBar.Maximum = (int)(imageSize * scale);

            int newScrollValue = (int)(shift * scale);

            if (newScrollValue > scrollBar.Maximum - scrollBar.LargeChange)
            {
                vScrollBar.Maximum = newScrollValue + vScrollBar.LargeChange;
            }

            if (newScrollValue < scrollBar.Minimum)
            {
                scrollBar.Minimum = newScrollValue;
            }
            scrollBar.Value = newScrollValue;
            scrollBar.Visible = UpdateScrollVisible(scrollBar);
            return;
        }
        private void ScaleChangedByWheel(bool isScaleUp)
        {
            Point controlCursor = PointToClient(Cursor.Position);
            (float X, float Y) imageCursor = CoordinatesCalculator.GetImageCursorF(controlCursor, originZoneShift, currentScale);

            bool isOutOfRange = false;
            if (isScaleUp)
            {
                if (currentScaleStepIndex < scaleSteps.Length - 1)
                {
                    currentScaleStepIndex += 1;
                }
                else
                {
                    isOutOfRange = true;
                }
            }
            else
            {
                if (currentScaleStepIndex > 0)
                {
                    currentScaleStepIndex -= 1;
                }
                else
                {
                    isOutOfRange = true;
                }
            }
            if (!isOutOfRange)
            {
                currentScale = scaleSteps[currentScaleStepIndex];
            }
            else
            {
                if (isScaleUp)
                {
                    currentScale += scaleFixedStep;
                }
            }
            isBlockScrollValueChangedEvent = true;
            originZoneShift = CoordinatesCalculator.GetScroll(controlCursor, imageCursor, currentScale);
            SynchronizeScrollBarWithShift(vScrollBar, origin.Height, originZoneShift.Y, currentScale);
            SynchronizeScrollBarWithShift(hScrollBar, origin.Width, originZoneShift.X, currentScale);
            isBlockScrollValueChangedEvent = false;
            Refresh();

            return;
        }

        private bool UpdateScrollVisible(ScrollBar scrollBar)
        {
            return scrollBar.Maximum > scrollBar.LargeChange;
        }

        private int ConfirmScrollBarValue(float shift, int maximum, int minimum)
        {
            int newValue = (int)(shift);

            if (newValue > maximum)
            {
                newValue = maximum;
            }

            if (newValue < minimum)
            {
                newValue = minimum;
            }
            return newValue;
        }

        private void RedrawImage(Graphics userControlGraphics)
        {
            userControlGraphics.Clear(BackColor);
            if (currentScale < 1)
            {
                userControlGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            }
            else
            {
                userControlGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            }
            userControlGraphics.DrawImage(origin, currentControlRect, originZoneShift.X, originZoneShift.Y,
                                     Size.Width / currentScale, Size.Height / currentScale,
                                     GraphicsUnit.Pixel);
            
            return;
        }
        

        private void ScrollWheelMove(bool isMoveUp, ScrollBar scrollBar, bool isVertical = true)
        {
            int newValue;
            if (isMoveUp)
            {
                newValue = scrollBar.Value + (int)(scrollCoefficient * currentScale);
            }
            else
            {
                newValue = scrollBar.Value - (int)(scrollCoefficient * currentScale);
            }

            scrollBar.Value = ConfirmScrollBarValue(newValue, scrollBar.Maximum - scrollBar.LargeChange, scrollBar.Minimum);

            if (isVertical)
            {
                originZoneShift.Y = scrollBar.Value / currentScale;
            }
            else
            {
                originZoneShift.X = scrollBar.Value / currentScale;
            }
            Refresh();
            return;
        }

        private void DisplayUserControl_MouseClick(object sender, MouseEventArgs e)
        {
            //IControlDrawable draw = new DrawUserRectangles(this);

            //Point controlCursor = PointToClient(Cursor.Position);
            //(float X, float Y) imageCursor = CoordinatesCalculator.GetImageCursor(controlCursor, originZoneShift, currentScale);

            //draw.DrawOver(CreateGraphics(), imageCursor);
        }
    }
}
