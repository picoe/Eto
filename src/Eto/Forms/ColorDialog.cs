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
		/// Gets or sets the selected color.
		/// </summary>
		/// <value>The selected color.</value>
		public Color Color
		{
			get { return Handler.Color; }
			set { Handler.Color = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the user can adjust the Alpha component of the Color.
		/// </summary>
		/// <remarks>
		/// This may or may not be supported in all platforms (e.g. WinForms).  
		/// Use <see cref="SupportsAllowAlpha"/> to determine if the current platform supports this feature.
		/// </remarks>
		/// <value><c>true</c> to allow adjustment of alpha; otherwise, <c>false</c>.</value>
		/// <seealso cref="SupportsAllowAlpha"/>
		public bool AllowAlpha
		{
			get { return Handler.AllowAlpha; }
			set { Handler.AllowAlpha = value; }
		}

		/// <summary>
		/// Gets a value indicating that the current platform supports the <see cref="AllowAlpha"/> property.
		/// </summary>
		/// <remarks>
		/// If not supported, the setting will be ignored.
		/// </remarks>
		/// <value><c>true</c> AllowAlpha is supported; otherwise, <c>false</c>.</value>
		/// <seealso cref="AllowAlpha"/>
		public bool SupportsAllowAlpha => Handler.SupportsAllowAlpha;

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
				using (widget.Platform.Context)
					widget.OnColorChanged(e);
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

			/// <summary>
			/// Gets or sets a value indicating whether the user can adjust the Alpha component of the Color.
			/// </summary>
			/// <remarks>
			/// This may or may not be supported in all platforms (e.g. WinForms).  
			/// Use <see cref="SupportsAllowAlpha"/> to determine if the current platform supports this feature.
			/// </remarks>
			/// <value><c>true</c> to allow adjustment of alpha; otherwise, <c>false</c>.</value>
			/// <seealso cref="SupportsAllowAlpha"/>
			bool AllowAlpha { get; set; }

			/// <summary>
			/// Gets a value indicating that the current platform supports the <see cref="AllowAlpha"/> property.
			/// </summary>
			/// <remarks>
			/// If not supported, the setting will be ignored.
			/// </remarks>
			/// <value><c>true</c> AllowAlpha is supported; otherwise, <c>false</c>.</value>
			/// <seealso cref="AllowAlpha"/>
			bool SupportsAllowAlpha { get; }
		}
	}
}

