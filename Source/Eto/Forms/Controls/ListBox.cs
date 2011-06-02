using System;
using System.Collections;

namespace Eto.Forms
{
	public interface IListBox : IListControl
	{
	}
	
	public class ListBox : ListControl
	{
		public event EventHandler<EventArgs> Activated;
		
		//private IListBox inner;

		public ListBox() : this(Generator.Current)
		{
			
		}
		
		public ListBox(Generator g) : base(g, typeof(IListBox))
		{
			//inner = (IListBox)base.InnerControl;
		}

		public virtual void OnActivated(EventArgs e)
		{
			if (Activated != null) Activated(this, e);
		}
	}

}
