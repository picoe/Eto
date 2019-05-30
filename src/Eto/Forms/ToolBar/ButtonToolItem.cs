using System;

namespace Eto.Forms
{
	/// <summary>
	/// Tool item to execute an action
	/// </summary>
	[Handler(typeof(ButtonToolItem.IHandler))]
	public class ButtonToolItem : ToolItem
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.ButtonToolItem"/> class.
		/// </summary>
		public ButtonToolItem()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.ButtonToolItem"/> class with the specified <paramref name="click"/> handler.
		/// </summary>
		/// <remarks>
		/// This is a convenience constructor to set up the click event.
		/// </remarks>
		/// <param name="click">Delegate to handle when the tool item is clicked.</param>
		public ButtonToolItem(EventHandler<EventArgs> click)
		{
			Click += click;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.ButtonToolItem"/> class with the specified <paramref name="command"/>.
		/// </summary>
		/// <param name="command">Command for the tool item.</param>
		public ButtonToolItem(Command command)
			: base(command)
		{
			Handler.CreateFromCommand(command);
		}

		/// <summary>
		/// Handler for the <see cref="ButtonToolItem"/>.
		/// </summary>
		public new interface IHandler : ToolItem.IHandler
		{
		}
	}
}
