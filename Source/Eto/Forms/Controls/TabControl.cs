using System;
using System.Collections;
using System.Linq;

#if XAML
using System.Windows.Markup;
#endif

namespace Eto.Forms
{
	public interface ITabControl : IControl
	{
		int SelectedIndex { get; set; }

		void InsertTab(int index, TabPage page);
		
		void ClearTabs();

		void RemoveTab(int index, TabPage page);
	}
	
	[ContentProperty("TabPages")]
	public class TabControl : Control
	{
		TabPageCollection pages;
		ITabControl handler;
		
		public event EventHandler<EventArgs> SelectedIndexChanged;

		public virtual void OnSelectedIndexChanged(EventArgs e)
		{
			if (SelectedIndexChanged != null)
				SelectedIndexChanged(this, e);
		}
		
		public TabControl() : this (Generator.Current)
		{
		}

		public TabControl(Generator g) : this (g, typeof(ITabControl))
		{
		}
		
		protected TabControl(Generator generator, Type type, bool initialize = true)
			: base (generator, type, initialize)
		{
			pages = new TabPageCollection(this);
			handler = (ITabControl)base.Handler;
		}

		public int SelectedIndex
		{
			get { return handler.SelectedIndex; }
			set { handler.SelectedIndex = value; }
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
			if (Loaded)
			{
				page.OnPreLoad(EventArgs.Empty);
				page.OnLoad(EventArgs.Empty);
				page.OnLoadComplete(EventArgs.Empty);
			}
			page.SetParent(this);
			handler.InsertTab(index, page);
		}

		internal void RemoveTab(int index, TabPage page)
		{
			handler.RemoveTab(index, page);
			page.SetParent(null);
		}
		
		internal void ClearTabs()
		{
			handler.ClearTabs();
		}

		public override void OnPreLoad(EventArgs e)
		{
			base.OnPreLoad(e);
			foreach (var page in pages)
			{
				page.OnPreLoad(e);
			}
		}
		
		public override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			foreach (var page in pages)
			{
				page.OnLoad(e);
			}
		}
		
		public override void OnLoadComplete(EventArgs e)
		{
			base.OnLoadComplete(e);
			foreach (var page in pages)
			{
				page.OnLoadComplete(e);
			}
		}

		public override void OnUnLoad(EventArgs e)
		{
			base.OnUnLoad(e);
			foreach (var page in pages)
			{
				page.OnUnLoad(e);
			}
		}
		
		internal protected override void OnDataContextChanged(EventArgs e)
		{
			base.OnDataContextChanged(e);
			foreach (var tab in TabPages)
			{
				tab.OnDataContextChanged(e);
			}
		}

		public override void UpdateBindings()
		{
			base.UpdateBindings();
			foreach (var tab in TabPages)
			{
				tab.UpdateBindings();
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
