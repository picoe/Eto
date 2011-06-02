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
		ICheckMenuItem inner;

		public CheckMenuItem() : this(Generator.Current)
		{
		}
		
		public CheckMenuItem(Generator g) : base(g, typeof(ICheckMenuItem))
		{
			inner = (ICheckMenuItem)base.Handler;
		}


		public bool Checked
		{
			get { return inner.Checked; }
			set { inner.Checked = value; }
		}
	}
}
