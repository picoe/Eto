using System.Windows.Forms;

namespace Eto.WinForms
{
	class ScrollMessageFilter : IMessageFilter
	{
		public static bool IsScrollable(Control control)
		{
			var p = control as ScrollableControl;
			if (p != null)
				return p.AutoScroll;
			return control is DataGridView
#if !NETCOREAPP3_1
			|| control is DataGrid
#endif
			|| control is TreeView
			|| control is ListControl
			|| control is RichTextBox;
		}

		public bool PreFilterMessage(ref Message m)
		{
			if (m.Msg == (int)Win32.WM.MOUSEWHEEL)
			{
				var c = Control.FromHandle(m.HWnd);
				if (c == null)
					return false;
				while (c.Parent != null)
					c = c.Parent;

				var cp = Cursor.Position;
				Control scrollChild = null;
				while (c != null)
				{
					if (IsScrollable(c))
						scrollChild = c;

					if (!c.HasChildren)
						break;

					c = c.GetChildAtPoint(c.PointToClient(cp), GetChildAtPointSkip.Invisible);
				}

				if (scrollChild == null)
					return false;
				Win32.SendMessage(scrollChild.Handle, (Win32.WM)m.Msg, m.WParam, m.LParam);

				return true;
			}
			return false;			
		}
	}
}
