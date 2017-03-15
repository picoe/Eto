using System;
using Eto.Drawing;
using System.ComponentModel;

namespace Eto.Forms
{
	/// <summary>
	/// Event arguments for <see cref="Scrollable.Scroll"/> events
	/// </summary>
	public class ScrollEventArgs : EventArgs
	{
		/// <summary>
		/// Gets the scroll position of the Scrollable
		/// </summary>
		/// <value>The scroll position.</value>
		public Point ScrollPosition { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.ScrollEventArgs"/> class.
		/// </summary>
		/// <param name="scrollPosition">Scroll position.</param>
		public ScrollEventArgs(Point scrollPosition)
		{
			this.ScrollPosition = scrollPosition;
		}
	}

	/// <summary>
	/// Border types
	/// </summary>
	public enum BorderType
	{
		/// <summary>
		/// Shows a bezel, if that is the default border for controls
		/// </summary>
		Bezel,

		/// <summary>
		/// Shows a single line border
		/// </summary>
		Line,

		/// <summary>
		/// Show no border
		/// </summary>
		None
	}

	/// <summary>
	/// Control to show content in a scrollable container
	/// </summary>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	[Handler(typeof(Scrollable.IHandler))]
	public class Scrollable : Panel
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Event identifier for handlers when attaching the <see cref="Scrollable.Scroll"/> event
		/// </summary>
		public const string ScrollEvent = "Scrollable.ScrollEvent";

		/// <summary>
		/// Event to handle when the <see cref="ScrollPosition"/> changes
		/// </summary>
		public event EventHandler<ScrollEventArgs> Scroll
		{
			add { Properties.AddHandlerEvent(ScrollEvent, value); }
			remove { Properties.RemoveEvent(ScrollEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="Scroll"/> event
		/// </summary>
		/// <param name="e">Scroll event arguments</param>
		protected virtual void OnScroll(ScrollEventArgs e)
		{
			Properties.TriggerEvent(ScrollEvent, this, e);
		}

		static Scrollable()
		{
			EventLookup.Register<Scrollable>(c => c.OnScroll(null), Scrollable.ScrollEvent);
		}

		/// <summary>
		/// Manually updates the scroll sizes based on the content
		/// </summary>
		/// <remarks>
		/// This should not typically be necessary as it should be done automatically
		/// </remarks>
		public void UpdateScrollSizes()
		{
			Handler.UpdateScrollSizes();
		}

		/// <summary>
		/// Gets or sets the scroll position from the top-left origin
		/// </summary>
		/// <value>The scroll position.</value>
		public Point ScrollPosition
		{
			get { return Handler.ScrollPosition; }
			set { Handler.ScrollPosition = value; }
		}

		/// <summary>
		/// Gets or sets the size of the scrollable region manually
		/// </summary>
		/// <remarks>
		/// Typically you do not need to set the scroll size manually, as the content will be used to determine the size
		/// automatically.
		/// </remarks>
		/// <value>The size of the scrollable region.</value>
		public Size ScrollSize
		{
			get { return Handler.ScrollSize; }
			set { Handler.ScrollSize = value; }
		}

		/// <summary>
		/// Gets or sets the border type
		/// </summary>
		/// <value>The border.</value>
		public BorderType Border
		{
			get { return Handler.Border; }
			set { Handler.Border = value; }
		}

		/// <summary>
		/// Gets the coordinates of the rectangle that is visible to the user
		/// </summary>
		/// <value>The visible rect.</value>
		public Rectangle VisibleRect
		{
			get { return Handler.VisibleRect; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Eto.Forms.Scrollable"/> expands content to the width of the control
		/// </summary>
		/// <remarks>
		/// This controls whether content that is smaller than the size of the control will be expanded to fill the available space.
		/// Content that is larger than the available space will make the horizontal scrollbar appear, regardless of this setting.
		/// </remarks>
		/// <value><c>true</c> to expand content to the width of the control; otherwise, <c>false</c>.</value>
		[DefaultValue(true)]
		public bool ExpandContentWidth
		{
			get { return Handler.ExpandContentWidth; }
			set { Handler.ExpandContentWidth = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Eto.Forms.Scrollable"/> expands content to the height of the control
		/// </summary>
		/// <remarks>
		/// This controls whether content that is smaller than the size of the control will be expanded to fill the available space.
		/// Content that is larger than the available space will make the vertical scrollbar appear, regardless of this setting.
		/// </remarks>
		/// <value><c>true</c> to expand content to the height of the control; otherwise, <c>false</c>.</value>
		[DefaultValue(true)]
		public bool ExpandContentHeight
		{
			get { return Handler.ExpandContentHeight; }
			set { Handler.ExpandContentHeight = value; }
		}

		/// <summary>
		/// Hint to get or set the minimum zoom of the scrollable region, if the platform supports it
		/// </summary>
		/// <value>The minimum zoom.</value>
		[DefaultValue(1f)]
		public float MinimumZoom
		{
			get { return Handler.MinimumZoom; }
			set { Handler.MinimumZoom = value; }
		}

		/// <summary>
		/// Hint to get or set the maximum zoom of the scrollable region, if the platform supports it
		/// </summary>
		/// <value>The maximum zoom.</value>
		[DefaultValue(1f)]
		public float MaximumZoom
		{
			get { return Handler.MaximumZoom; }
			set { Handler.MaximumZoom = value; }
		}

		/// <summary>
		/// Hint to set the zoom level of the scrollable region, if the platform supports it
		/// </summary>
		/// <value>The current zoom level</value>
		[DefaultValue(1f)]
		public float Zoom
		{
			get { return Handler.Zoom; }
			set { Handler.Zoom = value; }
		}

		#region Callback

		static readonly object callback = new Callback();
		/// <summary>
		/// Gets an instance of an object used to perform callbacks to the widget from handler implementations
		/// </summary>
		/// <returns>The callback instance to use for this widget</returns>
		protected override object GetCallback() { return callback; }

		/// <summary>
		/// Callback interface for the <see cref="Scrollable"/>
		/// </summary>
		public new interface ICallback : Panel.ICallback
		{
			/// <summary>
			/// Raises the scroll event.
			/// </summary>
			void OnScroll(Scrollable widget, ScrollEventArgs e);
		}

		/// <summary>
		/// Callback implementation for the <see cref="Scrollable"/>
		/// </summary>
		protected new class Callback : Panel.Callback, ICallback
		{
			/// <summary>
			/// Raises the scroll event.
			/// </summary>
			public void OnScroll(Scrollable widget, ScrollEventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnScroll(e);
			}
		}

		#endregion

		#region Handler

		/// <summary>
		/// Handler interface for the <see cref="Scrollable"/> control
		/// </summary>
		/// <copyright>(c) 2014 by Curtis Wensley</copyright>
		/// <license type="BSD-3">See LICENSE for full terms</license>
		public new interface IHandler : Panel.IHandler
		{
			/// <summary>
			/// Manually updates the scroll sizes based on the content
			/// </summary>
			/// <remarks>
			/// This should not typically be necessary as it should be done automatically
			/// </remarks>
			void UpdateScrollSizes();

			/// <summary>
			/// Gets or sets the scroll position from the top-left origin
			/// </summary>
			/// <value>The scroll position.</value>
			Point ScrollPosition { get; set; }

			/// <summary>
			/// Gets or sets the size of the scrollable region manually
			/// </summary>
			/// <remarks>
			/// Typically you do not need to set the scroll size manually, as the content will be used to determine the size
			/// automatically.
			/// </remarks>
			/// <value>The size of the scrollable region.</value>
			Size ScrollSize { get; set; }

			/// <summary>
			/// Gets or sets the border type
			/// </summary>
			/// <value>The border.</value>
			BorderType Border { get; set; }

			/// <summary>
			/// Gets the coordinates of the rectangle that is visible to the user
			/// </summary>
			/// <value>The visible rect.</value>
			Rectangle VisibleRect { get; }

			/// <summary>
			/// Gets or sets a value indicating whether this <see cref="Eto.Forms.Scrollable"/> expands content to the width of the control
			/// </summary>
			/// <remarks>
			/// This controls whether content that is smaller than the size of the control will be expanded to fill the available space.
			/// Content that is larger than the available space will make the horizontal scrollbar appear, regardless of this setting.
			/// </remarks>
			/// <value><c>true</c> to expand content to the width of the control; otherwise, <c>false</c>.</value>
			bool ExpandContentWidth { get; set; }

			/// <summary>
			/// Gets or sets a value indicating whether this <see cref="Eto.Forms.Scrollable"/> expands content to the height of the control
			/// </summary>
			/// <remarks>
			/// This controls whether content that is smaller than the size of the control will be expanded to fill the available space.
			/// Content that is larger than the available space will make the vertical scrollbar appear, regardless of this setting.
			/// </remarks>
			/// <value><c>true</c> to expand content to the height of the control; otherwise, <c>false</c>.</value>
			bool ExpandContentHeight { get; set; }

			/// <summary>
			/// Hint to get or set the minimum zoom of the scrollable region, if the platform supports it
			/// </summary>
			/// <value>The minimum zoom.</value>
			float MinimumZoom { get; set; }

			/// <summary>
			/// Hint to get or set the maximum zoom of the scrollable region, if the platform supports it
			/// </summary>
			/// <value>The maximum zoom.</value>
			float MaximumZoom { get; set; }

			/// <summary>
			/// Hint to set the zoom level of the scrollable region, if the platform supports it
			/// </summary>
			/// <value>The current zoom level</value>
			float Zoom { get; set; }
		}

		#endregion
	}
}
