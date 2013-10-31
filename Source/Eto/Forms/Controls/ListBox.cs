using System;
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
		new IListBox Handler { get { return (IListBox)base.Handler; } }

		public event EventHandler<EventArgs> Activated;

		public virtual void OnActivated (EventArgs e)
		{
			if (Activated != null)
				Activated (this, e);
		}

		public ListBox () : this (Generator.Current)
		{
		}
		
		public ListBox (Generator g) : this (g, typeof(IListBox))
		{
		}
		
		protected ListBox (Generator generator, Type type, bool initialize = true)
			: base (generator, type, initialize)
		{
		}
	}

}
