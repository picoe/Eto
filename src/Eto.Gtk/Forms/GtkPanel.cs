using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.GtkSharp.Forms
{
	static class GtkPanel
	{
		public static readonly object ContextMenu_Key = new object();
		public static readonly object MinimumSize_Key = new object();
	}

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
		}

		protected virtual bool UseMinimumSizeRequested { get { return true; } }

		protected override void Initialize()
		{
			base.Initialize();
			SetContainerContent(alignment);

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
			public new GtkPanel<TControl, TWidget, TCallback> Handler => (GtkPanel<TControl, TWidget, TCallback>)base.Handler;
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


		public ContextMenu ContextMenu
		{
			get => Widget.Properties.Get<ContextMenu>(GtkPanel.ContextMenu_Key);
			set => Widget.Properties.Set(GtkPanel.ContextMenu_Key, value); // TODO
		}

		public virtual Size MinimumSize
		{
			get => Widget.Properties.Get<Size>(GtkPanel.MinimumSize_Key);
			set
			{
				Widget.Properties.Set(GtkPanel.MinimumSize_Key, value);
				ContainerControl.QueueResize();
				SetSize(UserPreferredSize);
			}
		}

		protected override void SetSize(Size size)
		{
			var min = MinimumSize;
			if (min.Width > 0)
				size.Width = Math.Max(size.Width, min.Width);
			if (min.Height > 0)
				size.Height = Math.Max(size.Height, min.Height);

			base.SetSize(size);
		}

		public virtual Padding Padding
		{
			get => new Padding((int)alignment.LeftPadding, (int)alignment.TopPadding, (int)alignment.RightPadding, (int)alignment.BottomPadding);
			set
			{
				alignment.LeftPadding = (uint)value.Left;
				alignment.RightPadding = (uint)value.Right;
				alignment.TopPadding = (uint)value.Top;
				alignment.BottomPadding = (uint)value.Bottom;
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
						widget.ShowAll();
						alignment.Child = widget;
					}
				}
			}
		}

		public override Gtk.Widget BackgroundControl => Control;

		protected abstract void SetContainerContent(Gtk.Widget content);
	}
}
