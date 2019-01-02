using System;
using Eto.Drawing;

namespace Eto.Forms
{
	/// <summary>
	/// System toast notification.
	/// </summary>
	/// <remarks>
	/// Create a new instance of this class for each notification sent as they are not reusable. 
	/// The ID and <see cref="Notification.UserData"/> should be used to specify what action would happen when the user
	/// clicks the notification (if applicable).
	/// 
	/// All notifications should be handled by the <see cref="Application.NotificationActivated"/> event.
	/// 
	/// Note that in some platforms (e.g. macOS), the application may have a notification clicked when
	/// the application isn't even started.  In this case, the application is started an then sent the notification
	/// to the <see cref="Application.NotificationActivated"/> immediately.
	/// </remarks>
	[Handler(typeof(Notification.IHandler))]
	public class Notification : Widget
	{
		new IHandler Handler => (IHandler)base.Handler;

		/// <summary>
		/// Gets or sets the icon for the <see cref="Notification"/>.
		/// </summary>
		/// <remarks>
		/// Currently does nothing on WPF and WinForms.
		/// </remarks>
		/// <value>The icon of the <see cref="Notification"/>.</value>
		[Obsolete("Since 2.4, use ContentImage instead")]
		public Icon Icon
		{
			get { return ContentImage as Icon; }
			set { ContentImage = value; }
		}

		/// <summary>
		/// Gets or sets the content image of the notification
		/// </summary>
		/// <remarks>
		/// This is used to provide context to the user for what the notification is for.
		/// Currently does nothing on WPF and WinForms. On these platforms set-up a global Style
		/// Style.Add&lt;NotificationHandler&gt;("info", h =&gt; h.NotificationIcon = NotificationIcon.Info);
		/// and the use it via
		/// new Notification { Style = "info" }
		/// </remarks>
		public Image ContentImage
		{
			get { return Handler.ContentImage; }
			set { Handler.ContentImage = value; }
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
		/// <remarks>
		/// Usually if a tray is required but not provided, one will be created for the purposes of
		/// showing the notification with the same icon as the <see cref="Application.MainForm"/>.
		/// </remarks>
		/// <value><c>true</c> if <see cref="TrayIndicator"/> is required; otherwise, <c>false</c>.</value>
		public bool RequiresTrayIndicator => Handler.RequiresTrayIndicator;

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
		/// Gets or sets user data for the notification.
		/// </summary>
		/// <remarks>
		/// Use this to store application-specific data that would be useful for knowing what caused the notification.
		/// 
		/// The data you store would usually determine what action to perform in the application.
		/// 
		/// This is returned via <see cref="Application.NotificationActivated"/> via the <see cref="NotificationEventArgs"/>.
		/// </remarks>
		public string UserData
		{
			get { return Handler.UserData; }
			set { Handler.UserData = value; }
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
		public void Show(TrayIndicator indicator = null) => Handler.Show(indicator);

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
			/// Gets or sets user data for the notification.
			/// </summary>
			/// <remarks>
			/// Use this to store application-specific data that would be useful for knowing what caused the notification.
			/// 
			/// The data you store would usually determine what action to perform in the application.
			/// 
			/// This is returned via <see cref="Application.NotificationActivated"/> via the <see cref="NotificationEventArgs"/>.
			/// </remarks>
			string UserData { get; set; }

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
			/// Gets or sets the content image of the notification
			/// </summary>
			/// <remarks>
			/// This is used to provide context to the user for what the notification is for.
			/// </remarks>
			Image ContentImage { get; set; }

			/// <summary>
			/// Shows the current notification.
			/// </summary>
			/// <param name="indicator">Indicator to use to show the notification.</param>
			void Show(TrayIndicator indicator = null);
		}

		/// <summary>
		/// Event identifier for handlers when attaching the <see cref="Activated"/> event.
		/// </summary>
		[Obsolete("Since 2.4, Use Application.NotificationActivatedEvent instead.")]
		public const string ActivatedEvent = "Notification.Activated";

		/// <summary>
		/// Event to handle when the user left click the <see cref="Notification"/>.
		/// </summary>
		[Obsolete("Since 2.4, Use Application.NotificationActivated instead.")]
		public event EventHandler<EventArgs> Activated
		{
			add { Properties.AddHandlerEvent(ActivatedEvent, value); }
			remove { Properties.RemoveEvent(ActivatedEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="Activated"/> event.
		/// </summary>
		/// <param name="e">Event arguments.</param>
		[Obsolete("Since 2.4, Use Application.OnNotificationActivated instead.")]
		protected virtual void OnActivated(EventArgs e)
		{
			Properties.TriggerEvent(ActivatedEvent, this, e);
		}
	}
}
