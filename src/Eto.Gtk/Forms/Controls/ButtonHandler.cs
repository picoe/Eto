using System;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.GtkSharp.Forms.Controls
{
	/// <summary>
	/// Button handler.
	/// </summary>
	/// <copyright>(c) 2012-2013 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class ButtonHandler : GtkControl<Gtk.Button, Button, Button.ICallback>, Button.IHandler
	{
		readonly Gtk.AccelLabel label;
		readonly Gtk.Image gtkimage;
		readonly Gtk.Table table;

		public static int MinimumWidth = 80;

		protected override Gtk.Widget FontControl
		{
			get { return label; }
		}

		public ButtonHandler()
		{
			Control = new Gtk.Button();
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
			Control.Child = table;
		}

		protected override void Initialize()
		{
			base.Initialize();
			Control.Clicked += Connector.HandleClicked;
			Control.SizeAllocated += Connector.HandleButtonSizeAllocated;
#if GTK2
			Control.SizeRequested += Connector.HandleButtonSizeRequested;
#else
			Control.WidthRequest = MinimumWidth;
#endif

			SetImagePosition(false);
		}

		protected new ButtonConnector Connector { get { return (ButtonConnector)base.Connector; } }

		protected override WeakConnector CreateConnector()
		{
			return new ButtonConnector();
		}

		protected class ButtonConnector : GtkControlConnector
		{
			public new ButtonHandler Handler { get { return (ButtonHandler)base.Handler; } }

			public void HandleClicked(object sender, EventArgs e)
			{
				Handler.Callback.OnClick(Handler.Widget, EventArgs.Empty);
			}

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

			#if GTK2
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

		static readonly object Image_Key = new object();

		public Image Image
		{
			get { return Widget.Properties.Get<Image>(Image_Key); }
			set
			{
				Widget.Properties.Set(Image_Key, value, () =>
				{
					Image.SetGtkImage(gtkimage);
					if (value == null)
						gtkimage.Hide();
					else
						gtkimage.Show();
				});
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

		}

		static readonly object ImagePosition_Key = new object();

		public ButtonImagePosition ImagePosition
		{
			get { return Widget.Properties.Get<ButtonImagePosition>(ImagePosition_Key); }
			set { Widget.Properties.Set(ImagePosition_Key, value, () => SetImagePosition()); }
		}

		public Color TextColor
		{
			get { return label.GetForeground(); }
			set { label.SetForeground(value); }
		}

		static readonly object MinimumSize_Key = new object();

		public Size MinimumSize
		{
			get { return Widget.Properties.Get(MinimumSize_Key, () => new Size(MinimumWidth, 0)); }
			set
			{
				if (MinimumSize != value)
				{
					Widget.Properties[MinimumSize_Key] = value;
					Control.QueueResize(); 
				}
			}
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
