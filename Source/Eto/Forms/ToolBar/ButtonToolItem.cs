using System;

namespace Eto.Forms
{
	public interface IButtonToolItem : IToolItem
	{
	}
	
	public class ButtonToolItem : ToolItem
	{
		new IButtonToolItem Handler { get { return (IButtonToolItem)base.Handler; } }

		public ButtonToolItem()
			: this((Generator)null)
		{
		}

		public ButtonToolItem(Generator generator) : base(generator, typeof(IButtonToolItem))
		{
		}

		public ButtonToolItem(Command command, Generator generator = null)
			: base(command, generator, typeof(IButtonToolItem))
		{
			Handler.CreateFromCommand(command);
		}
	}
}
