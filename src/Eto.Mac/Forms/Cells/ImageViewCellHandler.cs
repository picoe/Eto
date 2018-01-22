using System;
using Eto.Forms;
using Eto.Drawing;
using Eto.Mac.Drawing;
using Eto.Mac.Forms.Controls;

#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
#if Mac64
using nfloat = System.Double;
using nint = System.Int64;
using nuint = System.UInt64;
#else
using nfloat = System.Single;
using nint = System.Int32;
using nuint = System.UInt32;
#endif
#if SDCOMPAT
using CGSize = System.Drawing.SizeF;
using CGRect = System.Drawing.RectangleF;
using CGPoint = System.Drawing.PointF;
#endif
#endif

namespace Eto.Mac.Forms.Cells
{
	public class ImageViewCellHandler : CellHandler<ImageViewCell, ImageViewCell.ICallback>, ImageViewCell.IHandler
	{
		public class EtoCell : NSImageCell, IMacControl
		{
			public WeakReference WeakHandler { get; set; }

			public object Handler
			{ 
				get { return WeakHandler.Target; }
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

			public NSImageInterpolation ImageInterpolation { get; set; }

			public override void DrawInteriorWithFrame(CGRect cellFrame, NSView inView)
			{
				var nscontext = NSGraphicsContext.CurrentContext;

				if (DrawsBackground)
				{
					var context = nscontext.GraphicsPort;
					context.SetFillColor(BackgroundColor.ToCG());
					context.FillRect(cellFrame);
				}

				nscontext.ImageInterpolation = ImageInterpolation;

				base.DrawInteriorWithFrame(cellFrame, inView);
			}
		}

		public ImageViewCellHandler()
		{
			Control = new EtoCell { Handler = this, Enabled = true };
		}

		public override NSObject GetObjectValue(object dataItem)
		{
			if (Widget.Binding != null)
			{
				var img = Widget.Binding.GetValue(dataItem) as Image;
				if (img != null)
				{
					var imgHandler = ((IImageSource)img.Handler);
					return imgHandler.GetImage();
				}
			}
			return new NSImage();
		}

		public override void SetObjectValue(object dataItem, NSObject value)
		{
		}

		public override nfloat GetPreferredWidth(object value, CGSize cellSize, int row, object dataItem)
		{
			var img = value as Image;
			if (img != null)
			{
				return cellSize.Height / img.Size.Height * img.Size.Width;
			}
			return 16;
		}

		public ImageInterpolation ImageInterpolation { get; set; }

		public override Color GetBackgroundColor(NSView view)
		{
			return ((EtoImageView)view).BackgroundColor.ToEto();
		}

		public override void SetBackgroundColor(NSView view, Color color)
		{
			var field = ((EtoImageView)view);
			field.BackgroundColor = color.ToNSUI();
		}

		public override NSView GetViewForItem(NSTableView tableView, NSTableColumn tableColumn, int row, NSObject obj, Func<NSObject, int, object> getItem)
		{
			var view = tableView.MakeView(tableColumn.Identifier, tableView) as EtoImageView;
			if (view == null)
			{
				view = new EtoImageView { Identifier = tableColumn.Identifier };
			}
			var args = new MacCellFormatArgs(ColumnHandler.Widget, getItem(obj, row), row, view);
			ColumnHandler.DataViewHandler.OnCellFormatting(args);
			return view;
		}

		public class EtoImageView : NSImageView
		{
			public EtoImageView() { }

			public EtoImageView(IntPtr handle)
				: base(handle)
			{
			}

			[Export("backgroundColor")]
			public NSColor BackgroundColor { get; set; }

			public override void DrawRect(CGRect dirtyRect)
			{
				if (BackgroundColor != null && BackgroundColor.AlphaComponent > 0)
				{
					BackgroundColor.Set();
					NSGraphics.RectFill(dirtyRect);
				}
				base.DrawRect(dirtyRect);
			}
		}
	}
}

