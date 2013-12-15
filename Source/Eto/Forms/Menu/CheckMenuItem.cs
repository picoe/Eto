using System;

namespace Eto.Forms
{
	public interface ICheckMenuItem : IMenuItem
	{
		bool Checked { get; set; }
	}
	
	public class CheckMenuItem : MenuItem
	{
		new ICheckMenuItem Handler { get { return (ICheckMenuItem)base.Handler; } }

		public CheckMenuItem()
			: this((Generator)null)
		{
		}

		public CheckMenuItem (Generator generator) : this (generator, typeof(ICheckMenuItem))
		{
		}

		protected CheckMenuItem (Generator generator, Type type, bool initialize = true)
			: base (generator, type, initialize)
		{
		}

		public bool Checked {
			get { return Handler.Checked; }
			set { Handler.Checked = value; }
		}
	}
}