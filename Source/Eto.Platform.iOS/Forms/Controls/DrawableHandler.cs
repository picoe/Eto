using System;
using SD = System.Drawing;
using Eto.Drawing;
using Eto.Forms;
using Eto.Platform.iOS.Drawing;
using MonoTouch.UIKit;
using MonoTouch.CoreAnimation;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.OpenGLES;
using MonoTouch.GLKit;

namespace Eto.Platform.iOS.Forms.Controls
{
	public class DrawableHandler : iosView<DrawableHandler.MyView, Drawable>, IDrawable
	{
		/*
		[Register("FastLayer")]
		public class FastLayer : CATiledLayer
		{
			[Export("fadeDuration")]
			public static float fadeDuration {
				get { return 0.0f; }
			}
		}*/
		
		public class MyTiledView : MyView {
			
			[Export ("layerClass")]
			public static Class LayerClass ()
			{
				return new Class (typeof(CATiledLayer));
			}
			
			public MyTiledView()
			{
				var tiledLayer = (CATiledLayer)this.Layer;
				if (UIScreen.MainScreen.RespondsToSelector (new Selector("scale")) && UIScreen.MainScreen.Scale == 2.0f) {
					tiledLayer.TileSize = new SD.SizeF(512, 512);
				}
				tiledLayer.LevelsOfDetail = 4;
			}
		}
		
		public class MyView : UIView
		{
			public override void TouchesBegan (NSSet touches, UIEvent evt)
			{
				var args = Generator.ConvertMouse (this, touches, evt);
				Handler.Widget.OnMouseDown (args);
				if (!args.Handled)
					base.TouchesBegan (touches, evt);
			}
			
			public override void TouchesEnded (NSSet touches, UIEvent evt)
			{
				var args = Generator.ConvertMouse (this, touches, evt);
				Handler.Widget.OnMouseUp (args);
				if (!args.Handled)
					base.TouchesEnded (touches, evt);
			}
			
			public override void TouchesMoved (NSSet touches, UIEvent evt)
			{
				var args = Generator.ConvertMouse (this, touches, evt);
				Handler.Widget.OnMouseMove (args);
				if (!args.Handled)
					base.TouchesMoved (touches, evt);
			}
			
			public MyView ()
			{
				//var tiledLayer = this.Layer as CATiledLayer;
				this.BackgroundColor = UIColor.Clear;
			}

			public DrawableHandler Handler { get; set; }

			public override void Draw (System.Drawing.RectangleF rect)
			{
				//Console.WriteLine ("Drawing {0}, {1}", rect, new System.Diagnostics.StackTrace ());
				Handler.Update (rect.ToEtoRectangle ());
			}

			public bool CanFocus { get; set; }

			public override bool CanBecomeFirstResponder {
				get { return CanFocus; }
			}

			public override bool BecomeFirstResponder ()
			{
				Handler.Widget.OnGotFocus (EventArgs.Empty);
				return base.BecomeFirstResponder ();
			}

			public override bool ResignFirstResponder ()
			{
				Handler.Widget.OnLostFocus (EventArgs.Empty);
				return base.ResignFirstResponder ();
			}
			
			static IntPtr selFrame = Selector.GetHandle("frame");
			
			public SD.RectangleF BaseFrame
			{
				get {
					SD.RectangleF result;
					Messaging.RectangleF_objc_msgSend_stret (out result, base.Handle, selFrame);
					return result;
				}
			}
		}
		
		public override Eto.Drawing.Size Size {
			get {
				return base.Size;
			}
			set {
				base.Size = value;
			}
		}
		
		public void Create ()
		{
			Control = new MyTiledView{ Handler = this };
		}
		
		public void Create (bool largeCanvas)
		{
			if (largeCanvas)
				Control = new MyTiledView{ Handler = this };
			else
				Control = new MyView{ Handler = this };
		}
		
		
		public bool CanFocus {
			get { return Control.CanFocus; }
			set { Control.CanFocus = value; }
		}
		
		public void Update (Rectangle rect)
		{
			var context = UIGraphics.GetCurrentContext ();
			if (context != null) {
				/*var scale = context.GetCTM().xx;  // .a			// http://developer.apple.com/library/ios/#documentation/GraphicsImaging/Reference/CGAffineTransform/Reference/reference.html#//apple_ref/doc/c_ref/CGAffineTransform
				var tiledLayer = (CATiledLayer)this.Layer;
				var tileSize = tiledLayer.TileSize;
				
			    tileSize.Width /= scale;
			    tileSize.Height /= scale;*/
				//lock (this) {
				//context.TranslateCTM(0, 0);
				//context.ScaleCTM(1, -1);
				//var oldCheck = UIApplication.CheckForIllegalCrossThreadCalls;
				//UIApplication.CheckForIllegalCrossThreadCalls = false;
				
				using (var graphics = new Graphics (Widget.Generator, new GraphicsHandler (context, Control.BaseFrame.Height, false))) {
					Widget.OnPaint (new PaintEventArgs (graphics, rect));
				}
				//UIApplication.CheckForIllegalCrossThreadCalls = oldCheck;
				//}
			}
		}

	}
}
