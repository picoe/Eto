//#define BUTTON_DEBUG

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
using nfloat = System.Double;
using nint = System.Int64;
using nuint = System.UInt64;
#else
using nfloat = System.Single;
using nint = System.Int32;
using nuint = System.UInt32;
#endif
#if SDCOMPAT
using CGSize = System.Drawing.SizeF;
using CGRect = System.Drawing.RectangleF;
using CGPoint = System.Drawing.PointF;
#endif
#endif

namespace Eto.Mac.Forms.Controls
{
	public class ButtonHandler : ButtonHandler<Button, Button.ICallback>, Button.IHandler
	{
		public static int MinimumWidth = 80;
		internal static readonly object CustomBezelStyle_Key = new object();
		internal static readonly object Image_Key = new object();
		internal static readonly object ImagePosition_Key = new object();


		internal static readonly Size originalSize;
		protected override NSButtonType DefaultButtonType => NSButtonType.MomentaryPushIn;

		protected override Size DefaultMinimumSize => new Size(MinimumWidth, originalSize.Height);

		static ButtonHandler()
		{
			// store the normal size for a rounded button, so we can determine what style to give it based on actual size
			var b = new EtoButton(NSButtonType.MomentaryPushIn);
			originalSize = b.GetAlignmentRectForFrame(new CGRect(CGPoint.Empty, b.FittingSize)).Size.ToEtoSize();
		}
	}

	public class EtoButtonCell : NSButtonCell
	{
		ColorizeView colorize;
		public Color? Color
		{
			get => colorize?.Color;
			set => ColorizeView.Create(ref colorize, value);
		}

		public override void DrawBezelWithFrame(CGRect frame, NSView controlView)
		{
			colorize?.Begin(frame, controlView);
			base.DrawBezelWithFrame(frame, controlView);
			colorize?.End();
		}

		public EtoButtonCell()
		{
			ImageScale = NSImageScale.ProportionallyDown;
		}

		public EtoButtonCell(IntPtr handle) : base(handle)
		{
		}
	}

	public interface IButtonHandler
	{
		bool SetBezel(Size size);
		bool AutoSize { get; }
		Size MinimumSize { get; }
		int DisableSetBezel { get; set; }
		void TriggerSizeChanged();
		void InvalidateMeasure();
	}

	public class EtoButton : NSButton, IMacControl
	{
		public WeakReference WeakHandler { get; set; }

		public IButtonHandler Handler
		{
			get { return (IButtonHandler)WeakHandler?.Target; }
			set { WeakHandler = new WeakReference(value); }
		}

		public override void SizeToFit()
		{
			var h = Handler;
			if (h == null)
			{
				base.SizeToFit();
				return;
			}
			h.DisableSetBezel++;
			base.SizeToFit();
			if (h.AutoSize)
			{
				var size = Frame.Size;
				var minSize = h.MinimumSize;
				size.Height = (nfloat)Math.Max(size.Height, minSize.Height);
				size.Width = (nfloat)Math.Max(size.Width, minSize.Width);
				SetFrameSize(size);
			}
			h.DisableSetBezel--;
		}

		public override void SetFrameSize(CGSize newSize)
		{
			base.SetFrameSize(newSize);
			var h = Handler;
			if (h == null)
				return;

#if BUTTON_DEBUG
			Console.WriteLine($"Setting frame size to {newSize}");
#endif

			if (h.SetBezel(GetAlignmentRectForFrame(new CGRect(CGPoint.Empty, newSize)).Size.ToEtoSize()))
				Application.Instance.AsyncInvoke(() => h.InvalidateMeasure());

			h.TriggerSizeChanged();
		}

		public EtoButton(IntPtr handle) : base(handle)
		{
		}

		public EtoButton() : this(NSButtonType.MomentaryPushIn)
		{
		}

		public EtoButton(NSButtonType buttonType)
		{
			Cell = new EtoButtonCell();
			Title = string.Empty;
			BezelStyle = NSBezelStyle.Rounded;
			SetButtonType(buttonType);
			ImagePosition = NSCellImagePosition.ImageLeft;
		}
	}

	/// <summary>
	/// Button handler.
	/// </summary>
	/// <copyright>(c) 2012-2019 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public abstract class ButtonHandler<TWidget, TCallback> : MacButton<NSButton, TWidget, TCallback>, Button.IHandler, IButtonHandler
		where TWidget: Button
		where TCallback: Button.ICallback
	{

		protected abstract NSButtonType DefaultButtonType { get; }


		protected override void Initialize()
		{
			base.Initialize();

			Control.Activated += HandleActivated;
		}

		protected override NSButton CreateControl() => new EtoButton(DefaultButtonType);

		static void HandleActivated(object sender, EventArgs e)
		{
			var handler = GetHandler(sender) as ButtonHandler<TWidget, TCallback>;
			handler?.OnActivated();
		}

		protected virtual void OnActivated()
		{
			TriggerMouseCallback();
			Callback.OnClick(Widget, EventArgs.Empty);
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

		public Image Image
		{
			get { return Widget.Properties.Get<Image>(ButtonHandler.Image_Key); }
			set
			{
				if (Widget.Properties.TrySet(ButtonHandler.Image_Key, value))
				{
					Control.Image = value.ToNS();
					SetImagePosition();
					InvalidateMeasure();
				}
			}
		}

		protected override SizeF GetNaturalSize(SizeF availableSize)
		{
			var size = base.GetNaturalSize(availableSize);
			// when bezel is RegularSquare, it reports different than other bezels
			// so we adjust here to report a more consistent natural size
			if (Control.BezelStyle == NSBezelStyle.RegularSquare)
				size.Height -= 2;
			size.Height = Math.Max(size.Height, DefaultMinimumSize.Height);

#if BUTTON_DEBUG
			Console.WriteLine($"Button.GetNaturalSize {Text}, Size: {size}, Bezel: {Control.BezelStyle}");
#endif
			return size;
		}

		protected override bool DefaultUseAlignmentFrame => true; 

		public override void SetAlignmentFrame(CGRect frame)
		{
			if (SetBezel(frame.Size.ToEtoSize()))
				Application.Instance.AsyncInvoke(InvalidateMeasure);

			base.SetAlignmentFrame(frame);
		}

		/// <summary>
		/// Gets the bezel style of the button based on its size and image position
		/// </summary>
		protected virtual NSBezelStyle GetBezelStyle(Size? frameSize = null)
		{
			if (BezelStyle != null)
				return BezelStyle.Value;
			var size = frameSize ?? GetAlignmentFrame().Size.ToEtoSize();

			// use the preferred size to determine style to use, if set
			var userPreferredSize = UserPreferredSize;
			bool autoSize = true;
			if (userPreferredSize.Width > -1)
				size.Width = userPreferredSize.Width;
			if (userPreferredSize.Height > -1)
			{
				size.Height = userPreferredSize.Height;
				autoSize = false;
			}

			if (Widget.Loaded || !size.IsEmpty)
			{
				if (size.IsEmpty)
					return Control.BezelStyle;

				var originalSize = ButtonHandler.originalSize;
				if (size.Height < originalSize.Height || size.Width < originalSize.Width)
					return NSBezelStyle.SmallSquare;
				if (size.Height > originalSize.Height)
					return NSBezelStyle.RegularSquare;
			}
			var image = Image;
			if (image == null)
				return NSBezelStyle.Rounded;
			if (autoSize && image.Size.Height > 18)
				return NSBezelStyle.RegularSquare;
			switch (Control.ImagePosition)
			{
				case NSCellImagePosition.ImageAbove:
				case NSCellImagePosition.ImageBelow:
				case NSCellImagePosition.ImageOnly:
					if (!string.IsNullOrEmpty(Text))
						return NSBezelStyle.RegularSquare;
					break;
			}
			return NSBezelStyle.Rounded;
		}

		protected virtual bool SetBezel(Size size)
		{
			if (DisableSetBezel > 0)
				return false;
			var bezel = GetBezelStyle(size);
			if (bezel != Control.BezelStyle)
			{
				DisableSetBezel++;
				// setting the bezel style can fire a size changed?
				Control.BezelStyle = bezel;
#if BUTTON_DEBUG
				Console.WriteLine($"Changing Button {Text} to bezel {bezel}");
#endif
				DisableSetBezel--;
				return true;
			}
			return false;
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

		protected virtual void SetImagePosition()
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
			SetBezel(GetAlignmentFrame().Size.ToEtoSize());
		}

		public ButtonImagePosition ImagePosition
		{
			get { return Widget.Properties.Get<ButtonImagePosition>(ButtonHandler.ImagePosition_Key); }
			set
			{
				if (Widget.Properties.TrySet(ButtonHandler.ImagePosition_Key, value))
				{
					SetImagePosition();
					InvalidateMeasure();
				}
			}
		}

		public NSBezelStyle? BezelStyle
		{
			get { return Widget.Properties.Get<NSBezelStyle?>(ButtonHandler.CustomBezelStyle_Key); }
			set
			{
				if (Widget.Properties.TrySet(ButtonHandler.CustomBezelStyle_Key, value))
				{
					if (value != null)
						Control.BezelStyle = value.Value;
					InvalidateMeasure();
				}
			}
		}

		public override Size MinimumSize
		{
			get { return base.MinimumSize; }
			set
			{
				base.MinimumSize = value;
				SetImagePosition();
				InvalidateMeasure();
			}
		}

		public int DisableSetBezel { get; set; }

		public void TriggerSizeChanged()
		{
			OnSizeChanged(EventArgs.Empty);
			Callback.OnSizeChanged(Widget, EventArgs.Empty);
		}

		bool IButtonHandler.SetBezel(Size size) => SetBezel(size);
	}
}
