using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.GtkSharp.Forms
{
	public abstract class GtkPanel<TControl, TWidget, TCallback> : GtkContainer<TControl, TWidget, TCallback>
		where TControl: Gtk.Widget
		where TWidget: Panel
		where TCallback: Panel.ICallback
	{
		Gtk.VBox vboxPanel;
		readonly Gtk.Alignment alignment;
		Control content;
		Gtk.Box contentHolder;
		Gtk.Box toolbarHolder;
		Eto.Forms.ToolBar toolBar;

		public override Gtk.Widget ContainerContentControl
		{
			get { return alignment; }
		}

		protected GtkPanel()
		{
			vboxPanel = new Gtk.VBox();

			alignment = new Gtk.Alignment(0, 0, 1, 1);

			contentHolder = new Gtk.VBox();
			contentHolder.Visible = true;
			contentHolder.PackStart(alignment, false, false, 0);

			toolbarHolder = new Gtk.VBox();
			vboxPanel.PackStart(toolbarHolder, false, false, 0);
			vboxPanel.PackStart(contentHolder, true, true, 0);

		}

		protected virtual bool UseMinimumSizeRequested { get { return true; } }

		protected override void Initialize()
		{
			base.Initialize();

			SetContainerContent(vboxPanel);

			#if GTK2
			if (UseMinimumSizeRequested)
				ContainerControl.SizeRequested += Connector.HandleContentSizeRequested;
			#endif
		}

		protected new GtkPanelEventConnector Connector { get { return (GtkPanelEventConnector)base.Connector; } }

		protected override WeakConnector CreateConnector()
		{
			return new GtkPanelEventConnector();
		}

		protected class GtkPanelEventConnector : GtkControlConnector
		{
			public new GtkPanel<TControl, TWidget, TCallback> Handler { get { return (GtkPanel<TControl, TWidget, TCallback>)base.Handler; } }
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

		ContextMenu contextMenu;

		public ContextMenu ContextMenu
		{
			get { return contextMenu; }
			set { contextMenu = value; } // TODO
		}

		Size minimumSize;
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
			get
			{
				uint top, left, right, bottom;
				alignment.GetPadding(out top, out bottom, out left, out right);
				return new Padding((int)left, (int)top, (int)right, (int)bottom);
			}
			set
			{
				alignment.SetPadding((uint)value.Top, (uint)value.Bottom, (uint)value.Left, (uint)value.Right);
			}
		}

		public Control Content
		{
			get { return content; }
			set
			{
				if (content != value)
				{
					if (content != null)
						alignment.Remove(content.GetContainerWidget());
					content = value;
					var widget = content.GetContainerWidget();
					if (widget != null)
					{
						if (widget.Parent != null)
							((Gtk.Container)widget.Parent).Remove(widget);
						alignment.Child = widget;
						widget.ShowAll();
					}
				}
			}
		}

		public Eto.Forms.ToolBar ToolBar
		{
			get
			{
				return toolBar;
			}
			set
			{
				if (toolBar != null)
					toolbarHolder.Remove((Gtk.Widget)toolBar.ControlObject);
				toolBar = value;
				if (toolBar != null)
					toolbarHolder.Add((Gtk.Widget)toolBar.ControlObject);
				toolbarHolder.ShowAll();
			}
		}

		protected abstract void SetContainerContent(Gtk.Widget content);
	}
}
