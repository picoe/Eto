//-----------------------------------------------------------------------
// <copyright file="RectangleSelectionGesture.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;

namespace Microsoft.Sample.Controls
{
    /// <summary>
    /// This class provides the ability to draw a rectangle on a zoomable object and zoom into that location.
    /// </summary>
    internal class RectangleSelectionGesture
    {
        SelectionRectVisual _selectionRectVisual;
        Point _start;
        bool _watching;
        FrameworkElement _target;
        MapZoom _zoom;
        Panel _container;
        Point _mouseDownPoint;
        Rect _selectionRect;
        bool _zoomSelection;
        int _zoomSizeThreshold = 20;
        int _selectionThreshold = 5; // allow some mouse wiggle on mouse down without actually selecting stuff!
        ModifierKeys _mods;

        public event EventHandler Selected;

        /// <summary>
        /// Construct new RectangleSelectionGesture object for selecting things in the given target object.
        /// </summary>
        /// <param name="target">A FrameworkElement</param>
        /// <param name="zoom">The MapZoom object that wraps this same target object</param>
        public RectangleSelectionGesture(FrameworkElement target, MapZoom zoom, ModifierKeys mods)
        {
            _mods = mods;
            _target = target;
            _container = target.Parent as Panel;
            if (_container == null)
            {
                throw new ArgumentException("Target object must live in a Panel");
            }
            _zoom = zoom;
            _container.MouseLeftButtonDown += new MouseButtonEventHandler(OnMouseLeftButtonDown);
            _container.MouseLeftButtonUp += new MouseButtonEventHandler(OnMouseLeftButtonUp);
            _container.MouseMove += new MouseEventHandler(OnMouseMove);
        }

        /// <summary>
        /// Get the rectangle the user drew on the target object.
        /// </summary>
        public Rect SelectionRectangle
        {
            get { return _selectionRect; }
        }

        /// <summary>
        /// Get/Set whether to also zoom the selected rectangle.
        /// </summary>
        public bool ZoomSelection
        {
            get { return _zoomSelection; }
            set { _zoomSelection = value; }
        }            

        /// <summary>
        /// Handle the mouse left button down event
        /// </summary>
        /// <param name="sender">Mouse</param>
        /// <param name="e">Mouse down information</param>
        void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!e.Handled && (Keyboard.Modifiers & _mods) == _mods)
            {
                _start = e.GetPosition(_container);
                _watching = true;
                _mouseDownPoint = _start;
            }
        }

        /// <summary>
        /// Get/Set threshold that sets the minimum size rectangle we will allow user to draw.
        /// This allows user to start drawing a rectangle by then change their mind and mouse up
        /// without trigging an almost infinite zoom out to a very smalle piece of real-estate.
        /// </summary>
        public int ZoomSizeThreshold
        {
            get { return _zoomSizeThreshold; }
            set { _zoomSizeThreshold = value; }
        }

        /// <summary>
        /// Handle Mouse Move event.  Here we detect whether we've exceeded the _selectionThreshold
        /// and if so capture the mouse and create the visual zoom rectangle on the container object.
        /// </summary>
        /// <param name="sender">Mouse</param>
        /// <param name="e">Mouse move information.</param>
        void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_watching)
            {
                Point pos = e.GetPosition(_container);
                if (new Vector(_start.X - pos.X, _start.Y - pos.Y).Length > _selectionThreshold)
                {
                    _watching = false;
                    Mouse.Capture(_target, CaptureMode.SubTree);
                    _selectionRectVisual = new SelectionRectVisual(_start, _start, _zoom.Zoom);
                    _container.Children.Add(_selectionRectVisual);
                }
            }
            if (_selectionRectVisual != null)
            {
                if (_selectionRectVisual.Zoom != _zoom.Zoom)
                {
                    _selectionRectVisual.Zoom = _zoom.Zoom;
                }
                _selectionRectVisual.SecondPoint = e.GetPosition(_container);
            }
        }

        /// <summary>
        /// Handle the mouse left button up event.  Here we actually process the selected rectangle
        /// if any by first raising an event for client to receive then also zooming to that rectangle
        /// if ZoomSelection is true
        /// </summary>
        /// <param name="sender">Mouse</param>
        /// <param name="e">Mouse button information</param>
        void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _watching = false;
            if (_selectionRectVisual != null)
            {
                Mouse.Capture(_target, CaptureMode.None);
                Point pos = e.GetPosition(_container);
                double f = Math.Min(Math.Abs(pos.X - _mouseDownPoint.X), Math.Abs(pos.Y - _mouseDownPoint.Y));
                Rect r = GetSelectionRect(pos);
                _selectionRect = r;
                if (Selected != null)
                {
                    Selected(this, EventArgs.Empty);
                }

                if (_zoomSelection && f > _zoomSizeThreshold )
                {
                    _zoom.ZoomToRect(r);
                }

                _container.Children.Remove(_selectionRectVisual);
                _selectionRectVisual = null;
            }
        }

        /// <summary>
        /// Get the actual selection rectangle that encompasses the mouse down position and the given point.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        Rect GetSelectionRect(Point p)
        {
            Rect r = new Rect(_start, p);
            return _container.TransformToDescendant(_target).TransformBounds(r);
        }
    }
}