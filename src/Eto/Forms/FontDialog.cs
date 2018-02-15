using System;
using Eto.Drawing;

namespace Eto.Forms
{
	/// <summary>
	/// Dialog for a user to pick a font and style
	/// </summary>
	/// <remarks>
	/// The font dialog on some platforms may run asynchronously, and return immediately after
	/// the <see cref="CommonDialog.ShowDialog(Control)"/> call. For example, on OS X the font dialog is a non-modal
	/// shared tool window that stays on the screen until the user dismisses it.
	/// 
	/// You should always handle the <see cref="FontChanged"/> event to determine when the value has changed.
	/// </remarks>
	[Handler(typeof(FontDialog.IHandler))]
	public class FontDialog : CommonDialog
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Identifier for handlers when attaching the <see cref="FontChanged"/> event.
		/// </summary>
		public const string FontChangedEvent = "FontDialog.FontChanged";

		/// <summary>
		/// Occurs when the <see cref="Font"/> is changed.
		/// </summary>
		public event EventHandler<EventArgs> FontChanged
		{
			add { Properties.AddHandlerEvent(FontChangedEvent, value); }
			remove { Properties.RemoveEvent(FontChangedEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="FontChanged"/> event.
		/// </summary>
		/// <param name="e">E.</param>
		protected virtual void OnFontChanged(EventArgs e)
		{
			Properties.TriggerEvent(FontChangedEvent, this, e);
		}

		static FontDialog()
		{
			EventLookup.Register<FontDialog>(c => c.OnFontChanged(null), FontDialog.FontChangedEvent);
		}

		/// <summary>
		/// Gets or sets the currently selected font.
		/// </summary>
		/// <value>The selected font.</value>
		public Font Font
		{
			get { return Handler.Font; }
			set { Handler.Font = value; }
		}

		/// <summary>
		/// Gets an instance of an object used to perform callbacks to the widget from handler implementations
		/// </summary>
		/// <returns>The callback.</returns>
		protected override object GetCallback() { return new Callback(); }

		/// <summary>
		/// Callback interface for handlers of the <see cref="FontDialog"/>.
		/// </summary>
		public new interface ICallback : CommonDialog.ICallback
		{
			/// <summary>
			/// Raises the font changed event.
			/// </summary>
			void OnFontChanged(FontDialog widget, EventArgs e);
		}

		/// <summary>
		/// Callback implementation for handlers of the <see cref="FontDialog"/>.
		/// </summary>
		protected class Callback : ICallback
		{
			/// <summary>
			/// Raises the font changed event.
			/// </summary>
			public void OnFontChanged(FontDialog widget, EventArgs e)
			{
				using (widget.Platform.Context)
					widget.OnFontChanged(e);
			}
		}

		/// <summary>
		/// Handler interface for the <see cref="FontDialog"/>.
		/// </summary>
		public new interface IHandler : CommonDialog.IHandler
		{
			/// <summary>
			/// Gets or sets the currently selected font.
			/// </summary>
			/// <value>The selected font.</value>
			Font Font { get; set; }
		}
	}
}

