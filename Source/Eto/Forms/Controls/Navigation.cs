using System;
using System.Collections.Generic;

namespace Eto.Forms
{
	public interface INavigation : IControl
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

		[Obsolete("Use IsSupported() instead")]
		public static bool Supported { get { return IsSupported(); } }

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

		public Navigation ()
			: this(Generator.Current)
		{
		}
		
		public Navigation (Generator g)
			: base(g, typeof(INavigation))
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
		
		public virtual void Push (Control content, string title = null)
		{
			Push (new NavigationItem { Content = content, Text = title });
		}
		
		public virtual void Push (INavigationItem item)
		{
			var loaded = item.Content.Loaded;
			SetParent(item.Content);
			if (!loaded) {
				item.Content.OnPreLoad (EventArgs.Empty);
				item.Content.OnLoad (EventArgs.Empty);
			}

			Handler.Push (item);
			if (!loaded)
				item.Content.OnLoadComplete (EventArgs.Empty);
		}
		
		public virtual void Pop ()
		{
			Handler.Pop ();
		}

		public override void Remove(Control child)
		{
			throw new NotImplementedException();
		}
	}
}

