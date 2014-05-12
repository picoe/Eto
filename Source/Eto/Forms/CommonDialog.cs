using System;

namespace Eto.Forms
{
	/// <summary>
	/// Result codes for <see cref="CommonDialog"/> or <see cref="MessageBox"/> dialogs
	/// </summary>
	public enum DialogResult
	{
		/// <summary>
		/// No specific result
		/// </summary>
		None,
		/// <summary>
		/// User clicked 'OK'
		/// </summary>
		Ok,
		/// <summary>
		/// User clicked 'Cancel' or pressed escape to cancel
		/// </summary>
		Cancel,
		/// <summary>
		/// User clicked 'Yes'
		/// </summary>
		Yes,
		/// <summary>
		/// User clicked 'No'
		/// </summary>
		No,
		/// <summary>
		/// User clicked 'Abort'
		/// </summary>
		Abort,
		/// <summary>
		/// User clicked 'Ignore'
		/// </summary>
		Ignore,
		/// <summary>
		/// User clicked 'Retry'
		/// </summary>
		Retry
	}

	public abstract class CommonDialog : Widget
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		protected CommonDialog()
		{
		}

		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected CommonDialog (Generator g, Type type, bool initialize = true)
			: base (g, type, initialize)
		{
		}

		public DialogResult ShowDialog (Control parent)
		{
			return ShowDialog (parent.ParentWindow);
		}
		
		public DialogResult ShowDialog (Window parent)
		{
			return Handler.ShowDialog (parent);
		}
		
		public new interface IHandler : Widget.IHandler
		{
			DialogResult ShowDialog (Window parent);
		}

	}
}

