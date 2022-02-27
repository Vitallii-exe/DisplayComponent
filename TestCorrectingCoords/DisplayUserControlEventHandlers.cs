namespace ImageDisplayComponent
{
    public partial class DisplayUserControl : UserControl
    {
        private void DisplayUserControlLoad(object sender, EventArgs e)
        {
            currentControlRect = new Rectangle(0, 0, Size.Width, Size.Height);

            draw = new DrawUserRectangles(this);

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
        private void DisplayUserControlMouseMove(object sender, MouseEventArgs e)
        {
            if (isMiddleMouseButtonHolding)
            {
                Point controlCursor = PointToClient(Cursor.Position);
                isBlockScrollValueChangedEvent = true;
                originZoneShift = CoordinatesCalculator.GetScroll(controlCursor, initialCursorPosition, currentScale);

                SynchronizeScrollBarWithShift(vScrollBar, origin.Height, originZoneShift.Y, currentScale);
                SynchronizeScrollBarWithShift(hScrollBar, origin.Width, originZoneShift.X, currentScale);
                isBlockScrollValueChangedEvent = false;
                Refresh();
            }
        }

        private void DisplayUserControlMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                Point controlCursor = PointToClient(Cursor.Position);
                initialCursorPosition = CoordinatesCalculator.GetImageCursorF(controlCursor, originZoneShift, currentScale);
                isMiddleMouseButtonHolding = true;
            }
            return;
        }

        private void DisplayUserControlMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                isMiddleMouseButtonHolding = false;
            }
            return;
        }
    }
}
