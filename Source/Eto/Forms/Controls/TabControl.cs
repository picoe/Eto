using System;
using System.Collections.Generic;
using System.Linq;

#if XAML
using System.Windows.Markup;
#endif

namespace Eto.Forms
{
	public interface ITabControl : IContainer
	{
		int SelectedIndex { get; set; }

		void InsertTab(int index, TabPage page);
		
		void ClearTabs();

		void RemoveTab(int index, TabPage page);
	}

	public class TabRemovingEventArgs : EventArgs
	{
		public TabPage Page { get; private set; }

		public TabRemovingEventArgs(TabPage page)
		{
			this.Page = page;
		}
	}

	[ContentProperty("TabPages")]
	public class TabControl : Container
	{
		readonly TabPageCollection pages;
		new ITabControl Handler { get { return (ITabControl)base.Handler; } }

		public override IEnumerable<Control> Controls
		{
			get { return pages; }
		}
		
		public event EventHandler<EventArgs> SelectedIndexChanged;

		public virtual void OnSelectedIndexChanged(EventArgs e)
		{
			if (SelectedIndexChanged != null)
				SelectedIndexChanged(this, e);
		}

		public event EventHandler<TabRemovingEventArgs> TabRemoving;

		public virtual void OnTabRemoving(TabRemovingEventArgs e)
		{
			if (TabRemoving != null)
				TabRemoving(this, e);
		}
		
		public TabControl()
			: this((Generator)null)
		{
		}

		public TabControl(Generator generator) : this (generator, typeof(ITabControl))
		{
		}
		
		protected TabControl(Generator generator, Type type, bool initialize = true)
			: base (generator, type, initialize)
		{
			pages = new TabPageCollection(this);
		}

		protected TabControl(Generator generator, ITabControl handler, bool initialize = true)
			: base(generator, handler, initialize)
		{
			pages = new TabPageCollection(this);
		}

		public int SelectedIndex
		{
			get { return Handler.SelectedIndex; }
			set { Handler.SelectedIndex = value; }
		}
		
		public TabPage SelectedPage
		{
			get { return SelectedIndex < 0 ? null : TabPages[SelectedIndex]; }
			set { SelectedIndex = pages.IndexOf(value); }
		}

		public TabPageCollection TabPages
		{
			get { return pages; }
		}

		internal void InsertTab(int index, TabPage page)
		{
			var load = SetParent(page);
			Handler.InsertTab(index, page);
			if (load)
				page.OnLoadComplete(EventArgs.Empty);
		}

		internal void RemoveTab(int index, TabPage page)
		{
			OnTabRemoving(new TabRemovingEventArgs(page));
			Handler.RemoveTab(index, page);
			RemoveParent(page);
		}
		
		internal void ClearTabs()
		{
			Handler.ClearTabs();
			foreach (var page in TabPages.ToArray())
				Remove(page);
		}

		public override void Remove(Control child)
		{
			Remove(child as TabPage);
		}

		private void Remove(TabPage page)
		{
			if (page != null)
			{
				var index = pages.IndexOf(page);
				if (index >= 0)
					RemoveTab(index, page);
			}
		}

		public ObjectBinding<TabControl, int> SelectedIndexBinding
		{
			get
			{
				return new ObjectBinding<TabControl, int>(
					this, 
					c => c.SelectedIndex, 
					(c, v) => c.SelectedIndex = v, 
					(c, h) => c.SelectedIndexChanged += h, 
					(c, h) => c.SelectedIndexChanged -= h
				);
			}
		}
	}
}
