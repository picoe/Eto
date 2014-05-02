using System;

namespace Eto.Forms
{
	public interface IRadioMenuItem : IMenuItem
	{
		void Create(RadioMenuItem controller);

		bool Checked { get; set; }
	}

	public class RadioMenuItem : MenuItem
	{
		new IRadioMenuItem Handler { get { return (IRadioMenuItem)base.Handler; } }

		public RadioMenuItem()
			: this(null, typeof(IRadioMenuItem), null)
		{
		}

		public RadioMenuItem(RadioMenuItem controller, Generator generator = null)
			: this(generator, typeof(IRadioMenuItem), controller)
		{
		}

		public RadioMenuItem(RadioCommand command, RadioMenuItem controller, Generator generator = null)
			: base(command, generator, typeof(IRadioMenuItem), false)
		{
			Checked = command.Checked;
			Click += (sender, e) => command.Checked = Checked;
			command.CheckedChanged += (sender, e) => Checked = command.Checked;
			Handler.Create(controller);
			Initialize();
			Handler.CreateFromCommand(command);
		}

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
	}
}