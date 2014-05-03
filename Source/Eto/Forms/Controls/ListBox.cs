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

	[Handler(typeof(IListBox))]
	public partial class ListBox : ListControl
	{
		new IListBox Handler { get { return (IListBox)base.Handler; } }

		public event EventHandler<EventArgs> Activated;

		public virtual void OnActivated (EventArgs e)
		{
			if (Activated != null)
				Activated (this, e);
		}

		public ListBox()
		{
		}

		[Obsolete("Use default constructor instead")]
		public ListBox (Generator generator) : this (generator, typeof(IListBox))
		{
		}

		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected ListBox (Generator generator, Type type, bool initialize = true)
			: base (generator, type, initialize)
		{
		}
	}

}
