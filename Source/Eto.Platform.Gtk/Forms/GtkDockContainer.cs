using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Platform.GtkSharp
{
	public abstract class GtkDockContainer<T, W> : GtkContainer<T, W>, IContainer
		where T: Gtk.Widget
		where W: Container
	{
		Gtk.Alignment alignment;
		Control content;

		public override Gtk.Widget ContainerContentControl
		{
			get { return Control; }
		}

		public GtkDockContainer()
		{
			alignment = new Gtk.Alignment(0, 0, 1, 1);
			this.Padding = DockContainer.DefaultPadding;
		}

		protected override void Initialize()
		{
			base.Initialize();
			SetContainerContent(alignment);
		}

		bool loaded;

		public override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (!loaded)
			{
				ContainerContentControl.SizeRequested += delegate(object o, Gtk.SizeRequestedArgs args)
				{
					var alloc = args.Requisition;
					if (MinimumSize.Width > 0)
						alloc.Width = Math.Max(alloc.Width, MinimumSize.Width);
					if (MinimumSize.Height > 0)
						alloc.Height = Math.Max(alloc.Height, MinimumSize.Height);
					args.Requisition = alloc;
				};
				loaded = true;
			}
		}

		public Size MinimumSize { get; set; }

		public Eto.Drawing.Padding Padding
		{
			get
			{
				uint top, left, right, bottom;
				alignment.GetPadding(out top, out bottom, out left, out right);
				return new Eto.Drawing.Padding((int)left, (int)top, (int)right, (int)bottom);
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
					foreach (var child in alignment.Children)
						alignment.Remove(child);
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
