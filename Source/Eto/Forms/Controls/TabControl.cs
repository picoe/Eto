using System;
using System.Collections;
using System.Linq;

namespace Eto.Forms
{
	public interface ITabControl : IControl
	{
		int SelectedIndex { get; set; }

		void AddTab (TabPage page);

		void RemoveTab (TabPage page);
	}
	
	public class TabControl : Control
	{
		TabPageCollection pages;
		ITabControl inner;
		
		public event EventHandler<EventArgs> SelectedIndexChanged;

		public virtual void OnSelectedIndexChanged (EventArgs e)
		{
			if (SelectedIndexChanged != null)
				SelectedIndexChanged (this, e);
		}
		
		public TabControl ()
			: this(Generator.Current)
		{
		}

		public TabControl (Generator g) : base(g, typeof(ITabControl))
		{
			pages = new TabPageCollection (this);
			inner = (ITabControl)base.Handler;
		}

		public int SelectedIndex {
			get { return inner.SelectedIndex; }
			set { inner.SelectedIndex = value; }
		}

		public TabPageCollection TabPages {
			get { return pages; }
		}

		protected internal void AddTab (TabPage page)
		{
			if (Loaded) {
				page.OnPreLoad (EventArgs.Empty);
				page.OnLoad (EventArgs.Empty);
				page.OnLoadComplete (EventArgs.Empty);
			}
			page.SetParent (this);
			inner.AddTab (page);
		}

		protected internal void RemoveTab (TabPage page)
		{
			page.SetParent (null);
			inner.RemoveTab (page);
		}

		public override void OnPreLoad (EventArgs e)
		{
			base.OnPreLoad (e);
			foreach (var page in pages) {
				page.OnPreLoad (e);
			}
		}
		
		public override void OnLoad (EventArgs e)
		{
			base.OnLoad (e);
			foreach (var page in pages) {
				page.OnLoad (e);
			}
		}
		
		public override void OnLoadComplete (EventArgs e)
		{
			base.OnLoadComplete (e);
			foreach (var page in pages) {
				page.OnLoadComplete (e);
			}
		}

	}
}
