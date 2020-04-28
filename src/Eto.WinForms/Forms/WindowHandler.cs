using System;
using System.ComponentModel;
using sd = System.Drawing;
using swf = System.Windows.Forms;
using Eto.Drawing;
using Eto.Forms;
using Eto.WinForms.Forms;
using Eto.WinForms.Drawing;

namespace Eto.WinForms.Forms
{
	public interface IWindowHandler
	{
		swf.ToolTip ToolTips { get; }

		swf.IWin32Window Win32Window { get; }
	}

	static class WindowHandler
	{
		internal static readonly object MovableByWindowBackground_Key = new object();
	}

	public abstract class WindowHandler<TControl, TWidget, TCallback> : WindowsPanel<TControl, TWidget, TCallback>, Window.IHandler, IWindowHandler
		where TControl : swf.Form
		where TWidget : Window
		where TCallback : Window.ICallback
	{
		MenuBar menu;
		Icon icon;
		Eto.Forms.ToolBar toolBar;
		swf.Panel menuHolder;
		swf.Panel content;
		swf.Panel toolbarHolder;
		readonly swf.ToolTip tooltips = new swf.ToolTip();
		bool resizable;
		bool clientWidthSet;
		bool clientHeightSet;

		public bool ShowShellDropForWindow
		{
			get => AllowDrop;
			set => AllowDrop = value;
		}

		public virtual bool IsAttached => false;
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
				clientWidthSet = value.Width != -1;
				clientHeightSet = value.Height != -1;
			}
		}

		public bool MovableByWindowBackground
		{
			get => Widget.Properties.Get<bool>(WindowHandler.MovableByWindowBackground_Key);
			set
			{
				if (Widget.Properties.TrySet(WindowHandler.MovableByWindowBackground_Key, value))
				{
					if (value)
						Widget.MouseDown += Content_MouseDown;
					else
						Widget.MouseDown -= Content_MouseDown;
				}
			}
		}

		private void Content_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Buttons == MouseButtons.Primary)
			{
				Win32.ReleaseCapture();
				Win32.SendMessage(Control.Handle, Win32.WM.NCLBUTTONDOWN, (IntPtr)Win32.HT.CAPTION, IntPtr.Zero);
				e.Handled = true;
			}
		}

		protected override bool SetMinimumSize(Size size)
		{
			var sdsize = Size.Max(size, MinimumSize).ToSD();
			if (Control.MinimumSize != sdsize)
			{
				Control.MinimumSize = sdsize;
				return true;
			}
			return false;
		}

		protected override void Initialize()
		{
			if (IsAttached)
				return;

			// show shell drop images for the entire window
			ShowShellDropForWindow = true;

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

			Control.Load += Control_Load;
			Control.Size = sd.Size.Empty;

			base.Initialize();

			// Always handle closing because we want to send Application.Terminating event
			HandleEvent(Window.ClosingEvent);
		}

		void Control_Load(object sender, EventArgs e)
		{
			// ensure we auto size to the content
			if ((!clientWidthSet || !clientHeightSet) /*&& Control.AutoSize*/ && contentSize != null)
			{
				var sz = contentSize.Value.ToSD(); // Content.GetPreferredSize().ToSD();
				var min = new sd.Size();
				if (!clientWidthSet)
					min.Width = sz.Width;
				if (!clientHeightSet)
					min.Height = sz.Height;
				ContainerContentControl.MinimumSize = min;
			}
			// turn off auto sizing so user can then resize the forms
			var size = Control.Size;
			Control.AutoSize = false;
			Control.Size = size;
			content.MinimumSize = content.MaximumSize = sd.Size.Empty;
			ContainerContentControl.MinimumSize = sd.Size.Empty;
		}

		public override Size Size
		{
			get
			{
				return base.Size;
			}
			set
			{
				base.Size = value;
				clientWidthSet = value.Width != -1;
				clientHeightSet = value.Height != -1;
			}
		}

		public override void OnLoadComplete(EventArgs e)
		{
			if (IsAttached)
				return;
			base.OnLoadComplete(e);

			if ((!clientWidthSet || !clientHeightSet))// && Control.AutoSize)
			{
				var availableSize = ClientSize;
				if (!clientWidthSet)
					availableSize.Width = 0;
				if (!clientHeightSet)
					availableSize.Height = 0;
				contentSize = Content.GetPreferredSize(availableSize);
			}
		}
		Size? contentSize;

		protected override void SetContent(Control control, swf.Control contentControl)
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
			get { return swf.FormBorderStyle.Fixed3D; }
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

		public Eto.Forms.ToolBar ToolBar
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

		public Rectangle RestoreBounds
		{
			get
			{
				if (WindowState == WindowState.Normal || Control.RestoreBounds.IsEmpty)
					return Widget.Bounds;
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

		public virtual void BringToFront()
		{
			if (Control.WindowState == swf.FormWindowState.Minimized)
				Control.WindowState = swf.FormWindowState.Normal;
			Control.BringToFront();
			Control.Activate();
		}

		public void SendToBack()
		{
			Control.SendToBack();
		}

		public virtual void SetOwner(Window owner)
		{
			Control.Owner = owner.ToSWF();
		}

		public Screen Screen
		{
			get { return swf.Screen.FromControl(Control).ToEto(); }
		}

		public override bool Visible
		{
			get { return base.Visible; }
			set
			{
				if (Visible != value)
				{
					base.Visible = value;
					if (Widget.Loaded && value)
						Callback.OnShown(Widget, EventArgs.Empty);
				}
			}
		}

		public float LogicalPixelSize
		{
			get
			{
				return 1f;
			}
		}
	}
}
