using System;
using Eto.Forms;
using Eto.Drawing;
using System.Runtime.InteropServices;
using GLib;

namespace Eto.GtkSharp.Forms.Controls
{
	public class TextBoxHandler : TextBoxHandler<Gtk.Entry, TextBox, TextBox.ICallback>
	{
		public TextBoxHandler()
		{
			Control = new Gtk.Entry();
			Control.WidthRequest = 100;
		}

	}

	public class TextBoxHandler<TControl, TWidget, TCallback> : GtkControl<TControl, TWidget, TCallback>, TextBox.IHandler
		where TControl: Gtk.Entry
		where TWidget: TextBox
		where TCallback: TextBox.ICallback
	{
		string placeholderText;

		protected override void Initialize()
		{
			base.Initialize();
			Control.ActivatesDefault = true;
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case TextControl.TextChangedEvent:
					Control.Changed += Connector.HandleTextChanged;
					break;
				case TextBox.TextChangingEvent:
					Control.ClipboardPasted += Connector.HandleClipboardPasted;
					Control.TextDeleted += Connector.HandleTextDeleted;
					Widget.TextInput += Connector.HandleTextInput;
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
			public new TextBoxHandler<TControl, TWidget, TCallback> Handler { get { return (TextBoxHandler<TControl, TWidget, TCallback>)base.Handler; } }

			public void HandleTextChanged(object sender, System.EventArgs e)
			{
				Handler.Callback.OnTextChanged(Handler.Widget, System.EventArgs.Empty);
			}

			static Clipboard clipboard;

			static Clipboard Clipboard
			{
				get { return clipboard ?? (clipboard = new Clipboard()); }
			}

			public void HandleTextInput(object sender, TextInputEventArgs e)
			{
				if (!e.Cancel)
				{
					var tia = new TextChangingEventArgs(e.Text, Handler.Selection);
					Handler.Callback.OnTextChanging(Handler.Widget, tia);
					e.Cancel = tia.Cancel;
				}
			}

			[GLib.ConnectBefore]
			public void HandleClipboardPasted(object sender, System.EventArgs e)
			{
				var tia = new TextChangingEventArgs(Clipboard.Text, Handler.Selection);
				Handler.Callback.OnTextChanging(Handler.Widget, tia);
				if (tia.Cancel)
					NativeMethods.g_signal_stop_emission_by_name(Handler.Control.Handle, "paste-clipboard");
			}

			bool deleting;

			[GLib.ConnectBefore]
			public void HandleTextDeleted(object o, Gtk.TextDeletedArgs args)
			{
				if (!deleting)
				{
					deleting = true;
					if (args.StartPos < args.EndPos)
					{
						var tia = new TextChangingEventArgs(string.Empty, new Range<int>(args.StartPos, Math.Min(args.EndPos - 1, Handler.Control.Text.Length - 1)));
						Handler.Callback.OnTextChanging(Handler.Widget, tia);
						if (tia.Cancel)
							NativeMethods.g_signal_stop_emission_by_name(Handler.Control.Handle, "delete-text");
					}
					deleting = false;
				}
			}

			#if GTK2

			public virtual void HandleExposeEvent(object o, Gtk.ExposeEventArgs args)
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

		public override Size Size
		{
			get { return base.Size; }
			set
			{
				Control.WidthChars = (value.Width == -1) ? -1 : 0;
				base.Size = value;
			}
		}

		public override string Text
		{
			get { return Control.Text; }
			set { Control.Text = value ?? string.Empty; }
		}

		public virtual bool ReadOnly
		{
			get { return !Control.IsEditable; }
			set { Control.IsEditable = !value; }
		}

		public int MaxLength
		{
			get { return Control.MaxLength == -1 ? 0 : Control.MaxLength; }
			set { Control.MaxLength = value == 0 ? -1 : value; }
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
				if (Widget.Loaded)
					Invalidate(false);
#else
				placeholderText = value;

				NativeMethods.gtk_entry_set_placeholder_text(Control.Handle, value);
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
			get { return Control.GetTextColor(); }
			set { Control.SetTextColor(value); }
		}

		public override Color BackgroundColor
		{
			get { return Control.GetBackground(); }
			set
			{ 
				Control.SetBackground(value); 
				Control.SetBase(value); 
			}
		}

		public int CaretIndex
		{
			get
			{ 
				int start, end;
				Control.GetSelectionBounds(out start, out end);
				return Math.Min(start, end);
			}
			set
			{
				Control.SelectRegion(value, value);
			}
		}

		public Range<int> Selection
		{
			get
			{
				int start, end;
				Control.GetSelectionBounds(out start, out end);
				return new Range<int>(Math.Min(start, end), Math.Max(start, end) - 1);
			}
			set
			{
				Control.SelectRegion(value.Start, value.End + 1);
			}
		}

		public TextAlignment TextAlignment
		{
			get
			{
				return Control.Alignment < 0.5f ? TextAlignment.Left
						  : Control.Alignment > 0.5f ? TextAlignment.Right
						  : TextAlignment.Center;
			}
			set
			{
				switch (value)
				{
					case TextAlignment.Left:
						Control.Alignment = 0;
						break;
					case TextAlignment.Center:
						Control.Alignment = 0.5f;
						break;
					case TextAlignment.Right:
						Control.Alignment = 1;
						break;
					default:
						throw new NotSupportedException();
				}

			}
		}

		public override bool ShowBorder
		{
			get { return Control.HasFrame; }
			set { Control.HasFrame = value; }
		}
	}
}
