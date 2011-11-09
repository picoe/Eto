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
		
		public class MyView : UIView
		{
			[Export ("layerClass")]
			public static Class LayerClass ()
			{
				return new Class (typeof(CATiledLayer));
			}
			
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
				Handler.Update (Generator.ConvertF (rect));
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
		}
		
		public override Eto.Drawing.Size Size {
			get {
				return base.Size;
			}
			set {
				base.Size = value;
			}
		}
		
		public DrawableHandler ()
		{
			Control = new MyView{ Handler = this };
		}

		public override void OnLoad (EventArgs e)
		{
			base.OnLoad (e);
			var tiledLayer = Control.Layer as CATiledLayer;
			if (tiledLayer != null) {
				tiledLayer.LevelsOfDetail = 4;
			}
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
				var graphics = new Graphics (Widget.Generator, new GraphicsHandler (context, Control.Frame.Height, false));
				Widget.OnPaint (new PaintEventArgs (graphics, rect));
				//}
			}
		}

	}
}
