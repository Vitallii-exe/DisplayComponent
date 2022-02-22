using System.Runtime.InteropServices;

namespace ImageDisplayComponent
{
    public partial class DisplayUserControl : UserControl
    {

        float currentScale = 1F;
        Image origin;
        (float X, float Y) originZoneShift;
        Rectangle currentControlRect;
        float[] scaleSteps = { 0.25F, 0.5F, 1F, 1.5F, 2F, 3F, 4F, 5F };
        float scaleFixedStep = 0.5F;
        int scrollCoefficient = 10;
        int currentScaleStepIndex = 2;

        bool isBlockScrollValueChangedEvent = false;

        public DisplayUserControl()
        {
            InitializeComponent();
            origin = Properties.Resources.templateImage3;
        }

        private void DisplayUserControlLoad(object sender, EventArgs e)
        {
            currentControlRect = new Rectangle(0, 0, Size.Width, Size.Height);

            vScrollBar.Maximum = (int)(origin.Height * currentScale);
            vScrollBar.LargeChange = Size.Height;
            if (vScrollBar.Maximum - vScrollBar.LargeChange < 0)
            {
                vScrollBar.Visible = false;
            }

            hScrollBar.Maximum = (int)(origin.Width * currentScale);
            hScrollBar.LargeChange = Size.Width;
            if (hScrollBar.Maximum - hScrollBar.LargeChange < 0)
            {
                hScrollBar.Visible = false;
            }

            return;
        }

        private void ScaleChangedByWheel(bool isScaleUp)
        {
            Point controlCursor = PointToClient(Cursor.Position);
            (float X, float Y) imageCursor = GetImageCursor(controlCursor, originZoneShift, currentScale);

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
            originZoneShift = GetScroll(controlCursor, imageCursor, currentScale);

            vScrollBar.Maximum = (int)(origin.Height * currentScale);
            hScrollBar.Maximum = (int)(origin.Width * currentScale);

            int newVScrollValue = (int)(originZoneShift.Y * currentScale);//ConfirmScrollBarValue(originZoneShift.Y, vScrollBar.Maximum, vScrollBar.Minimum, currentScale);
            int newHScrollValue = (int)(originZoneShift.X * currentScale);//ConfirmScrollBarValue(originZoneShift.X, hScrollBar.Maximum, hScrollBar.Minimum, currentScale);

            if (newVScrollValue > vScrollBar.Maximum - vScrollBar.LargeChange)
            {
                vScrollBar.Maximum = newVScrollValue + vScrollBar.LargeChange;
            }

            if (newHScrollValue > hScrollBar.Maximum - hScrollBar.LargeChange)
            {
                hScrollBar.Maximum = newHScrollValue + hScrollBar.LargeChange;
            }

            if (newVScrollValue < vScrollBar.Minimum)
            {
                vScrollBar.Minimum = newVScrollValue;
            }

            if (newHScrollValue < hScrollBar.Minimum)
            {
                hScrollBar.Minimum = newHScrollValue;
            }

            vScrollBar.Value = newVScrollValue;
            hScrollBar.Value = newHScrollValue;

            hScrollBar.Visible = UpdateScrollVisible(hScrollBar);
            vScrollBar.Visible = UpdateScrollVisible(vScrollBar);

            isBlockScrollValueChangedEvent = false;
            Refresh();

            return;
        }

        private bool UpdateScrollVisible(ScrollBar scrollBar)
        {
            if (scrollBar.Maximum < scrollBar.LargeChange)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private int ConfirmScrollBarValue(float shift, int maximum, int minimum, float scale = 1F)
        {
            int newValue = (int)(scale * shift);

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

        private void ScrollBarsValueChanged(object sender, EventArgs e)
        {
            bool isVertical = sender.Equals(vScrollBar);
            ScrollBar scrollBar;
            ref float currentZoneShift = ref originZoneShift.Y;
            if (isVertical)
            {
                scrollBar = vScrollBar;
                currentZoneShift = ref originZoneShift.Y;

                if (scrollBar.Maximum > origin.Height * currentScale)
                {
                    if (scrollBar.Value < scrollBar.Maximum - scrollBar.LargeChange)
                    {
                        scrollBar.Maximum = scrollBar.Value + scrollBar.LargeChange;
                    }
                }

                else if (scrollBar.Minimum < 0)
                {
                    if (scrollBar.Value > scrollBar.Minimum)
                    {
                        if (scrollBar.Value < 0)
                        {
                            scrollBar.Minimum = scrollBar.Value;
                        }
                        else
                        {
                            scrollBar.Minimum = 0;
                        }
                    }
                }
            }
            else
            {
                scrollBar = hScrollBar;
                currentZoneShift = ref originZoneShift.X;

                if (scrollBar.Maximum > origin.Width * currentScale)
                {
                    if (scrollBar.Value < scrollBar.Maximum - scrollBar.LargeChange)
                    {
                        scrollBar.Maximum = scrollBar.Value + scrollBar.LargeChange;
                    }
                }

                else if (scrollBar.Minimum < 0)
                {
                    if (scrollBar.Value > scrollBar.Minimum)
                    {
                        if (scrollBar.Value < 0)
                        {
                            scrollBar.Minimum = scrollBar.Value;
                        }
                        else
                        {
                            scrollBar.Minimum = 0;
                        }
                    }
                }
            }
            if (!isBlockScrollValueChangedEvent)
            {
                currentZoneShift = scrollBar.Value / currentScale;
                Refresh();
            }
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

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            RedrawImage(e.Graphics);
            return;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Ignore UserControl backgroung drawing before OnPaint()
            return;
        }
    }
}
