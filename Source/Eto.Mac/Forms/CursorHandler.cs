using System;
using Eto.Forms;
#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
#endif

namespace Eto.Mac.Forms
{
	public class CursorHandler : WidgetHandler<NSCursor, Cursor>, Cursor.IHandler
	{
		public void Create(CursorType cursor)
		{
			switch (cursor)
			{
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
					Control = NSCursor.ResizeUpDownCursor;
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
					Control = NSCursor.ResizeLeftRightCursor;
					break;
				default:
					throw new NotSupportedException();
			}
		}
	}
}

