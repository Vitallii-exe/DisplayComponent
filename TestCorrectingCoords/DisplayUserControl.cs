using System.Runtime.InteropServices;

namespace ImageDisplayComponent
{
    public partial class DisplayUserControl : UserControl
    {

        float currentScale = 1F;
        Image origin;
        (float X, float Y) Scroll;
        Bitmap buffer;
        Rectangle currentControlRect;
        float[] scaleSteps = { 0.25F, 0.5F, 1F, 1.5F, 2F, 3F, 4F, 5F };
        float scaleFixedStep = 0.5F;
        int currentScaleStepIndex = 2;
        bool myRedraw = true;

        int lastVScroll = 0;
        int lastHScroll = 0;

        public DisplayUserControl()
        {
            InitializeComponent();
            origin = Properties.Resources.templateImage3;
        }

        private void DisplayUserControlLoad(object sender, EventArgs e)
        {
            buffer = new Bitmap(Size.Width, Size.Height);
            currentControlRect = new Rectangle(0, 0, Size.Width, Size.Height);
            PrepareBuffer();
            vScrollBar.Height = Height;
            hScrollBar.Width = Width;

            vScrollBar.Maximum = (int)(origin.Height * currentScale);
            vScrollBar.LargeChange = Size.Height;

            hScrollBar.Maximum = (int)(origin.Width * currentScale);
            hScrollBar.LargeChange = Size.Width;
            return;
        }

        private void ScaleChangedByWheel(bool isScaleUp)
        {
            Point controlCursor = PointToClient(Cursor.Position);
            (float X, float Y) imageCursor = GetImageCursor(controlCursor, Scroll, currentScale);

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
            Scroll = GetScroll(controlCursor, imageCursor, currentScale);

            vScrollBar.Maximum = (int)(origin.Height * currentScale);
            hScrollBar.Maximum = (int)(origin.Width * currentScale);

            int newVScrollValue = ConfirmScrollBarValue((int)Scroll.Y, vScrollBar.Maximum, vScrollBar.Minimum, currentScale);
            int newHScrollValue = ConfirmScrollBarValue((int)Scroll.X, hScrollBar.Maximum, hScrollBar.Minimum, currentScale);


            vScrollBar.Value = newVScrollValue;
            hScrollBar.Value = newHScrollValue;
            myRedraw = true;
            Invalidate();

            return;
        }

        private int ConfirmScrollBarValue(int shift, int maximum, int minimum, float scale)
        {
            int newValue = (int)(scale * shift);

            if (newValue < minimum)
            {
                newValue = minimum;
            }
            else if (newValue > maximum)
            {
                newValue = maximum;
            }
            return newValue;
        }

        private void PrepareBuffer()
        {
            Graphics bufferGraphics = Graphics.FromImage(buffer);
            bufferGraphics.Clear(BackColor);

            bufferGraphics.DrawImage(origin, currentControlRect, Scroll.X, Scroll.Y,
                                     Size.Width / currentScale, Size.Height / currentScale,
                                     GraphicsUnit.Pixel);
            return;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (myRedraw)
            {
                PrepareBuffer();
                e.Graphics.DrawImage(buffer, 0, 0);
                myRedraw = false;
            }
            return;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (!myRedraw)
            {
                base.OnPaintBackground(e);
            }
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

        private void vScrollBarValueChanged(object sender, EventArgs e)
        {
            if (Math.Abs(lastVScroll - vScrollBar.Value) >= 5 * currentScale)
            {
                Scroll.Y = vScrollBar.Value / currentScale;
                PrepareBuffer();
                myRedraw = true;
                Invalidate();
                lastVScroll = vScrollBar.Value;
            }
            return;
        }

        private void hScrollBarValueChanged(object sender, EventArgs e)
        {
            if (Math.Abs(lastHScroll - hScrollBar.Value) >= 5 * currentScale)
            {
                Scroll.X = hScrollBar.Value / currentScale;
                PrepareBuffer();
                myRedraw = true;
                Invalidate();
                lastVScroll = hScrollBar.Value;
            }
            return;
        }
    }
}
