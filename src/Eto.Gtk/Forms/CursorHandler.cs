using Eto.Forms;

namespace Eto.GtkSharp.Forms
{
	public class CursorHandler : WidgetHandler<Gdk.Cursor, Cursor>, Cursor.IHandler
	{
		
		public void Create (CursorType cursor)
		{
			Control = new Gdk.Cursor(cursor.ToGdk ());
		}
	}
}

