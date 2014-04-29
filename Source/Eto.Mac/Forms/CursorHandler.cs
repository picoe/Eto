using System;
using MonoMac.AppKit;
using Eto.Forms;

namespace Eto.Mac.Forms
{
	public class CursorHandler : WidgetHandler<NSCursor, Cursor>, ICursor
	{
		public void Create (CursorType cursor)
		{
			switch (cursor) {
			case CursorType.Arrow:
				Control = NSCursor.ArrowCursor;
				break;
			case CursorType.Crosshair:
				Control = NSCursor.CrosshairCursor;
				break;
			case CursorType.Default:
				Control = NSCursor.CurrentSystemCursor;
				break;
			case CursorType.HorizontalSplit:
				Control = NSCursor.ResizeLeftRightCursor;
				break;
			case CursorType.IBeam:
				Control = NSCursor.IBeamCursor;
				break;
			case CursorType.Move:
				Control = NSCursor.OpenHandCursor;
				break;
			case CursorType.Pointer:
				Control = NSCursor.PointingHandCursor;
				break;
			case CursorType.VerticalSplit:
				Control = NSCursor.ResizeUpDownCursor;
				break;
			default:
				throw new NotSupportedException();
			}
		}
	}
}

