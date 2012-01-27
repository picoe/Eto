//-----------------------------------------------------------------------
// <copyright file="AutoScroll.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Input;

namespace Microsoft.Sample.Controls
{
    /// <summary>
    /// This class implements a mouse middle-button auto-scrolling feature over any target view.
    /// </summary>
    internal class AutoScroll
    {
        Panel _container;
        FrameworkElement _target;
        bool _autoScrolling ;
        Point _startPos;
        MapZoom _zoom;
        Canvas _marker;

        /// <summary>
        /// Construct new AutoScroll object that will scroll the given target object within it's container
        /// by attaching to the mouse events of the container.
        /// </summary>
        /// <param name="target">The target object to scroll</param>
        /// <param name="zoom">The master MapZoom object that manages the actual render transform</param>
        public AutoScroll(FrameworkElement target, MapZoom zoom)
        {
            _container = target.Parent as Panel;
            _target = target;
            _container.MouseDown += new MouseButtonEventHandler(OnMouseDown);
            _container.MouseMove += new MouseEventHandler(OnMouseMove);
            _container.MouseWheel += new MouseWheelEventHandler(OnMouseWheel);
            Keyboard.AddKeyDownHandler(_container, new KeyEventHandler(OnKeyDown));
            _zoom = zoom;
        }

        /// <summary>
        /// Receive mouse wheel event and stop any active autoscroll behavior.
        /// </summary>
        /// <param name="sender">The container</param>
        /// <param name="e">Mouse wheel info</param>
        void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            StopAutoScrolling();
        }

        /// <summary>
        /// Receive mouse move event and do the actual autoscroll if it is active.
        /// </summary>
        /// <param name="sender">The container</param>
        /// <param name="e">Mouse move info</param>
        void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_autoScrolling)
            {
                Point pt = e.GetPosition(_container);
                Vector v = new Vector(pt.X - _startPos.X, pt.Y - _startPos.Y);
                Vector v2 = new Vector(pt.X - _startPos.X, _startPos.Y);
                double angle = Vector.AngleBetween(v, v2);

                // Calculate which quadrant the mouse is in relative to start position.
                Cursor c = null;
                if (angle > -22.5 && angle < 22.5)
                {
                    c = Cursors.ScrollS;
                }
                else if (angle <= -22.5 && angle > -67.5)
                {
                    c = Cursors.ScrollSW;
                }
                else if (angle <= -67.5 && angle > -112.5)
                {
                    c = Cursors.ScrollW;
                }
                else if (angle <= -112.5 && angle > -157.5)
                {
                    c = Cursors.ScrollNW;
                }
                else if (angle <= -157.5 || angle > 157.5)
                {
                    c = Cursors.ScrollN;
                }
                else if (angle <= 157.5 && angle > 112.5)
                {
                    c = Cursors.ScrollNE;
                }
                else if (angle <= 112.5 && angle > 67.5)
                {
                    c = Cursors.ScrollE;
                }
                else if (angle <= 67.5 && angle > 22.5)
                {
                    c = Cursors.ScrollSE;
                }
                _container.Cursor = c;

                double length = v.Length;
                if (length > 0)
                {
                    v.Normalize();
                    v = Vector.Multiply(length / 50, v);

                    Point translate = _zoom.Offset;
                    translate.X -= v.X;
                    translate.Y -= v.Y;
                    _zoom.Offset = translate;
                }
            }
        }

        /// <summary>
        /// Handle the mouse down event which toggles autoscrolling behavior.
        /// </summary>
        /// <param name="sender">Mouse</param>
        /// <param name="e">Mouse button information</param>
        void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                if (!_autoScrolling)
                {
                    _startPos = e.GetPosition(_container);
                    Mouse.Capture(_container, CaptureMode.SubTree);
                    _autoScrolling = true;
                    _container.Cursor = Cursors.ScrollAll;
                    if (_marker == null)
                    {
                        _marker = new Canvas();
                        Ellipse sign = new Ellipse();
                        Brush brush = new SolidColorBrush(Color.FromArgb(0x90, 0x90, 0x90, 0x90));
                        sign.Stroke = brush;
                        sign.StrokeThickness = 2;
                        sign.Width = 40;
                        sign.Height = 40;
                        _marker.Children.Add(sign);

                        Polygon down = new Polygon();
                        down.Points = new PointCollection(new Point[] { new Point(20 - 6, 28), new Point(20 + 6, 28), new Point(20, 34) });
                        down.Fill = brush;
                        _marker.Children.Add(down);

                        Polygon up = new Polygon();
                        up.Points = new PointCollection(new Point[] { new Point(20 - 6, 12), new Point(20 + 6, 12), new Point(20, 6) });
                        up.Fill = brush;
                        _marker.Children.Add(up);

                        Polygon left = new Polygon();
                        left.Points = new PointCollection(new Point[] { new Point(28, 20-6), new Point(28, 20+6), new Point(34, 20) });
                        left.Fill = brush;
                        _marker.Children.Add(left);

                        Polygon right = new Polygon();
                        right.Points = new PointCollection(new Point[] { new Point(12, 20 - 6), new Point(12, 20 + 6), new Point(6, 20) });
                        right.Fill = brush;
                        _marker.Children.Add(right);

                        Ellipse dot = new Ellipse();
                        dot.Fill = brush;
                        dot.Width = 3;
                        dot.Height = 3;
                        dot.RenderTransform = new TranslateTransform(18,18);
                        _marker.Children.Add(dot);
                    }
                    _container.Children.Add(_marker);
                    _marker.Arrange(new Rect(_startPos.X - 20, _startPos.Y - 20, 40, 40));
                    _container.InvalidateVisual();
                }
                else
                {
                    StopAutoScrolling();
                }
                e.Handled = true;
            }
            else
            {
                StopAutoScrolling();
            }
        }

        /// <summary>
        /// Handle key down event and stop any autoscrolling behavior
        /// </summary>
        /// <param name="sender">Keyboard</param>
        /// <param name="e">Event information</param>
        void OnKeyDown(object sender, RoutedEventArgs e)
        {
            StopAutoScrolling();
        }

        /// <summary>
        /// Stop any active auto-scrolling behavior.
        /// </summary>
        void StopAutoScrolling()
        {
            if (_autoScrolling)
            {
                Mouse.Capture(_container, CaptureMode.None);
                _autoScrolling = false;
                _container.Cursor = Cursors.Arrow;
                _container.Children.Remove(_marker);
                _container.InvalidateVisual();
            }
        }
    }
}
