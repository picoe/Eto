using System;

namespace Eto.Forms
{
	public interface ICheckMenuItem : IMenuItem
	{
		bool Checked { get; set; }
	}

	[Handler(typeof(ICheckMenuItem))]
	public class CheckMenuItem : MenuItem
	{
		new ICheckMenuItem Handler { get { return (ICheckMenuItem)base.Handler; } }

		public CheckMenuItem()
		{
		}

		public CheckMenuItem(CheckCommand command)
			: base(command)
		{
			Checked = command.Checked;
			command.CheckedChanged += (sender, e) => Checked = command.Checked;
			Click += (sender, e) => command.Checked = Checked;
			Handler.CreateFromCommand(command);
		}

		[Obsolete("Use default constructor instead")]
		public CheckMenuItem(Generator generator)
			: this(generator, typeof(ICheckMenuItem))
		{
		}

		[Obsolete("Use constructor without generator instead")]
		public CheckMenuItem(CheckCommand command, Generator generator = null)
			: base(command, generator, typeof(ICheckMenuItem))
		{
			Checked = command.Checked;
			command.CheckedChanged += (sender, e) => Checked = command.Checked;
			Click += (sender, e) => command.Checked = Checked;
		}

		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected CheckMenuItem(Generator generator, Type type, bool initialize = true)
			: base(generator, type, initialize)
		{
		}

		public bool Checked
		{
			get { return Handler.Checked; }
			set { Handler.Checked = value; }
		}
	}
}