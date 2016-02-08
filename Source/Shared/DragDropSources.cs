using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;

namespace Eto
{

	public static class DragDropSources
	{
		private static Dictionary<IntPtr, Control> dragSources = new Dictionary<IntPtr, Control>();

		public static void AddControl(IntPtr handle, Control control)
		{
			if (!dragSources.ContainsKey(handle))
			{
				dragSources.Add(handle, control);
			}
		}

		public static Control GetControl(IntPtr handle)
		{
			if (dragSources.ContainsKey(handle))
			{
				return dragSources[handle];
			}
			else
			{
				return null;
			}
		}

		public static void DeleteControl(IntPtr handle)
		{
			if (dragSources.ContainsKey(handle))
			{
				dragSources.Remove(handle);
			}
		}
	}
}
