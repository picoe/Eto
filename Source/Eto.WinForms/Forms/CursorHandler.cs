using System;
using SWF = System.Windows.Forms;
using Eto.Forms;

namespace Eto.WinForms.Forms
{
	public class CursorHandler : WidgetHandler<SWF.Cursor, Cursor>, Cursor.IHandler
	{
		public void Create (CursorType cursor)
		{
			switch (cursor) {
			case CursorType.Arrow:
				Control = SWF.Cursors.Arrow;
				break;
			case CursorType.Crosshair:
				Control = SWF.Cursors.Cross;
				break;
			case CursorType.Default:
				Control = SWF.Cursors.Default;
				break;
			case CursorType.HorizontalSplit:
				Control = SWF.Cursors.HSplit;
				break;
			case CursorType.IBeam:
				Control = SWF.Cursors.IBeam;
				break;
			case CursorType.Move:
				Control = SWF.Cursors.SizeAll;
				break;
			case CursorType.Pointer:
				Control = SWF.Cursors.Hand;
				break;
			case CursorType.VerticalSplit:
				Control = SWF.Cursors.VSplit;
				break;
			default:
				throw new NotSupportedException();
			}
		}
	}
}

