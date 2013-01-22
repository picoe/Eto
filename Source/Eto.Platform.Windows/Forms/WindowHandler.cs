using System;
using System.ComponentModel;
using sd = System.Drawing;
using swf = System.Windows.Forms;
using Eto.Drawing;
using Eto.Forms;
using System.Linq;
using Eto.Platform.Windows.Forms;

namespace Eto.Platform.Windows
{
	public interface IWindowHandler
	{
		swf.ToolTip ToolTips { get; }

		swf.IWin32Window Win32Window { get; }
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

		public swf.IWin32Window Win32Window
		{
			get { return Control; }
		}

		public override swf.Control ContentContainer
		{
			get { return content; }
		}

		public override Size ClientSize
		{
			get
			{
				return (Widget.Loaded ? content.Size : content.MinimumSize).ToEto ();
			}
			set
			{
				if (Widget.Loaded)
				{
					var size = Control.Size - content.Size;
					Control.Size = new sd.Size(value.Width + size.Width, value.Height + size.Height);
				}
				else
					content.MinimumSize = value.ToSD ();
			}
		}

		protected override void Initialize ()
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
			Control.Size = sd.Size.Empty;

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
				return Control.Location.ToEto ();
			}
			set {
				Control.Location = value.ToSD ();
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
				else return Control.RestoreBounds.ToEto ();
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

		public Screen Screen
		{
			get { return new Screen (Generator, new ScreenHandler (swf.Screen.FromControl (Control))); }
		}
    }
}
