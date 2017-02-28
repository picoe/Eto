using System;
using Eto.Drawing;

namespace Eto.Forms
{
	/// <summary>
	/// System toast notification.
	/// </summary>
	[Handler(typeof(Notification.IHandler))]
	public class Notification : Widget
	{
		private Icon icon;

		new IHandler Handler { get { return (IHandler)base.Handler; } }

		static readonly object callback = new Callback();

		/// <summary>
		/// Gets an instance of an object used to perform callbacks to the widget from handler implementations.
		/// </summary>
		/// <returns>The callback instance to use for this widget.</returns>
		protected override object GetCallback() { return callback; }

		/// <summary>
		/// Gets or sets the icon for the <see cref="Notification"/>.
		/// </summary>
		/// <remarks>
		/// Does nothing on WPF and WinForms.
		/// </remarks>
		/// <value>The icon of the <see cref="Notification"/>.</value>
		public Icon Icon
		{
			get { return icon; }
			set
			{
				icon = value;
				Handler.SetIcon(value);
			}
		}

		/// <summary>
		/// Gets or sets the message of the <see cref="Notification"/>.
		/// </summary>
		/// <value>The message of the <see cref="Notification"/>.</value>
		public string Message
		{
			get { return Handler.Message; }
			set { Handler.Message = value; }
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="Notification"/>
		/// requires a <see cref="TrayIndicator"/> to be displayed.
		/// </summary>
		/// <value><c>true</c> if <see cref="TrayIndicator"/> is required; otherwise, <c>false</c>.</value>
		public bool RequiresTrayIndicator
		{
			get { return false; }
		}

		/// <summary>
		/// Gets or sets the title for the <see cref="Notification"/>.
		/// </summary>
		/// <value>The title for the <see cref="Notification"/>.</value>
		public string Title
		{
			get { return Handler.Title; }
			set { Handler.Title = value; }
		}

		/// <summary>
		/// Shows the current notification.
		/// </summary>
		/// <remarks>
		/// On some platforms like Gtk and Windows 10 the indicator
		/// is not needed, while on the others like Windows 7 it's required.
		/// 
		/// You can find out if the indicator is needed by
		/// looking at <see cref="RequiresTrayIndicator"/> property.
		/// </remarks>
		/// <param name="indicator">Indicator to use to show the notification.</param>
		public void Show(TrayIndicator indicator = null)
		{
			Handler.Show(indicator);
		}

		/// <summary>
		/// Handler interface for the <see cref="Notification"/> control
		/// </summary>
		public new interface IHandler : Widget.IHandler
		{
			/// <summary>
			/// Gets or sets the title for the <see cref="Notification"/>.
			/// </summary>
			/// <value>The title for the <see cref="Notification"/>.</value>
			string Title { get; set; }

			/// <summary>
			/// Gets or sets the message of the <see cref="Notification"/>.
			/// </summary>
			/// <value>The message of the <see cref="Notification"/>.</value>
			string Message { get; set; }

			/// <summary>
			/// Gets a value indicating whether this <see cref="Notification"/>
			/// requires a <see cref="TrayIndicator"/> to be displayed.
			/// </summary>
			/// <value><c>true</c> if <see cref="TrayIndicator"/> is required; otherwise, <c>false</c>.</value>
			bool RequiresTrayIndicator { get; }

			/// <summary>
			/// Sets the icon for the <see cref="Notification"/>.
			/// </summary>
			/// <param name="icon">The icon of the <see cref="Notification"/>.</param>
			void SetIcon(Icon icon);

			/// <summary>
			/// Shows the current notification.
			/// </summary>
			/// <param name="indicator">Indicator to use to show the notification.</param>
			void Show(TrayIndicator indicator = null);
		}

		/// <summary>
		/// Event identifier for handlers when attaching the <see cref="Activated"/> event.
		/// </summary>
		public const string ActivatedEvent = "Notification.Activated";

		/// <summary>
		/// Event to handle when the user left click the <see cref="Notification"/>.
		/// </summary>
		public event EventHandler<EventArgs> Activated
		{
			add { Properties.AddHandlerEvent(ActivatedEvent, value); }
			remove { Properties.RemoveEvent(ActivatedEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="Activated"/> event.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		protected virtual void OnActivated(EventArgs e)
		{
			Properties.TriggerEvent(ActivatedEvent, this, e);
		}

		/// <summary>
		/// Callback interface for <see cref="Notification"/>
		/// </summary>
		public new interface ICallback : Widget.ICallback
		{
			/// <summary>
			/// Raises activated event.
			/// </summary>
			void OnActivated(Notification widget, EventArgs e);
		}

		/// <summary>
		/// Callback implementation for handlers of <see cref="Notification"/>
		/// </summary>
		protected class Callback : ICallback
		{
			/// <summary>
			/// Raises activated event.
			/// </summary>
			public void OnActivated(Notification widget, EventArgs e)
			{
				widget.Platform.Invoke(() => widget.OnActivated(e));
			}
		}
	}
}
