using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Eto.Platform.Windows
{
	class ScrollMessageFilter : IMessageFilter
	{
		int WM_MOUSEWHEEL = 0x20A;

		[DllImport ("User32.dll")]
		static extern Int32 SendMessage (IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

		public bool PreFilterMessage (ref Message m)
		{
			if (m.Msg == WM_MOUSEWHEEL) {
				var c = Control.FromHandle(m.HWnd);
				if (c == null) return false;
				while (c.Parent != null) c = c.Parent;

				var cp = Cursor.Position;
				Control scrollChild = null;
				while (c != null && c.HasChildren)
				{
					var p = c as Panel;
					if (p != null && p.AutoScroll)
						scrollChild = c;

					c = c.GetChildAtPoint (c.PointToClient (cp));
				} 
				
				if (scrollChild == null) return false;
				SendMessage (scrollChild.Handle, m.Msg, m.WParam, m.LParam);

				return true;
			}
			return false;			
		}
	}
}
