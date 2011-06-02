using System;
using System.Collections;

namespace Eto.Forms
{
	public interface IMenuBar
	{

	}
	
	public class MenuBar : Menu
	{
		//IMenuBar inner;

		public MenuBar(Generator g) : base(g, typeof(IMenuBar))
		{
			//BindingContext = new BindingContext();
			//inner = (IMenuBar)base.InnerControl;
		}

		public MenuBar(Generator g, ActionItemCollection actionItems) : this(g)
		{
			GenerateActions(actionItems);
		}


	}
}
