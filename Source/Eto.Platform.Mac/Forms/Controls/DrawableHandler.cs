using System;
using SD = System.Drawing;
using Eto.Drawing;
using Eto.Forms;
using Eto.Platform.Mac.Drawing;
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac;
using MonoMac.ObjCRuntime;

namespace Eto.Platform.Mac.Forms.Controls
{
	public class DrawableHandler : MacView<DrawableHandler.MyView, Drawable>, IDrawable
	{
		public class MyView : MacEventView
		{
			Drawable Drawable {
				get { return Widget as Drawable; }
			}
			
			public override void DrawRect (System.Drawing.RectangleF dirtyRect)
			{
				if (Widget == null)
					return;
				//if (!WantsLayer) {
					dirtyRect.Y = this.Frame.Height - dirtyRect.Y - dirtyRect.Height;
					Drawable.Update (Generator.ConvertF (dirtyRect));
				/*} else {
					var rects = GetRectsBeingDrawn??
					//rect.Y = this.Frame.Height - rect.Y - rect.Height;
					//Drawable.Update (Generator.ConvertF (rect));
				}*/
			}
			
			public bool CanFocus { get; set; }

			public override bool AcceptsFirstResponder ()
			{
				return CanFocus;
			}

			public override bool AcceptsFirstMouse (NSEvent theEvent)
			{
				return CanFocus;
			}
			
			public override bool BecomeFirstResponder ()
			{
				if (Widget != null) 
					Widget.OnGotFocus (EventArgs.Empty);
				return base.BecomeFirstResponder ();
			}
			
			public override bool ResignFirstResponder ()
			{
				if (Widget != null)
					Widget.OnLostFocus (EventArgs.Empty);
				return base.ResignFirstResponder ();
			}
			
		}
	
		public override bool Enabled { get; set; }
		
		public void Create ()
		{
			Enabled = true;
			Control = new MyView{ Handler = this };
		}
		
		public bool CanFocus {
			get { return Control.CanFocus; }
			set { Control.CanFocus = value; }
		}
		
		public void Update (Rectangle rect)
		{
			var context = NSGraphicsContext.CurrentContext;
			if (context != null) {
				var graphics = new Graphics (Widget.Generator, new GraphicsHandler (context, Control.Frame.Height));
				Widget.OnPaint (new PaintEventArgs (graphics, rect));
			}
		}
		

	}
}
