using System;
using swf = System.Windows.Forms;
using sd = System.Drawing;
using Eto.Forms;
using Eto.Drawing;
using System.IO;
using System.Runtime.InteropServices;

namespace Eto.WinForms.Forms
{
	public class CursorHandler : WidgetHandler<swf.Cursor, Cursor>, Cursor.IHandler
	{
		public void Create (CursorType cursor)
		{
			switch (cursor) {
			case CursorType.Arrow:
				Control = swf.Cursors.Arrow;
				break;
			case CursorType.Crosshair:
				Control = swf.Cursors.Cross;
				break;
			case CursorType.Default:
				Control = swf.Cursors.Default;
				break;
			case CursorType.HorizontalSplit:
				Control = swf.Cursors.HSplit;
				break;
			case CursorType.IBeam:
				Control = swf.Cursors.IBeam;
				break;
			case CursorType.Move:
				Control = swf.Cursors.SizeAll;
				break;
			case CursorType.Pointer:
				Control = swf.Cursors.Hand;
				break;
			case CursorType.VerticalSplit:
				Control = swf.Cursors.VSplit;
				break;
			default:
				throw new NotSupportedException();
			}
		}

		public struct IconInfo
		{
			public bool fIcon;
			public int xHotspot;
			public int yHotspot;
			public IntPtr hbmMask;
			public IntPtr hbmColor;
		}
		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetIconInfo(IntPtr hIcon, ref IconInfo pIconInfo);

		[DllImport("user32.dll")]
		public static extern IntPtr CreateIconIndirect(ref IconInfo icon);

		[DllImport("user32.dll", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
		private static extern IntPtr LoadCursorFromFile(String str);

		public void Create(Image image, PointF hotspot)
		{
			if (image.ControlObject is sd.Icon ico)
			{
				IntPtr ptr = ico.Handle;
				IconInfo tmp = new IconInfo();
				GetIconInfo(ptr, ref tmp);
				tmp.xHotspot = (int)hotspot.X;
				tmp.yHotspot = (int)hotspot.Y;
				tmp.fIcon = false;
				ptr = CreateIconIndirect(ref tmp);
				Control = new swf.Cursor(ptr);
			}
			else if (image.ToSD() is sd.Bitmap bmp)
			{
				IntPtr ptr = bmp.GetHicon();
				IconInfo tmp = new IconInfo();
				GetIconInfo(ptr, ref tmp);
				tmp.xHotspot = (int)hotspot.X;
				tmp.yHotspot = (int)hotspot.Y;
				tmp.fIcon = false;
				ptr = CreateIconIndirect(ref tmp);
				Control = new swf.Cursor(ptr);
			}
			else throw new NotSupportedException("Bitmap must be backed by a System.Drawing.Bitmap");
		}

		public void Create(string fileName)
		{
			// using Cursor constructor doesn't support 32-bit cursors.
			IntPtr ptr = LoadCursorFromFile(fileName);
			Control = new swf.Cursor(ptr);
		}

		public void Create(Stream stream)
		{
			// using Cursor constructor doesn't support 32-bit cursors
			// so we save to a temp file and use LoadCursorFromFile.
			var tmp = Path.GetTempFileName();
			try
			{
				using (var tmpStream = File.Create(tmp))
				{
					stream.CopyTo(tmpStream);
				}
				Create(tmp);
			}
			finally
			{
				if (File.Exists(tmp))
					File.Delete(tmp);
			}
		}
	}
}

