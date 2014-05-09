using System;
using SD = System.Drawing;
using Eto.Drawing;
using Eto.Forms;
using MonoMac.AppKit;

namespace Eto.Mac.Forms.Controls
{
	/// <summary>
	/// Button handler.
	/// </summary>
	/// <copyright>(c) 2012-2013 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class ButtonHandler : MacButton<NSButton, Button, Button.ICallback>, IButton
	{
		Image image;
		ButtonImagePosition imagePosition;
		static readonly Size originalSize;

		public static int MinimumWidth = 80;

		class EtoButtonCell : NSButtonCell
		{
			public Color? Color { get; set; }

			public override void DrawBezelWithFrame(System.Drawing.RectangleF frame, NSView controlView)
			{
				if (Color != null)
				{
					MacEventView.Colourize(controlView, Color.Value, delegate
					{
						base.DrawBezelWithFrame(frame, controlView);
					});
				}
				else 
					base.DrawBezelWithFrame(frame, controlView);
			}
		}

		class EtoButton : NSButton, IMacControl
		{
			bool setBezel = true;
			public WeakReference WeakHandler { get; set; }

			public ButtonHandler Handler
			{ 
				get { return (ButtonHandler)WeakHandler.Target; }
				set { WeakHandler = new WeakReference(value); } 
			}

			public override void SizeToFit()
			{
				setBezel = false;
				base.SizeToFit();

				if (Handler.AutoSize)
				{
					var size = Frame.Size;
					size.Width = Math.Max(size.Width, MinimumWidth);
					SetFrameSize(size);
				}
				setBezel = true;
			}

			public override void SetFrameSize(SD.SizeF newSize)
			{
				base.SetFrameSize(newSize);
				if (setBezel)
					Handler.SetBezel();
			}
		}

		static ButtonHandler()
		{
			// store the normal size for a rounded button, so we can determine what style to give it based on actual size
			var b = new NSButton { BezelStyle = NSBezelStyle.Rounded };
			b.SizeToFit();
			originalSize = b.Frame.Size.ToEtoSize();
		}

		public ButtonHandler()
		{
			Control = new EtoButton
			{ 
				Handler = this,
				Cell = new EtoButtonCell (),
				Title = string.Empty,
				BezelStyle = NSBezelStyle.Rounded,
				ImagePosition = NSCellImagePosition.ImageLeft
			};
			Control.SetButtonType(NSButtonType.MomentaryPushIn);
			Control.Activated += HandleActivated;
		}

		public override void OnLoadComplete(EventArgs e)
		{
			base.OnLoadComplete(e);
			SetBezel();
		}

		static void HandleActivated(object sender, EventArgs e)
		{
			var handler = GetHandler(sender) as ButtonHandler;
			if (handler != null)
			{
				handler.Callback.OnClick(handler.Widget, EventArgs.Empty);
			}
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				default:
					base.AttachEvent(id);
					break;
			}
		}

		public override Color BackgroundColor
		{
			get
			{
				var cell = (EtoButtonCell)Control.Cell;
				return cell.Color ?? Colors.Transparent;
			}
			set
			{
				var cell = (EtoButtonCell)Control.Cell;
				cell.Color = value.A > 0 ? (Color?)value : null;
				Control.SetNeedsDisplay();
			}
		}

		public Image Image
		{
			get { return image; }
			set
			{
				var oldSize = GetPreferredSize(Size.MaxValue);
				image = value;
				Control.Image = image.ToNS();
				SetBezel();
				LayoutIfNeeded(oldSize);
			}
		}

		public override Size Size
		{
			get { return base.Size; }
			set
			{
				base.Size = value;
				SetBezel();
			}
		}

		/// <summary>
		/// Gets the bezel style of the button based on its size and image position
		/// </summary>
		NSBezelStyle GetBezelStyle()
		{
			var size = Control.Frame.Size.ToEtoSize();
			if (size.Height < 22 || size.Width < 22)
				return NSBezelStyle.SmallSquare;
			if (size.Height > originalSize.Height)
				return NSBezelStyle.RegularSquare;
			if (Image == null)
				return NSBezelStyle.Rounded;
			if (image.Size.Height > 18)
				return NSBezelStyle.RegularSquare;
			switch (Control.ImagePosition)
			{
				case NSCellImagePosition.ImageAbove:
				case NSCellImagePosition.ImageBelow:
					if (!string.IsNullOrEmpty(Text))
						return NSBezelStyle.RegularSquare;
					break;
			}
			return NSBezelStyle.Rounded;
		}

		void SetBezel()
		{
			Control.BezelStyle = GetBezelStyle();
		}

		public override string Text
		{
			get { return base.Text; }
			set
			{
				base.Text = value;
				SetImagePosition();
			}
		}

		void SetImagePosition()
		{
			var position = imagePosition.ToNS();
			if ((position == NSCellImagePosition.ImageAbove || position == NSCellImagePosition.ImageBelow) && string.IsNullOrEmpty(Text))
				position = NSCellImagePosition.ImageOnly;
			Control.ImagePosition = position;
			SetBezel();
		}

		public ButtonImagePosition ImagePosition
		{
			get { return imagePosition; }
			set
			{
				if (imagePosition != value)
				{
					var oldSize = GetPreferredSize(Size.MaxValue);
					imagePosition = value;
					SetImagePosition();
					LayoutIfNeeded(oldSize);
				}
			}
		}
	}
}
