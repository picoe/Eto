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

		public sealed override Gtk.Widget ContainerControl
		{
			get { return alignment; }
		}

		public override Gtk.Widget ContainerContentControl
		{
			get { return Control; }
		}

		public virtual Size ClientSize
		{
			get { return this.Size; }
			set
			{
				this.Size = value;
			}
		}

		public GtkDockContainer()
		{
			alignment = new Gtk.Alignment(0, 0, 1, 1);
			this.Padding = DockLayout.DefaultPadding;
		}

		protected override void Initialize()
		{
			base.Initialize();
			alignment.Child = ContainerContentControl;
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
					content = value;
					SetContent(content);
				}
			}
		}

		protected abstract void SetContent(Control content);
	}
}
