using System;
using Eto.Drawing;

namespace Eto.Forms
{
	public interface IImageListItem : IListItem
	{
		Image Image { get; }
	}
	
	[Handler(typeof(ListBox.IHandler))]
	public class ListBox : ListControl
	{
		new IHandler Handler { get { return (IHandler)base.Handler; } }

		public event EventHandler<EventArgs> Activated;

		protected virtual void OnActivated(EventArgs e)
		{
			if (Activated != null)
				Activated(this, e);
		}

		public ListBox()
		{
		}

		[Obsolete("Use default constructor instead")]
		public ListBox (Generator generator) : this (generator, typeof(IHandler))
		{
		}

		[Obsolete("Use default constructor and HandlerAttribute instead")]
		protected ListBox (Generator generator, Type type, bool initialize = true)
			: base (generator, type, initialize)
		{
		}

		public ContextMenu ContextMenu
		{
			get { return Handler.ContextMenu; }
			set { Handler.ContextMenu = value; }
		}

		static readonly object callback = new Callback();
		protected override object GetCallback() { return callback; }

		public interface ICallback : ListControl.ICallback
		{
			void OnActivated(ListBox widget, EventArgs e);
		}

		protected class Callback : ListControl.Callback, ICallback
		{
			public void OnActivated(ListBox widget, EventArgs e)
			{
				widget.OnActivated(e);
			}
		}

		public interface IHandler : ListControl.IHandler, IContextMenuHost
		{
		}
	}
}
