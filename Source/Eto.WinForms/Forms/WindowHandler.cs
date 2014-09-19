using System;
using System.ComponentModel;
using sd = System.Drawing;
using swf = System.Windows.Forms;
using Eto.Drawing;
using Eto.Forms;
using Eto.WinForms.Forms;
using Eto.WinForms.Drawing;

namespace Eto.WinForms
{
	public interface IWindowHandler
	{
		swf.ToolTip ToolTips { get; }

		swf.IWin32Window Win32Window { get; }
	}

	public abstract class WindowHandler<TControl, TWidget, TCallback> : WindowsPanel<TControl, TWidget, TCallback>, Window.IHandler, IWindowHandler
		where TControl : swf.Form
		where TWidget : Window
		where TCallback : Window.ICallback
	{
		MenuBar menu;
		Icon icon;
		ToolBar toolBar;
		swf.Panel menuHolder;
		swf.Panel content;
		swf.Panel toolbarHolder;
		readonly swf.ToolTip tooltips = new swf.ToolTip();
		bool resizable;
		bool clientSizeSet;

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
				clientSizeSet = value.Width != -1 || value.Height != -1;
			}
		}

		protected override void Initialize()
		{
			Control.KeyPreview = !ApplicationHandler.BubbleKeyEvents;
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
				// ensure we auto size to the content
				if (!clientSizeSet && Control.AutoSize)
					ContainerContentControl.MinimumSize = Content.GetPreferredSize().ToSD();
				Control.MinimumSize = Control.Size;
				Control.AutoSize = false;
				Control.MinimumSize = sd.Size.Empty;
				content.MinimumSize = content.MaximumSize = sd.Size.Empty;
				ContainerContentControl.MinimumSize = sd.Size.Empty;
			};
			Control.Size = sd.Size.Empty;

			base.Initialize();

			// Always handle closing because we want to send Application.Terminating event
			HandleEvent(Window.ClosingEvent);
		}

		protected override void SetContent(swf.Control contentControl)
		{
			contentControl.Dock = swf.DockStyle.Fill;
			ContainerContentControl.Controls.Add(contentControl);
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case Window.ClosedEvent:
					Control.FormClosed += (sender, e) => Callback.OnClosed(Widget, EventArgs.Empty);
					break;
				case Window.ClosingEvent:
					Control.FormClosing += delegate(object sender, swf.FormClosingEventArgs e)
					{
						var args = new CancelEventArgs(e.Cancel);
						Callback.OnClosing(Widget, args);

						if (!e.Cancel && swf.Application.OpenForms.Count <= 1
						    || e.CloseReason == swf.CloseReason.ApplicationExitCall
						    || e.CloseReason == swf.CloseReason.WindowsShutDown)
						{
							var app = ((ApplicationHandler)Application.Instance.Handler);
							app.Callback.OnTerminating(app.Widget, args);
						}

						e.Cancel = args.Cancel;
					};
					break;
				case Eto.Forms.Control.ShownEvent:
					Control.Shown += delegate
					{
						Callback.OnShown(Widget, EventArgs.Empty);
					};
					break;
				case Eto.Forms.Control.GotFocusEvent:
					Control.Activated += delegate
					{
						Callback.OnGotFocus(Widget, EventArgs.Empty);
					};
					break;
				case Eto.Forms.Control.LostFocusEvent:
					Control.Deactivate += delegate
					{
						Callback.OnLostFocus(Widget, EventArgs.Empty);
					};
					break;
				case Window.WindowStateChangedEvent:
					var oldState = Control.WindowState;
					Control.Resize += (sender, e) =>
					{
						if (Control.WindowState != oldState)
						{
							oldState = Control.WindowState;
							Callback.OnWindowStateChanged(Widget, EventArgs.Empty);
						}
					};
					break;
				case Window.LocationChangedEvent:
					Control.LocationChanged += (sender, e) => Callback.OnLocationChanged(Widget, EventArgs.Empty);
					break;
				default:
					base.AttachEvent(id);
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
					menuHolder.Controls.Add(c);
					Control.MainMenuStrip = c;
				}
				menuHolder.ResumeLayout();
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

		public virtual bool Topmost
		{
			get { return Control.TopMost; }
			set { Control.TopMost = value; }
		}

		public ToolBar ToolBar
		{
			get
			{
				return toolBar;
			}
			set
			{
				toolbarHolder.SuspendLayout();
				if (toolBar != null)
					toolbarHolder.Controls.Remove((swf.Control)toolBar.ControlObject);
				toolBar = value;
				if (toolBar != null)
				{
					var c = ((swf.Control)toolBar.ControlObject);
					c.Dock = swf.DockStyle.Top;
					toolbarHolder.Controls.Add(c);
				}
				toolbarHolder.ResumeLayout();
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
				Control.Icon = ((IWindowsIconSource)icon.Handler).GetIcon();
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
				if (WindowState == WindowState.Normal || Control.RestoreBounds.IsEmpty)
					return null;
				return Control.RestoreBounds.ToEto();
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
				Control.AllowTransparency = Math.Abs(value - 1.0) > 0.01f;
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
			get { return new Screen(new ScreenHandler(swf.Screen.FromControl(Control))); }
		}
	}
}
