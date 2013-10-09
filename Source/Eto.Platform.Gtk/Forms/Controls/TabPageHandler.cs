using System;
using Eto.Forms;
using Eto.Drawing;
using Eto.Platform.GtkSharp.Drawing;

namespace Eto.Platform.GtkSharp
{
	public class TabPageHandler : GtkDockContainer<Gtk.VBox, TabPage>, ITabPage
	{
		Gtk.Label label;
		Gtk.HBox tab;
		Gtk.Image gtkimage;
		Image image;
		
		public static Size MaxImageSize = new Size(16, 16);

		public TabPageHandler()
		{
			Control = new Gtk.VBox();
			tab = new Gtk.HBox();
			label = new Gtk.Label();
			tab.PackEnd (label, true, true, 0);
			tab.ShowAll ();
		}
		
		public Gtk.Widget LabelControl {
			get { return tab; }
		}

		protected override void SetContainerContent(Gtk.Widget content)
		{
			Control.PackStart(content, true, true, 0);
		}
		
		public Eto.Drawing.Image Image {
			get { return image; }
			set {
				if (gtkimage == null) {
					gtkimage = new Gtk.Image();
					tab.PackStart (gtkimage, true, true, 0);
				}
				image = value;
				if (image != null) {
					var imagehandler = image.Handler as IGtkPixbuf;
					gtkimage.Pixbuf = imagehandler.GetPixbuf (MaxImageSize);
					gtkimage.ShowAll ();
				}
				else {
					gtkimage.Visible = false;
					gtkimage.Pixbuf = null;
				}
				
			}
		}

		public override string Text
		{
			get { return MnuemonicToString (label.Text); }
			set { label.TextWithMnemonic = StringToMnuemonic (value); }
		}

		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);
			if (disposing) {
				if (label != null) { label.Dispose(); label = null; }
			}
		}
	}
}
