#if GTK2
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
		readonly Gtk.Image gtkimage;
		readonly Gtk.Table table;

		protected override Gtk.Widget FontControl => label;

		protected virtual int DefaultMinimumWidth => 0;

		public ButtonHandler()
		{
			// need separate widgets as the theme can (and usually) disables images on buttons
			// gtk3 can override the theme per button, but gtk2 cannot
			table = new Gtk.Table(3, 3, false);
			table.ColumnSpacing = 0;
			table.RowSpacing = 0;
			label = new Gtk.AccelLabel(string.Empty);
			label.NoShowAll = true;
			table.Attach(label, 1, 2, 1, 2, Gtk.AttachOptions.Expand, Gtk.AttachOptions.Expand, 0, 0);
			gtkimage = new Gtk.Image();
			gtkimage.NoShowAll = true;
		}

		protected override void Initialize()
		{
			base.Initialize();
			Control.Child = table;
			Control.Clicked += Connector.HandleClicked;
#if GTK2
			Control.SizeAllocated += Connector.HandleButtonSizeAllocated;
			Control.SizeRequested += Connector.HandleButtonSizeRequested;
#endif

			SetImagePosition(false);
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

#if GTK2
			public void HandleButtonSizeAllocated(object o, Gtk.SizeAllocatedArgs args)
			{
				var handler = Handler;
				if (handler != null)
				{
					var size = args.Allocation;
					//if (handler.PreferredSize.Width == -1 && (size.Width > 1 || size.Height > 1))
					{
						var minSize = handler.MinimumSize;
						size.Width = Math.Max(size.Width, minSize.Width);
						size.Height = Math.Max(size.Height, minSize.Height);
						if (args.Allocation != size)
						{
							var c = (Gtk.Button)o;
							c.SetSizeRequest(size.Width, size.Height);
						}
						handler.SetImage();
					}
				}
			}

			public void HandleButtonSizeRequested(object o, Gtk.SizeRequestedArgs args)
			{
				var handler = Handler;
				if (handler != null)
				{
					var size = args.Requisition;
					//if (handler.PreferredSize.Width == -1 && (size.Width > 1 || size.Height > 1))
					{
						var minSize = handler.MinimumSize;
						size.Width = Math.Max(size.Width, minSize.Width);
						size.Height = Math.Max(size.Height, minSize.Height);
						args.Requisition = size;
					}
				}
			}
#endif
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
					Image.SetGtkImage(gtkimage);
					if (value == null)
						gtkimage.Hide();
					else
						gtkimage.Show();
					Control.QueueResize();
				};
			}
		}

		Size? lastImageSize;
		void SetImage()
		{
			var icon = Image as Icon;
			if (icon != null)
			{
				var size = table.Allocation.Size.ToEto();
				var iconSize = icon.Size;
				var maxScale = Math.Min((double)size.Width / iconSize.Width, (double)size.Height/ iconSize.Height);
				size = new Size((int)Math.Ceiling(iconSize.Width * maxScale), (int)Math.Ceiling(iconSize.Height * maxScale));
				if (lastImageSize != size)
				{
					var frame = icon.GetFrame(1, size); // get frame that matches the size best
					var iconHandler = (Drawing.BitmapHandler)frame.Bitmap.Handler;
					gtkimage.Pixbuf = iconHandler.GetPixbuf(size, shrink: true);
					lastImageSize = size;
				}
			}
		}

		void SetImagePosition(bool removeImage = true)
		{
			uint left, top;
			bool shouldHideLabel = false;

			switch (ImagePosition)
			{
				case ButtonImagePosition.Above:
					left = 1;
					top = 0;
					shouldHideLabel = true;
					break;
				case ButtonImagePosition.Below:
					left = 1;
					top = 2;
					shouldHideLabel = true;
					break;
				case ButtonImagePosition.Left:
					left = 0;
					top = 1;
					break;
				case ButtonImagePosition.Right:
					left = 2;
					top = 1;
					break;
				case ButtonImagePosition.Overlay:
					left = 1;
					top = 1;
					break;
				default:
					throw new NotSupportedException();
			}
			shouldHideLabel &= string.IsNullOrEmpty(label.Text);
			if (shouldHideLabel)
				label.Hide();
			else
				label.Show();

			var right = left + 1;
			var bottom = top + 1;
			var options = shouldHideLabel ? Gtk.AttachOptions.Expand : Gtk.AttachOptions.Shrink;
			if (removeImage)
				table.Remove(gtkimage);
			table.Attach(gtkimage, left, right, top, bottom, options, options, 0, 0);
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