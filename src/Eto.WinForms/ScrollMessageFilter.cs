namespace Eto.WinForms
{
	class ScrollMessageFilter : swf.IMessageFilter
	{
		public static bool IsScrollable(swf.Control control)
		{
			var p = control as swf.ScrollableControl;
			if (p != null)
				return p.AutoScroll;
			return control is swf.DataGridView
#if NETFRAMEWORK
			|| control is swf.DataGrid
#endif
			|| control is swf.TreeView
			|| control is swf.ListControl
			|| control is swf.RichTextBox;
		}

		public bool PreFilterMessage(ref swf.Message m)
		{
			if (m.Msg == (int)Win32.WM.MOUSEWHEEL)
			{
				var c = swf.Control.FromHandle(m.HWnd);
				if (c == null)
					return false;
				while (c.Parent != null)
					c = c.Parent;

				var cp = swf.Cursor.Position;
				swf.Control scrollChild = null;
				while (c != null)
				{
					if (IsScrollable(c))
						scrollChild = c;

					if (!c.HasChildren)
						break;

					c = c.GetChildAtPoint(c.PointToClient(cp), swf.GetChildAtPointSkip.Invisible);
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
