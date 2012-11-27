using System;
using Eto.Forms;
using System.Runtime.InteropServices;
using GLib;

namespace Eto.Platform.GtkSharp
{
	public class TextBoxHandler : GtkControl<Gtk.Entry, TextBox>, ITextBox
	{
		Pango.Layout placeholderLayout;
		string placeholderText;

		public TextBoxHandler ()
		{
			Control = new Gtk.Entry ();
			Control.SetSizeRequest (100, -1);
			Control.ActivatesDefault = true;
		}

		public override void AttachEvent (string handler)
		{
			switch (handler) {
				case TextBox.TextChangedEvent:
					Control.Changed += delegate {
						Widget.OnTextChanged (EventArgs.Empty);
					};
					break;
				default:
					base.AttachEvent (handler);
					break;
			}
		}

		void HandleExposeEvent (object o, Gtk.ExposeEventArgs args)
		{
			if (!string.IsNullOrEmpty(Control.Text) || args.Event.Window == Control.GdkWindow)
				return;

			if (placeholderLayout == null) {
				placeholderLayout = new Pango.Layout (Control.PangoContext);
				placeholderLayout.FontDescription = Control.PangoContext.FontDescription.Copy ();
			}
			placeholderLayout.SetText (placeholderText);

			int currentHeight, currentWidth;
			args.Event.Window.GetSize (out currentWidth, out currentHeight);

			int width, height;
			placeholderLayout.GetPixelSize (out width, out height);

			var style = Control.Style;
			var bc = style.Base (Gtk.StateType.Normal);
			var tc = style.Text (Gtk.StateType.Normal);

			using (var gc = new Gdk.GC (args.Event.Window)) {
				gc.Copy (style.TextGC (Gtk.StateType.Normal));

				gc.RgbFgColor = new Gdk.Color ((byte)(((int)bc.Red + tc.Red) / 2 / 256), (byte)(((int)bc.Green + (int)tc.Green) / 2 / 256), (byte)((bc.Blue + tc.Blue) / 2 / 256));

				args.Event.Window.DrawLayout (gc, 2, (currentHeight - height) / 2 + 1, placeholderLayout);
			}
		}

		public override Eto.Drawing.Font Font
		{
			get { return base.Font; }
			set {
				base.Font = value;
				placeholderLayout = null;
			}
		}

		public override string Text {
			get { return Control.Text; }
			set { Control.Text = value ?? string.Empty; }
		}

		public bool ReadOnly {
			get { return !Control.IsEditable; }
			set { Control.IsEditable = !value; }
		}
		
		public int MaxLength {
			get { return Control.MaxLength; }
			set { Control.MaxLength = value; }
		}

		public string PlaceholderText {
			get { return placeholderText; }
			set	{
				if (!string.IsNullOrEmpty(placeholderText))
					Control.ExposeEvent -= HandleExposeEvent;
				placeholderText = value;
				if (!string.IsNullOrEmpty (placeholderText))
					Control.ExposeEvent += HandleExposeEvent;
			}
		}

		public void SelectAll()
		{
			Control.GrabFocus ();
			if (!string.IsNullOrEmpty(Control.Text))
				Control.SelectRegion(0, Control.Text.Length);
		}
	}
}
