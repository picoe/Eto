using Eto.GtkSharp.Drawing;

namespace Eto.GtkSharp.Forms.Controls
{
	public class TabPageHandler : GtkPanel<Gtk.Box, TabPage, TabPage.ICallback>, TabPage.IHandler
	{
		Gtk.Label label;
		readonly Gtk.Box tab;
		Gtk.Image gtkimage;
		Image image;
		public static Size MaxImageSize = new Size(16, 16);

		public TabPageHandler()
		{
			Control = new Gtk.Box(Gtk.Orientation.Vertical, 0);
			tab = new Gtk.Box(Gtk.Orientation.Horizontal, 0);
			label = new Gtk.Label();
			tab.PackEnd(label, true, true, 0);
			tab.ShowAll();
		}

		public Gtk.Widget LabelControl
		{
			get { return tab; }
		}

		protected override void SetContainerContent(Gtk.Widget content)
		{
			Control.PackStart(content, true, true, 0);
		}

		public Image Image
		{
			get { return image; }
			set
			{
				if (gtkimage == null)
				{
					gtkimage = new Gtk.Image();
					tab.PackStart(gtkimage, true, true, 0);
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
