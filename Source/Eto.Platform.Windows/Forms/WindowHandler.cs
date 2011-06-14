using System;
using System.ComponentModel;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Platform.Windows
{
	public abstract class WindowHandler<T, W> : WindowsContainer<T, W>, IWindow
		where T: System.Windows.Forms.Form
		where W: Window
	{
		MenuBar menu;
		Icon icon;
		ToolBar toolBar;
		System.Windows.Forms.Panel top;
		SWF.Panel toolbarHolder;
		
		public override void Initialize ()
		{
			base.Initialize ();
			Control.Closed += Control_Closed;
			Control.Closing += new CancelEventHandler (Control_Closing);
			
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
		
		private void Control_Closing (object sender, CancelEventArgs e)
		{
			Widget.OnClosing (e);
		}
		
		public override void AttachEvent (string handler)
		{
			switch (handler) {
			case Window.ShownEvent:
				Control.Shown += delegate {
					Widget.OnShown (EventArgs.Empty);
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
				if (menu != null)
					top.Controls.Remove ((SWF.MenuStrip)menu.ControlObject);
				
				if (value == null) {
					Control.MainMenuStrip = null;
				} else {
					var c = (SWF.MenuStrip)value.ControlObject;
					c.Dock = SWF.DockStyle.Top;
					top.Controls.Add (c);
					Control.MainMenuStrip = c;
				}
				this.Control.ResumeLayout ();
				top.PerformLayout ();
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
				//toolbarHolder.SuspendLayout();
				if (toolBar != null)
					toolbarHolder.Controls.Remove ((SWF.Control)toolBar.ControlObject);
				toolBar = value;
				if (toolBar != null) {
					var c = ((SWF.Control)toolBar.ControlObject);
					c.Dock = SWF.DockStyle.Top;
					toolbarHolder.Controls.Add (c);
				}
				//toolbarHolder.ResumeLayout();
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
		
		public void Minimize ()
		{
			Control.WindowState = System.Windows.Forms.FormWindowState.Minimized;
		}

		private void Control_Closed (object sender, EventArgs e)
		{
			Widget.OnClosed (e);
		}


		
	}
}
