using Eto.Forms;
using Eto.Drawing;
using Eto.GtkSharp.Drawing;
using System;

namespace Eto.GtkSharp.Forms.Controls
{
	public class DocumentPageHandler : GtkPanel<Gtk.VBox, DocumentPage, DocumentPage.ICallback>, DocumentPage.IHandler
	{
		Gtk.Label label;

		internal Gtk.Button closeButton;
		readonly Gtk.HBox tab;
		Gtk.Image gtkimage;
		Image image;
		public static Size MaxImageSize = new Size(16, 16);

		DocumentControlHandler Parent => Widget.LogicalParent?.Handler as DocumentControlHandler;

		public DocumentPageHandler()
		{
			Control = new Gtk.VBox();
			tab = new Gtk.HBox();
			closeButton = new Gtk.Button();
			closeButton.Relief = Gtk.ReliefStyle.None;
			closeButton.CanFocus = false;

#if GTK3
			tab.Expand = true;
			closeButton.Image = new Gtk.Image(Gtk.IconTheme.Default.LoadIcon("window-close", 12, Gtk.IconLookupFlags.ForceSize));
#else
			closeButton.Image = new Gtk.Image(Gtk.IconTheme.Default.LoadIcon("window-close", 12, 0));
#endif

			tab.PackEnd(closeButton, false, true, 0);
			label = new Gtk.Label();
			label.SetAlignment(0.5f, 0.5f);
			tab.PackEnd(label, true, true, 0);

			tab.SizeAllocated += Tab_SizeAllocated;
			tab.ShowAll();

			closeButton.Clicked += (o, args) => Parent?.ClosePage(ContainerControl, Widget);
			tab.ButtonPressEvent += (o, args) =>
			{
				if (args.Event.Button == 2 && Closable)
					Parent?.ClosePage(ContainerControl, Widget);
			};
		}

		private void Tab_SizeAllocated(object o, Gtk.SizeAllocatedArgs args)
		{
			var imagewidth = (gtkimage != null) ? gtkimage.Allocation.Width : 0;
			var closewidth = (closeButton.Visible) ? closeButton.Allocation.Width : 0;
			var width = (float)label.Allocation.Width / (label.Allocation.Width - Math.Abs(closewidth - imagewidth));

			if (imagewidth >= closewidth)
				label.SetAlignment(1 - width / 2, 0.5f);
			else
				label.SetAlignment(width / 2, 0.5f);
		}

		public Gtk.Widget LabelControl
		{
			get { return tab; }
		}

		protected override void SetContainerContent(Gtk.Widget content)
		{
			Control.PackStart(content, true, true, 0);
		}

		public bool Closable
		{
			get { return closeButton.Visible; }
			set { closeButton.Visible = value; }
		}

		public Image Image
		{
			get { return image; }
			set
			{
				if (gtkimage == null)
				{
					gtkimage = new Gtk.Image();
					tab.PackStart(gtkimage, false, true, 0);
				}
				image = value;
				if (image != null)
				{
					var imagehandler = (IGtkPixbuf)image.Handler;
					gtkimage.Pixbuf = imagehandler.GetPixbuf(MaxImageSize);
					gtkimage.ShowAll();
				}
				else
				{
					gtkimage.Visible = false;
					gtkimage.Pixbuf = null;
				}

			}
		}

		public override string Text
		{
			get { return label.Text.ToEtoMnemonic(); }
			set { label.TextWithMnemonic = value.ToPlatformMnemonic(); }
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				if (label != null)
				{
					label.Dispose();
					label = null;
				}
			}
		}
	}
}
