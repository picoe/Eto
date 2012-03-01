using System;
using MonoMac.AppKit;
using Eto.Forms;
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;

namespace Eto.Platform.Mac.Forms.Controls
{
	public interface ICellHandler
	{
		NSCell Control { get; }
		
		IDataColumnHandler ColumnHandler { get; set; }

		NSObject GetObjectValue (object val);

		object SetObjectValue (NSObject val);
		
		float GetPreferredSize (object value, System.Drawing.SizeF cellSize);
		
		void HandleEvent (string handler);
	}
	
	public abstract class CellHandler<T, W> : MacObject<T, W>, ICell, ICellHandler
		where T: NSCell
		where W: Cell
	{
		public IDataColumnHandler ColumnHandler { get; set; }
		
		NSCell ICellHandler.Control {
			get { return Control; }
		}
		
		public virtual NSObject GetObjectValue (object val)
		{
			return NSObject.FromObject (val);
		}
		
		public abstract object SetObjectValue (NSObject val);
		
		public abstract float GetPreferredSize (object value, System.Drawing.SizeF cellSize);
	}
}

