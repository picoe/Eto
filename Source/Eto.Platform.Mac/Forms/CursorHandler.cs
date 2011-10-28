using System;
using MonoMac.AppKit;
using Eto.Forms;

namespace Eto.Platform.Mac.Forms
{
	public class CursorHandler : WidgetHandler<NSCursor, Cursor>, ICursor
	{
		public void Create (CursorType cursor)
		{
			switch (cursor) {
			case CursorType.Arrow:
				this.Control = NSCursor.ArrowCursor;
				break;
			case CursorType.Crosshair:
				this.Control = NSCursor.CrosshairCursor;
				break;
			case CursorType.Default:
				this.Control = NSCursor.CurrentSystemCursor;
				break;
			case CursorType.HorizontalSplit:
				this.Control = NSCursor.ResizeLeftRightCursor;
				break;
			case CursorType.IBeam:
				this.Control = NSCursor.IBeamCursor;
				break;
			case CursorType.Move:
				this.Control = NSCursor.OpenHandCursor;
				break;
			case CursorType.Pointer:
				this.Control = NSCursor.PointingHandCursor;
				break;
			case CursorType.VerticalSplit:
				this.Control = NSCursor.ResizeUpDownCursor;
				break;
			default:
				throw new NotSupportedException();
			}
		}
	}
}

