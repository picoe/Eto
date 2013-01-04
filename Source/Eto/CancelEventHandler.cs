using System;

namespace Eto
{
#if IOS

	/// <summary>
	/// Arguments for events that can be cancelled
	/// </summary>
	/// <remarks>
	/// Provided as MonoTouch does not include this
	/// </remarks>
	public class CancelEventArgs : EventArgs
	{
		/// <summary>
		/// Gets or sets a value indicating whether to cancel the event action
		/// </summary>
		/// <value>true to cancel the event action, false otherwise</value>
		public bool Cancel { get; set; }

		/// <summary>
		/// Initializes a new instance of the CancelEventArgs class
		/// </summary>
		public CancelEventArgs ()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.CancelEventArgs"/> class.
		/// </summary>
		/// <param name="cancel">Initial state of the cancel flag</param>
		public CancelEventArgs (bool cancel)
		{
			this.Cancel = cancel;
		}
	}	
	
	/// <summary>
	/// Delegate for a cancel event
	/// </summary>
	/// <remarks>
	/// Provided as MonoTouch does not include this
	/// </remarks>
	/// <param name="sender">sender of the event</param>
	/// <param name="e">cancel event arguments</param>
	public delegate void CancelEventHandler(object sender, CancelEventArgs e);

#endif
}

