#if GTK3
using System;
using Eto.Forms;
using Eto.Drawing;

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

		Gtk.Image gtkimage;

		protected override Gtk.Widget FontControl => label;

		protected virtual int DefaultMinimumWidth => 0;

		public ButtonHandler()
		{
			label = new Gtk.AccelLabel(string.Empty);
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
			get { return label.Text.ToEtoMnemonic(); }
			set
			{
				label.TextWithMnemonic = value.ToPlatformMnemonic();
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
				Gtk.VBox vbox;
				Gtk.HBox hbox;
				switch (ImagePosition)
				{
					case ButtonImagePosition.Above:
						child = vbox = new Gtk.VBox(false, 2);
						vbox.PackStart(gtkimage, true, true, 0);
						vbox.PackEnd(label, false, true, 0);
						break;
					case ButtonImagePosition.Below:
						child = vbox = new Gtk.VBox(false, 2);
						vbox.PackStart(label, false, true, 0);
						vbox.PackEnd(gtkimage, true, true, 0);
						break;
					case ButtonImagePosition.Left:
						child = hbox = new Gtk.HBox(false, 2);
						hbox.PackStart(gtkimage, false, true, 0);
						hbox.PackStart(label, true, true, 0);
						break;
					case ButtonImagePosition.Right:
						child = hbox = new Gtk.HBox(false, 2);
						hbox.PackStart(label, true, true, 0);
						hbox.PackEnd(gtkimage, false, true, 0);
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