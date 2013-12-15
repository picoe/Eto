#if DESKTOP
using System;

namespace Eto.Forms
{
	public interface IRadioMenuItem : IMenuItem
	{
		void Create (RadioMenuItem controller);

		bool Checked { get; set; }
	}
	
	public class RadioMenuItem : MenuItem
	{
		new IRadioMenuItem Handler { get { return (IRadioMenuItem)base.Handler; } }

		public RadioMenuItem()
			: this(null, null)
		{
		}

		public RadioMenuItem (RadioMenuItem controller = null, Generator generator = null)
			: this (generator, typeof(IRadioMenuItem), controller)
		{
		}

		protected RadioMenuItem (Generator generator, Type type, RadioMenuItem controller, bool initialize = true)
			: base (generator, type, false)
		{
			Handler.Create (controller);
			if (initialize)
				Initialize ();
		}

		public bool Checked {
			get { return Handler.Checked; }
			set { Handler.Checked = value; }
		}
	}
}
#endif