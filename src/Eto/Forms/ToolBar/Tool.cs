using System;

namespace Eto.Forms
{
	/// <summary>
	/// Base class for tool items on a <see cref="ToolBar"/>
	/// </summary>
	public class Tool : BindableWidget
	{
		new IHandler Handler {  get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Called before the tool item is assigned to a control/window
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected internal virtual void OnPreLoad(EventArgs e)
		{
			Handler.OnPreLoad(e);
		}

		/// <summary>
		/// Called when the tool item is assigned to a control/window
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected internal virtual void OnLoad(EventArgs e)
		{
			Handler.OnLoad(e);
		}

		/// <summary>
		/// Called when the tool item is removed from a control/window
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected internal virtual void OnUnLoad(EventArgs e)
		{
			Handler.OnUnLoad(e);
		}

		internal void TriggerPreLoad(EventArgs e)
		{
			using (Platform.Context)
				OnPreLoad(e);
		}

		internal void TriggerLoad(EventArgs e)
		{
			using (Platform.Context)
				OnLoad(e);
		}

		internal void TriggerUnLoad(EventArgs e)
		{
			using (Platform.Context)
				OnUnLoad(e);
		}

		/// <summary>
		/// Interface for platform implementations of the <see cref="Tool"/> class.
		/// </summary>
		public new interface IHandler : BindableWidget.IHandler
		{
			/// <summary>
			/// Called when the tool item is assigned to a control/window
			/// </summary>
			/// <param name="e">Event arguments</param>
			void OnLoad(EventArgs e);
			/// <summary>
			/// Called before the tool item is assigned to a control/window
			/// </summary>
			/// <param name="e">Event arguments</param>
			void OnPreLoad(EventArgs e);
			/// <summary>
			/// Called when the tool item is removed from a control/window
			/// </summary>
			/// <param name="e">Event arguments</param>
			void OnUnLoad(EventArgs e);
		}

	}

}

