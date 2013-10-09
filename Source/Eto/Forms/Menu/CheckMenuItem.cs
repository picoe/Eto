#if DESKTOP
using System;
using System.Collections;
using Eto.Drawing;

namespace Eto.Forms
{
	public interface ICheckMenuItem : IMenuActionItem
	{
		bool Checked { get; set; }
	}
	
	public class CheckMenuItem : MenuActionItem
	{
		new ICheckMenuItem Handler { get { return (ICheckMenuItem)base.Handler; } }

		public CheckMenuItem () : this (Generator.Current)
		{
		}
		
		public CheckMenuItem (Generator g) : this (g, typeof(ICheckMenuItem))
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
#endif