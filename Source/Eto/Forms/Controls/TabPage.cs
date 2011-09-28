using System;
using System.Collections;
using Eto.Collections;
using Eto.Drawing;

namespace Eto.Forms
{
	public interface ITabPage : IContainer
	{
		string Text { get; set; }

		Image Image { get; set; }
	}

	public class TabPage : Container, IImageListItem
	{
		ITabPage inner;
		
		public TabPage () : this(Generator.Current)
		{
		}
		
		public TabPage (Generator g) : base(g, typeof(ITabPage))
		{
			inner = (ITabPage)Handler;
		}
		
		public event EventHandler<EventArgs> Click;

		protected void OnClick (EventArgs e)
		{
			if (Click != null)
				Click (this, e);
		}
		
		public string Text {
			get { return inner.Text; }
			set { inner.Text = value; }
		}

		public Image Image {
			get { return inner.Image; }
			set { inner.Image = value; }
		}
		
		public virtual string Key {
			get;
			set;
		}
	}

	public class TabPageCollection : BaseList<TabPage>
	{
		TabControl control;

		public TabPageCollection (TabControl control)
		{
			this.control = control;
		}

		protected override void OnAdded (ListEventArgs<TabPage> e)
		{
			base.OnAdded (e);
			e.Item.SetParent (control);
			control.AddTab (e.Item);
		}
		
		protected override void OnRemoved (ListEventArgs<TabPage> e)
		{
			base.OnRemoved (e);
			e.Item.SetParent (null);
			control.RemoveTab (e.Item);
		}
	}
}
