namespace TestCorrectingCoords
{
    public partial class DisplayUserControl : UserControl
    {
        const int WM_MOUSEWHEEL = 0x020A;
        const int WM_PAINT = 0xf;
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
                        //HorizontalScrollChanged(true);
                    }
                    else if (wParam.delta == wheelBackward)
                    {
                        //HorizontalScrollChanged(false);
                    }
                    return;
                }
            }
            //else if (m.Msg == WM_PAINT)
            //{
            //    if (isBlockRedraw)
            //    {
            //        return;
            //    }
            //}
            base.WndProc(ref m);
        }
    }
}