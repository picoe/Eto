using System;
using System.Reflection;
using Eto.Drawing;
using Eto.Forms;
using MonoMac.AppKit;
using MonoMac.CoreImage;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.Foundation;

namespace Eto.Platform.Mac
{
	public class ButtonHandler : MacButton<NSButton, Button>, IButton
	{
		class MyButtonCell : NSButtonCell {
			public CGColor Color { get; set; }
			public override void DrawInteriorWithFrame (System.Drawing.RectangleF cellFrame, NSView inView)
			{
				var context = NSGraphicsContext.CurrentContext;
				var graphics = context.GraphicsPort;
				graphics.SetFillColor(Color);
				cellFrame.Inflate(-1, -1);
				graphics.FillRect(cellFrame);
			}
		}
		
		class MyButton : NSButton {
			
			public ButtonHandler Handler { get; set; }
			
			public override void SizeToFit ()
			{
				base.SizeToFit ();
				
				if (Handler.AutoSize)
				{
					var frame = this.Frame;
					var defaultSize = Button.DefaultSize;
					if (frame.Width < defaultSize.Width) frame.Width = defaultSize.Width;
					if (frame.Height < defaultSize.Height) frame.Height = defaultSize.Height;
					this.Frame = frame;
				}
			}
			
		}
		
		
		public ButtonHandler ()
		{
			Control = new MyButton{ Handler = this };
			Control.Title = string.Empty;
			Control.SetButtonType(NSButtonType.MomentaryPushIn);
			Control.BezelStyle = NSBezelStyle.Rounded;
			Control.SetFrameSize(Generator.ConvertF(Button.DefaultSize));
			Control.Activated += delegate {
				Widget.OnClick(EventArgs.Empty);
			};
		}
		
		public override Color BackgroundColor {
			get {
				return Generator.Convert(Control.Cell.BackgroundColor);
			}
			set {
				var cell = Control.Cell as MyButtonCell;
				if (cell == null) {
					Control.BezelStyle = NSBezelStyle.Recessed;
					Control.Cell = cell = new MyButtonCell();
					cell.Activated += delegate {
						Widget.OnClick(EventArgs.Empty);
					};
				}
				cell.Color = Generator.Convert(value);
				Control.SetNeedsDisplay();
			}
		}
		
	}
}
