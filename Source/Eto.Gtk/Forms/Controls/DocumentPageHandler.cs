using Eto.Forms;
using Eto.Drawing;
using Eto.GtkSharp.Drawing;
using System;

namespace Eto.GtkSharp.Forms.Controls
{
	public class DocumentPageHandler : GtkPanel<Gtk.VBox, DocumentPage, DocumentPage.ICallback>, DocumentPage.IHandler
	{
		Gtk.Label label;
		internal Gtk.Button closebutton1;
		readonly Gtk.EventBox eventBox1;
		readonly Gtk.HBox tab;
		Gtk.Image gtkimage;
		Image image;
		public static Size MaxImageSize = new Size(16, 16);

		public DocumentPageHandler()
		{
			Control = new Gtk.VBox();
			eventBox1 = new Gtk.EventBox();
			tab = new Gtk.HBox();
			closebutton1 = new Gtk.Button();
			closebutton1.Relief = Gtk.ReliefStyle.None;
			closebutton1.CanFocus = false;

#if GTK3
			tab.Expand = true;
			closebutton1.Image = new Gtk.Image(Gtk.IconTheme.Default.LoadIcon("window-close", 12, Gtk.IconLookupFlags.ForceSize));
#else
			closebutton1.Image = new Gtk.Image(Gtk.IconTheme.Default.LoadIcon("window-close", 12, 0));
#endif

			tab.PackEnd(closebutton1, false, true, 0);
			label = new Gtk.Label();
			label.SetAlignment(0.5f, 0.5f);
			tab.PackEnd(label, true, true, 0);

			eventBox1.Child = tab;
			eventBox1.ShowAll();

			tab.SizeAllocated += Tab_SizeAllocated;
		}

		private void Tab_SizeAllocated(object o, Gtk.SizeAllocatedArgs args)
		{
			var imagewidth = (gtkimage != null) ? gtkimage.Allocation.Width : 0;
			var closewidth = (closebutton1.Visible) ? closebutton1.Allocation.Width : 0;
			var width = (float)label.Allocation.Width / (label.Allocation.Width - Math.Abs(closewidth - imagewidth));

			if (imagewidth >= closewidth)
				label.SetAlignment(1 - width / 2, 0.5f);
			else
				label.SetAlignment(width / 2, 0.5f);
		}

		public Gtk.Widget LabelControl
		{
			get { return eventBox1; }
		}

		protected override void SetContainerContent(Gtk.Widget content)
		{
			Control.PackStart(content, true, true, 0);
		}

		public bool Closable
		{
			get { return closebutton1.Visible; }
			set { closebutton1.Visible = value; }
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
