using System;

namespace Eto.Forms
{
	/// <summary>
	/// Event arguments when handling a notification event
	/// </summary>
	public sealed class NotificationEventArgs : EventArgs
	{
		/// <summary>
		/// Identifier of the notification that was sent
		/// </summary>
		public string ID { get; }

		/// <summary>
		/// Custom user data of the notification
		/// </summary>
		public string UserData { get; }

		/// <summary>
		/// Initializes a new instance of the NotificationEventArgs class
		/// </summary>
		/// <param name="id">Identifier of the notification that was sent</param>
		/// <param name="userData">Custom user data of the notification</param>
		public NotificationEventArgs(string id, string userData)
		{
			ID = id;
			UserData = userData;
		}
	}
}
