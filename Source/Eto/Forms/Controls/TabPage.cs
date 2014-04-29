using System;
using Eto.Drawing;
using System.Collections.ObjectModel;

namespace Eto.Forms
{
	public interface ITabPage : IPanel
	{
		string Text { get; set; }

		Image Image { get; set; }
	}

	public class TabPage : Panel, IImageListItem
	{
		new ITabPage Handler { get { return (ITabPage)base.Handler; } }
		
		public TabPage (Control control, Padding? padding = null)
			: this (control.Platform)
		{
			if (padding != null)
				this.Padding = padding.Value;
			this.Content = control;
		}

		public TabPage ()
			: this ((Generator)null)
		{
		}
		
		public TabPage (Generator generator)
			: this (generator, typeof(ITabPage))
		{
		}
		
		protected TabPage (Generator generator, Type type, bool initialize = true)
			: base (generator, type, initialize)
		{
		}
		
		public event EventHandler<EventArgs> Click;

		public void OnClick (EventArgs e)
		{
			if (Click != null)
				Click (this, e);
		}
		
		public string Text {
			get { return Handler.Text; }
			set { Handler.Text = value; }
		}

		public Image Image {
			get { return Handler.Image; }
			set { Handler.Image = value; }
		}
		
		public virtual string Key { get; set; }
	}

	public class TabPageCollection : Collection<TabPage>
	{
		readonly TabControl control;

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
			control.RemoveTab (index, this [index]);
			base.RemoveItem (index);
		}
	}
}
