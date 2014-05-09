using System;
using System.Collections.Generic;
using System.Linq;

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
	[Handler(typeof(ITabControl))]
	public class TabControl : Container
	{
		TabPageCollection pages;
		new ITabControl Handler { get { return (ITabControl)base.Handler; } }

		public override IEnumerable<Control> Controls
		{
			get { return pages; }
		}
		
		public event EventHandler<EventArgs> SelectedIndexChanged;

		protected virtual void OnSelectedIndexChanged(EventArgs e)
		{
			if (SelectedIndexChanged != null)
				SelectedIndexChanged(this, e);
		}

		public TabControl()
		{
		}

		protected TabControl(ITabControl handler)
			: base(handler)
		{
		}

		[Obsolete("Use default constructor instead")]
		public TabControl(Generator generator) : this (generator, typeof(ITabControl))
		{
		}

		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected TabControl(Generator generator, Type type, bool initialize = true)
			: base (generator, type, initialize)
		{
		}

		[Obsolete("Use TabControl(ITabControl) instead")]
		protected TabControl(Generator generator, ITabControl handler, bool initialize = true)
			: base(generator, handler, initialize)
		{
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
			get { return pages ?? (pages = new TabPageCollection(this)); }
		}

		internal void InsertTab(int index, TabPage page)
		{
			SetParent(page, () => Handler.InsertTab(index, page));
		}

		internal void RemoveTab(int index, TabPage page)
		{
			Handler.RemoveTab(index, page);
			RemoveParent(page);
		}
		
		internal void ClearTabs()
		{
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

		static readonly object callback = new Callback();
		protected override object GetCallback() { return callback; }

		public interface ICallback : Control.ICallback
		{
			void OnSelectedIndexChanged(TabControl widget, EventArgs e);
		}

		protected class Callback : Control.Callback, ICallback
		{
			public void OnSelectedIndexChanged(TabControl widget, EventArgs e)
			{
				widget.OnSelectedIndexChanged(e);
			}
		}
	}
}
