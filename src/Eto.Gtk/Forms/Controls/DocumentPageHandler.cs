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

		static Lazy<Gdk.Pixbuf> s_closeImage = new Lazy<Gdk.Pixbuf>(LoadCloseImage);

		private static Gdk.Pixbuf LoadCloseImage()
		{
#if GTK3
			var flags = Gtk.IconLookupFlags.ForceSize;
#else
			var flags = Gtk.IconLookupFlags.UseBuiltin;
#endif
			return Gtk.IconTheme.Default.ChooseIcon(new[] { "window-close", "gtk-cancel" }, 12, flags)?.LoadIcon();
		}

		public DocumentPageHandler()
		{
			Control = new Gtk.VBox();
			tab = new Gtk.HBox();
			closeButton = new Gtk.Button();
			closeButton.Relief = Gtk.ReliefStyle.None;
			closeButton.CanFocus = false;

#if GTK3
			tab.Expand = true;
#endif
			var closeImage = s_closeImage.Value;
			if (closeImage != null)
				closeButton.Image = new Gtk.Image(closeImage);
			else
				closeButton.Child = new Gtk.Label("x");

			tab.PackEnd(closeButton, false, true, 0);
			label = new Gtk.Label();
			label.Xalign = 0.5f;
			label.Yalign = 0.5f;
			tab.PackEnd(label, true, true, 0);

			tab.ShowAll();
		}

		protected override void Initialize()
		{
			base.Initialize();

			tab.SizeAllocated += Connector.HandleTabSizeAllocated;
			closeButton.Clicked += Connector.HandleCloseButton;
			tab.ButtonPressEvent += Connector.HandleTabButtonPress;
		}

		private void HandleTabSizeAllocated(object o, Gtk.SizeAllocatedArgs args)
		{
			var imagewidth = (gtkimage != null) ? gtkimage.Allocation.Width : 0;
			var closewidth = (closeButton.Visible) ? closeButton.Allocation.Width : 0;
			var width = (float)label.Allocation.Width / (label.Allocation.Width - Math.Abs(closewidth - imagewidth));

			label.Yalign = 0.5f;
			if (imagewidth >= closewidth)
				label.Xalign = 1 - width / 2;
			else
				label.Xalign = width / 2;
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

		void HandleTabButtonPress(Gtk.ButtonPressEventArgs args)
		{
			if (args.Event.Button == 2 && Closable)
				Parent?.ClosePage(ContainerControl, Widget);
		}

		void HandleCloseButton() => Parent?.ClosePage(ContainerControl, Widget);

		protected new DocumentPageConnector Connector => (DocumentPageConnector)base.Connector;

		protected override WeakConnector CreateConnector() => new DocumentPageConnector();

		protected class DocumentPageConnector : GtkPanelEventConnector
		{
			public new DocumentPageHandler Handler => (DocumentPageHandler)base.Handler;

			internal void HandleCloseButton(object sender, EventArgs e) => Handler?.HandleCloseButton();

			internal void HandleTabButtonPress(object o, Gtk.ButtonPressEventArgs args) => Handler?.HandleTabButtonPress(args);

			internal void HandleTabSizeAllocated(object o, Gtk.SizeAllocatedArgs args) => Handler?.HandleTabSizeAllocated(o, args);
		}
	}
}
