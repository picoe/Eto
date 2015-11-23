using System;
using Eto.Drawing;
using Eto.Forms;

#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
using CoreImage;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
using MonoMac.CoreImage;
#if Mac64
using CGSize = MonoMac.Foundation.NSSize;
using CGRect = MonoMac.Foundation.NSRect;
using CGPoint = MonoMac.Foundation.NSPoint;
using nfloat = System.Double;
using nint = System.Int64;
using nuint = System.UInt64;
#else
using CGSize = System.Drawing.SizeF;
using CGRect = System.Drawing.RectangleF;
using CGPoint = System.Drawing.PointF;
using nfloat = System.Single;
using nint = System.Int32;
using nuint = System.UInt32;
#endif
#endif

namespace Eto.Mac.Forms.Controls
{
	/// <summary>
	/// Button handler.
	/// </summary>
	/// <copyright>(c) 2012-2013 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class ButtonHandler : MacButton<NSButton, Button, Button.ICallback>, Button.IHandler
	{
		static readonly Size originalSize;

		public static int MinimumWidth = 80;

		protected override Size DefaultMinimumSize
		{
			get { return new Size(MinimumWidth, originalSize.Height); }
		}

		public class EtoButtonCell : NSButtonCell
		{
			public Color? Color { get; set; }

			public override void DrawBezelWithFrame(CGRect frame, NSView controlView)
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

		public class EtoButton : NSButton, IMacControl
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
					var minSize = Handler.MinimumSize;
					size.Height = (nfloat)Math.Max(size.Height, minSize.Height);
					size.Width = (nfloat)Math.Max(size.Width, minSize.Width);
					SetFrameSize(size);
				}
				setBezel = true;
			}

			public override void SetFrameSize(CGSize newSize)
			{
				base.SetFrameSize(newSize);
				if (setBezel)
					Handler.SetBezel();
			}

			public EtoButton()
			{
				Cell = new EtoButtonCell();
				Title = string.Empty;
				BezelStyle = NSBezelStyle.Rounded;
				ImagePosition = NSCellImagePosition.ImageLeft;
				SetButtonType(NSButtonType.MomentaryPushIn);
			}
		}

		static ButtonHandler()
		{
			// store the normal size for a rounded button, so we can determine what style to give it based on actual size
			var b = new NSButton { BezelStyle = NSBezelStyle.Rounded };
			b.SizeToFit();
			originalSize = b.Frame.Size.ToEtoSize();
		}

		protected override void Initialize()
		{
			base.Initialize();

			Control.Activated += HandleActivated;
		}

		protected override NSButton CreateControl()
		{
			return new EtoButton();
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
				handler.TriggerMouseCallback();
				handler.Callback.OnClick(handler.Widget, EventArgs.Empty);
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

		static readonly object Image_Key = new object();

		public Image Image
		{
			get { return Widget.Properties.Get<Image>(Image_Key); }
			set
			{
				Widget.Properties.Set(Image_Key, value, () =>
				{
					Control.Image = value.ToNS();
					SetImagePosition();
					LayoutIfNeeded();
				});
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
			if (size.Width == 0 || size.Height == 0)
				return Control.BezelStyle;
			if (size.Height < 22 || size.Width < 22)
				return NSBezelStyle.SmallSquare;
			if (size.Height > originalSize.Height)
				return NSBezelStyle.RegularSquare;
			var image = Image;
			if (image == null)
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
			var bezel = Control.BezelStyle;
			var requiredBezel = GetBezelStyle();
			if (bezel != requiredBezel)
			{
				Control.BezelStyle = requiredBezel;
				LayoutIfNeeded();
			}
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
			var position = ImagePosition.ToNS();
			if (string.IsNullOrEmpty(Text) &&
			    (
			        position == NSCellImagePosition.ImageAbove
			        || position == NSCellImagePosition.ImageBelow
			        || Image != null && Image.Width > MinimumSize.Width
			    ))
				position = NSCellImagePosition.ImageOnly;
			Control.ImagePosition = position;
			SetBezel();
		}

		static readonly object ImagePosition_Key = new object();

		public ButtonImagePosition ImagePosition
		{
			get { return Widget.Properties.Get<ButtonImagePosition>(ImagePosition_Key); }
			set
			{
				var oldSize = GetPreferredSize(Size.MaxValue);
				Widget.Properties.Set(ImagePosition_Key, value, () =>
				{
					SetImagePosition();
					LayoutIfNeeded(oldSize);
				});
			}
		}

		public override Size MinimumSize
		{
			get { return base.MinimumSize; }
			set
			{
				var oldSize = GetPreferredSize(Size.MaxValue);
				base.MinimumSize = value;
				SetImagePosition();
				LayoutIfNeeded(oldSize);
			}
		}
	}
}
