using System;
using MonoMac.AppKit;
using Eto.Forms;
using Eto.Drawing;
using Eto.Mac.Drawing;
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;
using MonoMac.CoreGraphics;
using sd = System.Drawing;

namespace Eto.Mac.Forms.Controls
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
				val.Retain();
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

			public override sd.SizeF CellSizeForBounds(sd.RectangleF bounds)
			{
				return sd.SizeF.Empty;
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

			public override void DrawInteriorWithFrame(sd.RectangleF cellFrame, NSView inView)
			{
				var nscontext = NSGraphicsContext.CurrentContext;

				if (DrawsBackground)
				{
					var context = nscontext.GraphicsPort;
					context.SetFillColor(BackgroundColor.ToCGColor());
					context.FillRect(cellFrame);
				}

				var handler = Handler;
				var graphicsHandler = new GraphicsHandler(null, nscontext, cellFrame.Height, flipped: true);
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

		public override float GetPreferredSize(object value, sd.SizeF cellSize, NSCell cell)
		{
			return 10f;
		}
	}
}

