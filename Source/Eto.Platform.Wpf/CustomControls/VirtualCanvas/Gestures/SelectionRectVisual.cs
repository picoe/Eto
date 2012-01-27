//-----------------------------------------------------------------------
// <copyright file="SelectionRectVisual.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Microsoft.Sample.Controls
{

    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Diagnostics;

    /// <summary>
    /// Create a host visual derived from the FrameworkElement class for drawing the
    /// selection rubber band.
    /// </summary>
    internal class SelectionRectVisual : FrameworkElement
    {

        private const double _dashRepeatLength = 8;
        private TileBrush _horizontalDashBrush;
        private TileBrush _verticalDashBrush;

        Point _firstPoint;
        Point _secondPoint;
        double _zoom;
        DrawingVisual _visualForRect;

        /// <summary>
        /// Construct new SelectionRectVisual object for the given rectangle
        /// </summary>
        public SelectionRectVisual(Point firstPointP, Point secondPointP, double zoomP)
        {
            DrawingGroup drawing = new DrawingGroup();
            DrawingContext context = drawing.Open();
            context.DrawRectangle(Brushes.White, null, new Rect(-1, -1, 3, 3));
            context.DrawRectangle(Brushes.Black, null, new Rect(0.25, -1, 0.5, 3));
            context.Close();
            drawing.Freeze();

            // Create a drawing brush that tiles the unit square from the drawing created above.
            // The size of the viewport and the rotation angle will be updated as we use the
            // dashed pen.
            DrawingBrush drawingBrush = new DrawingBrush(drawing);
            drawingBrush.ViewportUnits = BrushMappingMode.Absolute;
            drawingBrush.Viewport = new Rect(0, 0, _dashRepeatLength, _dashRepeatLength);
            drawingBrush.ViewboxUnits = BrushMappingMode.Absolute;
            drawingBrush.Viewbox = new Rect(0, 0, 1, 1);
            drawingBrush.Stretch = Stretch.Uniform;
            drawingBrush.TileMode = TileMode.Tile;

            // Store the drawing brush and a copy that's rotated by 90 degrees.
            _horizontalDashBrush = drawingBrush;
            _verticalDashBrush = drawingBrush.Clone();
            _verticalDashBrush.Transform = new RotateTransform(90);

            this._firstPoint = firstPointP;
            this._secondPoint = secondPointP;
            this._zoom = zoomP;
            _visualForRect = new DrawingVisual();
            this.AddVisualChild(_visualForRect);
            this.AddLogicalChild(_visualForRect);      
        }

        /// <summary>
        /// Get/Set the first point in the rectangle (could be before or after second point).
        /// </summary>
        public Point FirstPoint
        {
            get { return _firstPoint; }
            set { _firstPoint = value; }
        }

        /// <summary>
        /// Get/Set the second point in the rectangle (could be before or after first point).
        /// </summary>
        public Point SecondPoint
        {
            get { return _secondPoint; }
            set
            {
                _secondPoint = value;
                DrawOnTheContext();
            }
        }

        /// <summary>
        /// Get/Set the current Zoom level.
        /// </summary>
        public double Zoom
        {
            get { return _zoom; }
            set { _zoom = value; }
        }

        /// <summary>
        /// Actually draw the rubber band
        /// </summary>
        void DrawOnTheContext()
        {
            Rect bounds = SelectedRect;
            Point topLeftCorner = bounds.TopLeft;
            Point bottomRightCorner = bounds.BottomRight;
            Point topRightCorner = bounds.TopRight;
            Point bottomLeftCorner = bounds.BottomLeft;

            if (topLeftCorner.X != bottomRightCorner.X || topLeftCorner.Y != bottomRightCorner.Y)
            {
                using (DrawingContext drawingContext = this._visualForRect.RenderOpen())
                {

                    // Calculate line thickness.
                    double thickness = 1;
                    Vector cornerSize = new Vector(thickness, thickness);
                    Vector lineOffset = cornerSize / 2;

                    // Draw the two horizontal lines.
                    _horizontalDashBrush.Viewport = new Rect(0, 0, _dashRepeatLength, _dashRepeatLength);
                    drawingContext.DrawRectangle(_horizontalDashBrush, null, new Rect(topLeftCorner - lineOffset, topRightCorner + lineOffset));
                    drawingContext.DrawRectangle(_horizontalDashBrush, null, new Rect(bottomLeftCorner - lineOffset, bottomRightCorner + lineOffset));

                    // Draw the two vertical lines.
                    _verticalDashBrush.Viewport = _horizontalDashBrush.Viewport;
                    drawingContext.DrawRectangle(_verticalDashBrush, null, new Rect(topLeftCorner - lineOffset, bottomLeftCorner + lineOffset));
                    drawingContext.DrawRectangle(_verticalDashBrush, null, new Rect(topRightCorner - lineOffset, bottomRightCorner + lineOffset));

                    // Draw black squares at the corners.  This covers up the ugliness that occurs where two lines of
                    // different colors meet at a point.
                    drawingContext.DrawRectangle(Brushes.Black, null, new Rect(topLeftCorner - lineOffset, cornerSize));
                    drawingContext.DrawRectangle(Brushes.Black, null, new Rect(topRightCorner - lineOffset, cornerSize));
                    drawingContext.DrawRectangle(Brushes.Black, null, new Rect(bottomLeftCorner - lineOffset, cornerSize));
                    drawingContext.DrawRectangle(Brushes.Black, null, new Rect(bottomRightCorner - lineOffset, cornerSize));
                }
            }
        }


        /// <summary>
        /// Provide a required override for the VisualChildrenCount property
        /// </summary>
        protected override int VisualChildrenCount
        {
            get { return 1; }
        }

        /// <summary>
        /// Provide a required override for the GetVisualChild method.
        /// </summary>
        /// <param name="index">Index of the child</param>
        /// <returns>The child visual</returns>
        protected override Visual GetVisualChild(int index)
        {
            Debug.Assert(index == 0);
            return this._visualForRect;
        }

        /// <summary>
        /// Get the actual Rectangle of the rubber band.
        /// </summary>
        internal Rect SelectedRect
        {
            get { return new Rect(_firstPoint, SecondPoint); }
        }
    }
}
