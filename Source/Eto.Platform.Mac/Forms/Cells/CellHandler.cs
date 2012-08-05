using System;
using MonoMac.AppKit;
using Eto.Forms;
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;
using Eto.Drawing;

namespace Eto.Platform.Mac.Forms.Controls
{
	public interface ICellHandler
	{
		NSCell Control { get; }
		
		IDataColumnHandler ColumnHandler { get; set; }

		NSObject GetObjectValue (object dataItem);

		void SetObjectValue (object dataItem, NSObject val);
		
		float GetPreferredSize (object value, System.Drawing.SizeF cellSize);
		
		void HandleEvent (string handler);

		void SetBackgroundColor (NSCell cell, Color color);

		Color GetBackgroundColor (NSCell cell);

		void SetForegroundColor (NSCell cell, Color color);

		Color GetForegroundColor (NSCell cell);

		bool Editable { get; set; }
	}
	
	public abstract class CellHandler<T, W> : MacObject<T, W>, ICell, ICellHandler
		where T: NSCell
		where W: Cell
	{
		public IDataColumnHandler ColumnHandler { get; set; }
		
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

		public abstract void SetBackgroundColor (NSCell cell, Color color);

		public abstract Color GetBackgroundColor (NSCell cell);

		public abstract void SetForegroundColor (NSCell cell, Color color);

		public abstract Color GetForegroundColor (NSCell cell);

		public abstract void SetObjectValue (object dataItem, NSObject value);
		
		public abstract float GetPreferredSize (object value, System.Drawing.SizeF cellSize);
	}
}

