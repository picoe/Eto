using System;
using Eto.Drawing;

namespace Eto.Forms
{
	/// <summary>
	/// Event arguments when painting using the <see cref="Drawable.Paint"/> event
	/// </summary>
	public class PaintEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.PaintEventArgs"/> class.
		/// </summary>
		/// <param name="graphics">Graphics for the paint event</param>
		/// <param name="clipRectangle">Rectangle of the region being painted</param>
		public PaintEventArgs(Graphics graphics, RectangleF clipRectangle)
		{
			ClipRectangle = clipRectangle;
			Graphics = graphics;
		}

		/// <summary>
		/// Gets the graphics for the paint operation
		/// </summary>
		/// <value>The graphics.</value>
		public Graphics Graphics { get; private set; }

		/// <summary>
		/// Gets the rectangle of the region being painted
		/// </summary>
		/// <remarks>
		/// This should be used to optimize what is drawn by only drawing content that intersects with this rectangle.
		/// </remarks>
		/// <value>The clip rectangle for the current paint operation</value>
		public RectangleF ClipRectangle { get; private set; }
	}

	/// <summary>
	/// Control with a paintable user interface
	/// </summary>
	/// <remarks>
	/// The drawable control is used to perform custom painting.
	/// </remarks>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	[Handler(typeof(Drawable.IHandler))]
	public class Drawable : Panel
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Event to handle custom painting on the control
		/// </summary>
		public event EventHandler<PaintEventArgs> Paint;

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.Drawable"/> class.
		/// </summary>
		public Drawable()
		{
			Handler.Create();
			Initialize();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.Drawable"/> class with the specified handler
		/// </summary>
		/// <param name="handler">Handler interface for the drawable</param>
		protected Drawable(IHandler handler)
			: base(handler)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.Drawable"/> class with a hint whether it is intended for a large canvas
		/// </summary>
		/// <remarks>
		/// Some platforms can optimize large canvases, such as mobile platforms by tiling the painting of the canvas.
		/// 
		/// A large canvas is one that is larger than the intended screen size.
		/// 
		/// Platforms are not required to change the behaviour of the drawable depending on this value.  Desktop platforms
		/// typically do not change their behaviour based on this.
		/// </remarks>
		/// <param name="largeCanvas">If set to <c>true</c> the drawable is created to have a large canvas.</param>
		public Drawable(bool largeCanvas)
		{
			Handler.Create(largeCanvas);
			Initialize();
		}

		/// <summary>
		/// Raises the <see cref="Paint"/> event
		/// </summary>
		/// <param name="e">Paint event arguments</param>
		protected virtual void OnPaint(PaintEventArgs e)
		{
			if (Paint != null)
				Paint(this, e);
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="Eto.Forms.Drawable"/> supports the <see cref="CreateGraphics"/> method
		/// </summary>
		/// <value><c>true</c> if supports creating graphics; otherwise, <c>false</c>.</value>
		public bool SupportsCreateGraphics
		{
			get { return Handler.SupportsCreateGraphics; }
		}

		/// <summary>
		/// Creates a graphics context for this control
		/// </summary>
		/// <remarks>
		/// This can be used to draw directly onto the control.
		/// Ensure you dispose the graphics object after performing any painting.
		/// Note that not all platforms support drawing directly on the control, use <see cref="SupportsCreateGraphics"/>.
		/// </remarks>
		/// <returns>A new graphics context that can be used to draw directly onto the control</returns>
		public Graphics CreateGraphics()
		{
			return Handler.CreateGraphics();
		}

		/// <summary>
		/// Gets or sets a value indicating whether this instance can recieve the input/keyboard focus
		/// </summary>
		/// <remarks>
		/// If this is true, by default all platforms will focus the control automatically when it is clicked.
		/// </remarks>
		/// <value><c>true</c> if this instance can be focussed; otherwise, <c>false</c>.</value>
		public bool CanFocus
		{
			get { return Handler.CanFocus; }
			set { Handler.CanFocus = value; }
		}

		/// <summary>
		/// Update the specified <paramref name="region"/> directly
		/// </summary>
		/// <remarks>
		/// This forces the region to be painted immediately.  On some platforms, this will be similar to calling
		/// <see cref="Control.Invalidate(Rectangle)"/>, and queue the repaint instead of blocking until it is painted.
		/// </remarks>
		/// <param name="region">Region to update the control</param>
		public void Update(Rectangle region)
		{
			Handler.Update(region);
		}

		static readonly object callback = new Callback();
		/// <summary>
		/// Gets an instance of an object used to perform callbacks to the widget from handler implementations
		/// </summary>
		/// <returns>The callback instance to use for this widget</returns>
		protected override object GetCallback() { return callback; }

		/// <summary>
		/// Callback interface for <see cref="Drawable"/>
		/// </summary>
		public new interface ICallback : Panel.ICallback
		{
			/// <summary>
			/// Raises the paint event.
			/// </summary>
			void OnPaint(Drawable widget, PaintEventArgs e);
		}

		/// <summary>
		/// Callback implementation for handlers of <see cref="Drawable"/>
		/// </summary>
		protected new class Callback : Panel.Callback, ICallback
		{
			/// <summary>
			/// Raises the paint event.
			/// </summary>
			public void OnPaint(Drawable widget, PaintEventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnPaint(e);
			}
		}

		#region Handler

		/// <summary>
		/// Handler interface for the <see cref="Drawable"/> control
		/// </summary>
		/// <copyright>(c) 2014 by Curtis Wensley</copyright>
		/// <license type="BSD-3">See LICENSE for full terms</license>
		[AutoInitialize(false)]
		public new interface IHandler : Panel.IHandler
		{
			/// <summary>
			/// Gets a value indicating whether this <see cref="Eto.Forms.Drawable"/> supports the <see cref="CreateGraphics"/> method
			/// </summary>
			/// <value><c>true</c> if supports creating graphics; otherwise, <c>false</c>.</value>
			bool SupportsCreateGraphics { get; }

			/// <summary>
			/// Creates a new drawable control without specifying a large canvas or not
			/// </summary>
			void Create();

			/// <summary>
			/// Called when creating a drawable control with a hint whether it is intended for a large canvas
			/// </summary>
			/// <remarks>
			/// Some platforms can optimize large canvases, such as mobile platforms by tiling the painting of the canvas.
			/// 
			/// A large canvas is one that is larger than the intended screen size.
			/// 
			/// Platforms are not required to change the behaviour of the drawable depending on this value.  Desktop platforms
			/// typically do not change their behaviour based on this.
			/// </remarks>
			/// <param name="largeCanvas">If set to <c>true</c> the drawable is created to have a large canvas.</param>
			void Create(bool largeCanvas);

			/// <summary>
			/// Update the specified <paramref name="region"/> directly
			/// </summary>
			/// <remarks>
			/// This forces the region to be painted immediately.  On some platforms, this will be similar to calling
			/// <see cref="Control.Invalidate(Rectangle)"/>, and queue the repaint instead of blocking until it is painted.
			/// </remarks>
			/// <param name="region">Region to update the control</param>
			void Update(Rectangle region);

			/// <summary>
			/// Gets or sets a value indicating whether this instance can recieve the input/keyboard focus
			/// </summary>
			/// <remarks>
			/// If this is true, by default all platforms will focus the control automatically when it is clicked.
			/// </remarks>
			/// <value><c>true</c> if this instance can be focussed; otherwise, <c>false</c>.</value>
			bool CanFocus { get; set; }

			/// <summary>
			/// Creates a graphics context for this control
			/// </summary>
			/// <remarks>
			/// This can be used to draw directly onto the control.
			/// Ensure you dispose the graphics object after performing any painting.
			/// Note that not all platforms support drawing directly on the control, use <see cref="SupportsCreateGraphics"/>.
			/// </remarks>
			/// <returns>A new graphics context that can be used to draw directly onto the control</returns>
			Graphics CreateGraphics();
		}

		#endregion
	}
}
