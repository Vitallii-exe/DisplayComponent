namespace ImageDisplayComponent
{
    public class DrawUserRectangles : IControlDrawable
    {
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
            display.MouseDown += new MouseEventHandler(DisplayMouseDown);
            display.MouseUp += new MouseEventHandler(DisplayMouseUp);
            display.MouseMove += new MouseEventHandler(DisplayMouseMove);

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

        void PrewiewDraw(Graphics graphics, Rectangle rectangle)
        {
            graphics.DrawRectangle(Pens.Azure, rectangle);
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

        //this.Cursor = System.Windows.Forms.Cursors.No;
        private Cursor GetCursorView(Rectangle rectangle, Point cursor)
        {
            if (Math.Abs(cursor.X - rectangle.Left) < 5 & Math.Abs(cursor.Y - rectangle.Top) < 5)
            {
                return Cursors.SizeNWSE;
            }
            else if (Math.Abs(cursor.X - rectangle.Right) < 5 & Math.Abs(cursor.Y - rectangle.Bottom) < 5)
            {
                return Cursors.SizeNWSE;
            }
            else if (Math.Abs(cursor.X - rectangle.Right) < 5 & Math.Abs(cursor.Y - rectangle.Top) < 5)
            {
                return Cursors.SizeNESW;
            }
            else if (Math.Abs(cursor.X - rectangle.Left) < 5 & Math.Abs(cursor.Y - rectangle.Bottom) < 5)
            {
                return Cursors.SizeNESW;
            }
            else if (Math.Abs(cursor.Y - rectangle.Top) < 5 | Math.Abs(cursor.Y - rectangle.Bottom) < 5)
            {
                return Cursors.SizeNS;
            }
            else if (Math.Abs(cursor.X - rectangle.Left) < 5 | Math.Abs(cursor.X - rectangle.Right) < 5)
            {
                return Cursors.SizeWE;
            }
            return Cursors.Arrow;
        }
        private void DisplayMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point controlCursor = display.PointToClient(Cursor.Position);
                initialCursorPosition = CoordinatesCalculator.GetImageCursorF(controlCursor, display.originZoneShift, display.currentScale);
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
                Rectangle rectangle = CalculateRect(initialCursorPosition, imageCursor);
                rectangles.Add(rectangle);
                isLeftButtonHolding = false;
            }
            return;
        }

        private void DisplayMouseMove(object sender, MouseEventArgs e)
        {
            if (isLeftButtonHolding)
            {
                // Prewiew drawing rect to user
                Point controlCursor = display.PointToClient(Cursor.Position);
                (float X, float Y) nowCursor = (controlCursor.X, controlCursor.Y);
                (float X, float Y) initialControlPos = CoordinatesCalculator.GetControlCursor(initialCursorPosition, display.originZoneShift, display.currentScale);
                prewiew = CalculateRect(initialControlPos, nowCursor);
                isTemporaryDraw = true;
                display.Refresh();
                isTemporaryDraw = false;
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
                    display.Cursor = GetCursorView(rectangles[activeRectangleIndex], imageCursor);
                }
                display.Refresh();
            }
            return;
        }
    }
}
