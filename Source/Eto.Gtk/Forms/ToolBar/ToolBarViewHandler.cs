using System;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.GtkSharp.Forms.ToolBar
{
	public class ToolBarViewHandler : GtkControl<Gtk.Toolbar, ToolBarView, ToolBarView.ICallback>, ToolBarView.IHandler
	{
		Control content;
		ContextMenu contextMenu;
		DockPosition dock = DockPosition.None;
		Size minimumSize;
		
		public ToolBarViewHandler()
		{
			Control = new Gtk.Toolbar();
		}

		protected override void Initialize()
		{
			base.Initialize();

#if GTK2
			if (UseMinimumSizeRequested)
				Control.SizeRequested += Connector.HandleContentSizeRequested;
#endif
		}

		public virtual Size ClientSize
		{
			get { return Size; }
			set { Size = value; }
		}

		protected new ToolBarViewEventConnector Connector
		{
			get { return (ToolBarViewEventConnector)base.Connector; }
		}

		public Control Content
		{
			get { return content; }
			set
			{
				if (content != value)
				{
					if (content != null)
						Control = null;
					content = value;
					var widget = content.GetContainerWidget();
					if (widget != null)
					{
						Control = widget as Gtk.Toolbar;
					}
				}
			}
		}

		public ContextMenu ContextMenu
		{
			get { return contextMenu; }
			set { contextMenu = value; } // TODO
		}

		protected override WeakConnector CreateConnector()
		{
			return new ToolBarViewEventConnector();
		}

		protected class ToolBarViewEventConnector : GtkControlConnector
		{
			public new ToolBarViewHandler Handler
			{
				get { return (ToolBarViewHandler)base.Handler; }
			}
#if GTK2
			public void HandleContentSizeRequested(object o, Gtk.SizeRequestedArgs args)
			{
				var handler = Handler;
				if (handler != null)
				{
					var alloc = args.Requisition;
					var minimumSize = handler.MinimumSize;
					if (minimumSize.Width > 0)
						alloc.Width = Math.Max(alloc.Width, minimumSize.Width);
					if (minimumSize.Height > 0)
						alloc.Height = Math.Max(alloc.Height, minimumSize.Height);
					args.Requisition = alloc;
				}
			}
#endif
		}

		public DockPosition Dock
		{
			get { return dock; }
			set { dock = value; }
		}

		public virtual Size MinimumSize
		{
			get { return minimumSize; }
			set
			{
				minimumSize = value;
#if GTK3
				ContainerControl.SetSizeRequest(value.Width > 0 ? value.Width : -1, value.Height > 0 ? value.Height : -1);
#endif
			}
		}

		public Padding Padding
		{
			get;
			set;
		}

		public bool RecurseToChildren
		{
			get { return true; }
		}

		protected virtual bool UseMinimumSizeRequested
		{
			get { return true; }
		}

	}
}
