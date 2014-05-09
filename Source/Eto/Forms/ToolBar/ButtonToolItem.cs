using System;

namespace Eto.Forms
{
	[Handler(typeof(ButtonToolItem.IHandler))]
	public class ButtonToolItem : ToolItem
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		public ButtonToolItem()
		{
		}

		public ButtonToolItem(Command command)
			: base(command)
		{
			Handler.CreateFromCommand(command);
		}

		[Obsolete("Use default constructor instead")]
		public ButtonToolItem(Generator generator) : base(generator, typeof(IHandler))
		{
		}

		[Obsolete("Use ButtonToolItem(Command) instead")]
		public ButtonToolItem(Command command, Generator generator = null)
			: base(command, generator, typeof(IHandler))
		{
			Handler.CreateFromCommand(command);
		}

		public interface IHandler : ToolItem.IHandler
		{
		}
	}
}
