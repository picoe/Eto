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
					Control = swi.Cursors.Arrow;
					break;
				case CursorType.Crosshair:
					Control = swi.Cursors.Cross;
					break;
				case CursorType.Default:
					Control = swi.Cursors.Arrow;
					break;
				case CursorType.HorizontalSplit:
					Control = swi.Cursors.ScrollWE;
					break;
				case CursorType.IBeam:
					Control = swi.Cursors.IBeam;
					break;
				case CursorType.Move:
					Control = swi.Cursors.SizeAll;
					break;
				case CursorType.Pointer:
					Control = swi.Cursors.Hand;
					break;
				case CursorType.VerticalSplit:
					Control = swi.Cursors.SizeNS;
					break;
				default:
					throw new NotSupportedException ();
			}
		}
	}
}

