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
using Eto.Platform.Mac.Forms.Controls;

namespace Eto.Platform.Mac
{
	public class ButtonHandler : MacButton<NSButton, Button>, IButton
	{
		class MyButtonCell : NSButtonCell
		{
			public Color? Color { get; set; }
			
			public override void DrawBezelWithFrame (System.Drawing.RectangleF frame, NSView controlView)
			{
				if (Color != null) {
					MacEventView.Colourize(controlView, Color.Value, delegate {
						base.DrawBezelWithFrame (frame, controlView);
					});
				} else 
					base.DrawBezelWithFrame (frame, controlView);
			}
		}
		
		class MyButton : NSButton
		{
			
			public ButtonHandler Handler { get; set; }
			
			public override void SizeToFit ()
			{
				base.SizeToFit ();
				
				if (Handler.AutoSize) {
					var frame = this.Frame;
					var defaultSize = Button.DefaultSize;
					if (frame.Width < defaultSize.Width)
						frame.Width = defaultSize.Width;
					if (frame.Height < defaultSize.Height)
						frame.Height = defaultSize.Height;
					this.Frame = frame;
				}
			}
			
		}
		
		
		public ButtonHandler ()
		{
			Control = new MyButton{ Handler = this };
			Control.Cell = new MyButtonCell ();
			Control.Title = string.Empty;
			Control.SetButtonType (NSButtonType.MomentaryPushIn);
			Control.BezelStyle = NSBezelStyle.Rounded;
			Control.SetFrameSize (Generator.ConvertF (Button.DefaultSize));
			Control.Activated += delegate {
				Widget.OnClick (EventArgs.Empty);
			};
		}
		
		public override Color BackgroundColor {
			get {
				var cell = Control.Cell as MyButtonCell;
				return cell.Color ?? Color.Transparent;
			}
			set {
				var cell = Control.Cell as MyButtonCell;
				cell.Color = value;
				Control.SetNeedsDisplay ();
			}
		}
		
	}
}
