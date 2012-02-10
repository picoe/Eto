using System;
using Eto.Forms;
using sw = System.Windows;
using swi = System.Windows.Input;

namespace Eto.Platform.Wpf.Forms
{
	public class CursorHandler : WidgetHandler<swi.Cursor, Cursor>, ICursor
	{
		public void Create (CursorType cursor)
		{
			switch (cursor) {
				case CursorType.Arrow:
					this.Control = swi.Cursors.Arrow;
					break;
				case CursorType.Crosshair:
					this.Control = swi.Cursors.Cross;
					break;
				case CursorType.Default:
					this.Control = swi.Cursors.Arrow;
					break;
				case CursorType.HorizontalSplit:
					this.Control = swi.Cursors.ScrollWE;
					break;
				case CursorType.IBeam:
					this.Control = swi.Cursors.IBeam;
					break;
				case CursorType.Move:
					this.Control = swi.Cursors.SizeAll;
					break;
				case CursorType.Pointer:
					this.Control = swi.Cursors.Hand;
					break;
				case CursorType.VerticalSplit:
					this.Control = swi.Cursors.SizeNS;
					break;
				default:
					throw new NotSupportedException ();
			}
		}
	}
}

