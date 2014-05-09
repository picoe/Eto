using System;

namespace Eto.Forms
{
	[Handler(typeof(RadioMenuItem.IHandler))]
	public class RadioMenuItem : MenuItem
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		public RadioMenuItem()
		{
			Handler.Create(null);
		}

		public RadioMenuItem(RadioMenuItem controller)
		{
			Handler.Create(controller);
		}

		public RadioMenuItem(RadioCommand command, RadioMenuItem controller = null)
			: base(command)
		{
			Checked = command.Checked;
			Click += (sender, e) => command.Checked = Checked;
			command.CheckedChanged += (sender, e) => Checked = command.Checked;
			Handler.Create(controller);
			Handler.CreateFromCommand(command);
		}

		[Obsolete("Use constructor without generator instead")]
		public RadioMenuItem(RadioMenuItem controller, Generator generator = null)
			: this(generator, typeof(RadioMenuItem.IHandler), controller)
		{
		}

		[Obsolete("Use constructor without generator instead")]
		public RadioMenuItem(RadioCommand command, RadioMenuItem controller, Generator generator = null)
			: base(command, generator, typeof(RadioMenuItem.IHandler), false)
		{
			Checked = command.Checked;
			Click += (sender, e) => command.Checked = Checked;
			command.CheckedChanged += (sender, e) => Checked = command.Checked;
			Handler.Create(controller);
			Initialize();
			Handler.CreateFromCommand(command);
		}

		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected RadioMenuItem(Generator generator, Type type, RadioMenuItem controller, bool initialize = true)
			: base(generator, type, false)
		{
			Handler.Create(controller);
			if (initialize)
				Initialize();
		}

		public bool Checked
		{
			get { return Handler.Checked; }
			set { Handler.Checked = value; }
		}

		public interface IHandler : MenuItem.IHandler
		{
			void Create(RadioMenuItem controller);

			bool Checked { get; set; }
		}
	}
}