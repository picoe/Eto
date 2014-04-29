using Eto.Forms;

namespace Eto.GtkSharp.Forms
{
	public class CursorHandler : WidgetHandler<Gdk.Cursor, Cursor>, ICursor
	{
		
		public void Create (CursorType cursor)
		{
			Control = new Gdk.Cursor(cursor.ToGdk ());
		}
	}
}

