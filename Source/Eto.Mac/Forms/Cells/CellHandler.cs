using MonoMac.AppKit;
using Eto.Forms;
using MonoMac.Foundation;
using Eto.Drawing;
#if !Mac64
using NSSize = System.Drawing.SizeF;
using NSRect = System.Drawing.RectangleF;
using NSPoint = System.Drawing.PointF;
#endif

namespace Eto.Mac.Forms.Controls
{
	public interface ICellHandler
	{
		NSCell Control { get; }

		Eto.Platform Platform { get; }
		
		IDataColumnHandler ColumnHandler { get; set; }

		NSObject GetObjectValue (object dataItem);

		void SetObjectValue (object dataItem, NSObject val);
		
		float GetPreferredSize (object value, NSSize cellSize, int row, object dataItem);
		
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
		
		public abstract float GetPreferredSize (object value, NSSize cellSize, NSCell cell);

		public float GetPreferredSize (object value, NSSize cellSize, int row, object dataItem)
		{
			if (copy == null)
				copy = Control.Copy () as NSCell;
			ColumnHandler.DataViewHandler.OnCellFormatting (ColumnHandler.Widget, dataItem, row, copy);
			return GetPreferredSize (value, cellSize, copy);
		}

	}
}

