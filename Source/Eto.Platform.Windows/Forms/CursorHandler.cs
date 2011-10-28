using System;
using SWF = System.Windows.Forms;
using Eto.Forms;

namespace Eto.Platform.Windows.Forms
{
	public class CursorHandler : WidgetHandler<SWF.Cursor, Cursor>, ICursor
	{
		public void Create (CursorType cursor)
		{
			switch (cursor) {
			case CursorType.Arrow:
				this.Control = SWF.Cursors.Arrow;
				break;
			case CursorType.Crosshair:
				this.Control = SWF.Cursors.Cross;
				break;
			case CursorType.Default:
				this.Control = SWF.Cursors.Default;
				break;
			case CursorType.HorizontalSplit:
				this.Control = SWF.Cursors.HSplit;
				break;
			case CursorType.IBeam:
				this.Control = SWF.Cursors.IBeam;
				break;
			case CursorType.Move:
				this.Control = SWF.Cursors.SizeAll;
				break;
			case CursorType.Pointer:
				this.Control = SWF.Cursors.Hand;
				break;
			case CursorType.VerticalSplit:
				this.Control = SWF.Cursors.VSplit;
				break;
			default:
				throw new NotSupportedException();
			}
		}
	}
}

