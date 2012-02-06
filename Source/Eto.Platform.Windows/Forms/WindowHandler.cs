using System;
using System.ComponentModel;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Drawing;
using Eto.Forms;
using System.Linq;

namespace Eto.Platform.Windows
{
	public interface IWindowHandler
	{
		SWF.ToolTip ToolTips { get; }
	}
	
	public abstract class WindowHandler<T, W> : WindowsContainer<T, W>, IWindow, IWindowHandler
		where T: System.Windows.Forms.Form
		where W: Window
	{
		MenuBar menu;
		Icon icon;
		ToolBar toolBar;
		System.Windows.Forms.Panel top;
		SWF.Panel toolbarHolder;
		SWF.ToolTip tooltips = new SWF.ToolTip();
		
		public SWF.ToolTip ToolTips
		{
			get { return tooltips; }
		}
		
		public override void Initialize ()
		{
			base.Initialize ();
			
			toolbarHolder = new SWF.Panel ();
			toolbarHolder.Dock = System.Windows.Forms.DockStyle.Top;
			toolbarHolder.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			toolbarHolder.AutoSize = true;
			Control.Controls.Add (toolbarHolder);
			
			top = new SWF.Panel ();
			top.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			top.AutoSize = true;
			top.Dock = SWF.DockStyle.Top;
			Control.Controls.Add (top);
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
				Control.FormClosing += delegate(object sender, SWF.FormClosingEventArgs e) {
					var args = new CancelEventArgs(e.Cancel);
					Widget.OnClosing (args);
					
					if (!e.Cancel && SWF.Application.OpenForms.Count <= 1 
						|| e.CloseReason == SWF.CloseReason.ApplicationExitCall
						|| e.CloseReason == SWF.CloseReason.WindowsShutDown)
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
					if (Control.WindowState == SWF.FormWindowState.Maximized) {
						Widget.OnMaximized (EventArgs.Empty);
					}
				};
				break;
			case Window.MinimizedEvent:
				Control.Resize += delegate {
					if (Control.WindowState == SWF.FormWindowState.Minimized) {
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
				top.SuspendLayout ();
				if (menu != null)
					top.Controls.Remove ((SWF.MenuStrip)menu.ControlObject);
				
				if (value == null) {
					Control.MainMenuStrip = null;
				} else {
					var c = (SWF.MenuStrip)value.ControlObject;
					c.Dock = SWF.DockStyle.Top;
					c.ResumeLayout ();
					top.Controls.Add (c);
					Control.MainMenuStrip = c;
				}
				this.Control.ResumeLayout ();
				top.ResumeLayout ();
				top.Update ();
				menu = value;
			}
		}
		
		public bool Resizable {
			get {
				if (Control.FormBorderStyle == SWF.FormBorderStyle.Sizable)
					return true;
				else
					return false;
			}
			set {
				if (value) {
					Control.FormBorderStyle = SWF.FormBorderStyle.Sizable;
				} else {
					Control.FormBorderStyle = SWF.FormBorderStyle.FixedDialog;
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
					toolbarHolder.Controls.Remove ((SWF.Control)toolBar.ControlObject);
				toolBar = value;
				if (toolBar != null) {
					var c = ((SWF.Control)toolBar.ControlObject);
					c.Dock = SWF.DockStyle.Top;
					c.ResumeLayout ();
					toolbarHolder.Controls.Add (c);
				}
				toolbarHolder.ResumeLayout();
				this.Control.ResumeLayout ();
				toolbarHolder.Update ();
			}
		}
		
		public void AddToolbar (ToolBar toolBar)
		{
			Control.Controls.Add ((SWF.Control)toolBar.ControlObject);
		}

		public void RemoveToolbar (ToolBar toolBar)
		{
			Control.Controls.Remove ((SWF.Control)toolBar.ControlObject);
		}

		public void ClearToolbars ()
		{
			foreach (SWF.Control c in Control.Controls) {
				if (c is SWF.ToolBar)
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
				Control.Icon = (SD.Icon)icon.ControlObject;
			}
		}
		
		public Point Location {
			get {
				return Generator.Convert (Control.Location);
			}
			set {
				Control.Location = Generator.Convert (value);
				Control.StartPosition = SWF.FormStartPosition.Manual;
			}
		}
		
		public WindowState State {
			get {
				switch (Control.WindowState) {
				case SWF.FormWindowState.Maximized:
					return WindowState.Maximized;
				case SWF.FormWindowState.Minimized:
					return WindowState.Minimized;
				case SWF.FormWindowState.Normal:
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
	}
}
