namespace ImageDisplayComponent
{
    public partial class DisplayUserControl : UserControl
    {
        const int WM_MOUSEWHEEL = 0x020A;
        const int MK_CONTROL = 0x8;
        const int MK_SHIFT = 0x4;
        const int wheelForward = 120;
        const int wheelBackward = -120;

        private (int, int) SplitWParam(IntPtr _wParam)
        {
            uint wParam = unchecked(IntPtr.Size == 8 ? (uint)_wParam.ToInt64() : (uint)_wParam.ToInt32());
            int lowOrder = unchecked((short)wParam);
            int highOrder = unchecked((short)(wParam >> 16));
            return (lowOrder, highOrder);
        }
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_MOUSEWHEEL)
            {
                (int withKey, int delta) wParam = SplitWParam(m.WParam);
                if (wParam.withKey == MK_CONTROL)
                {
                    if (wParam.delta == wheelForward)
                    {
                        ScaleChangedByWheel(true);
                    }
                    else if (wParam.delta == wheelBackward)
                    {
                        ScaleChangedByWheel(false);
                    }
                    return;
                }
                else if (wParam.withKey == MK_SHIFT)
                {
                    if (wParam.delta == wheelForward)
                    {
                        ScrollWheelMove(MoveDirection.Up, hScrollBar, Axis.Horisontal);
                    }
                    else if (wParam.delta == wheelBackward)
                    {
                        ScrollWheelMove(MoveDirection.Down, hScrollBar, Axis.Horisontal);
                    }
                    return;
                }
                else
                {
                    if (wParam.delta == wheelForward)
                    {
                        ScrollWheelMove(MoveDirection.Down, vScrollBar, Axis.Vertical);
                    }
                    else if (wParam.delta == wheelBackward)
                    {
                        ScrollWheelMove(MoveDirection.Up, vScrollBar, Axis.Vertical);
                    }
                }
            }
            base.WndProc(ref m);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (!isOverlayRedraw)
            {
                Graphics graphics = Graphics.FromImage(buffer);
                RedrawImage(graphics);
            }
            e.Graphics.DrawImageUnscaled(buffer, 0, 0);
            draw.DrawOver(e.Graphics);
            return;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Ignore UserControl backgroung drawing before OnPaint()
            return;
        }
    }
}