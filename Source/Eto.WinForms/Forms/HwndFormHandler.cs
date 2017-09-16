using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using Eto.Drawing;
using sd = System.Drawing;
using swf = System.Windows.Forms;

#if WPF
using swi = System.Windows.Interop;

namespace Eto.Wpf.Forms
#elif WINFORMS

namespace Eto.WinForms.Forms
#endif
{
	public class HwndFormHandler : WidgetHandler<IntPtr, Form>, Form.IHandler,
#if WPF
 IWpfWindow
#elif WINFORMS
    IWindowHandler
#endif
	{
#if WPF
		System.Windows.Window IWpfWindow.Control
		{
			get { throw new NotImplementedException(); }
		}
		public void SetOwnerFor(System.Windows.Window child)
		{
			new swi.WindowInteropHelper(child).Owner = Control;
		}
#elif WINFORMS

    class Win32WindowImplementation : swf.IWin32Window
    {
      public Win32WindowImplementation(IntPtr hWnd)
      {
        Handle = hWnd;
      }

      public IntPtr Handle { get; private set; }
    }

    public swf.ToolTip ToolTips
    {
      get { throw new NotImplementedException(); }
    }

    public swf.IWin32Window Win32Window
    {
      get { return new Win32WindowImplementation(Control); }
    }

#endif

		public HwndFormHandler(IntPtr hWnd)
		{
			Control = hWnd;
		}

		public void Show()
		{
			throw new NotImplementedException();
		}

		public Eto.Forms.ToolBar ToolBar
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public void Close()
		{
			throw new NotImplementedException();
		}

		public float LogicalPixelSize
		{
			get
			{
				if (Win32.PerMonitorDpiSupported)
					return Win32.GetWindowDpi(Control) / 96.0f;
				var screen = swf.Screen.FromHandle(Control);
				if (screen == null)
					return 1;
				return screen.GetDpi() / 96f;
			}
		}

		public Eto.Drawing.Point Location
		{
			get
			{
				Win32.RECT rect;
				Win32.GetWindowRect(Control, out rect);
				var location = new PointF(rect.left, rect.top);
				return Point.Round(location / (float)LogicalPixelSize);
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public double Opacity
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public string Title
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public Screen Screen
		{
			get { throw new NotImplementedException(); }
		}

		public MenuBar Menu
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public Eto.Drawing.Icon Icon
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public bool Resizable
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public bool Maximizable
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public bool Minimizable
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public bool ShowInTaskbar
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public bool Topmost
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public WindowState WindowState
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public Eto.Drawing.Rectangle RestoreBounds
		{
			get { throw new NotImplementedException(); }
		}

		public WindowStyle WindowStyle
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public void BringToFront()
		{
			throw new NotImplementedException();
		}

		public void SendToBack()
		{
			throw new NotImplementedException();
		}

		public Control Content
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public Eto.Drawing.Padding Padding
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public Eto.Drawing.Size MinimumSize
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public Eto.Drawing.Size ClientSize
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public bool RecurseToChildren
		{
			get { throw new NotImplementedException(); }
		}

		public Eto.Drawing.Color BackgroundColor
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public Eto.Drawing.Size Size
		{
			get
			{
				Win32.RECT rect;
				Win32.GetWindowRect(Control, out rect);
				var size = new SizeF(rect.right - rect.left, rect.bottom - rect.top);
				return Size.Round(size / (float)LogicalPixelSize);
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public bool Enabled
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public void Invalidate(bool invalidateChildren)
		{
			throw new NotImplementedException();
		}

		public void Invalidate(Eto.Drawing.Rectangle rect, bool invalidateChildren)
		{
			throw new NotImplementedException();
		}

		public void SuspendLayout()
		{
			throw new NotImplementedException();
		}

		public void ResumeLayout()
		{
			throw new NotImplementedException();
		}

		public void Focus()
		{
			throw new NotImplementedException();
		}

		public bool HasFocus
		{
			get { throw new NotImplementedException(); }
		}

		public bool Visible
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public void OnPreLoad(EventArgs e)
		{
			throw new NotImplementedException();
		}

		public void OnLoad(EventArgs e)
		{
			throw new NotImplementedException();
		}

		public void OnLoadComplete(EventArgs e)
		{
			throw new NotImplementedException();
		}

		public void OnUnLoad(EventArgs e)
		{
			throw new NotImplementedException();
		}

		public void SetParent(Container parent)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<string> SupportedPlatformCommands
		{
			get { throw new NotImplementedException(); }
		}

		public void MapPlatformCommand(string systemCommand, Command command)
		{
			throw new NotImplementedException();
		}

		public Eto.Drawing.PointF PointFromScreen(Eto.Drawing.PointF point)
		{
			throw new NotImplementedException();
		}

		public Eto.Drawing.PointF PointToScreen(Eto.Drawing.PointF point)
		{
			throw new NotImplementedException();
		}

		public void SetOwner(Window owner)
		{
			throw new NotImplementedException();
		}

		public string ToolTip
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public Cursor Cursor
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public ContextMenu ContextMenu
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public bool ShowActivated
		{
			get
			{
				throw new NotImplementedException();
			}

			set
			{
				throw new NotImplementedException();
			}
		}

		public bool CanFocus
		{
			get
			{
				throw new NotImplementedException();
			}

			set
			{
				throw new NotImplementedException();
			}
		}

		public int TabIndex { get; set; }

		public IEnumerable<Control> VisualControls => Widget.Controls;
	}
}
