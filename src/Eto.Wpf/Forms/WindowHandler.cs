using System;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Wpf.Forms
{
	
	public class WindowHandler : Window.IWindowHandler
	{
		internal static readonly object MovableByWindowBackground_Key = new object();

		public Window FromPoint(PointF point)
		{
			var screenPoint = point.LogicalToScreen();
			var windowHandle = Win32.WindowFromPoint(new Win32.POINT(screenPoint.X, screenPoint.Y));
			if (windowHandle == IntPtr.Zero)
				return null;
			windowHandle = Win32.GetAncestor(windowHandle, Win32.GA.GA_ROOT);
			
			foreach (var window in Application.Instance.Windows)
			{
				if (window.NativeHandle == windowHandle)
				{
					return window;
				}
			}
			return null;
		}
	}
}