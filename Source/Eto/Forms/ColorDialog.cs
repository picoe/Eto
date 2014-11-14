using System;
using Eto.Drawing;

namespace Eto.Forms
{
	/// <summary>
	/// Dialog for choosing a color
	/// </summary>
	/// <remarks>
	/// The color dialog on some platforms may run asynchronously, and return immediately after
	/// the <see cref="CommonDialog.ShowDialog(Control)"/> call. For example, on OS X the color picker is a non-modal
	/// shared tool window that stays on the screen until the user dismisses it.
	/// 
	/// You should always handle the <see cref="ColorChanged"/> event to determine when the value has changed.
	/// </remarks>
	[Handler(typeof(ColorDialog.IHandler))]
	public class ColorDialog : CommonDialog
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Occurs when the <see cref="Color"/> has changed.
		/// </summary>
		public event EventHandler<EventArgs> ColorChanged;

		/// <summary>
		/// Raises the <see cref="ColorChanged"/> event.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		protected virtual void OnColorChanged(EventArgs e)
		{
			if (ColorChanged != null)
				ColorChanged(this, e);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.ColorDialog"/> class.
		/// </summary>
		public ColorDialog()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.ColorDialog"/> class.
		/// </summary>
		/// <param name="generator">Generator.</param>
		[Obsolete("Use default constructor instead")]
		public ColorDialog(Generator generator)
			: this(generator, typeof(ColorDialog.IHandler))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.ColorDialog"/> class.
		/// </summary>
		/// <param name="generator">Generator.</param>
		/// <param name="type">Type.</param>
		/// <param name="initialize">If set to <c>true</c> initialize.</param>
		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected ColorDialog(Generator generator, Type type, bool initialize = true)
			: base(generator, type, initialize)
		{
		}

		/// <summary>
		/// Gets or sets the selected color.
		/// </summary>
		/// <value>The selected color.</value>
		public Color Color
		{
			get { return Handler.Color; }
			set { Handler.Color = value; }
		}

		static readonly object callback = new Callback();
		/// <summary>
		/// Gets an instance of an object used to perform callbacks to the widget from handler implementations
		/// </summary>
		/// <returns>The callback instance to use for this widget</returns>
		protected override object GetCallback() { return callback; }

		/// <summary>
		/// Callback interface for the <see cref="ColorDialog"/>
		/// </summary>
		public new interface ICallback
		{
			/// <summary>
			/// Raises the color changed event.
			/// </summary>
			void OnColorChanged(ColorDialog widget, EventArgs e);
		}

		/// <summary>
		/// Callback implementation for handlers of the <see cref="ColorDialog"/>
		/// </summary>
		protected class Callback : ICallback
		{
			/// <summary>
			/// Raises the color changed event.
			/// </summary>
			public void OnColorChanged(ColorDialog widget, EventArgs e)
			{
				widget.Platform.Invoke(() => widget.OnColorChanged(e));
			}
		}

		/// <summary>
		/// Handler interface for the <see cref="ColorDialog"/>
		/// </summary>
		public new interface IHandler : CommonDialog.IHandler
		{
			/// <summary>
			/// Gets or sets the selected color.
			/// </summary>
			/// <value>The selected color.</value>
			Color Color { get; set; }
		}
	}
}

