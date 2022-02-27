namespace ImageDisplayComponent
{
    public class CoordinatesCalculator
    {
        public static (float, float) GetImageCursorF(Point controlCursor, (float X, float Y) scroll, float scale)
        {
            float resultCursorX = controlCursor.X / scale + scroll.X;
            float resultCursorY = controlCursor.Y / scale + scroll.Y;
            return (resultCursorX, resultCursorY);
        }

        public static Point GetImageCursor(Point controlCursor, (float X, float Y) scroll, float scale)
        {
            float resultCursorX = controlCursor.X / scale + scroll.X;
            float resultCursorY = controlCursor.Y / scale + scroll.Y;
            return new Point((int)resultCursorX, (int)resultCursorY);
        }

        public static (float, float) GetControlCursor((float X, float Y) imageCursor, (float X, float Y) scroll, float scale)
        {
            float resultCursorX = (imageCursor.X - scroll.X) * scale;
            float resultCursorY = (imageCursor.Y - scroll.Y) * scale;
            return (resultCursorX, resultCursorY);
        }

        public static (float, float) GetScroll(Point controlCursor, (float X, float Y) imageCursor, float scale)
        {
            float resultScrollX = imageCursor.X - controlCursor.X / scale;
            float resultScrollY = imageCursor.Y - controlCursor.Y / scale;
            return (resultScrollX, resultScrollY);
        }
    }
}
