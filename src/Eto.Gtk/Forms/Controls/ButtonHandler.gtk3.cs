#if GTK3
namespace Eto.GtkSharp.Forms.Controls
{
	/// <summary>
	/// Button handler.
	/// </summary>
	/// <copyright>(c) 2012-2020 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class ButtonHandler<TControl, TWidget, TCallback> : GtkControl<TControl, TWidget, TCallback>, Button.IHandler
		where TControl : Gtk.Button
		where TWidget : Button
		where TCallback : Button.ICallback
	{
		readonly Gtk.AccelLabel label;
		string _text;
		bool _alwaysShowMnemonic;

		Gtk.Image gtkimage;

		protected override Gtk.Widget FontControl => label;

		protected virtual int DefaultMinimumWidth => 0;

		public ButtonHandler()
		{
			label = new Gtk.AccelLabel(string.Empty);
			label.UseUnderline = true;
			label.Ellipsize = Pango.EllipsizeMode.End;
			label.Show();
		}

		protected override void Initialize()
		{
			base.Initialize();
			Control.Clicked += Connector.HandleClicked;
		}

		protected new ButtonConnector Connector => (ButtonConnector)base.Connector;

		protected override WeakConnector CreateConnector() => new ButtonConnector();

		protected class ButtonConnector : GtkControlConnector
		{
			new ButtonHandler<TControl, TWidget, TCallback> Handler => (ButtonHandler<TControl, TWidget, TCallback>)base.Handler;

			public virtual void HandleClicked(object sender, EventArgs e)
			{
				var h = Handler;
				if (h == null)
					return;
				h.Callback.OnClick(h.Widget, EventArgs.Empty);
			}

		}

		public override string Text
		{
			get => _text;
			set
			{
				_text = value;
				if (label.UseUnderline)
				{
					label.TextWithMnemonic = _text.ToPlatformMnemonic();
					label.Pattern = _alwaysShowMnemonic ? GtkMnemonicHelper.ToPatternWithMnemonicUnderline(_text) : null;
				}
				else
				{
					label.Pattern = null;
					label.Text = _text;
				}
				SetImagePosition();
			}
		}

		public Image Image
		{
			get { return Widget.Properties.Get<Image>(ButtonHandler.Image_Key); }
			set
			{
				if (Widget.Properties.TrySet(ButtonHandler.Image_Key, value))
				{
					if (value != null && gtkimage == null) {
						gtkimage = new Gtk.Image();
						gtkimage.Show();
					}
					value.SetGtkImage(gtkimage);
					SetImagePosition();
				};
			}
		}

		void SetImagePosition()
		{
			if (Control.Child != null)
				Control.Remove(Control.Child);
			(label.Parent as Gtk.Container)?.Remove(label);
			(gtkimage?.Parent as Gtk.Container)?.Remove(gtkimage);

			Gtk.Widget child = null;
			var showImage = Image != null;
			var showLabel = !string.IsNullOrEmpty(label.Text);
			if (showImage && showLabel)
			{
				// when hugging the text, nothing in the box expands so the box gets only its natural size,
				// which is then centered in the button.
				var expand = !ImageNextToText;
				Gtk.Box box;
				switch (ImagePosition)
				{
					case ButtonImagePosition.Above:
						child = box = new Gtk.Box(Gtk.Orientation.Vertical, 2);
						box.PackStart(gtkimage, expand, true, 0);
						box.PackEnd(label, false, true, 0);
						break;
					case ButtonImagePosition.Below:
						child = box = new Gtk.Box(Gtk.Orientation.Vertical, 2);
						box.PackStart(label, false, true, 0);
						box.PackEnd(gtkimage, expand, true, 0);
						break;
					case ButtonImagePosition.Left:
						child = box = new Gtk.Box(Gtk.Orientation.Horizontal, 2);
						box.PackStart(gtkimage, false, true, 0);
						box.PackStart(label, expand, true, 0);
						break;
					case ButtonImagePosition.Right:
						child = box = new Gtk.Box(Gtk.Orientation.Horizontal, 2);
						box.PackStart(label, expand, true, 0);
						box.PackEnd(gtkimage, false, true, 0);
						break;
					case ButtonImagePosition.Overlay:
#if GTK2
						var table = new Gtk.Table(1, 1, false);
						child = table;
						table.Attach(label, 0, 0, 1, 1, Gtk.AttachOptions.Expand, Gtk.AttachOptions.Expand, 0, 0);
						table.Attach(gtkimage, 0, 0, 1, 1, Gtk.AttachOptions.Expand, Gtk.AttachOptions.Expand, 0, 0);
#else
						var grid = new Gtk.Grid();
						child = grid;
						label.Hexpand = label.Vexpand = true;
						gtkimage.Hexpand = gtkimage.Vexpand = true;
						grid.Attach(label, 0, 0, 1, 1);
						grid.Attach(gtkimage, 0, 0, 1, 1);
#endif
						break;
					default:
						throw new NotSupportedException();
				}
				if (ImageNextToText)
				{
					child.Halign = Gtk.Align.Center;
					child.Valign = Gtk.Align.Center;
				}
			}
			else if (showLabel)
			{
				child = label;
			}
			else if (showImage)
			{
				child = gtkimage;
			}

			if (child != null)
			{
				child.Show();
				Control.Child = child;
			}

			Control.QueueResize();
		}

		public ButtonImagePosition ImagePosition
		{
			get { return Widget.Properties.Get<ButtonImagePosition>(ButtonHandler.ImagePosition_Key); }
			set
			{
				if (Widget.Properties.TrySet(ButtonHandler.ImagePosition_Key, value))
					SetImagePosition();
			}
		}

		public bool ImageNextToText
		{
			get { return Widget.Properties.Get<bool>(ButtonHandler.ImageNextToText_Key); }
			set
			{
				if (Widget.Properties.TrySet(ButtonHandler.ImageNextToText_Key, value))
					SetImagePosition();
			}
		}

		public Color TextColor
		{
			get { return label.GetForeground(); }
			set { label.SetForeground(value); }
		}
		
		public Size MinimumSize
		{
			get { return Widget.Properties.Get<Size?>(ButtonHandler.MinimumSize_Key) ?? new Size(DefaultMinimumWidth, 0); }
			set
			{
				if (MinimumSize != value)
				{
					Widget.Properties[ButtonHandler.MinimumSize_Key] = value;
					Control.QueueResize(); 
				}
			}
		}

		public bool UseMnemonic
		{
			get => label.UseUnderline;
			set
			{
				if (value == label.UseUnderline)
					return; // no change
				var text = Text;
				label.UseUnderline = value;
				Text = text;
			}
		}
		
		public bool AlwaysShowMnemonic
		{
			get => _alwaysShowMnemonic;
			set
			{
				if (_alwaysShowMnemonic == value)
					return;
				_alwaysShowMnemonic = value;
				if (_text != null)
					Text = _text;
			}
		}

		protected override void SetSize(Size size)
		{
			base.SetSize(Size.Max(size, MinimumSize));
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case TextControl.TextChangedEvent:
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}
	}
}
#endif
