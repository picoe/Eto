using System;
using System.Collections;
using System.Linq;

#if DESKTOP
using System.Windows.Markup;
#endif

namespace Eto.Forms
{
	public interface ITabControl : IControl
	{
		int SelectedIndex { get; set; }

		void InsertTab (int index, TabPage page);
		
		void ClearTabs ();

		void RemoveTab (int index, TabPage page);
	}
	
#if DESKTOP
	[ContentProperty("TabPages")]
#endif
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
		
		public TabPage SelectedPage {
			get { return SelectedIndex < 0 ? null : TabPages [SelectedIndex]; }
			set { SelectedIndex = pages.IndexOf (value); }
		}

		public TabPageCollection TabPages {
			get { return pages; }
		}

		internal void InsertTab (int index, TabPage page)
		{
			if (Loaded) {
				page.OnPreLoad (EventArgs.Empty);
				page.OnLoad (EventArgs.Empty);
				page.OnLoadComplete (EventArgs.Empty);
			}
			page.SetParent (this);
			inner.InsertTab (index, page);
		}

		internal void RemoveTab (int index, TabPage page)
		{
			page.SetParent (null);
			inner.RemoveTab (index, page);
		}
		
		internal void ClearTabs ()
		{
			inner.ClearTabs ();
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
