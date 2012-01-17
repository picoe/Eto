using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Platform.Wpf.Forms
{
	public abstract class WpfWindow<T, W> : WpfControl<T, W>, IWindow
		where T: System.Windows.Window
		where W: Window
	{
		Icon icon;
		MenuBar menu;
		ToolBar toolBar;
		System.Windows.Controls.DockPanel main;
		System.Windows.Controls.DockPanel content;

		protected void Setup ()
		{
			main = new System.Windows.Controls.DockPanel ();
			content = new System.Windows.Controls.DockPanel ();
			main.Children.Add (content);
			Control.Content = main;
		}

		public ToolBar ToolBar
		{
			get { return toolBar; }
			set {
				toolBar = value;
			}
		}

		public void Close()
		{
			Control.Close();
		}

		public MenuBar Menu
		{
			get {
				return menu;
			}
			set {
				menu = value;
			}
		}

		public Icon Icon
		{
			get { return icon; }
			set
			{
				icon = value;
				if (value != null)
				{
					Control.Icon = (System.Windows.Media.ImageSource)icon.ControlObject;
				}
			}
		}

		public bool Resizable
		{
			get { return Control.ResizeMode == System.Windows.ResizeMode.CanResize || Control.ResizeMode == System.Windows.ResizeMode.CanResizeWithGrip; }
			set
			{
				if (value) Control.ResizeMode = System.Windows.ResizeMode.CanResizeWithGrip;
				else Control.ResizeMode = System.Windows.ResizeMode.CanMinimize;
			}
		}

		public void Minimize()
		{
			Control.WindowState = System.Windows.WindowState.Minimized;
		}

		public Size ClientSize
		{
			get { return this.Size; }
			set { this.Size = value; }
		}

		public object ContainerObject
		{
			get { return Control; }
		}

		public virtual void SetLayout(Layout layout)
		{
			content.Children.Clear ();
			content.Children.Add((System.Windows.UIElement)layout.ControlObject);
		}

		public string Text
		{
			get { return Control.Title; }
			set { Control.Title = value; }
		}


		public Point Location
		{
			get
			{
				return new Point ((int)Control.Left, (int)Control.Top);
			}
			set
			{
				Control.Left = value.X;
				Control.Top = value.Y;
			}
		}

		public WindowState State
		{
			get
			{
				switch (Control.WindowState) {
					case System.Windows.WindowState.Maximized:
						return WindowState.Maximized;
					case System.Windows.WindowState.Minimized:
						return WindowState.Minimized;
					case System.Windows.WindowState.Normal:
						return WindowState.Normal;
					default:
						throw new NotSupportedException ();
				}
			}
			set
			{
				switch (value) {
					case WindowState.Maximized:
						Control.WindowState = System.Windows.WindowState.Maximized;
						break;
					case WindowState.Minimized:
						Control.WindowState = System.Windows.WindowState.Minimized;
						break;
					case WindowState.Normal:
						Control.WindowState = System.Windows.WindowState.Normal;
						break;
					default:
						throw new NotSupportedException ();
				}
			}
		}

		public Rectangle? RestoreBounds
		{
			get { return Generator.Convert(Control.RestoreBounds); }
		}


		public Size? MinimumSize
		{
			get
			{
				if (Control.MinWidth > 0 && Control.MinHeight > 0)
					return new Size ((int)Control.MinWidth, (int)Control.MinHeight);
				else
					return null;
			}
			set
			{
				if (value != null) {
					Control.MinWidth = value.Value.Width;
					Control.MinHeight = value.Value.Height;
				}
				else {
					Control.MinHeight = 0;
					Control.MinWidth = 0;
				}
			}
		}
	}
}
