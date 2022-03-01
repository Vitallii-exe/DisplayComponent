namespace ImageDisplayComponent
{
    public class DrawUserRectangles : IControlDrawable
    { enum Border
        {
            LeftTop,
            LeftBottom,
            RightTop,
            RightBottom,
            Left,
            Right,
            Top,
            Bottom,
            No
        }
        enum Condition
        {
            Create,
            Edit
        }
        Condition currentCondition = Condition.Create;
        Border currentBorder = Border.No;

        DisplayUserControl display;
        List<Rectangle> rectangles = new List<Rectangle>();
        Rectangle prewiew = new Rectangle(0, 0, 0, 0);
        int activeRectangleIndex = 0;
        (float X, float Y) initialCursorPosition;
        bool isLeftButtonHolding = false;
        bool isTemporaryDraw = false;

        public DrawUserRectangles(DisplayUserControl display)
        {
            this.display = display;
            display.MouseDown += new MouseEventHandler(DisplayMouseDown!);
            display.MouseUp += new MouseEventHandler(DisplayMouseUp!);
            display.MouseMove += new MouseEventHandler(DisplayMouseMove!);

        }
        void IControlDrawable.DrawOver(Graphics graphics)
        {
            if (isTemporaryDraw)
            {
                graphics.DrawRectangle(Pens.Azure, prewiew);
            }
            for (int i = 0; i < rectangles.Count; i++)
            {
                Rectangle scaledElement = ResizeRect(rectangles[i]);
                if (i == activeRectangleIndex)
                {
                    graphics.DrawRectangle(Pens.Lime, scaledElement);
                }
                else
                {
                    graphics.DrawRectangle(Pens.Black, scaledElement);
                }
            }
            return;
        }

        private Rectangle CalculateRect((float X, float Y) firstPoint, (float X, float Y) secondPoint)
        {
            Size size = new Size((int)Math.Abs(firstPoint.X - secondPoint.X), (int)Math.Abs(firstPoint.Y - secondPoint.Y));
            Point location = new Point(0, 0);

            if (firstPoint.X < secondPoint.X)
            {
                location.X = (int)firstPoint.X;
            }
            else
            {
                location.X = (int)secondPoint.X;
            }

            if (firstPoint.Y < secondPoint.Y)
            {
                location.Y = (int)firstPoint.Y;
            }
            else
            {
                location.Y = (int)secondPoint.Y;
            }
            return new Rectangle(location, size);
        }

        private Rectangle ResizeRect(Rectangle rectangle)
        {
            (float, float) location = (rectangle.X, rectangle.Y);
            (float X, float Y) elementLocation = CoordinatesCalculator.GetControlCursor(location, display.originZoneShift, display.currentScale);
            Size scaledSize = new Size();
            scaledSize.Width = (int)(rectangle.Width * display.currentScale);
            scaledSize.Height = (int)(rectangle.Height * display.currentScale);
            return new Rectangle((int)elementLocation.X, (int)elementLocation.Y, scaledSize.Width, scaledSize.Height);
        }

        private int FindNearestRectangle(List<Rectangle> rectangles, Point cursor)
        {
            int nearestRectIndex = -1;
            Point nearestBorder = new Point(100000, 100000);
            for (int i = 0; i < rectangles.Count; i++)
            {
                if (rectangles[i].Contains(cursor))
                {
                    Point firstDistance = new Point(Math.Abs(cursor.X - rectangles[i].Left), Math.Abs(cursor.Y - rectangles[i].Top));
                    Point secondDistance = new Point(Math.Abs(cursor.X - rectangles[i].Right), Math.Abs(cursor.Y - rectangles[i].Bottom));

                    if (firstDistance.X < nearestBorder.X & firstDistance.Y < nearestBorder.Y)
                    {
                        nearestBorder = firstDistance;
                        nearestRectIndex = i;
                    }
                    if (secondDistance.X < nearestBorder.X & secondDistance.Y < nearestBorder.Y)
                    {
                        nearestBorder = secondDistance;
                        nearestRectIndex = i;
                    }
                }
            }
            return nearestRectIndex;
        }
        private Border GetActiveBorder(Rectangle rectangle, Point cursor)
        {
            int[] rectCoordinatesX = { rectangle.Left, rectangle.Right };
            int[] rectCoordinatesY = { rectangle.Top, rectangle.Bottom };
            //int[] allCoordinates = { rectangle.Left, rectangle.Right, rectangle.Top, rectangle.Bottom };
            int variantsCount = 0;
            Border[] diagonalCursors = { Border.LeftTop, Border.LeftBottom, Border.RightTop, Border.RightBottom };

            foreach (int rectCoordX in rectCoordinatesX)
            {
                foreach (int rectCoordY in rectCoordinatesY)
                {
                    if (Math.Abs(cursor.X - rectCoordX) < 5 & Math.Abs(cursor.Y - rectCoordY) < 5)
                    {
                        return diagonalCursors[variantsCount];
                    }
                    else
                    {
                        variantsCount += 1;
                    }
                }
            }
            if (Math.Abs(cursor.X - rectangle.Left) < 5)
            {
                return Border.Left;
            }
            else if (Math.Abs(cursor.X - rectangle.Right) < 5)
            {
                return Border.Right;
            }
            else if (Math.Abs(cursor.Y - rectangle.Top) < 5)
            {
                return Border.Top;
            }
            else if (Math.Abs(cursor.Y - rectangle.Bottom) < 5)
            {
                return Border.Bottom;
            }
            return Border.No;
        }

        private Cursor GetCursorView(Border border)
        {
            Cursor result = Cursors.Arrow;
            switch (border)
            {
                case Border.Left:
                    result = Cursors.SizeWE;
                    break;
                case Border.Right:
                    result = Cursors.SizeWE;
                    break;
                case Border.Top:
                    result = Cursors.SizeNS;
                    break;
                case Border.Bottom:
                    result = Cursors.SizeNS;
                    break;
                case Border.LeftTop:
                    result = Cursors.SizeNWSE;
                    break;
                case Border.LeftBottom:
                    result = Cursors.SizeNESW;
                    break;
                case Border.RightTop:
                    result = Cursors.SizeNESW;
                    break;
                case Border.RightBottom:
                    result = Cursors.SizeNWSE;
                    break;
            }
            return result;
        }

        private Rectangle ResizeRectangle(Rectangle rectangle, Point cursor, Border border)
        {
            switch (border)
            {
                case Border.Left:
                    rectangle.Width = rectangle.Right - cursor.X;
                    rectangle.X = cursor.X;
                    break;
                case Border.Right:
                    rectangle.Width = cursor.X - rectangle.X;
                    break;
                case Border.Top:
                    rectangle.Height = rectangle.Bottom - cursor.Y;
                    rectangle.Y = cursor.Y;
                    break;
                case Border.Bottom:
                    rectangle.Height = cursor.Y - rectangle.Y;
                    break;
                case Border.LeftTop:
                    rectangle.Width = rectangle.Right - cursor.X;
                    rectangle.Height = rectangle.Bottom - cursor.Y;
                    rectangle.Location = cursor;
                    break;
                case Border.LeftBottom:
                    rectangle.Width = rectangle.Right - cursor.X;
                    rectangle.X = cursor.X;
                    rectangle.Height = cursor.Y - rectangle.Y;
                    break;
                case Border.RightTop:
                    rectangle.Width = cursor.X - rectangle.X;
                    rectangle.Height = rectangle.Bottom - cursor.Y;
                    rectangle.Y = cursor.Y;
                    break;
                case Border.RightBottom:
                    rectangle.Width = cursor.X - rectangle.X;
                    rectangle.Height = cursor.Y - rectangle.Y;
                    break;
            }
            return rectangle;
        }
        private void DisplayMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point controlCursor = display.PointToClient(Cursor.Position);
                if (currentCondition == Condition.Create)
                {
                    initialCursorPosition = CoordinatesCalculator.GetImageCursorF(controlCursor, display.originZoneShift, display.currentScale);
                }
                isLeftButtonHolding = true;
            }
            return;
        }

        private void DisplayMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point controlCursor = display.PointToClient(Cursor.Position);
                (float X, float Y) imageCursor;
                imageCursor = CoordinatesCalculator.GetImageCursorF(controlCursor, display.originZoneShift, display.currentScale);
                if (currentCondition == Condition.Create)
                {
                    Rectangle rectangle = CalculateRect(initialCursorPosition, imageCursor);
                    rectangles.Add(rectangle);
                }
                isLeftButtonHolding = false;

            }
            return;
        }

        private Border InvertBorder(Border border, bool isHorisontal)
        {
            Border result = Border.No;
            switch (border)
            {
                case Border.Left:
                    result = Border.Right;
                    break;
                case Border.Right:
                    result = Border.Left;
                    break;
                case Border.Top:
                    result = Border.Bottom;
                    break;
                case Border.Bottom:
                    result = Border.Top;
                    break;
                case Border.LeftTop:
                    result = isHorisontal ? Border.RightTop : Border.LeftBottom;
                    break;
                case Border.LeftBottom:
                    result = isHorisontal ? Border.RightBottom : Border.LeftTop;
                    break;
                case Border.RightTop:
                    result = isHorisontal ? Border.LeftTop : Border.RightBottom;
                    break;
                case Border.RightBottom:
                    result = isHorisontal ? Border.LeftBottom : Border.RightTop;
                    break;
            }
            return result;
        }

        private Rectangle ValidateRectangle(Rectangle rectangle, ref Border border)
        {
            if (rectangle.Width <= 0)
            {
                rectangle.X = rectangle.Right;
                rectangle.Width = Math.Abs(rectangle.Width);
                border = InvertBorder(border, true);
            }
            if (rectangle.Height <= 0)
            {
                rectangle.Y = rectangle.Bottom;
                rectangle.Height = Math.Abs(rectangle.Height);
                border = InvertBorder(border, false);
            }
            return rectangle;
        }
        private void DisplayMouseMove(object sender, MouseEventArgs e)
        {
            if (isLeftButtonHolding)
            {
                Point controlCursor = display.PointToClient(Cursor.Position);
                Point imageCursor = CoordinatesCalculator.GetImageCursor(controlCursor, display.originZoneShift, display.currentScale);
                if (currentCondition == Condition.Edit)
                {
                    if (currentBorder != Border.No)
                    {
                        Rectangle newRect = ResizeRectangle(rectangles[activeRectangleIndex], imageCursor, currentBorder);
                        rectangles[activeRectangleIndex] = ValidateRectangle(newRect, ref currentBorder);
                        display.Refresh();
                    }
                }
                if (currentCondition == Condition.Create)
                {
                    // Prewiew drawing rect to user
                    //Point controlCursor = display.PointToClient(Cursor.Position);
                    (float X, float Y) nowCursor = (controlCursor.X, controlCursor.Y);
                    (float X, float Y) initialControlPos = CoordinatesCalculator.GetControlCursor(initialCursorPosition, display.originZoneShift, display.currentScale);
                    prewiew = CalculateRect(initialControlPos, nowCursor);
                    isTemporaryDraw = true;
                    display.Refresh();
                    isTemporaryDraw = false;
                }
            }
            else
            {
                Point controlCursor = display.PointToClient(Cursor.Position);
                Point imageCursor = CoordinatesCalculator.GetImageCursor(controlCursor, display.originZoneShift, display.currentScale);
                int nearestRectIndex = FindNearestRectangle(rectangles, imageCursor);
                if (nearestRectIndex != -1)
                {
                    activeRectangleIndex = nearestRectIndex;
                }
                if (rectangles.Count != 0)
                {
                    currentBorder = GetActiveBorder(rectangles[activeRectangleIndex], imageCursor);
                    display.Cursor = GetCursorView(currentBorder);
                    if (currentBorder == Border.No)
                    {
                        currentCondition = Condition.Create;
                    }
                    else
                    {
                        currentCondition = Condition.Edit;
                    }
                }
                display.Refresh();
            }
            return;
        }
    }
}
