//-----------------------------------------------------------------------
// <copyright file="MapZoom.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Text;
using System.Windows.Input;
using System.Windows.Controls.Primitives;

namespace Microsoft.Sample.Controls
{
    /// <summary>
    /// The MapZoom gesture provides animated mouse wheel zooming behavior as well as a
    /// general management of the scale and translate transformations and a "ScrollIntoView" 
    /// method than can be used by other gestures.
    /// </summary>
    class MapZoom : Animatable
    {
        // sensitivity is a number between 0 and 1 that controls how much each mouse wheel click
        // zooms in.  This value means one mouse click will zoom to 0.90 of the original size, but
        // multiple mouse clicks when you flick the mouse wheel accumulate so it can be more than this.
        // 0.10 feels about right, but you can increase the number if you feel you have to work the
        // mouse wheel to hard to zoom in as fast as you want to.
        const double _sensitivity = .10;
        // defaultZoomTime is the amount of time in milliseconds the zoom animation takes to give 
        // a smooth zoom in behavior.  This time increases when you flick the mouse wheel up to maxZoomTime.
        const double _defaultZoomTime = 100;
        const double _maxZoomTime = 300;

        FrameworkElement _container;
        FrameworkElement _target;
                
        // Note that because we want to animate the zoom and translate while spinning the mouse
        // we keep two sets of values.  Note that when an animation is active on a given property it
        // will blow away any value you might try to set programatically on that same property.        
        // So the master non-animated values live in the actual Transform objects and anyone can
        // set these values and they will "stick".  The animated values are stored in _zoom and
        // _offset and these values are copied to the master _scale and _translate on each animation
        // step.  If some external change is made to the _scale and _translate transforms then
        // we "StopAnimation" and sync up the _offset and _zoom variables.
        ScaleTransform _scale;
        TranslateTransform _translate;
        double _zoom = 1;
        double _newZoom = 1;
        Point _offset;
        Point _mouse;
        Point _onTarget;
        double _zoomTime = _defaultZoomTime;
        long _startTime;
        double _lastAmount;
        Rect _targetRect;

        /// <summary>
        /// The offset property that can be animated.
        /// </summary>
        public static readonly DependencyProperty OffsetProperty = DependencyProperty.Register("Offset", typeof(Point), typeof(MapZoom));
        /// <summary>
        /// When we are zooming to a point this property can be animated
        /// </summary>
        public static readonly DependencyProperty ZoomToPointProperty = DependencyProperty.Register("ZoomToPoint", typeof(double), typeof(MapZoom));
        /// <summary>
        /// When we are zooming to a rectangle this property can be animated.
        /// </summary>
        public static readonly DependencyProperty ZoomToRectProperty = DependencyProperty.Register("ZoomToRect", typeof(double), typeof(MapZoom));

        /// <summary>
        /// This event is raised when the scale or translation is changed.
        /// </summary>
        public event EventHandler ZoomChanged;

        /// <summary>
        /// Construct new MapZoom object that manages the RenderTransform of the given target object.
        /// The target object must have a parent container.
        /// </summary>
        /// <param name="target">The target object we will be zooming.</param>
        public MapZoom(FrameworkElement target)
        {
            this._container = target.Parent as FrameworkElement;
            this._target = target;

            _container.MouseMove += new MouseEventHandler(OnMouseMove);
            _container.MouseWheel += new MouseWheelEventHandler(OnMouseWheel);
            
            Keyboard.AddKeyDownHandler(_container, new KeyEventHandler(OnKeyDown));
            Keyboard.AddKeyUpHandler(_container, new KeyEventHandler(OnKeyUp));

            _container.Focusable = true;
            _container.Focus();

            // Try and reuse the existing TransformGroup if we can.
            TransformGroup g = target.RenderTransform as TransformGroup;
            if (g != null)
            {
                this._scale = g.Children.Count > 1 ? g.Children[0] as ScaleTransform : null;
                this._translate = g.Children.Count > 0 ? g.Children[1] as TranslateTransform : null;
                if (this._scale == null || this._translate == null)
                {
                    g = null; // then the TransformGroup cannot be re-used
                }
            }
            if (g == null)
            {
                g = new TransformGroup();
                this._scale = new ScaleTransform(1, 1);
                g.Children.Add(this._scale);
                this._translate = new TranslateTransform();
                g.Children.Add(this._translate);
                target.RenderTransform = g;
            }

            this._zoom = this._newZoom = _scale.ScaleX;

            // track changes made by the ScrolLViewer.
            this._translate.Changed += new EventHandler(OnTranslateChanged);
            this._scale.Changed += new EventHandler(OnScaleChanged);
        }

        /// <summary>
        /// Handle event when the TranslateTransform is changed by keeping our offset member in sync
        /// the new TranslateTransform values.
        /// </summary>
        /// <param name="sender">The TranslateTransform object</param>
        /// <param name="e">noop</param>
        void OnTranslateChanged(object sender, EventArgs e)
        {
            if (_offset.X != _translate.X || _offset.Y != _translate.Y)
            {
                _offset.X = _translate.X;
                _offset.Y = _translate.Y;
            }
        }

        /// <summary>
        /// Handle event when the ScaleTransform is changed by keeping our zoom member in sync with the
        /// new ScaleTransform values.
        /// </summary>
        /// <param name="sender">The ScaleTransform object</param>
        /// <param name="e">noop</param>
        void OnScaleChanged(object sender, EventArgs e)
        {
            if (_zoom != _scale.ScaleX)
            {
                _zoom = _scale.ScaleX;
            }
        }

        /// <summary>
        /// Handle the key down event and show special cursor when control key is pressed to
        /// indicate we can do some special behavior
        /// </summary>
        /// <param name="sender">Keyboard</param>
        /// <param name="e">Key information</param>
        void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.RightCtrl || e.Key == Key.LeftCtrl)
            {
                _container.Cursor = Cursors.SizeAll;
            }
        }

        /// <summary>
        /// Handle the key up event and remove any special cursor we set in Key down.
        /// </summary>
        /// <param name="sender">Keyboard</param>
        /// <param name="e">Key information</param>
        void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.RightCtrl || e.Key == Key.LeftCtrl)
            {
                _container.Cursor = Cursors.Arrow;
            }
        }

        /// <summary>
        /// Handle mouse move event and record the position since we want the zoom to be centered around
        /// this position.
        /// </summary>
        /// <param name="sender">Mouse</param>
        /// <param name="e">Mouse move information</param>
        void OnMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            this._mouse = _container.PointToScreen(e.GetPosition(_container));
            this._onTarget = e.GetPosition(_target);

            // Create a 50 pixel margin on the edges of the target object so that if the mouse is inside
            // that band, then that edge stays pinned on screen and doesn't slide off the edge as we zoom in.
            if (this._onTarget.X < 50 / _zoom) this._onTarget.X = 0;
            if (this._onTarget.Y < 50 / _zoom) this._onTarget.Y = 0;
            if (this._onTarget.X + 50 / _zoom > _target.ActualWidth) this._onTarget.X = _target.ActualWidth;
            if (this._onTarget.Y + 50 / _zoom > _target.ActualHeight) this._onTarget.Y = _target.ActualHeight;
        }

        /// <summary>
        /// Handle mouse wheel event and do the actuall zooming if the control key is down.
        /// </summary>
        /// <param name="sender">Mouse</param>
        /// <param name="e">Mouse wheel information</param>
        public void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta != 0 && (Keyboard.Modifiers & ModifierKeys.Control) != 0)
            {
                HandleZoom((double)e.Delta / (double)Mouse.MouseWheelDeltaForOneLine);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Apply the new animated _zoom value to the actual ScaleTransform and fire the ZoomChanged event.
        /// </summary>
        void OnZoomChanged()
        {
            _scale.ScaleX = _scale.ScaleY = this._zoom;

            if (ZoomChanged != null) ZoomChanged(this, EventArgs.Empty);

            // focus rectangles may need to be repainted.
            _target.InvalidateVisual();
        }

        /// <summary>
        /// Reset current translate offset back to zero and stop any existing zoom/translate animations.
        /// </summary>
        public void ResetTranslate()
        {
            StopAnimations();
            Translate(0, 0);
        }

        /// <summary>
        /// Return the visible size of the target object (the IScrollInfo Viewport dimensions).
        /// </summary>
        public Size ContainerSize
        {
            get
            {
                IScrollInfo si = _target as IScrollInfo;
                if (si != null)
                {
                    return new Size(si.ViewportWidth, si.ViewportHeight);
                }
                // Basically the visible rect...
                return new Size(this._container.ActualWidth, this._container.ActualHeight);
            }
        }

        /// <summary>
        /// The ScaleTransform scales around 0,0, but want it to scale around _onTarget position.
        /// So initially when we changed the ScaleTransform the mouse will have appeared to have
        /// slipped a bit on the target object which we don't want, so here we calculate the delta 
        /// between the current mouse position and the _onTarget position we want and apply a
        /// Translate correction so the user doesn't see any slippage.
        /// </summary>
        void KeepPositionStable()
        {

            // Make sure this point remains fixed since it is where the cursor is...            
            Point moved;
            try
            {
                moved = _target.PointFromScreen(this._mouse);
            }
            catch (Exception)
            {
                return;
            }

            Point delta = new Point(moved.X - this._onTarget.X, moved.Y - this._onTarget.Y);

            double x = this._translate.X + (delta.X * _zoom);
            double y = this._translate.Y + (delta.Y * _zoom);

            Size containerSize = ContainerSize;
            double width = containerSize.Width;
            double height = containerSize.Height;
            
            double right = (this._target.ActualWidth * _zoom) + x;
            if (right < width && x < 0)
            {
                x += (width - right);
            }

            double bottom = (this._target.ActualHeight * _zoom) + y;
            if (bottom < height && y < 0)
            {
                y += (height - bottom);
            }
            Translate(x, y);

        }

        /// <summary>
        /// This version keeps a given rectangle stable by centering it within the visible region 
        /// of the target object.
        /// </summary>
        void KeepRectStable()
        {
            // Find out where we are now that zoom has changed.
            Rect cr = _target.TransformToAncestor(this._container).TransformBounds(_targetRect);
            // Keep it centered.
            Size containerSize = ContainerSize;
            double width = containerSize.Width;
            double height = containerSize.Height;
            
            double cx = (width - cr.Width) / 2;
            double cy = (height - cr.Height) / 2;
            double dx = _translate.X + cx - cr.X;
            double dy = _translate.Y + cy - cr.Y;
            Translate(dx, dy);
        }

        /// <summary>
        /// Translate the target object to the given x-y positions.
        /// </summary>
        /// <param name="x">The x-cordinate</param>
        /// <param name="y">The y-coordinate</param>
        void Translate(double x, double y)
        {
            IScrollInfo si = _target as IScrollInfo;
            if (si != null)
            {
                // If the target object is smaller than the current viewport size then ignore this request.
                Size s = this.ContainerSize;
                if (_target.ActualWidth <= s.Width && _target.ActualHeight <= s.Height)
                {
                    x = y = 0;
                }
            }
            if (x > 0) x = 0;
            if (y > 0) y = 0;

            _translate.X = _offset.X = x;
            _translate.Y = _offset.Y = y;

            // focus rectangles may need to be repainted.
            _target.InvalidateVisual();
        }

        /// <summary>
        /// Get/set the current zoom level - this is a scale factor, 0.5 means zoom out so everything is half
        /// the normal size.  A value of 2 means zoom in so everything on the target object is twice the normal size.
        /// </summary>
        public double Zoom
        {
            get { return this._zoom; }
            set
            {
                StopAnimations();
                this._scale.ScaleX = this._scale.ScaleY = this._zoom = value;
                OnZoomChanged();
            }
        }

        /// <summary>
        /// Get/set the current translation offset.
        /// </summary>
        public Point Offset
        {
            get { return this._offset; }
            set
            {
                StopAnimations();
                Translate(value.X, value.Y);
            }
        }

        /// <summary>
        /// For operations that need to tweak the zoom or translate coordinates, we must first
        /// stop any animations that are currently changing those values otherwise things get
        /// very confused.
        /// </summary>
        void StopAnimations()
        {
            // Stop the animation at the current point.
            this._newZoom = this._zoom;
            this.BeginAnimation(ZoomToPointProperty, null);
            this.BeginAnimation(ZoomToRectProperty, null);

            // make sure offset and translate are in sync.
            Point t = new Point(_translate.X, _translate.Y);
            this.BeginAnimation(OffsetProperty, null);
            Translate(t.X, t.Y);
        }

        /// <summary>
        /// Helper method to return the center of the given rectangle.
        /// </summary>
        /// <param name="r">The rectangle</param>
        /// <returns>The center of that rectangle</returns>
        static Point Center(Rect r)
        {
            return new Point(r.X + (r.Width / 2), r.Y + (r.Height / 2));
        }

        /// <summary>
        /// Animate a zoom out and translate scroll so that the given rectangle is entirely visible.
        /// </summary>
        /// <param name="r">Given rectangle is in "target" coordinates.</param>
        public void ZoomToRect(Rect r)
        {
            StopAnimations();

            // Convert it to container coordinates 
            Rect cr = _target.TransformToAncestor(this._container).TransformBounds(r);

            Size containerSize = ContainerSize;
            double width = containerSize.Width;
            double height = containerSize.Height;
            
            double xzoom = width / cr.Width;
            double yzoom = height / cr.Height;
            double zoom = this._zoom * Math.Min(xzoom, yzoom);

            double oldZoom = this._zoom;

            this._targetRect = r;
            this._startTime = 0;
            AnimateZoom(ZoomToRectProperty, oldZoom, zoom, new Duration(TimeSpan.FromMilliseconds(_defaultZoomTime)));

            // focus rectangles may need to be repainted.
            _target.InvalidateVisual();
        }

        /// <summary>
        /// Reset any zoom/translate on the target object.
        /// </summary>
        public void Reset()
        {
            StopAnimations();
            _scale.ScaleX = _scale.ScaleY = this._zoom = this._newZoom = 1;
            this._translate.X = _offset.X = 0;
            this._translate.Y = _offset.Y = 0;
            this._startTime = 0;
            OnZoomChanged();
            // focus rectangles may need to be repainted.
            _target.InvalidateVisual();
        }

        /// <summary>
        /// Animate the actual zoom by the given amount.  The amount is a measure of how much zoom the
        /// user wants - for example, how hard did they spin the mouse.
        /// </summary>
        /// <param name="amount">Value between -1 and 1</param>
        void HandleZoom(double clicks)
        {
            double amount = clicks;
            double oldZoom = this._zoom;
            if (amount > 1) amount = 1;
            if (amount < -1) amount = -1;

            // If we've changed direction since the last animation then stop animations.
            bool sameSign = (Math.Sign(amount) == Math.Sign(this._lastAmount));
            if (!sameSign || _startTime == 0)
            {
                StopAnimations();
            }

            // Accumulate the desired _newZoom amount as this method is called while the
            // user is repeatedly spinning the mouse.
            double sensitivity = _sensitivity;
            double extra = Math.Abs(clicks);
            if (extra > 1)
            {
                // mouse wheel is spinning fast.
                sensitivity = Math.Min(0.5, sensitivity * extra);
            }
            double delta = 1 - (Math.Abs(amount) * sensitivity);
            if (amount < 0)
            {
                // zoom in
                this._newZoom *= delta;
            }
            else
            {
                // zoom out
                this._newZoom /= delta;
            }

            // Calculate how long we want to keep zooming (_zoomTime) and increase this time
            // if the user keeps spinning the mouse so we get a nice momentum effect.
            long tick = Environment.TickCount;

            if (sameSign && _startTime != 0 && _startTime + _zoomTime > tick)
            {
                // then make the time cumulative so you get nice smooth animation when you flick the wheel.
                this._zoomTime += (_startTime + _zoomTime - tick);
                if (this._zoomTime > _maxZoomTime) this._zoomTime = _maxZoomTime;
            }
            else
            {
                _startTime = tick;
                this._zoomTime = _defaultZoomTime;
            }
            this._lastAmount = amount;

            AnimateZoom(ZoomToPointProperty, oldZoom, _newZoom, new Duration(TimeSpan.FromMilliseconds(this._zoomTime)));
        }

        /// <summary>
        /// Start the zoom animation
        /// </summary>
        /// <param name="property">ZoomToPointProperty or ZoomToRectProperty.
        /// This tells us which type of zoom are we doing (around a point or a rectangle)</param>
        /// <param name="oldZoom">The old zoom value</param>
        /// <param name="newZoom">The new value we want to be at</param>
        /// <param name="d">The amound of time we can take to do the animation</param>
        void AnimateZoom(DependencyProperty property, double oldZoom, double newZoom, Duration d)
        {
            ExponentialDoubleAnimation a = new ExponentialDoubleAnimation(oldZoom, newZoom, 2, EdgeBehavior.EaseOut, d);
            this.BeginAnimation(property, a);
        }

        /// <summary>
        /// Scroll the given framework element into view using a smooth animation.
        /// </summary>
        /// <param name="e"></param>
        public Rect ScrollIntoView(FrameworkElement e)
        {
            StopAnimations();
            Rect rect = new Rect(0, 0, e.ActualWidth, e.ActualHeight);
            // Get zoomed & translated coordinates of this object by transforming to the container coordinates.
            rect = e.TransformToAncestor(_container).TransformBounds(rect);
            rect.Inflate(margin, margin);
            ScrollIntoView(rect, new Duration(TimeSpan.FromMilliseconds(_defaultZoomTime)), true);
            return rect;
        }

        /// <summary>
        /// Zoom/scroll all the given framework elements into view
        /// </summary>
        /// <param name="elements">The array of elements</param>
        public void ScrollIntoView(FrameworkElement[] elements)
        {
            StopAnimations();
            Rect result = Rect.Empty;
            foreach (FrameworkElement e in elements)
            {
                Rect rect = new Rect(0, 0, e.ActualWidth, e.ActualHeight);
                // Get zoomed & translated coordinates of this object by transforming to the container coordinates.
                rect = e.TransformToAncestor(_container).TransformBounds(rect);
                if (result == Rect.Empty)
                {
                    result = rect;
                }
                else
                {
                    result.Union(rect);
                }
            }
            result.Inflate(margin, margin);
            ScrollIntoView(result, new Duration(TimeSpan.FromMilliseconds(_defaultZoomTime)), true);
        }

        /// <summary>
        /// Zoom and scroll the given rectangle into view.
        /// </summary>
        /// <param name="rect">The rectangle to scroll into view</param>
        public void ScrollIntoView(Rect rect)
        {
            ScrollIntoView(rect, new Duration(TimeSpan.FromMilliseconds(_defaultZoomTime)), true);
        }

        /// <summary>
        /// The margin around the rectangle that we zoom into view so it doesn't take up all the visible space.
        /// </summary>
        const double margin = 20;

        /// <summary>
        /// Start the zoom/scroll animation
        /// </summary>
        /// <param name="rect">Rect is in 'container' coordinates.</param>
        /// <param name="duration">Time allowed for the animation</param>
        /// <param name="zoom">Whether we can zoom or not</param>
        void ScrollIntoView(Rect rect, Duration duration, bool zoom)
        {
            Size containerSize = ContainerSize;
            double width = containerSize.Width;
            double height = containerSize.Height;
            
            // Get the bounds of the container so we can see if the selected node is inside these bounds right now.
            Rect window = new Rect(0, 0, width, height);

            if ((rect.Width > width || rect.Height > height))
            {
                Rect tr = _container.TransformToDescendant(this._target).TransformBounds(rect);
                if (zoom)
                {
                    ZoomToRect(tr);
                    return;
                }
            }

            // Calculate the delta needed to get the node to be visible in the container.
            Point delta = new Point();
            if (rect.Left < window.Left)
            {
                delta.X = window.Left - rect.Left + margin;
            }
            else if (rect.Right > window.Right)
            {
                delta.X = window.Right - rect.Right - margin;
            }
            if (rect.Top < window.Top)
            {
                delta.Y = window.Top - rect.Top + margin;
            }
            else if (rect.Bottom > window.Bottom)
            {
                delta.Y = window.Bottom - rect.Bottom - margin;
            }
            if (delta.X != 0 || delta.Y != 0)
            {
                // Then we do need to move the "target" canvas by changing the current translation so that
                // the selected node is visble.  We need to change the current translation by the 'delta'
                // amount.

                Point startPos = new Point(_translate.X, _translate.Y);
                double x = startPos.X + delta.X;
                if (x > 0) x = 0;
                double y = startPos.Y + delta.Y;
                if (y > 0) y = 0;

                Point newPos = new Point(x, y);
                PointAnimation pa = new PointAnimation(startPos, newPos, duration);
                this.BeginAnimation(OffsetProperty, pa);
            }
        }

        /// <summary>
        /// Handle the actual animation of the properties
        /// </summary>
        /// <param name="e">Property change information</param>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == ZoomToPointProperty)
            {
                double o = (double)e.OldValue;
                double v = (double)e.NewValue;
                if (v != 0 && v != o)
                {
                    this._zoom = v;
                    OnZoomChanged();
                    KeepPositionStable();
                }
            }
            else if (e.Property == ZoomToRectProperty)
            {
                double o = (double)e.OldValue;
                double v = (double)e.NewValue;
                if (v != 0 && v != o)
                {
                    this._zoom = v;
                    OnZoomChanged();
                    KeepRectStable();
                }
            }
            else if (e.Property == OffsetProperty)
            {
                Point p = (Point)e.NewValue;
                Point o = (Point)e.OldValue;
                if (p.X != o.X || p.Y != o.Y)
                {
                    this.Offset = p;
                }
            }
        }

        /// <summary>
        /// Every Freezable subclass must implement this method.
        /// </summary>
        /// <returns>A new instance of this object</returns>
        protected override Freezable CreateInstanceCore()
        {
            return new MapZoom(this._target);
        }

    }
}