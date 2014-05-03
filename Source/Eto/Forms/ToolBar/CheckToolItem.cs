using System;

namespace Eto.Forms
{
	public interface ICheckToolItem : IToolItem
	{
		bool Checked { get; set; }
	}

	[Handler(typeof(ICheckToolItem))]
	public class CheckToolItem : ToolItem
	{
		new ICheckToolItem Handler { get { return (ICheckToolItem)base.Handler; } }

		public event EventHandler<EventArgs> CheckedChanged;

		public CheckToolItem()
		{
		}

		public CheckToolItem(CheckCommand command)
			: base(command)
		{
			Checked = command.Checked;
			command.CheckedChanged += (sender, e) => Checked = command.Checked;
			Click += (sender, e) => command.Checked = Checked;
			Handler.CreateFromCommand(command);
		}

		[Obsolete("Use default constructor instead")]
		public CheckToolItem(Generator generator)
			: base(generator, typeof(ICheckToolItem))
		{
		}

		[Obsolete("Use CheckToolItem(CheckCommand) instead")]
		public CheckToolItem(CheckCommand command, Generator generator = null)
			: base(command, generator, typeof(ICheckToolItem))
		{
			Checked = command.Checked;
			command.CheckedChanged += (sender, e) => Checked = command.Checked;
			Click += (sender, e) => command.Checked = Checked;
			Handler.CreateFromCommand(command);
		}

		public bool Checked
		{
			get { return Handler.Checked; }
			set { Handler.Checked = value; }
		}

		public void OnCheckedChanged(EventArgs e)
		{
			if (CheckedChanged != null)
				CheckedChanged(this, e);
		}
	}
}
