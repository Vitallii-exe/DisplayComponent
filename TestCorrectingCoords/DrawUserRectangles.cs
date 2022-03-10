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
            No,
            All
        }
        enum Condition
        {
            Create,
            Edit,
            Move
        }
        Condition currentCondition = Condition.Create;
        Border currentBorder = Border.No;

        DisplayUserControl display;
        List<Rectangle> rectangles = new List<Rectangle>();
        Rectangle preview = new Rectangle(0, 0, 0, 0);
        int activeRectangleIndex = 0;
        (Pen active, Pen passive) pens = (new Pen(Color.Red, 2), new Pen(Color.DarkRed, 2));
        Pen previewPen = new Pen(Color.LawnGreen, 2);
        PointF initialCursorPosition;
        Point relativeCursor = new Point(0, 0);
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
                Rectangle scaledElement = ResizeRect(preview);
                graphics.DrawRectangle(previewPen, scaledElement);
            }
            DrawRectangleArray(graphics, rectangles, activeRectangleIndex, pens);
            return;
        }

        private Rectangle CalculateRect(PointF firstPoint, PointF secondPoint)
        {
            Size size = new Size((int)Math.Abs(firstPoint.X - secondPoint.X), (int)Math.Abs(firstPoint.Y - secondPoint.Y));
            Point location = new Point(0, 0);
            location.X = (firstPoint.X < secondPoint.X) ? (int)firstPoint.X : (int)secondPoint.X;
            location.Y = (firstPoint.Y < secondPoint.Y) ? (int)firstPoint.Y : (int)secondPoint.Y;
            return new Rectangle(location, size);
        }

        private void DrawRectangleArray(Graphics graphics, List<Rectangle> rectangles, int activeIndex, (Pen active, Pen passive) pens)
        {
            for (int i = 0; i < rectangles.Count; i++)
            {
                Rectangle scaledElement = ResizeRect(rectangles[i]);
                Pen pen;
                if (i == activeIndex)
                {
                    pen = pens.active;
                }
                else
                {
                    pen = pens.passive;
                }
                pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;

                if (scaledElement.Width == 0)
                {
                    graphics.DrawLine(pen, scaledElement.Location, new Point(scaledElement.Left, scaledElement.Bottom));
                }
                else if (scaledElement.Height == 0)
                {
                    graphics.DrawLine(pen, scaledElement.Location, new Point(scaledElement.Right, scaledElement.Top));
                }
                else
                {
                    graphics.DrawRectangle(pen, scaledElement);
                }
            }
            return;
        }

        private Rectangle ResizeRect(Rectangle rectangle)
        {
            PointF location = new PointF(rectangle.X, rectangle.Y);
            PointF elementLocation = CoordinatesCalculator.GetControlCursor(location, display.originZoneShift, display.currentScale);
            Size scaledSize = new Size();
            scaledSize.Width = (int)(rectangle.Width * display.currentScale);
            scaledSize.Height = (int)(rectangle.Height * display.currentScale);
            return new Rectangle((int)elementLocation.X, (int)elementLocation.Y, scaledSize.Width, scaledSize.Height);
        }

        private int FindNearestRectangle(List<Rectangle> rectangles, Point cursor)
        {
            const int bigDistance = 10000;
            int nearestRectIndex = 0;
            int nearestDistance = bigDistance;
            for (int i = 0; i < rectangles.Count; i++)
            {
                int[] distances = { bigDistance, bigDistance, bigDistance, bigDistance };
                int leftDiff = Math.Abs(cursor.X - rectangles[i].Left);
                int rightDiff = Math.Abs(cursor.X - rectangles[i].Right);
                int topDiff = Math.Abs(cursor.Y - rectangles[i].Top);
                int bottomDiff = Math.Abs(cursor.Y - rectangles[i].Bottom);

                distances[0] = (int)Math.Sqrt(Math.Pow(leftDiff, 2) + Math.Pow(topDiff, 2));
                distances[1] = (int)Math.Sqrt(Math.Pow(rightDiff, 2) + Math.Pow(topDiff, 2));
                distances[2] = (int)Math.Sqrt(Math.Pow(leftDiff, 2) + Math.Pow(bottomDiff, 2));
                distances[3] = (int)Math.Sqrt(Math.Pow(rightDiff, 2) + Math.Pow(bottomDiff, 2));

                int minDistance = bigDistance;
                foreach (int currentDistance in distances)
                {
                    if (currentDistance < minDistance)
                    {
                        minDistance = currentDistance;
                    }
                }
                if (minDistance < nearestDistance)
                {
                    nearestDistance = minDistance;
                    nearestRectIndex = i;
                }
            }
            return nearestRectIndex;
        }
        private Border GetActiveBorder(Rectangle rectangle, Point cursor, float range)
        {
            RectangleF rectangleWithRange = new RectangleF(rectangle.X - range / 2,
                                                           rectangle.Y - range / 2,
                                                           rectangle.Width + range,
                                                           rectangle.Height + range);
            if (!rectangleWithRange.Contains(cursor))
            {
                return Border.No;
            }
            int[] rectCoordinatesX = { rectangle.Left, rectangle.Right };
            int[] rectCoordinatesY = { rectangle.Top, rectangle.Bottom };
            int variantsCount = 0;
            Border[] diagonalCursors = { Border.LeftTop, Border.LeftBottom, Border.RightTop, Border.RightBottom };

            foreach (int rectCoordX in rectCoordinatesX)
            {
                foreach (int rectCoordY in rectCoordinatesY)
                {
                    if (Math.Abs(cursor.X - rectCoordX) < range & Math.Abs(cursor.Y - rectCoordY) < range)
                    {
                        return diagonalCursors[variantsCount];
                    }
                    else
                    {
                        variantsCount += 1;
                    }
                }
            }
            if (Math.Abs(cursor.X - rectangle.Left) < range)
            {
                return Border.Left;
            }
            else if (Math.Abs(cursor.X - rectangle.Right) < range)
            {
                return Border.Right;
            }
            else if (Math.Abs(cursor.Y - rectangle.Top) < range)
            {
                return Border.Top;
            }
            else if (Math.Abs(cursor.Y - rectangle.Bottom) < range)
            {
                return Border.Bottom;
            }
            return Border.All;
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
                case Border.All:
                    result = Cursors.SizeAll;
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
                    rectangle.Height = cursor.Y - rectangle.Y;
                    rectangle.X = cursor.X;
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
                if (currentCondition == Condition.Move)
                {
                    Point imageCursor = CoordinatesCalculator.GetImageCursor(controlCursor, display.originZoneShift, display.currentScale);
                    relativeCursor.X = imageCursor.X - rectangles[activeRectangleIndex].X;
                    relativeCursor.Y = imageCursor.Y - rectangles[activeRectangleIndex].Y;
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
                PointF imageCursor = CoordinatesCalculator.GetImageCursorF(controlCursor, display.originZoneShift, display.currentScale);
                if (currentCondition == Condition.Create)
                {
                    Rectangle rectangle = CalculateRect(initialCursorPosition, imageCursor);
                    rectangle = CheckImageBoundaries(rectangle, display.origin, true);
                    if (rectangle.Width != 0 & rectangle.Height != 0)
                    {
                        rectangles.Add(rectangle);
                        OverlayRefresh();
                    }
                }
                isLeftButtonHolding = false;

            }
            return;
        }

        private void OverlayRefresh()
        {
            display.isOverlayRedraw = true;
            display.Refresh();
            display.isOverlayRedraw = false;
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

        private Rectangle CheckImageBoundaries(Rectangle rectangle, Image image, bool cutSize=false)
        {
            if (cutSize)
            {
                if (rectangle.Left < 0)
                {
                    rectangle.Width += rectangle.Left;
                    rectangle.X = 0;
                }
                if (rectangle.Top < 0)
                {
                    rectangle.Height += rectangle.Top;
                    rectangle.Y = 0;
                }

                if (rectangle.Right > image.Width)
                {
                    rectangle.Width = image.Width - rectangle.X;
                }
                if (rectangle.Bottom > image.Height)
                {
                    rectangle.Height = image.Height - rectangle.Y;
                }
            }
            else
            {
                if (rectangle.Left < 0)
                {
                    rectangle.X = 0;
                }
                if (rectangle.Top < 0)
                {
                    rectangle.Y = 0;
                }
                if (rectangle.Right > image.Width)
                {
                    rectangle.X = image.Width - rectangle.Width;
                }
                if (rectangle.Bottom > image.Height)
                {
                    rectangle.Y = image.Height - rectangle.Height;
                }
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
                        rectangles[activeRectangleIndex] = CheckImageBoundaries(rectangles[activeRectangleIndex], display.origin, true);
                        OverlayRefresh();
                    }
                }
                if (currentCondition == Condition.Create)
                {
                    // Prewiew drawing rect to user
                    preview = CalculateRect(initialCursorPosition, imageCursor);
                    preview = CheckImageBoundaries(preview, display.origin, true);
                    isTemporaryDraw = true;
                    OverlayRefresh();
                    isTemporaryDraw = false;
                }
                if (currentCondition == Condition.Move)
                {
                    Rectangle newRect = new Rectangle(imageCursor.X - relativeCursor.X,
                                                      imageCursor.Y - relativeCursor.Y,
                                                      rectangles[activeRectangleIndex].Width,
                                                      rectangles[activeRectangleIndex].Height);
                    newRect = CheckImageBoundaries(newRect, display.origin);
                    rectangles[activeRectangleIndex] = newRect;
                    OverlayRefresh();
                }
            }
            else
            {
                bool isNeedRefresh = false;
                Point controlCursor = display.PointToClient(Cursor.Position);
                Point imageCursor = CoordinatesCalculator.GetImageCursor(controlCursor, display.originZoneShift, display.currentScale);
                int nearestRectIndex = FindNearestRectangle(rectangles, imageCursor);
                if (nearestRectIndex != -1)
                {
                    if (activeRectangleIndex != nearestRectIndex)
                    {
                        activeRectangleIndex = nearestRectIndex;
                        isNeedRefresh = true;
                    }
                }
                if (rectangles.Count != 0)
                {
                    currentBorder = GetActiveBorder(rectangles[activeRectangleIndex], imageCursor, 5 / display.currentScale);
                    display.Cursor = GetCursorView(currentBorder);
                    switch (currentBorder)
                    {
                        case Border.No:
                            currentCondition = Condition.Create;
                            break;
                        case Border.All:
                            currentCondition = Condition.Move;
                            break;
                        default:
                            currentCondition = Condition.Edit;
                            break;
                    }
                }
                if (isNeedRefresh)
                {
                    OverlayRefresh();
                }
            }
            return;
        }
    }
}
