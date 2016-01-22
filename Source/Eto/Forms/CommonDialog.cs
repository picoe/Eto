using System;

namespace Eto.Forms
{
	/// <summary>
	/// Result codes for <see cref="CommonDialog"/> or <see cref="MessageBox"/> dialogs
	/// </summary>
	/// <copyright>(c) 2014 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
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

	/// <summary>
	/// Base class for common dialogs
	/// </summary>
	public abstract class CommonDialog : Widget
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.CommonDialog"/> class.
		/// </summary>
		protected CommonDialog()
		{
		}

		/// <summary>
		/// Shows the dialog with the specified parent, blocking until a result is returned.
		/// </summary>
		/// <returns>The dialog result.</returns>
		/// <param name="parent">Parent control</param>
		public DialogResult ShowDialog(Control parent)
		{
			return ShowDialog(parent != null ? parent.ParentWindow : null);
		}

		/// <summary>
		/// Shows the dialog with the specified parent window, blocking until a result is returned.
		/// </summary>
		/// <returns>The dialog result.</returns>
		/// <param name="parent">Parent window.</param>
		public virtual DialogResult ShowDialog(Window parent)
		{
			return Handler.ShowDialog(parent);
		}

		/// <summary>
		/// Handler interface for the <see cref="CommonDialog"/>
		/// </summary>
		public new interface IHandler : Widget.IHandler
		{
			/// <summary>
			/// Shows the dialog with the specified parent window, blocking until a result is returned.
			/// </summary>
			/// <returns>The dialog result.</returns>
			/// <param name="parent">Parent window.</param>
			DialogResult ShowDialog(Window parent);
		}
	}
}

