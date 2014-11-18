using Eto.Forms;
using Eto.Drawing;
using System;
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
	public interface ICellHandler
	{
		NSCell Control { get; }

		Eto.Platform Platform { get; }
		
		IDataColumnHandler ColumnHandler { get; set; }

		NSObject GetObjectValue (object dataItem);

		void SetObjectValue (object dataItem, NSObject val);
		
		nfloat GetPreferredSize (object value, CGSize cellSize, int row, object dataItem);
		
		void HandleEvent (string handler, bool defaultEvent = false);

		void SetBackgroundColor (NSCell cell, Color color);

		Color GetBackgroundColor (NSCell cell);

		void SetForegroundColor (NSCell cell, Color color);

		Color GetForegroundColor (NSCell cell);

		bool Editable { get; set; }
	}
	
	public abstract class CellHandler<TControl, TWidget, TCallback> : MacObject<TControl, TWidget, TCallback>, Cell.IHandler, ICellHandler
		where TControl: NSCell
		where TWidget: Cell
	{
		public IDataColumnHandler ColumnHandler { get; set; }
		NSCell copy;
		
		NSCell ICellHandler.Control {
			get { return Control; }
		}

		public virtual bool Editable {
			get { return Control.Editable; }
			set {
				Control.Enabled = Control.Editable = value;
			}
		}
		
		public virtual NSObject GetObjectValue (object dataItem)
		{
			return null;
		}

		Eto.Platform ICellHandler.Platform
		{
			get { return Widget.Platform; }
		}

		public abstract void SetBackgroundColor (NSCell cell, Color color);

		public abstract Color GetBackgroundColor (NSCell cell);

		public abstract void SetForegroundColor (NSCell cell, Color color);

		public abstract Color GetForegroundColor (NSCell cell);

		public abstract void SetObjectValue (object dataItem, NSObject value);
		
		public abstract nfloat GetPreferredSize (object value, CGSize cellSize, NSCell cell);

		public nfloat GetPreferredSize (object value, CGSize cellSize, int row, object dataItem)
		{
			if (copy == null)
				copy = Control.Copy () as NSCell;
			ColumnHandler.DataViewHandler.OnCellFormatting (ColumnHandler.Widget, dataItem, row, copy);
			return GetPreferredSize (value, cellSize, copy);
		}

	}
}

