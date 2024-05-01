#if WPF

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
			new swin.WindowInteropHelper(child).Owner = Control;
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

		public override IntPtr NativeHandle => Control;

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
				return SwfScreen?.GetLogicalPixelSize() ?? 1;
			}
		}

		swf.Screen SwfScreen => swf.Screen.FromHandle(Control);

		public Eto.Drawing.Point Location
		{
			get
			{
				var rect = new Win32.RECT();
				Win32.ExecuteInDpiAwarenessContext(() => Win32.GetWindowRect(Control, out rect));

				var location = new Point(rect.left, rect.top);
				return Point.Round(location.ScreenToLogical(SwfScreen));
			}
			set
			{
				var loc = ((PointF)value).LogicalToScreen();
				Win32.ExecuteInDpiAwarenessContext(() => Win32.SetWindowPos(Control, IntPtr.Zero, loc.X, loc.Y, 0, 0, Win32.SWP.NOSIZE | Win32.SWP.NOACTIVATE));
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
			get => Win32.GetWindowText(Control);
			set
			{
				throw new NotImplementedException();
			}
		}

		public Screen Screen
		{
			get
			{
				var swfscreen = swf.Screen.FromHandle(Control);
				if (swfscreen == null)
					return null;
				return new Screen(new ScreenHandler(swfscreen));
			}
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
				return (Win32.GetWindowLong(Control, Win32.GWL.STYLE) & (uint)Win32.WS.SIZEBOX) != 0;
			}
			set
			{
				var styleInt = Win32.GetWindowLong(Control, Win32.GWL.STYLE);
				if (value)
					styleInt |= (uint)Win32.WS.SIZEBOX;
				else
					styleInt &= (uint)~Win32.WS.SIZEBOX;

				Win32.SetWindowLong(Control, Win32.GWL.STYLE, styleInt);
			}
		}

		public bool Maximizable
		{
			get
			{
				return (Win32.GetWindowLong(Control, Win32.GWL.STYLE) & (uint)Win32.WS.MAXIMIZEBOX) != 0;
			}
			set
			{
				var styleInt = Win32.GetWindowLong(Control, Win32.GWL.STYLE);
				if (value)
					styleInt |= (uint)Win32.WS.MAXIMIZEBOX;
				else
					styleInt &= (uint)~Win32.WS.MAXIMIZEBOX;

				Win32.SetWindowLong(Control, Win32.GWL.STYLE, styleInt);
			}
		}

		public bool Minimizable
		{
			get
			{
				return (Win32.GetWindowLong(Control, Win32.GWL.STYLE) & (uint)Win32.WS.MINIMIZEBOX) != 0;
			}
			set
			{
				var styleInt = Win32.GetWindowLong(Control, Win32.GWL.STYLE);
				if (value)
					styleInt |= (uint)Win32.WS.MINIMIZEBOX;
				else
					styleInt &= (uint)~Win32.WS.MINIMIZEBOX;

				Win32.SetWindowLong(Control, Win32.GWL.STYLE, styleInt);
			}
		}

		public bool ShowInTaskbar
		{
			get
			{
				return (Win32.GetWindowLong(Control, Win32.GWL.EXSTYLE) & (uint) Win32.WS_EX.APPWINDOW) != 0;
			}
			set
			{
				var style = Win32.GetWindowLong(Control, Win32.GWL.EXSTYLE);
				if (value)
					style |= (uint)Win32.WS_EX.APPWINDOW;
				else
					style &= (uint)~Win32.WS_EX.APPWINDOW;

				Win32.SetWindowLong(Control, Win32.GWL.EXSTYLE, style);
			}
		}
		
		public virtual bool Closeable
		{
			get => ((Win32.WS)Win32.GetWindowLong(Control, Win32.GWL.STYLE)).HasFlag(Win32.WS.SYSMENU);
			set
			{
				var style = Win32.GetWindowLong(Control, Win32.GWL.STYLE);
				if (value)
					style |= (uint)Win32.WS.SYSMENU;
				else
					style &= (uint)~Win32.WS.SYSMENU;
				Win32.SetWindowLong(Control, Win32.GWL.STYLE, style);
			}
		}
		

		public bool Topmost
		{
			get
			{
				return (Win32.GetWindowLong(Control, Win32.GWL.EXSTYLE) & (uint)Win32.WS_EX.TOPMOST) != 0;
			}
			set
			{
				var styleInt = Win32.GetWindowLong(Control, Win32.GWL.EXSTYLE);
				if (value)
					styleInt |= (uint)Win32.WS_EX.TOPMOST;
				else
					styleInt &= (uint)~Win32.WS_EX.TOPMOST;

				Win32.SetWindowLong(Control, Win32.GWL.EXSTYLE, styleInt);
			}
		}

		public WindowState WindowState
		{
			get
			{
				if (Win32.IsIconic(Control))
					return WindowState.Minimized;

				if (Win32.IsZoomed(Control))
					return WindowState.Maximized;

				return WindowState.Normal;
			}
			set
			{
				if (WindowState != value)
				{
					switch (value)
					{
						case WindowState.Normal:
							if (!Win32.ShowWindow(Control, Win32.SW.RESTORE))
								Win32.ShowWindow(Control, Win32.SW.HIDE);
							break;
						case WindowState.Maximized:
							if(!Win32.ShowWindow(Control, Win32.SW.MAXIMIZE))
								Win32.ShowWindow(Control, Win32.SW.HIDE);
							break;
						case WindowState.Minimized:
							if(!Win32.ShowWindow(Control, Win32.SW.MINIMIZE))
								Win32.ShowWindow(Control, Win32.SW.HIDE);
							break;
					}
				}
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

		public virtual int Width
		{
			get => Size.Width;
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual int Height
		{
			get => Size.Height;
			set
			{
				throw new NotImplementedException();
			}
		}

		public bool Enabled
		{
			get
			{
				return Win32.IsWindowEnabled(Control);
			}
			set
			{
				Win32.EnableWindow(Control, value);
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
		}

		public void ResumeLayout()
		{
		}

		public void Focus() => Win32.SetActiveWindow(Control);

		public bool HasFocus => Win32.GetActiveWindow() == Control;

		public bool Visible
		{
			get => Win32.IsWindowVisible(Control);
			set => Win32.ShowWindow(Control, value ? Win32.SW.SHOWNA : Win32.SW.HIDE);
		}

		public void OnPreLoad(EventArgs e)
		{
		}

		public void OnLoad(EventArgs e)
		{
		}

		public void OnLoadComplete(EventArgs e)
		{
		}

		public void OnUnLoad(EventArgs e)
		{
		}

		public void SetParent(Container oldParent, Container newParent)
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

		public bool AllowDrop
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

		public bool MovableByWindowBackground { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool AutoSize { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public void DoDragDrop(DataObject data, DragEffects allowedAction, Image image, PointF offset)
		{
			throw new NotImplementedException();
		}

		public Window GetNativeParentWindow()
		{
			throw new NotImplementedException();
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Window.LogicalPixelSizeChangedEvent:
					// don't spam the output with warnings for this, many controls use it internally
					break;
				default:
					base.AttachEvent(id);
					break;
			}

		}

		public SizeF GetPreferredSize(SizeF availableSize)
		{
			return Size;
		}

		public void Print() => throw new NotImplementedException();

		public void UpdateLayout() => throw new NotImplementedException();

		public bool IsMouseCaptured => throw new NotImplementedException();
		public bool CaptureMouse() => throw new NotImplementedException();
		public void ReleaseMouseCapture() => throw new NotImplementedException();
	}
}
