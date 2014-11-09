using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.GtkSharp.Forms.Controls
{
	public class TextBoxHandler : GtkControl<Gtk.Entry, TextBox, TextBox.ICallback>, TextBox.IHandler
	{
		string placeholderText;

		public TextBoxHandler()
		{
			Control = new Gtk.Entry();
			Control.SetSizeRequest(100, -1);
			Control.ActivatesDefault = true;
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case TextControl.TextChangedEvent:
					Control.Changed += Connector.HandleTextChanged;
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		protected new TextBoxConnector Connector { get { return (TextBoxConnector)base.Connector; } }

		protected override WeakConnector CreateConnector()
		{
			return new TextBoxConnector();
		}

		protected class TextBoxConnector : GtkControlConnector
		{
			public new TextBoxHandler Handler { get { return (TextBoxHandler)base.Handler; } }

			public void HandleTextChanged(object sender, EventArgs e)
			{
				Handler.Callback.OnTextChanged(Handler.Widget, EventArgs.Empty);
			}

			#if GTK2

			public void HandleExposeEvent(object o, Gtk.ExposeEventArgs args)
			{
				var control = Handler.Control;
				if (!string.IsNullOrEmpty(control.Text) || args.Event.Window == control.GdkWindow)
					return;

				if (Handler.placeholderLayout == null)
				{
					Handler.placeholderLayout = new Pango.Layout(control.PangoContext);
					Handler.placeholderLayout.FontDescription = control.PangoContext.FontDescription.Copy();
				}
				Handler.placeholderLayout.SetText(Handler.placeholderText);

				int currentHeight, currentWidth;
				args.Event.Window.GetSize(out currentWidth, out currentHeight);

				int width, height;
				Handler.placeholderLayout.GetPixelSize(out width, out height);

				var style = control.Style;
				var bc = style.Base(Gtk.StateType.Normal);
				var tc = style.Text(Gtk.StateType.Normal);

				using (var gc = new Gdk.GC(args.Event.Window))
				{
					gc.Copy(style.TextGC(Gtk.StateType.Normal));

					gc.RgbFgColor = new Gdk.Color((byte)(((int)bc.Red + tc.Red) / 2 / 256), (byte)(((int)bc.Green + (int)tc.Green) / 2 / 256), (byte)((bc.Blue + tc.Blue) / 2 / 256));

					args.Event.Window.DrawLayout(gc, 2, (currentHeight - height) / 2 + 1, Handler.placeholderLayout);
				}
			}
			#endif
		}
		#if GTK2
		Pango.Layout placeholderLayout;
		public override Eto.Drawing.Font Font
		{
			get { return base.Font; }
			set
			{
				base.Font = value;
				placeholderLayout = null;
			}
		}
		#else
		protected override void SetBackgroundColor(Eto.Drawing.Color? color)
		{
		}
		#endif

		public override string Text
		{
			get { return Control.Text; }
			set { Control.Text = value ?? string.Empty; }
		}

		public bool ReadOnly
		{
			get { return !Control.IsEditable; }
			set { Control.IsEditable = !value; }
		}

		public int MaxLength
		{
			get { return Control.MaxLength; }
			set { Control.MaxLength = value; }
		}

		public string PlaceholderText
		{
			get { return placeholderText; }
			set
			{
#if GTK2
				if (!string.IsNullOrEmpty(placeholderText))
					Control.ExposeEvent -= Connector.HandleExposeEvent;
				placeholderText = value;
				if (!string.IsNullOrEmpty(placeholderText))
					Control.ExposeEvent += Connector.HandleExposeEvent;
#else
				placeholderText = value;
#endif
			}
		}

		public void SelectAll()
		{
			Control.GrabFocus();
			if (!string.IsNullOrEmpty(Control.Text))
				Control.SelectRegion(0, Control.Text.Length);
		}

		public Color TextColor
		{
			get { return Control.Style.Text(Gtk.StateType.Normal).ToEto(); }
			set { Control.ModifyText(Gtk.StateType.Normal, value.ToGdk()); }
		}

		public override Color BackgroundColor
		{
			get { return Control.Style.Base(Gtk.StateType.Normal).ToEto(); }
			set { Control.ModifyBase(Gtk.StateType.Normal, value.ToGdk()); }
		}
	}
}
