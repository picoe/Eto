using System;
using System.Collections.Generic;

namespace Eto.Forms
{
	public interface INavigation : IContainer
	{
		void Push (INavigationItem item);

		void Pop ();
	}
	
	public class Navigation : Container
	{
		new INavigation Handler { get { return (INavigation)base.Handler; } }

		public override IEnumerable<Control> Controls
		{
			get
			{
				yield break;
			}
		}

		public static bool IsSupported(Generator generator = null)
		{
			return (generator ?? Generator.Current).Supports<INavigation>();
		}

		public event EventHandler<EventArgs> ItemShown;
		
		public virtual void OnItemShown (EventArgs e)
		{
			if (ItemShown != null)
				ItemShown (this, e);
		}

		public Navigation()
			: this((Generator)null)
		{
		}

		public Navigation (Generator generator)
			: base(generator, typeof(INavigation))
		{
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
		
		public void Push (Control content, string title = null)
		{
			Push (new NavigationItem { Content = content, Text = title });
		}
		
		public void Push (INavigationItem item)
		{
			var load = SetParent(item.Content);

			Handler.Push (item);
			if (load)
				item.Content.OnLoadComplete (EventArgs.Empty);
		}
		
		public virtual void Pop ()
		{
			Handler.Pop ();
		}

		public override void Remove(Control child)
		{
			//throw new NotImplementedException();
		}
	}
}

