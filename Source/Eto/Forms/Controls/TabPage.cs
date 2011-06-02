using System;
using System.Collections;
using Eto.Collections;

namespace Eto.Forms
{
	public interface ITabPage : IContainer
	{

	}

	public class TabPage : Container
	{
		public TabPage() : this(Generator.Current) {}
		
		public TabPage(Generator g) : base(g, typeof(ITabPage))
		{
		}
		
		public event EventHandler<EventArgs> Click;

		protected void OnClick(EventArgs e)
		{
			if (Click != null) Click(this, e);
		}

	}

	public class TabPageCollection : BaseList<TabPage>
	{
		TabControl control;

		public TabPageCollection(TabControl control)
		{
			this.control = control;
		}

		protected override void OnAdded (ListEventArgs<TabPage> e)
		{
			base.OnAdded (e);
			e.Item.SetParent(control);
			control.AddTab(e.Item);
		}
		
		protected override void OnRemoved (ListEventArgs<TabPage> e)
		{
			base.OnRemoved (e);
			e.Item.SetParent(null);
			control.RemoveTab(e.Item);
		}
	}
}
