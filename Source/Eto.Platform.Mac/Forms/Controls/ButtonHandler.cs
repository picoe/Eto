using System;
using System.Reflection;
using SD = System.Drawing;
using Eto.Drawing;
using Eto.Forms;
using MonoMac.AppKit;
using MonoMac.CoreImage;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.Foundation;

namespace Eto.Platform.Mac.Forms.Controls
{
	/// <summary>
	/// Button handler.
	/// </summary>
	/// <copyright>(c) 2012-2013 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class ButtonHandler : MacButton<NSButton, Button>, IButton
	{
		Image image;
		ButtonImagePosition imagePosition;
		Size defaultSize;
		static readonly Size originalSize;

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
					var size = this.Frame.Size;
					if (size.Width < Handler.defaultSize.Width)
						size.Width = Handler.defaultSize.Width;
					if (size.Height < Handler.defaultSize.Height)
						size.Height = Handler.defaultSize.Height;
					this.SetFrameSize(size);
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
			defaultSize = Button.DefaultSize;
			Control.SetButtonType(NSButtonType.MomentaryPushIn);
			Control.SetFrameSize(defaultSize.ToSDSizeF());
			Control.Activated += HandleActivated;
			SetBezel();
		}

		static void HandleActivated(object sender, EventArgs e)
		{
			var handler = GetHandler(sender) as ButtonHandler;
			if (handler != null)
			{
				handler.Widget.OnClick(EventArgs.Empty);
			}
		}

		public override void AttachEvent(string handler)
		{
			switch (handler)
			{
				default:
					base.AttachEvent(handler);
					break;
			}
		}

		public override Color BackgroundColor
		{
			get
			{
				var cell = Control.Cell as EtoButtonCell;
				return cell.Color ?? Colors.Transparent;
			}
			set
			{
				var cell = Control.Cell as EtoButtonCell;
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
					if (!string.IsNullOrEmpty(this.Text))
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
			if ((position == NSCellImagePosition.ImageAbove || position == NSCellImagePosition.ImageBelow) && string.IsNullOrEmpty(this.Text))
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
