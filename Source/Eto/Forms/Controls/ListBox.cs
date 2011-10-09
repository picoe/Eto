using System;
using System.Collections;
using Eto.Drawing;

namespace Eto.Forms
{
	public interface IImageListItem : IListItem
	{
		Image Image { get; }
	}
	
	public partial interface IListBox : IListControl
	{
	}
	
	public partial class ListBox : ListControl
	{
		IListBox inner;

		public event EventHandler<EventArgs> Activated;

		public virtual void OnActivated (EventArgs e)
		{
			if (Activated != null)
				Activated (this, e);
		}

		public ListBox () : this(Generator.Current)
		{
		}
		
		public ListBox (Generator g) : base(g, typeof(IListBox))
		{
			inner = (IListBox)Handler;
		}

	}

}
