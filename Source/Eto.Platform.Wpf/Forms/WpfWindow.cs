using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using Eto.Drawing;
using sw = System.Windows;
using swm = System.Windows.Media;
using swc = System.Windows.Controls;

namespace Eto.Platform.Wpf.Forms
{
	public interface IWpfWindow
	{
		sw.Window Control { get; }
	}

	public abstract class WpfWindow<T, W> : WpfControl<T, W>, IWindow, IWpfWindow
		where T: sw.Window
		where W: Window
	{
		Icon icon;
		MenuBar menu;
		ToolBar toolBar;
		swc.DockPanel main;
		swc.ContentControl menuHolder;
		swc.ToolBarTray toolBarHolder;
		swc.DockPanel content;

		protected void Setup ()
		{
			main = new swc.DockPanel ();
			content = new swc.DockPanel ();
			menuHolder = new swc.ContentControl ();
			toolBarHolder = new swc.ToolBarTray { IsEnabled = true };
			content.Background = System.Windows.SystemColors.ControlBrush;
			swc.DockPanel.SetDock (menuHolder, swc.Dock.Top);
			swc.DockPanel.SetDock (toolBarHolder, swc.Dock.Top);
			main.Children.Add (menuHolder);
			main.Children.Add (toolBarHolder);
			main.Children.Add (content);
			Control.Content = main;
		}

		public ToolBar ToolBar
		{
			get { return toolBar; }
			set
			{
				toolBar = value;
				toolBarHolder.ToolBars.Clear ();
				if (toolBar != null) {
					toolBarHolder.ToolBars.Add((swc.ToolBar)toolBar.ControlObject);
				}
			}
		}

		public void Close()
		{
			Control.Close();
		}

		public MenuBar Menu
		{
			get { return menu; }
			set {
				menu = value;
				if (menu != null) {
					menuHolder.Content = (sw.UIElement)menu.ControlObject;
				}
				else {
					menuHolder.Content = null;
				}
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
					Control.Icon = (swm.ImageSource)icon.ControlObject;
				}
			}
		}

		public bool Resizable
		{
			get { return Control.ResizeMode == sw.ResizeMode.CanResize || Control.ResizeMode == sw.ResizeMode.CanResizeWithGrip; }
			set
			{
				if (value) Control.ResizeMode = sw.ResizeMode.CanResizeWithGrip;
				else Control.ResizeMode = sw.ResizeMode.CanMinimize;
			}
		}

		public void Minimize()
		{
			Control.WindowState = sw.WindowState.Minimized;
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
			content.Children.Add ((sw.UIElement)layout.ControlObject);
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
					case sw.WindowState.Maximized:
						return WindowState.Maximized;
					case sw.WindowState.Minimized:
						return WindowState.Minimized;
					case sw.WindowState.Normal:
						return WindowState.Normal;
					default:
						throw new NotSupportedException ();
				}
			}
			set
			{
				switch (value) {
					case WindowState.Maximized:
						Control.WindowState = sw.WindowState.Maximized;
						break;
					case WindowState.Minimized:
						Control.WindowState = sw.WindowState.Minimized;
						break;
					case WindowState.Normal:
						Control.WindowState = sw.WindowState.Normal;
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

		sw.Window IWpfWindow.Control
		{
			get { return this.Control; }
		}
	}
}
