using System.Drawing.Drawing2D;

namespace ImageDisplayComponent
{
    public class DrawUserRectangles : IControlDrawable
    {
        enum Border
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
        enum DrawCondition
        {
            Temporary,
            Permanent
        }
        Condition currentCondition = Condition.Create;
        Border currentBorder = Border.No;
        DrawCondition currentDrawCondition = DrawCondition.Temporary;

        DisplayUserControl display;
        List<Rectangle> rectangles = new List<Rectangle>();
        Rectangle preview = new Rectangle(0, 0, 0, 0);
        int activeRectangleIndex = 0;
        (Pen active, Pen passive) pens = (new Pen(Color.Red, 2), new Pen(Color.DarkRed, 2));
        Pen previewPen = new Pen(Color.LawnGreen, 2);
        PointF initialCursorPosition;
        Point relativeCursor = new Point(0, 0);
        bool isLeftButtonHolding = false;

        public DrawUserRectangles(DisplayUserControl display)
        {
            this.display = display;
            display.MouseDown += new MouseEventHandler(DisplayMouseDown!);
            display.MouseUp += new MouseEventHandler(DisplayMouseUp!);
            display.MouseMove += new MouseEventHandler(DisplayMouseMove!);

        }
        void IControlDrawable.DrawOver(Graphics graphics)
        {
            if (currentDrawCondition == DrawCondition.Temporary)
            {
                Rectangle scaledElement = ResizeRectangle(preview);
                graphics.DrawRectangle(previewPen, scaledElement);
            }
            DrawRectangleArray(graphics, rectangles, activeRectangleIndex, pens);
            return;
        }
        private void DrawRectangleArray(Graphics graphics, List<Rectangle> rectangles, int activeIndex, (Pen active, Pen passive) pens)
        {
            for (int i = 0; i < rectangles.Count; i++)
            {
                Pen pen;
                pen = (i == activeIndex) ? pens.active : pens.passive;
                pen.DashStyle = DashStyle.Dash;

                Rectangle scaledElement = ResizeRectangle(rectangles[i]);

                /// When Rectangle Size (Width or Height) == 0, DrawRectangle 
                /// can't draw it correctly. So this region of the code solve
                /// this problem.
                if (scaledElement.Width == 0)
                {
                    //Draw Vertical Line instead of Rectangle
                    Point secondPoint = new Point(scaledElement.Left, scaledElement.Bottom);
                    graphics.DrawLine(pen, scaledElement.Location, secondPoint);
                }
                else if (scaledElement.Height == 0)
                {
                    //Draw Vertical Line instead of Rectangle
                    Point secondPoint = new Point(scaledElement.Right, scaledElement.Top);
                    graphics.DrawLine(pen, scaledElement.Location, secondPoint);
                }
                else
                {
                    graphics.DrawRectangle(pen, scaledElement);
                }
            }
            return;
        }
        private Rectangle CalculateRectangle(PointF firstPoint, PointF secondPoint)
        {
            int xDistance = (int)Math.Abs(firstPoint.X - secondPoint.X);
            int yDistance = (int)Math.Abs(firstPoint.Y - secondPoint.Y);
            Size size = new Size(xDistance, yDistance);
            Point location = new Point(0, 0);
            location.X = (firstPoint.X < secondPoint.X) ? (int)firstPoint.X : (int)secondPoint.X;
            location.Y = (firstPoint.Y < secondPoint.Y) ? (int)firstPoint.Y : (int)secondPoint.Y;
            return new Rectangle(location, size);
        }
        private Rectangle ResizeRectangle(Rectangle rectangle)
        {
            Rectangle scaledRectangle;
            PointF location = new PointF(rectangle.X, rectangle.Y);
            Point elementLocation = CoordinatesCalculator.GetControlCursor(location, 
                                                                            display.originZoneShift, 
                                                                            display.currentScale);
            Size scaledSize = new Size();
            scaledSize.Width = (int)(rectangle.Width * display.currentScale);
            scaledSize.Height = (int)(rectangle.Height * display.currentScale);
            scaledRectangle = new Rectangle(elementLocation, scaledSize);
            return scaledRectangle;
        }

        private int FindNearestRectangle(List<Rectangle> rectangles, Point cursor)
        {
            int nearestRectIndex = 0;
            int nearestDistance = int.MaxValue;
            for (int i = 0; i < rectangles.Count; i++)
            {
                int currentDistance = GetDistanceFromCursorToRectangle(cursor, rectangles[i]);
                if (currentDistance < nearestDistance)
                {
                    nearestDistance = currentDistance;
                    nearestRectIndex = i;
                }
            }
            return nearestRectIndex;
        }

        private int GetDistanceFromCursorToRectangle(Point cursor, Rectangle rectangle)
        {
            int resultMinDistance;
            ///When cursor inside the rectangle
            if (rectangle.Contains(cursor))
            {
                // Compare distance to all (left / right / top/ bottom) borders
                int[] distances = { int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue };
                distances[0] = Math.Abs(cursor.X - rectangle.Left);
                distances[1] = Math.Abs(cursor.X - rectangle.Right);
                distances[2] = Math.Abs(cursor.Y - rectangle.Top);
                distances[3] = Math.Abs(cursor.Y - rectangle.Bottom);
                
                resultMinDistance = distances.Min();
            }
            ///When cursor in range Y-coordinates of rectangle
            else if (cursor.Y < rectangle.Bottom & cursor.Y > rectangle.Top)
            {
                //Calc distance to left / right borders
                int distanceToLeft = Math.Abs(cursor.X - rectangle.Left);
                int distanceToRight = Math.Abs(cursor.X - rectangle.Right);
                resultMinDistance = distanceToLeft < distanceToRight ? distanceToLeft : distanceToRight;
            }
            ///When cursor in range X-coordinates of rectangle
            else if (cursor.X < rectangle.Right & cursor.X > rectangle.Left)
            {
                //Calc distance to top / bottom borders
                int distanceToTop = Math.Abs(cursor.Y - rectangle.Top);
                int distanceToBottom = Math.Abs(cursor.Y - rectangle.Bottom);
                resultMinDistance = distanceToTop < distanceToBottom ? distanceToTop : distanceToBottom;
            }
            ///When cursor outside all coordinates of rectangle
            else
            {
                //Calc distance to rect angles
                int[] distances = { int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue };
                int leftDiff = Math.Abs(cursor.X - rectangle.Left);
                int rightDiff = Math.Abs(cursor.X - rectangle.Right);
                int topDiff = Math.Abs(cursor.Y - rectangle.Top);
                int bottomDiff = Math.Abs(cursor.Y - rectangle.Bottom);

                distances[0] = (int)Math.Sqrt(Math.Pow(leftDiff, 2) + Math.Pow(topDiff, 2));
                distances[1] = (int)Math.Sqrt(Math.Pow(rightDiff, 2) + Math.Pow(topDiff, 2));
                distances[2] = (int)Math.Sqrt(Math.Pow(leftDiff, 2) + Math.Pow(bottomDiff, 2));
                distances[3] = (int)Math.Sqrt(Math.Pow(rightDiff, 2) + Math.Pow(bottomDiff, 2));
                resultMinDistance = distances.Min();
            }
            return resultMinDistance;
        }
        private Border GetActiveBorder(Rectangle rectangle, Point cursor, float dispersion)
        {
            RectangleF rectangleWithRange = new RectangleF(rectangle.X - dispersion / 2,
                                                           rectangle.Y - dispersion / 2,
                                                           rectangle.Width + dispersion,
                                                           rectangle.Height + dispersion);
            if (!rectangleWithRange.Contains(cursor))
            {
                return Border.No;
            }
            int[] rectCoordinatesX = { rectangle.Left, rectangle.Right };
            int[] rectCoordinatesY = { rectangle.Top, rectangle.Bottom };
            int variantsCount = 0;
            Border[] diagonalCursors =
            {
                Border.LeftTop,
                Border.LeftBottom,
                Border.RightTop,
                Border.RightBottom
            };

            foreach (int rectCoordX in rectCoordinatesX)
            {
                foreach (int rectCoordY in rectCoordinatesY)
                {
                    int distanceX = Math.Abs(cursor.X - rectCoordX);
                    int distanceY = Math.Abs(cursor.Y - rectCoordY);
                    if (distanceX < dispersion & distanceY < dispersion)
                    {
                        return diagonalCursors[variantsCount];
                    }
                    else
                    {
                        variantsCount += 1;
                    }
                }
            }
            if (Math.Abs(cursor.X - rectangle.Left) < dispersion)
            {
                return Border.Left;
            }
            else if (Math.Abs(cursor.X - rectangle.Right) < dispersion)
            {
                return Border.Right;
            }
            else if (Math.Abs(cursor.Y - rectangle.Top) < dispersion)
            {
                return Border.Top;
            }
            else if (Math.Abs(cursor.Y - rectangle.Bottom) < dispersion)
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
        
        private Rectangle CheckImageBoundaries(Rectangle rectangle, Image image, bool cutSize = false)
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

        private Rectangle ValidateBorderEditing(Rectangle rectangle, ref Border border)
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
            rectangle = CheckImageBoundaries(rectangle, display.origin, true);
            return rectangle;
        }

        private void RefreshOverlay(DrawCondition condition)
        {
            currentDrawCondition = condition;
            display.isOverlayRedraw = true;
            display.Refresh();
            display.isOverlayRedraw = false;
            currentDrawCondition = DrawCondition.Permanent;
            return;
        }

        private void EditRectangle(Point imageCursor)
        {
            if (currentBorder != Border.No)
            {
                Rectangle newRect = ResizeRectangle(rectangles[activeRectangleIndex],
                                                    imageCursor,
                                                    currentBorder);
                rectangles[activeRectangleIndex] = ValidateBorderEditing(newRect, ref currentBorder);
                RefreshOverlay(DrawCondition.Permanent);
            }
            return;
        }

        private void DrawPreviewRectangle(Point imageCursor)
        {
            preview = CalculateRectangle(initialCursorPosition, imageCursor);
            preview = CheckImageBoundaries(preview, display.origin, true);
            RefreshOverlay(DrawCondition.Temporary);
            return;
        }

        private void MoveRectangle(Point imageCursor)
        {
            Rectangle newRect = new Rectangle(imageCursor.X - relativeCursor.X,
                                                      imageCursor.Y - relativeCursor.Y,
                                                      rectangles[activeRectangleIndex].Width,
                                                      rectangles[activeRectangleIndex].Height);
            newRect = CheckImageBoundaries(newRect, display.origin);
            rectangles[activeRectangleIndex] = newRect;
            RefreshOverlay(DrawCondition.Permanent);
            return;
        }

        private bool UpdateActiveRectangle(Point imageCursor)
        {
            int nearestRectIndex = FindNearestRectangle(rectangles, imageCursor);
            if (nearestRectIndex != -1)
            {
                if (activeRectangleIndex != nearestRectIndex)
                {
                    activeRectangleIndex = nearestRectIndex;
                    return true;
                }
            }
            return false;
        }

        private void UpdateCursorView(Point imageCursor)
        {
            if (rectangles.Count != 0)
            {
                currentBorder = GetActiveBorder(rectangles[activeRectangleIndex],
                                                imageCursor, 5 / display.currentScale);
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
            return;
        }
        private void DisplayMouseMove(object sender, MouseEventArgs e)
        {
            Point controlCursor = display.PointToClient(Cursor.Position);
            Point imageCursor = CoordinatesCalculator.GetImageCursor(controlCursor,
                                                                     display.originZoneShift,
                                                                     display.currentScale);
            if (isLeftButtonHolding)
            {
                if (currentCondition == Condition.Edit)
                {
                    EditRectangle(imageCursor);
                }
                if (currentCondition == Condition.Create)
                {
                    DrawPreviewRectangle(imageCursor);
                }
                if (currentCondition == Condition.Move)
                {
                    MoveRectangle(imageCursor);
                }
            }
            else
            {
                bool isNeedRefresh = UpdateActiveRectangle(imageCursor);
                UpdateCursorView(imageCursor);
                if (isNeedRefresh)
                {
                    RefreshOverlay(DrawCondition.Permanent);
                }
            }
            return;
        }

        private void DisplayMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point controlCursor = display.PointToClient(Cursor.Position);
                if (currentCondition == Condition.Create)
                {
                    initialCursorPosition = CoordinatesCalculator.GetImageCursorF(controlCursor,
                                                                                  display.originZoneShift,
                                                                                  display.currentScale);
                }
                if (currentCondition == Condition.Move)
                {
                    Point imageCursor = CoordinatesCalculator.GetImageCursor(controlCursor,
                                                                             display.originZoneShift,
                                                                             display.currentScale);
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
                PointF imageCursor = CoordinatesCalculator.GetImageCursorF(controlCursor,
                                                                           display.originZoneShift,
                                                                           display.currentScale);
                if (currentCondition == Condition.Create)
                {
                    Rectangle rectangle = CalculateRectangle(initialCursorPosition, imageCursor);
                    rectangle = CheckImageBoundaries(rectangle, display.origin, true);
                    if (rectangle.Width != 0 & rectangle.Height != 0)
                    {
                        rectangles.Add(rectangle);
                        RefreshOverlay(DrawCondition.Permanent);
                    }
                }
                isLeftButtonHolding = false;

            }
            return;
        }
    }
}
