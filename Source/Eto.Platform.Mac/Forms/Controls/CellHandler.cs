using System;
using MonoMac.AppKit;
using Eto.Forms;
using MonoMac.Foundation;

namespace Eto.Platform.Mac.Forms.Controls
{
	public interface ICellHandler
	{
		NSCell Control { get; }

		NSObject GetObjectValue (object val);

		object SetObjectValue (NSObject val);
		
		float GetPreferredSize(object value, System.Drawing.SizeF cellSize);
	}
	
	public abstract class CellHandler<T, W> : WidgetHandler<T, W>, ICell, ICellHandler
		where T: NSCell
		where W: Cell
	{
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

