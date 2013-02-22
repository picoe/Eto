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

		class MyButtonCell : NSButtonCell
		{
			public Color? Color { get; set; }
			
			public override void DrawBezelWithFrame (System.Drawing.RectangleF frame, NSView controlView)
			{
				if (Color != null) {
					MacEventView.Colourize (controlView, Color.Value, delegate {
						base.DrawBezelWithFrame (frame, controlView);
					});
				} else 
					base.DrawBezelWithFrame (frame, controlView);
			}
		}
		
		class EtoButton : NSButton, IMacControl
		{
			
			public ButtonHandler Handler { get; set; }
			
			public override void SizeToFit ()
			{
				base.SizeToFit ();
				
				if (Handler.AutoSize) {
					var frame = this.Frame;
					if (frame.Width < Handler.defaultSize.Width)
						frame.Width = Handler.defaultSize.Width;
					if (frame.Height < Handler.defaultSize.Height)
						frame.Height = Handler.defaultSize.Height;
					this.Frame = frame;
				}
			}
			
			object IMacControl.Handler { get { return Handler; } }
		}
		
		
		public ButtonHandler ()
		{
			Control = new EtoButton{ 
				Handler = this,
				Cell = new MyButtonCell (),
				Title = string.Empty,
				BezelStyle = NSBezelStyle.Rounded,
				ImagePosition = NSCellImagePosition.ImageLeft
			};
			defaultSize = Button.DefaultSize;
			Control.SetButtonType (NSButtonType.MomentaryPushIn);
			Control.SetFrameSize (defaultSize.ToSDSizeF ());
			Control.Activated += delegate {
				Widget.OnClick (EventArgs.Empty);
			};
			SetBezel ();
		}
		
		public override void AttachEvent (string handler)
		{
			switch (handler) {
			default:
				base.AttachEvent (handler);
				break;
			}
		}

		public override Color BackgroundColor {
			get {
				var cell = Control.Cell as MyButtonCell;
				return cell.Color ?? Colors.Transparent;
			}
			set {
				var cell = Control.Cell as MyButtonCell;
				cell.Color = value;
				Control.SetNeedsDisplay ();
			}
		}

		public Image Image
		{
			get { return image; }
			set
			{
				var oldSize = GetPreferredSize (Size.MaxValue);
				image = value;
				Control.Image = image.ToNS ();
				SetBezel ();
				LayoutIfNeeded(oldSize);
			}
		}

		public override Size Size
		{
			get { return base.Size; }
			set
			{
				base.Size = value;
				SetBezel ();
			}
		}

		bool NeedsBiggerBezel
		{
			get {
				var size = PreferredSize ?? defaultSize;
				if (size.Height > 26)
					return true;
				if (Image == null)
					return false;
				if (image.Size.Height > 18)
					return true;
				switch (Control.ImagePosition) {
				case NSCellImagePosition.ImageAbove:
				case NSCellImagePosition.ImageBelow:
					return !string.IsNullOrEmpty (this.Text);
				}
				return false;
			}
		}

		void SetBezel ()
		{
			Control.BezelStyle = NeedsBiggerBezel ? NSBezelStyle.RegularSquare : NSBezelStyle.Rounded;
		}

		public override string Text
		{
			get { return base.Text; }
			set
			{
				base.Text = value;
				SetImagePosition ();
			}
		}

		void SetImagePosition ()
		{
			var position = imagePosition.ToNS ();
			if ((position == NSCellImagePosition.ImageAbove || position == NSCellImagePosition.ImageBelow) && string.IsNullOrEmpty(this.Text))
				position = NSCellImagePosition.ImageOnly;
			Control.ImagePosition = position;
			SetBezel ();
		}

		public ButtonImagePosition ImagePosition
		{
			get { return imagePosition; }
			set {
				if (imagePosition != value) {
					var oldSize = GetPreferredSize (Size.MaxValue);
					imagePosition = value;
					SetImagePosition ();
					LayoutIfNeeded(oldSize);
				}
			}
		}
	}
}
