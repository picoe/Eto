using System;
using Eto.Forms;

namespace Eto.Platform.GtkSharp.Forms
{
	public class CursorHandler : WidgetHandler<Gdk.Cursor, Cursor>, ICursor
	{
		
		public void Create (CursorType cursor)
		{
			Control = new Gdk.Cursor(cursor.ToGdk ());
		}
	}
}

