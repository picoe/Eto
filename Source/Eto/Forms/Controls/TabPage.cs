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
		ITabPage handler;
		
		public TabPage () : this (Generator.Current)
		{
		}
		
		public TabPage (Generator g) : this (g, typeof(ITabPage))
		{
		}
		
		protected TabPage (Generator generator, Type type, bool initialize = true)
			: base (generator, type, initialize)
		{
			handler = (ITabPage)Handler;
		}
		
		public event EventHandler<EventArgs> Click;

		public void OnClick (EventArgs e)
		{
			if (Click != null)
				Click (this, e);
		}
		
		public string Text {
			get { return handler.Text; }
			set { handler.Text = value; }
		}

		public Image Image {
			get { return handler.Image; }
			set { handler.Image = value; }
		}
		
		public virtual string Key { get; set; }
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
			control.RemoveTab (index, this [index]);
			base.RemoveItem (index);
		}
	}
}
