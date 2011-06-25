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
			Control.Close();
		}

		public MenuBar Menu
		{
			get {
				throw new NotImplementedException ();
			}
			set {
				throw new NotImplementedException ();
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
	}
}
