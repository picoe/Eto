using System;

namespace Eto.Forms
{
	public interface IButtonToolItem : IToolItem
	{
	}

	[Handler(typeof(IButtonToolItem))]
	public class ButtonToolItem : ToolItem
	{
		new IButtonToolItem Handler { get { return (IButtonToolItem)base.Handler; } }

		public ButtonToolItem()
		{
		}

		public ButtonToolItem(Command command)
			: base(command)
		{
			Handler.CreateFromCommand(command);
		}

		[Obsolete("Use default constructor instead")]
		public ButtonToolItem(Generator generator) : base(generator, typeof(IButtonToolItem))
		{
		}

		[Obsolete("Use ButtonToolItem(Command) instead")]
		public ButtonToolItem(Command command, Generator generator = null)
			: base(command, generator, typeof(IButtonToolItem))
		{
			Handler.CreateFromCommand(command);
		}
	}
}
