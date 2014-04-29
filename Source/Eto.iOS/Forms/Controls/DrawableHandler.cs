using System;
using SD = System.Drawing;
using Eto.Drawing;
using Eto.Forms;
using Eto.iOS.Drawing;
using MonoTouch.UIKit;
using MonoTouch.CoreAnimation;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using Eto.Mac.Forms;

namespace Eto.iOS.Forms.Controls
{
	public class DrawableHandler : MacPanel<DrawableHandler.EtoView, Drawable>, IDrawable
	{
		public bool SupportsCreateGraphics { get { return false; } }

		public override UIView ContainerControl { get { return Control; } }

		public class EtoTiledView : EtoView
		{
			[Export("layerClass")]
			public static Class LayerClass()
			{
				return new Class(typeof(CATiledLayer));
			}

			public EtoTiledView()
			{
				var tiledLayer = (CATiledLayer)this.Layer;
				if (UIScreen.MainScreen.RespondsToSelector(new Selector("scale")) && UIScreen.MainScreen.Scale == 2.0f)
				{
					tiledLayer.TileSize = new SD.SizeF(512, 512);
				}
				tiledLayer.LevelsOfDetail = 4;
			}
		}

		[Register("EtoView")]
		public class EtoView : UIView
		{
			public override void TouchesBegan(NSSet touches, UIEvent evt)
			{
				var args = Conversions.ConvertMouse(this, touches, evt);
				Handler.Widget.OnMouseDown(args);
				if (!args.Handled)
					base.TouchesBegan(touches, evt);
			}

			public override void TouchesEnded(NSSet touches, UIEvent evt)
			{
				var args = Conversions.ConvertMouse(this, touches, evt);
				Handler.Widget.OnMouseUp(args);
				if (!args.Handled)
					base.TouchesEnded(touches, evt);
			}

			public override void TouchesMoved(NSSet touches, UIEvent evt)
			{
				var args = Conversions.ConvertMouse(this, touches, evt);
				Handler.Widget.OnMouseMove(args);
				if (!args.Handled)
					base.TouchesMoved(touches, evt);
			}

			public EtoView()
			{
				//var tiledLayer = this.Layer as CATiledLayer;
				this.BackgroundColor = UIColor.Clear;
				this.ContentMode = UIViewContentMode.Redraw;
			}

			WeakReference handler;

			public DrawableHandler Handler { get { return (DrawableHandler)handler.Target; } set { handler = new WeakReference(value); } }

			public override void Draw(System.Drawing.RectangleF rect)
			{
				Handler.Update(rect.ToEtoRectangle());
			}

			public bool CanFocus { get; set; }

			public override bool CanBecomeFirstResponder
			{
				get { return CanFocus; }
			}

			public override bool BecomeFirstResponder()
			{
				Handler.Widget.OnGotFocus(EventArgs.Empty);
				return base.BecomeFirstResponder();
			}

			public override bool ResignFirstResponder()
			{
				Handler.Widget.OnLostFocus(EventArgs.Empty);
				return base.ResignFirstResponder();
			}

			static readonly IntPtr selFrame = Selector.GetHandle("frame");

			public SD.RectangleF BaseFrame
			{
				get
				{
					SD.RectangleF result;
					Messaging.RectangleF_objc_msgSend_stret(out result, Handle, selFrame);
					return result;
				}
			}
		}

		public virtual void Create()
		{
			Control = new EtoView { Handler = this };
		}

		public void Create(bool largeCanvas)
		{
			if (largeCanvas)
				Control = new EtoTiledView { Handler = this };
			else
				Create();
		}

		public bool CanFocus
		{
			get { return Control.CanFocus; }
			set { Control.CanFocus = value; }
		}

		public void Update(Rectangle rect)
		{
			var context = UIGraphics.GetCurrentContext();
			if (context != null)
			{
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
				
				using (var graphics = new Graphics(Widget.Platform, new GraphicsHandler(Control, context, Control.BaseFrame.Height)))
				{
					Widget.OnPaint(new PaintEventArgs(graphics, rect));
				}
				//UIApplication.CheckForIllegalCrossThreadCalls = oldCheck;
				//}
			}
		}

		public ContextMenu ContextMenu
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}
	}
}
