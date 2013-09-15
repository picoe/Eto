using System;
using System.ComponentModel;
using sd = System.Drawing;
using swf = System.Windows.Forms;
using Eto.Drawing;
using Eto.Forms;
using System.Linq;
using Eto.Platform.Windows.Forms;
using System.CodeDom.Compiler;
using System.Diagnostics;

namespace Eto.Platform.Windows
{
	public interface IWindowHandler
	{
		swf.ToolTip ToolTips { get; }

		swf.IWin32Window Win32Window { get; }
	}

	public abstract class WindowHandler<T, W> : WindowsDockContainer<T, W>, IWindow, IWindowHandler
		where T : swf.Form
		where W : Window
	{
		MenuBar menu;
		Icon icon;
		ToolBar toolBar;
		swf.Panel menuHolder;
		swf.Panel content;
		swf.Panel toolbarHolder;
		swf.ToolTip tooltips = new swf.ToolTip();
		bool resizable;

		public override swf.Control ContainerContentControl
		{
			get { return content; }
		}

		public swf.ToolTip ToolTips
		{
			get { return tooltips; }
		}

		public swf.IWin32Window Win32Window
		{
			get { return Control; }
		}

		public override Size ClientSize
		{
			get
			{
				return (Widget.Loaded ? content.Size : content.MinimumSize).ToEto();
			}
			set
			{
				if (Widget.Loaded)
				{
					var size = Control.Size - content.Size;
					Control.Size = new sd.Size(value.Width + size.Width, value.Height + size.Height);
				}
				else
				{
					content.MinimumSize = content.MaximumSize = value.ToSD();
				}
			}
		}

		protected override void Initialize()
		{
			Control.KeyPreview = true;
			Control.FormBorderStyle = DefaultWindowStyle;
			resizable = Control.FormBorderStyle.IsResizable();
			content = new swf.Panel
			{
				AutoSize = true,
				AutoSizeMode = swf.AutoSizeMode.GrowAndShrink,
				Dock = swf.DockStyle.Fill
			};
			Control.Controls.Add(content);

			toolbarHolder = new swf.Panel
			{
				Dock = swf.DockStyle.Top,
				AutoSizeMode = swf.AutoSizeMode.GrowAndShrink,
				AutoSize = true
			};
			Control.Controls.Add(toolbarHolder);

			menuHolder = new swf.Panel
			{
				AutoSizeMode = swf.AutoSizeMode.GrowAndShrink,
				AutoSize = true,
				Dock = swf.DockStyle.Top
			};
			Control.Controls.Add(menuHolder);

			Control.Load += (sender, e) =>
			{
				content.MinimumSize = sd.Size.Empty;
				content.MaximumSize = sd.Size.Empty;
			};
			Control.Size = sd.Size.Empty;

			base.Initialize();

			// Always handle closing because we want to send Application.Terminating event
			HandleEvent(Window.ClosingEvent);
		}

		public override void AttachEvent(string handler)
		{
			switch (handler)
			{
				case Window.ClosedEvent:
					Control.FormClosed += delegate
					{
						Widget.OnClosed(EventArgs.Empty);
					};
					break;
				case Window.ClosingEvent:
					Control.FormClosing += delegate(object sender, swf.FormClosingEventArgs e)
					{
						var args = new CancelEventArgs(e.Cancel);
						Widget.OnClosing(args);

						if (!e.Cancel && swf.Application.OpenForms.Count <= 1
							|| e.CloseReason == swf.CloseReason.ApplicationExitCall
							|| e.CloseReason == swf.CloseReason.WindowsShutDown)
						{
							Application.Instance.OnTerminating(args);
						}

						e.Cancel = args.Cancel;
					};
					break;
				case Window.ShownEvent:
					Control.Shown += delegate
					{
						Widget.OnShown(EventArgs.Empty);
					};
					break;
				case Window.GotFocusEvent:
					Control.Activated += delegate
					{
						Widget.OnGotFocus(EventArgs.Empty);
					};
					break;
				case Window.LostFocusEvent:
					Control.Deactivate += delegate
					{
						Widget.OnLostFocus(EventArgs.Empty);
					};
					break;
				case Window.WindowStateChangedEvent:
					var oldState = Control.WindowState;
					Control.Resize += (sender, e) =>
					{
						if (Control.WindowState != oldState)
						{
							oldState = Control.WindowState;
							Widget.OnWindowStateChanged(EventArgs.Empty);
						}
					};
					break;
				case Window.LocationChangedEvent:
					Control.LocationChanged += (sender, e) =>
					{
						Widget.OnLocationChanged(EventArgs.Empty);
					};
					break;
				default:
					base.AttachEvent(handler);
					break;
			}
		}

		public MenuBar Menu
		{
			get
			{
				return menu;
			}
			set
			{
				this.Control.SuspendLayout();
				menuHolder.SuspendLayout();
				if (menu != null)
					menuHolder.Controls.Remove((swf.MenuStrip)menu.ControlObject);

				if (value == null)
				{
					Control.MainMenuStrip = null;
				}
				else
				{
					var c = (swf.MenuStrip)value.ControlObject;
					c.Dock = swf.DockStyle.Top;
					c.ResumeLayout();
					menuHolder.Controls.Add(c);
					Control.MainMenuStrip = c;
				}
				menuHolder.ResumeLayout();
				this.Control.ResumeLayout();
				menuHolder.Update();
				menu = value;
			}
		}

		protected virtual swf.FormBorderStyle DefaultWindowStyle
		{
			get { return swf.FormBorderStyle.Sizable; }
		}

		public virtual bool Resizable
		{
			get { return resizable; }
			set
			{
				if (value != resizable)
				{
					Control.FormBorderStyle = Control.FormBorderStyle.ToEto().ToSWF(value, DefaultWindowStyle);
					resizable = value;
				}
			}
		}

		public virtual bool Maximizable
		{
			get { return Control.MaximizeBox; }
			set { Control.MaximizeBox = value; }
		}

		public virtual bool Minimizable
		{
			get { return Control.MinimizeBox; }
			set { Control.MinimizeBox = value; }
		}

		public virtual bool ShowInTaskbar
		{
			get { return Control.ShowInTaskbar; }
			set { Control.ShowInTaskbar = value; }
		}

		public virtual bool TopMost
		{
			get { return Control.TopMost; }
			set { Control.TopMost = value; }
		}

		public ToolBar ToolBar
		{
			get
			{
				return this.toolBar;
			}
			set
			{
				this.Control.SuspendLayout();
				toolbarHolder.SuspendLayout();
				if (toolBar != null)
					toolbarHolder.Controls.Remove((swf.Control)toolBar.ControlObject);
				toolBar = value;
				if (toolBar != null)
				{
					var c = ((swf.Control)toolBar.ControlObject);
					c.Dock = swf.DockStyle.Top;
					c.ResumeLayout();
					toolbarHolder.Controls.Add(c);
				}
				toolbarHolder.ResumeLayout();
				this.Control.ResumeLayout();
				toolbarHolder.Update();
			}
		}

		public void AddToolbar(ToolBar toolBar)
		{
			Control.Controls.Add((swf.Control)toolBar.ControlObject);
		}

		public void RemoveToolbar(ToolBar toolBar)
		{
			Control.Controls.Remove((swf.Control)toolBar.ControlObject);
		}

		public void ClearToolbars()
		{
			foreach (swf.Control c in Control.Controls)
			{
				if (c is swf.ToolBar)
					Control.Controls.Remove(c);
			}
		}

		public void Close()
		{
			Control.Close();
		}

		public Icon Icon
		{
			get { return icon; }
			set
			{
				icon = value;
				Control.Icon = (sd.Icon)icon.ControlObject;
			}
		}

		public string Title
		{
			get { return Control.Text; }
			set { Control.Text = value; }
		}

		public new Point Location
		{
			get
			{
				return Control.Location.ToEto();
			}
			set
			{
				Control.Location = value.ToSD();
				Control.StartPosition = swf.FormStartPosition.Manual;
			}
		}

		public virtual WindowState WindowState
		{
			get
			{
				switch (Control.WindowState)
				{
					case swf.FormWindowState.Maximized:
						return WindowState.Maximized;
					case swf.FormWindowState.Minimized:
						return WindowState.Minimized;
					case swf.FormWindowState.Normal:
						return WindowState.Normal;
					default:
						throw new NotSupportedException();
				}
			}
			set
			{
				switch (value)
				{
					case WindowState.Maximized:
						Control.WindowState = swf.FormWindowState.Maximized;
						break;
					case WindowState.Minimized:
						Control.WindowState = swf.FormWindowState.Minimized;
						break;
					case WindowState.Normal:
						Control.WindowState = swf.FormWindowState.Normal;
						break;
					default:
						throw new NotSupportedException();
				}
			}
		}

		public Rectangle? RestoreBounds
		{
			get
			{
				if (this.WindowState == WindowState.Normal || Control.RestoreBounds.IsEmpty) return null;
				else return Control.RestoreBounds.ToEto();
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

		public WindowStyle WindowStyle
		{
			get { return Control.FormBorderStyle.ToEto(); }
			set { Control.FormBorderStyle = value.ToSWF(resizable, DefaultWindowStyle); }
		}

		public void BringToFront()
		{
			if (Control.WindowState == swf.FormWindowState.Minimized)
				Control.WindowState = swf.FormWindowState.Normal;
			Control.Activate();
		}

		public void SendToBack()
		{
			Control.SendToBack();
		}

		public Screen Screen
		{
			get { return new Screen(Generator, new ScreenHandler(swf.Screen.FromControl(Control))); }
		}
	}
}
