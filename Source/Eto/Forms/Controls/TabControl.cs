using System;
using System.Collections;

namespace Eto.Forms
{
	public interface ITabControl : IControl
	{
		int SelectedIndex { get; set; }
		void AddTab(TabPage page);
		void RemoveTab(TabPage page);
	}
	
	public class TabControl : Control
	{
		private TabPageCollection tabPages;
		private ITabControl inner;
		
		public event EventHandler<EventArgs> SelectedIndexChanged;

		public virtual void OnSelectedIndexChanged(EventArgs e)
		{
			if (SelectedIndexChanged != null) SelectedIndexChanged(this, e);
		}

		public TabControl(Generator g) : base(g, typeof(ITabControl))
		{
			tabPages = new TabPageCollection(this);
			inner = (ITabControl)base.Handler;
		}

		public int SelectedIndex
		{
			get { return inner.SelectedIndex; }
			set { inner.SelectedIndex = value; }
		}


		public TabPageCollection TabPages
		{
			get { return tabPages; }
		}

		protected internal void AddTab(TabPage page)
		{
			inner.AddTab(page);
		}

		protected internal void RemoveTab(TabPage page)
		{
			inner.RemoveTab(page);
		}

	}
}
