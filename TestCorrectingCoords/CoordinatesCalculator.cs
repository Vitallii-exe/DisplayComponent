namespace ImageDisplayComponent
{
    public class CoordinatesCalculator
    {
        public static PointF GetImageCursorF(Point controlCursor, PointF scroll, float scale)
        {
            float resultCursorX = controlCursor.X / scale + scroll.X;
            float resultCursorY = controlCursor.Y / scale + scroll.Y;
            return new PointF(resultCursorX, resultCursorY);
        }

        public static Point GetImageCursor(Point controlCursor, PointF scroll, float scale)
        {
            float resultCursorX = controlCursor.X / scale + scroll.X;
            float resultCursorY = controlCursor.Y / scale + scroll.Y;
            return new Point((int)resultCursorX, (int)resultCursorY);
        }

        public static PointF GetControlCursor(PointF imageCursor, PointF scroll, float scale)
        {
            float resultCursorX = (imageCursor.X - scroll.X) * scale;
            float resultCursorY = (imageCursor.Y - scroll.Y) * scale;
            return new PointF(resultCursorX, resultCursorY);
        }

        public static PointF GetScroll(Point controlCursor, PointF imageCursor, float scale)
        {
            float resultScrollX = imageCursor.X - controlCursor.X / scale;
            float resultScrollY = imageCursor.Y - controlCursor.Y / scale;
            return new PointF(resultScrollX, resultScrollY);
        }
    }
}
