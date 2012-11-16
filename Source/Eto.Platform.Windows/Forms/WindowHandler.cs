using System;
using System.ComponentModel;
using sd = System.Drawing;
using swf = System.Windows.Forms;
using Eto.Drawing;
using Eto.Forms;
using System.Linq;
using System.Runtime.InteropServices;

namespace Eto.Platform.Windows
{
	public interface IWindowHandler
	{
		swf.ToolTip ToolTips { get; }
	}

	public abstract class WindowHandler<T, W> : WindowsContainer<T, W>, IWindow, IWindowHandler
		where T: swf.Form
		where W: Window
	{
		MenuBar menu;
		Icon icon;
		ToolBar toolBar;
		swf.Panel menuHolder;
		swf.Panel content;
		swf.Panel toolbarHolder;
		swf.ToolTip tooltips = new swf.ToolTip();

		public override Size DesiredSize
		{
			get { return this.MinimumSize ?? this.Size; }
		}
		
		public swf.ToolTip ToolTips
		{
			get { return tooltips; }
		}

		public override swf.Control ContentContainer
		{
			get { return content; }
		}

		public override Size ClientSize
		{
			get
			{
				return Generator.Convert (Widget.Loaded ? content.Size : content.MinimumSize);
			}
			set
			{
				if (Widget.Loaded)
				{
					var size = Control.Size - content.Size;
					Control.Size = new sd.Size(value.Width + size.Width, value.Height + size.Height);
				}
				else
					content.MinimumSize = Generator.Convert (value);
			}
		}

		public override void Initialize ()
		{
			base.Initialize ();
			content = new swf.Panel {
				AutoSize = true,
				AutoSizeMode = swf.AutoSizeMode.GrowAndShrink,
				Dock = swf.DockStyle.Fill
			};
			Control.Controls.Add (content);

			toolbarHolder = new swf.Panel {
				Dock = swf.DockStyle.Top,
				AutoSizeMode = swf.AutoSizeMode.GrowAndShrink,
				AutoSize = true
			};
			Control.Controls.Add (toolbarHolder);

			menuHolder = new swf.Panel {
				AutoSizeMode = swf.AutoSizeMode.GrowAndShrink,
				AutoSize = true,
				Dock = swf.DockStyle.Top
			};
			Control.Controls.Add (menuHolder);

			Control.Load += (sender, e) => {
				content.MinimumSize = sd.Size.Empty;
			};

			// Always handle closing because we want to send Application.Terminating event
			HandleEvent (Window.ClosingEvent);
		}
		
		public override void AttachEvent (string handler)
		{
			switch (handler) {
			case Window.ClosedEvent:
				Control.FormClosed += delegate {
					Widget.OnClosed (EventArgs.Empty);
				};
				break;
			case Window.ClosingEvent:
				Control.FormClosing += delegate(object sender, swf.FormClosingEventArgs e) {
					var args = new CancelEventArgs(e.Cancel);
					Widget.OnClosing (args);
					
					if (!e.Cancel && swf.Application.OpenForms.Count <= 1 
						|| e.CloseReason == swf.CloseReason.ApplicationExitCall
						|| e.CloseReason == swf.CloseReason.WindowsShutDown)
					{
						Application.Instance.OnTerminating (args);
					}
		
					e.Cancel = args.Cancel;
				};
				break;
			case Window.ShownEvent:
				Control.Shown += delegate {
					Widget.OnShown (EventArgs.Empty);
				};
				break;
            case Window.ActivatedEvent:
                Control.Activated += delegate
                {
                    Widget.OnActivated(EventArgs.Empty);
                };
                break;
            case Window.MaximizedEvent:
				Control.Resize += delegate {
					if (Control.WindowState == swf.FormWindowState.Maximized) {
						Widget.OnMaximized (EventArgs.Empty);
					}
				};
				break;
			case Window.MinimizedEvent:
				Control.Resize += delegate {
					if (Control.WindowState == swf.FormWindowState.Minimized) {
						Widget.OnMaximized (EventArgs.Empty);
					}
				};
				break;
			default:
				base.AttachEvent (handler);
				break;
			}
		}

		public MenuBar Menu {
			get {
				return menu;
			}
			set {
				this.Control.SuspendLayout ();
				menuHolder.SuspendLayout ();
				if (menu != null)
					menuHolder.Controls.Remove ((swf.MenuStrip)menu.ControlObject);
				
				if (value == null) {
					Control.MainMenuStrip = null;
				} else {
					var c = (swf.MenuStrip)value.ControlObject;
					c.Dock = swf.DockStyle.Top;
					c.ResumeLayout ();
					menuHolder.Controls.Add (c);
					Control.MainMenuStrip = c;
				}
				menuHolder.ResumeLayout ();
				this.Control.ResumeLayout ();
				menuHolder.Update ();
				menu = value;
			}
		}
		
		public bool Resizable {
			get {
				if (Control.FormBorderStyle == swf.FormBorderStyle.Sizable)
					return true;
				else
					return false;
			}
			set {
				if (value) {
					Control.FormBorderStyle = swf.FormBorderStyle.Sizable;
				} else {
					Control.FormBorderStyle = swf.FormBorderStyle.FixedDialog;
				}
			}
		}
		
		public ToolBar ToolBar {
			get {
				return this.toolBar;
			}
			set {
				this.Control.SuspendLayout ();
				toolbarHolder.SuspendLayout();
				if (toolBar != null)
					toolbarHolder.Controls.Remove ((swf.Control)toolBar.ControlObject);
				toolBar = value;
				if (toolBar != null) {
					var c = ((swf.Control)toolBar.ControlObject);
					c.Dock = swf.DockStyle.Top;
					c.ResumeLayout ();
					toolbarHolder.Controls.Add (c);
				}
				toolbarHolder.ResumeLayout ();
				this.Control.ResumeLayout ();
				toolbarHolder.Update ();
			}
		}
		
		public void AddToolbar (ToolBar toolBar)
		{
			Control.Controls.Add ((swf.Control)toolBar.ControlObject);
		}

		public void RemoveToolbar (ToolBar toolBar)
		{
			Control.Controls.Remove ((swf.Control)toolBar.ControlObject);
		}

		public void ClearToolbars ()
		{
			foreach (swf.Control c in Control.Controls) {
				if (c is swf.ToolBar)
					Control.Controls.Remove (c);
			}
		}

		public void Close ()
		{
			Control.Close ();
		}
		
		public Icon Icon {
			get { return icon; }
			set {
				icon = value;
				Control.Icon = (sd.Icon)icon.ControlObject;
			}
		}

		public string Title {
			get { return Control.Text; }
			set { Control.Text = value; }
		}
		
		public Point Location {
			get {
				return Generator.Convert (Control.Location);
			}
			set {
				Control.Location = Generator.Convert (value);
				Control.StartPosition = swf.FormStartPosition.Manual;
			}
		}
		
		public WindowState State {
			get {
				switch (Control.WindowState) {
				case swf.FormWindowState.Maximized:
					return WindowState.Maximized;
				case swf.FormWindowState.Minimized:
					return WindowState.Minimized;
				case swf.FormWindowState.Normal:
					return WindowState.Normal;
				default:
					throw new NotSupportedException ();
				}
			}
			set {
				switch (value) {
				case WindowState.Maximized:
					Control.WindowState = System.Windows.Forms.FormWindowState.Maximized;
					break;
				case WindowState.Minimized:
					Control.WindowState = System.Windows.Forms.FormWindowState.Minimized;
					break;
				case WindowState.Normal:
					Control.WindowState = System.Windows.Forms.FormWindowState.Normal;
					break;
				default:
					throw new NotSupportedException ();
				}
			}
		}
		
		public Rectangle? RestoreBounds {
			get {
				if (this.State == WindowState.Normal || Control.RestoreBounds.IsEmpty) return null;
				else return Generator.Convert (Control.RestoreBounds);
			}
		}

		public double Opacity
		{
			get
			{
				return Control.AllowTransparency ? Control.Opacity : 1.0;
			}
			set
			{
				Control.AllowTransparency = value != 1.0;
				Control.Opacity = value;
			}
		}


        public void RemoveBorder()
        {
            this.Control.FormBorderStyle = swf.FormBorderStyle.None;
        }

        public void BringToFront()
        {
            // This is more reliable than 
            // this.Control.BringToFront()
            /*var prev = this.Control.TopMost;
            this.Control.TopMost = true;
            this.Control.TopMost = prev;*/
            NativeWindowMethods.SetWindowPos(
                this.Control.Handle,
                NativeWindowMethods.HWND_TOPMOST, 0, 0, 0, 0,
                NativeWindowMethods.SetWindowPosFlags.DoNotActivate |
                NativeWindowMethods.SetWindowPosFlags.ShowWindow |
                NativeWindowMethods.SetWindowPosFlags.IgnoreResize |
                NativeWindowMethods.SetWindowPosFlags.IgnoreMove);

            NativeWindowMethods.SetWindowPos(
                this.Control.Handle,
                NativeWindowMethods.HWND_NOTOPMOST, 0, 0, 0, 0,
                NativeWindowMethods.SetWindowPosFlags.DoNotActivate |
                NativeWindowMethods.SetWindowPosFlags.ShowWindow |
                NativeWindowMethods.SetWindowPosFlags.IgnoreResize |
                NativeWindowMethods.SetWindowPosFlags.IgnoreMove);

        }
    }

    internal static class NativeWindowMethods
    {
        [Flags()]
        public enum SetWindowPosFlags : uint
        {
            /// <summary>If the calling thread and the thread that owns the window are attached to different input queues, 
            /// the system posts the request to the thread that owns the window. This prevents the calling thread from
            /// blocking its execution while other threads process the request.</summary>
            /// <remarks>SWP_ASYNCWINDOWPOS</remarks>
            AsynchronousWindowPosition = 0x4000,
            /// <summary>Prevents generation of the WM_SYNCPAINT message.</summary>
            /// <remarks>SWP_DEFERERASE</remarks>
            DeferErase = 0x2000,
            /// <summary>Draws a frame (defined in the window's class description) around the window.</summary>
            /// <remarks>SWP_DRAWFRAME</remarks>
            DrawFrame = 0x0020,
            /// <summary>Applies new frame styles set using the SetWindowLong function. Sends a WM_NCCALCSIZE message to 
            /// the window, even if the window's size is not being changed. If this flag is not specified, WM_NCCALCSIZE 
            /// is sent only when the window's size is being changed.</summary>
            /// <remarks>SWP_FRAMECHANGED</remarks>
            FrameChanged = 0x0020,
            /// <summary>Hides the window.</summary>
            /// <remarks>SWP_HIDEWINDOW</remarks>
            HideWindow = 0x0080,
            /// <summary>Does not activate the window. If this flag is not set, the window is activated and moved to the 
            /// top of either the topmost or non-topmost group (depending on the setting of the hWndInsertAfter 
            /// parameter).</summary>
            /// <remarks>SWP_NOACTIVATE</remarks>
            DoNotActivate = 0x0010,
            /// <summary>Discards the entire contents of the client area. If this flag is not specified, the valid 
            /// contents of the client area are saved and copied back into the client area after the window is sized or 
            /// repositioned.</summary>
            /// <remarks>SWP_NOCOPYBITS</remarks>
            DoNotCopyBits = 0x0100,
            /// <summary>Retains the current position (ignores X and Y parameters).</summary>
            /// <remarks>SWP_NOMOVE</remarks>
            IgnoreMove = 0x0002,
            /// <summary>Does not change the owner window's position in the Z order.</summary>
            /// <remarks>SWP_NOOWNERZORDER</remarks>
            DoNotChangeOwnerZOrder = 0x0200,
            /// <summary>Does not redraw changes. If this flag is set, no repainting of any kind occurs. This applies to 
            /// the client area, the nonclient area (including the title bar and scroll bars), and any part of the parent 
            /// window uncovered as a result of the window being moved. When this flag is set, the application must 
            /// explicitly invalidate or redraw any parts of the window and parent window that need redrawing.</summary>
            /// <remarks>SWP_NOREDRAW</remarks>
            DoNotRedraw = 0x0008,
            /// <summary>Same as the SWP_NOOWNERZORDER flag.</summary>
            /// <remarks>SWP_NOREPOSITION</remarks>
            DoNotReposition = 0x0200,
            /// <summary>Prevents the window from receiving the WM_WINDOWPOSCHANGING message.</summary>
            /// <remarks>SWP_NOSENDCHANGING</remarks>
            DoNotSendChangingEvent = 0x0400,
            /// <summary>Retains the current size (ignores the cx and cy parameters).</summary>
            /// <remarks>SWP_NOSIZE</remarks>
            IgnoreResize = 0x0001,
            /// <summary>Retains the current Z order (ignores the hWndInsertAfter parameter).</summary>
            /// <remarks>SWP_NOZORDER</remarks>
            IgnoreZOrder = 0x0004,
            /// <summary>Displays the window.</summary>
            /// <remarks>SWP_SHOWWINDOW</remarks>
            ShowWindow = 0x0040,
        }

        [DllImport("user32.dll")]
        private static extern int ShowWindow(IntPtr hWnd, uint Msg);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(
            IntPtr hWnd,
            IntPtr hWndInsertAfter,
            int X,
            int Y,
            int cx,
            int cy,
            SetWindowPosFlags uFlags);

        public static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        public static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        public static readonly IntPtr HWND_TOP = new IntPtr(0);
        public static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
    }
}
