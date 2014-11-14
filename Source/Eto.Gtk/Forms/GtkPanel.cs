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
		readonly Gtk.Alignment alignment;
		Control content;

		public override Gtk.Widget ContainerContentControl
		{
			get { return Control; }
		}

		protected GtkPanel()
		{
			alignment = new Gtk.Alignment(0, 0, 1, 1);
			Padding = Panel.DefaultPadding;
		}

		protected override void Initialize()
		{
			base.Initialize();
			SetContainerContent(alignment);
			#if GTK2
			ContainerContentControl.SizeRequested += Connector.HandleContentSizeRequested;
			#else
			if (MinimumSize != Size.Empty)
				ContainerContentControl.SetSizeRequest(MinimumSize.Width, MinimumSize.Height);
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

		public Size MinimumSize { get; set; }

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

		protected abstract void SetContainerContent(Gtk.Widget content);
	}
}
