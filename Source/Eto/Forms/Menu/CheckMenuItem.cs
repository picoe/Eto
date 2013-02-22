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
		ICheckMenuItem handler;

		public CheckMenuItem () : this (Generator.Current)
		{
		}
		
		public CheckMenuItem (Generator g) : this (g, typeof(ICheckMenuItem))
		{
		}

		protected CheckMenuItem (Generator generator, Type type, bool initialize = true)
			: base (generator, type, initialize)
		{
			handler = (ICheckMenuItem)base.Handler;
		}

		public bool Checked {
			get { return handler.Checked; }
			set { handler.Checked = value; }
		}
	}
}
#endif