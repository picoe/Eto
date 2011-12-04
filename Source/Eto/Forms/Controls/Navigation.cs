using System;

namespace Eto.Forms
{
	public interface INavigation : IControl
	{
		void Push (INavigationItem item);

		void Pop ();
	}
	
	public class Navigation : Control
	{
		INavigation inner;
		
		public event EventHandler<EventArgs> ItemShown;
		
		public virtual void OnItemShown (EventArgs e)
		{
			if (ItemShown != null)
				ItemShown (this, e);
		}

		public Navigation ()
			: this(Generator.Current)
		{
		}
		
		public Navigation (Generator g)
			: base(g, typeof(INavigation))
		{
			inner = (INavigation)Handler;
		}
		
		public Navigation (Control content, string title = null)
			: this()
		{
			Push (content, title);
		}
		
		public Navigation (NavigationItem item)
			: this()
		{
			Push (item);
		}
		
		public virtual void Push (Control content, string title = null)
		{
			Push (new NavigationItem { Content = content, Text = title });
		}
		
		public virtual void Push (INavigationItem item)
		{
			inner.Push (item);
		}
		
		public virtual void Pop ()
		{
			inner.Pop ();
		}
	}
}

