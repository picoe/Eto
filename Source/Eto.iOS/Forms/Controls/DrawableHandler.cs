using System;
using SD = System.Drawing;
using Eto.Drawing;
using Eto.Forms;
using Eto.iOS.Drawing;
using UIKit;
using CoreAnimation;
using Foundation;
using ObjCRuntime;
using Eto.Mac.Forms;
using Eto.Mac;
using CoreGraphics;

namespace Eto.iOS.Forms.Controls
{
	public class DrawableHandler : MacPanel<DrawableHandler.EtoView, Drawable, Drawable.ICallback>, Drawable.IHandler
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
				if (UIScreen.MainScreen.RespondsToSelector(new Selector("scale")) && Math.Abs(UIScreen.MainScreen.Scale - 2.0f) < 0.01f)
				{
					tiledLayer.TileSize = new CoreGraphics.CGSize(512, 512);
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
				Handler.Callback.OnMouseDown(Handler.Widget, args);
				if (!args.Handled)
					base.TouchesBegan(touches, evt);
			}

			public override void TouchesEnded(NSSet touches, UIEvent evt)
			{
				var args = Conversions.ConvertMouse(this, touches, evt);
				Handler.Callback.OnMouseUp(Handler.Widget, args);
				if (!args.Handled)
					base.TouchesEnded(touches, evt);
			}

			public override void TouchesMoved(NSSet touches, UIEvent evt)
			{
				var args = Conversions.ConvertMouse(this, touches, evt);
				Handler.Callback.OnMouseMove(Handler.Widget, args);
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

			public override void Draw(CoreGraphics.CGRect rect)
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
				Handler.Callback.OnGotFocus(Handler.Widget, EventArgs.Empty);
				return base.BecomeFirstResponder();
			}

			public override bool ResignFirstResponder()
			{
				Handler.Callback.OnLostFocus(Handler.Widget, EventArgs.Empty);
				return base.ResignFirstResponder();
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
				using (var graphics = new Graphics(new GraphicsHandler(Control, context, Control.Frame.Height)))
				{
					Callback.OnPaint(Widget, new PaintEventArgs(graphics, rect));
				}
			}
		}
	}
}
