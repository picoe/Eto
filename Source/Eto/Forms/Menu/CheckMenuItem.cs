using System;

namespace Eto.Forms
{
	[Handler(typeof(CheckMenuItem.IHandler))]
	public class CheckMenuItem : MenuItem
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

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
			: this(generator, typeof(CheckMenuItem.IHandler))
		{
		}

		[Obsolete("Use constructor without generator instead")]
		public CheckMenuItem(CheckCommand command, Generator generator = null)
			: base(command, generator, typeof(CheckMenuItem.IHandler))
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

		public new interface IHandler : MenuItem.IHandler
		{
			bool Checked { get; set; }
		}
	}
}