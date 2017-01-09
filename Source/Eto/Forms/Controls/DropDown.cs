using System;
using System.ComponentModel;

namespace Eto.Forms
{
	/// <summary>
	/// Presents a drop down to select from a list of items
	/// </summary>
	[Handler(typeof(DropDown.IHandler))]
	public class DropDown : ListControl
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Gets or sets a value indicating whether to show the control's border.
		/// </summary>
		/// <remarks>
		/// This is a hint to omit the border of the control and show it as plainly as possible.
		/// 
		/// Typically used when you want to show the control within a cell of the <see cref="GridView"/>.
		/// </remarks>
		/// <value><c>true</c> to show the control border; otherwise, <c>false</c>.</value>
		[DefaultValue(true)]
		public bool ShowBorder
		{
			get { return Handler.ShowBorder; }
			set { Handler.ShowBorder = value; }
		}

		/// <summary>
		/// Event identifier for handlers when attaching the <see cref="DropDownOpening"/> event
		/// </summary>
		public const string DropDownOpeningEvent = "DropDown.DropDownOpening";

		/// <summary>
		/// Occurs right before the drop down is opened.
		/// </summary>
		/// <remarks>
		/// This is useful so you can fill the items of the drop down only when they are needed.
		/// </remarks>
		public event EventHandler<EventArgs> DropDownOpening
		{
			add { Properties.AddHandlerEvent(DropDownOpeningEvent, value); }
			remove { Properties.RemoveEvent(DropDownOpeningEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="DropDownOpening"/> event.
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected virtual void OnDropDownOpening(EventArgs e)
		{
			Properties.TriggerEvent(DropDownOpeningEvent, this, e);
		}

		/// <summary>
		/// Event identifier for handlers when attaching the <see cref="DropDownClosed"/> event.
		/// </summary>
		public const string DropDownClosedEvent = "DropDown.DropDownClosed";

		/// <summary>
		/// Occurs when the drop down is closed.
		/// </summary>
		public event EventHandler<EventArgs> DropDownClosed
		{
			add { Properties.AddHandlerEvent(DropDownClosedEvent, value); }
			remove { Properties.RemoveEvent(DropDownClosedEvent, value); }
		}

		/// <summary>
		/// Raises the <see cref="DropDownClosed"/> event
		/// </summary>
		/// <param name="e">Event arguments</param>
		protected virtual void OnDropDownClosed(EventArgs e)
		{
			Properties.TriggerEvent(DropDownClosedEvent, this, e);
		}

		/// <summary>
		/// Gets the callback.
		/// </summary>
		/// <returns>The callback.</returns>
		protected override object GetCallback() => new Callback();

		/// <summary>
		/// Callback interface for the DropDown
		/// </summary>
		public new interface ICallback : ListControl.ICallback
		{
			/// <summary>
			/// Raises the <see cref="DropDownOpening"/> event.
			/// </summary>
			/// <param name="widget">Widget to raise the event</param>
			/// <param name="e">Event arguments</param>
			void OnDropDownOpening(DropDown widget, EventArgs e);

			/// <summary>
			/// Raises the <see cref="DropDownClosed"/> event
			/// </summary>
			/// <param name="widget">Widget to raise the event</param>
			/// <param name="e">Event arguments</param>
			void OnDropDownClosed(DropDown widget, EventArgs e);
		}

		/// <summary>
		/// Callback implementation for the DropDown
		/// </summary>
		protected new class Callback : ListControl.Callback, ICallback
		{
			/// <summary>
			/// Raises the <see cref="DropDownOpening"/> event.
			/// </summary>
			/// <param name="widget">Widget to raise the event</param>
			/// <param name="e">Event arguments</param>
			public void OnDropDownOpening(DropDown widget, EventArgs e)
			{
				widget.Platform.Invoke(() => widget.OnDropDownOpening(e));
			}

			/// <summary>
			/// Raises the <see cref="DropDownClosed"/> event
			/// </summary>
			/// <param name="widget">Widget to raise the event</param>
			/// <param name="e">Event arguments</param>
			public void OnDropDownClosed(DropDown widget, EventArgs e)
			{
				widget.Platform.Invoke(() => widget.OnDropDownClosed(e));
			}
		}

		/// <summary>
		/// Handler interface for the <see cref="DropDown"/>
		/// </summary>
		public new interface IHandler : ListControl.IHandler
		{
			/// <summary>
			/// Gets or sets a value indicating whether to show the control's border.
			/// </summary>
			/// <remarks>
			/// This is a hint to omit the border of the control and show it as plainly as possible.
			/// 
			/// Typically used when you want to show the control within a cell of the <see cref="GridView"/>.
			/// </remarks>
			/// <value><c>true</c> to show the control border; otherwise, <c>false</c>.</value>
			bool ShowBorder { get; set; }
		}
	}
}
