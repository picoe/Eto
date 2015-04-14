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
		Image image;
		readonly Gtk.AccelLabel label;
		readonly Gtk.Image gtkimage;
		readonly Gtk.Table table;
		ButtonImagePosition imagePosition;

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
			SetImagePosition(false);
			Control.Add(table);
		}

		protected override void Initialize()
		{
			base.Initialize();
			Control.Clicked += Connector.HandleClicked;
			Control.SizeAllocated += Connector.HandleButtonSizeAllocated;
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
					var c = (Gtk.Button)o;
					var size = args.Allocation;
					if (Handler.PreferredSize.Width == -1 && (size.Width > 1 || size.Height > 1))
					{
						size.Width = Math.Max(size.Width, MinimumWidth);
						if (args.Allocation != size)
							c.SetSizeRequest(size.Width, size.Height);
					}
				}
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
			get { return image; }
			set
			{
				image = value;
				image.SetGtkImage(gtkimage);
				if (value == null)
					gtkimage.Hide();
				else
					gtkimage.Show();
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

		public ButtonImagePosition ImagePosition
		{
			get { return imagePosition; }
			set
			{
				if (imagePosition != value)
				{
					imagePosition = value;
					SetImagePosition();
				}
			}
		}

		public Color TextColor
		{
			get { return label.Style.Foreground(Gtk.StateType.Normal).ToEto(); }
			set { label.ModifyFg(Gtk.StateType.Normal, value.ToGdk()); }
		}
	}
}
