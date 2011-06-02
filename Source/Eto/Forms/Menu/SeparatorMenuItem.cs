using System;
using System.Collections;
using Eto.Drawing;

namespace Eto.Forms
{
	public interface ISeparatorMenuItem : IMenuItem
	{
	}
	
	public class SeparatorMenuItem : MenuItem
	{
		//ISeparatorMenuItem inner;

		public SeparatorMenuItem(Generator g) : base(g, typeof(ISeparatorMenuItem))
		{
			//inner = (ISeparatorMenuItem)base.InnerControl;
		}
	}
}
