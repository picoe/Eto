using System;
using System.Collections;
using Eto.Drawing;
using System.Collections.ObjectModel;

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

		public void OnClick (EventArgs e)
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

	public class TabPageCollection : Collection<TabPage>
	{
		TabControl control;

		internal TabPageCollection (TabControl control)
		{
			this.control = control;
		}
		
		protected override void InsertItem (int index, TabPage item)
		{
			base.InsertItem (index, item);
			control.InsertTab (index, item);
		}

		protected override void ClearItems ()
		{
			base.ClearItems ();
			control.ClearTabs ();
		}
		
		protected override void RemoveItem (int index)
		{
			control.RemoveTab (index, this[index]);
			base.RemoveItem (index);
		}
	}
}
