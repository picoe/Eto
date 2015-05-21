using System;
using Eto.Forms;
using Eto.Drawing;
using Eto.Mac.Drawing;

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


namespace Eto.Mac.Forms.Cells
{
	public class DrawableCellHandler : CellHandler<NSCell, DrawableCell, DrawableCell.ICallback>, DrawableCell.IHandler
	{
		public class EtoCellValue : NSObject
		{
			public object Item { get; set; }

			public EtoCellValue()
			{
			}

			public EtoCellValue(IntPtr handle) : base(handle)
			{
			}

			[Export("copyWithZone:")]
			NSObject CopyWithZone(IntPtr zone)
			{
				var val = new EtoCellValue { Item = Item };
				val.DangerousRetain();
				return val;
			}
		}

		public class EtoCell : NSCell, IMacControl
		{
			public WeakReference WeakHandler { get; set; }

			public DrawableCellHandler Handler
			{ 
				get { return (DrawableCellHandler)WeakHandler.Target; }
				set { WeakHandler = new WeakReference(value); } 
			}

			public EtoCell()
			{
			}

			public EtoCell(IntPtr handle) : base(handle)
			{
			}

			public Color BackgroundColor { get; set; }

			public bool DrawsBackground { get; set; }

			public override CGSize CellSizeForBounds(CGRect bounds)
			{
				return CGSize.Empty;
			}

			[Export("copyWithZone:")]
			NSObject CopyWithZone(IntPtr zone)
			{
				var ptr = Messaging.IntPtr_objc_msgSendSuper_IntPtr(
					          SuperHandle,
					          MacCommon.CopyWithZoneHandle,
					          zone
				          );
				return new EtoCell(ptr) { Handler = Handler };
			}

			public override void DrawInteriorWithFrame(CGRect cellFrame, NSView inView)
			{
				var nscontext = NSGraphicsContext.CurrentContext;

				if (DrawsBackground)
				{
					var context = nscontext.GraphicsPort;
					context.SetFillColor(BackgroundColor.ToCG());
					context.FillRect(cellFrame);
				}

				var handler = Handler;
				var graphicsHandler = new GraphicsHandler(null, nscontext, (float)cellFrame.Height, flipped: true);
				using (var graphics = new Graphics(graphicsHandler))
				{
					var state = Highlighted ? DrawableCellStates.Selected : DrawableCellStates.None;
					var item = ObjectValue as EtoCellValue;
					var args = new DrawableCellPaintEventArgs(graphics, cellFrame.ToEto(), state, item != null ? item.Item : null);
					handler.Callback.OnPaint(handler.Widget, args);
				}
			}
		}

		public override bool Editable
		{
			get { return base.Editable; }
			set { Control.Editable = value; }
		}

		public DrawableCellHandler()
		{
			Control = new EtoCell { Handler = this, Enabled = true };
		}

		public override void SetBackgroundColor(NSCell cell, Color color)
		{
			var c = (EtoCell)cell;
			c.BackgroundColor = color;
			c.DrawsBackground = color != Colors.Transparent;
		}

		public override Color GetBackgroundColor(NSCell cell)
		{
			return ((EtoCell)cell).BackgroundColor;
		}

		public override void SetForegroundColor(NSCell cell, Color color)
		{
		}

		public override Color GetForegroundColor(NSCell cell)
		{
			return Colors.Transparent;
		}

		public override NSObject GetObjectValue(object dataItem)
		{
			return new EtoCellValue { Item = dataItem };
		}

		public override void SetObjectValue(object dataItem, NSObject value)
		{
		}

		public override nfloat GetPreferredSize(object value, CGSize cellSize, NSCell cell)
		{
			return 10f;
		}
	}
}

