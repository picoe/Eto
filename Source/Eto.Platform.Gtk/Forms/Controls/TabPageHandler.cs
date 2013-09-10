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
			tab.PackEnd (label);
			tab.ShowAll ();
		}
		
		public Gtk.Widget LabelControl {
			get { return tab; }
		}

		protected override void SetContent(Eto.Forms.Control content)
		{
			if (Control.Children.Length > 0)
				foreach (Gtk.Widget child in Control.Children)
					Control.Remove(child);
			var containerWidget = content.GetContainerWidget();
			if (containerWidget != null)
			{
				if (containerWidget.Parent != null)
					containerWidget.Reparent(Control);
				Control.PackStart(containerWidget);
				containerWidget.ShowAll();
			}
		}
		
		public Eto.Drawing.Image Image {
			get { return image; }
			set {
				if (gtkimage == null) {
					gtkimage = new Gtk.Image();
					tab.PackStart (gtkimage);
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
