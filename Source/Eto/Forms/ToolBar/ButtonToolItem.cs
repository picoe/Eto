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
		/// Initializes a new instance of the <see cref="Eto.Forms.ButtonToolItem"/> class with the specified <paramref name="command"/>.
		/// </summary>
		/// <param name="command">Command for the tool item.</param>
		public ButtonToolItem(Command command)
			: base(command)
		{
			Handler.CreateFromCommand(command);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.ButtonToolItem"/> class.
		/// </summary>
		/// <param name="generator">Generator.</param>
		[Obsolete("Use default constructor instead")]
		public ButtonToolItem(Generator generator) : base(generator, typeof(IHandler))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Eto.Forms.ButtonToolItem"/> class.
		/// </summary>
		/// <param name="command">Command.</param>
		/// <param name="generator">Generator.</param>
		[Obsolete("Use ButtonToolItem(Command) instead")]
		public ButtonToolItem(Command command, Generator generator = null)
			: base(command, generator, typeof(IHandler))
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
