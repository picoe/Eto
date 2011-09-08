using System;
using System.Collections;
using Eto.Drawing;

namespace Eto.Forms
{
	public interface IImageListItem : IListItem
	{
		Image Image { get; }
	}
		
	
	public interface IListBox : IListControl
	{
		ContextMenu ContextMenu { get; set; }
	}
	
	public class ListBox : ListControl
	{
		public event EventHandler<EventArgs> Activated;
		
		private IListBox inner;

		public ListBox() : this(Generator.Current)
		{
			
		}
		
		public ListBox(Generator g) : base(g, typeof(IListBox))
		{
			inner = (IListBox)Handler;
		}
		
		public ContextMenu ContextMenu
		{
			get { return inner.ContextMenu; }
			set { inner.ContextMenu = value; }
		}

		public virtual void OnActivated(EventArgs e)
		{
			if (Activated != null) Activated(this, e);
		}
	}

}
