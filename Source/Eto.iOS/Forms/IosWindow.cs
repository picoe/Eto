using System;
using MonoTouch.UIKit;
using Eto.Forms;
using Eto.iOS.Forms.Controls;
using Eto.Drawing;
using Eto.Mac.Forms;
using NSToolbar = MonoTouch.UIKit.UIToolbar;
using NSToolbarItem = MonoTouch.UIKit.UIBarButtonItem;
using sd = System.Drawing;

namespace Eto.iOS.Forms
{
	public abstract class IosWindow<TControl, TWidget, TCallback> : MacPanel<TControl, TWidget, TCallback>, IWindow
		where TControl: UIView
		where TWidget: Window
		where TCallback: Window.ICallback
	{
		public override UIView ContainerControl { get { return Control; } }

		public new Point Location
		{
			get
			{
				return Control.Frame.Location.ToEtoPoint();
			}
			set
			{
				var frame = this.Control.Frame;
				frame.Location = value.ToSDPointF();
				this.Control.Frame = frame;
			}
		}

		public double Opacity
		{
			get { return Control.Alpha; }
			set { Control.Alpha = (float)value; }
		}

		public override void AttachEvent(string handler)
		{
			switch (handler)
			{
				case Window.ClosedEvent:
				case Window.ClosingEvent:
				// TODO
					break;
				default:
					base.AttachEvent(handler);
					break;
			}
		}

		public virtual void Close()
		{
		}

		ToolBar toolBar;
		public ToolBar ToolBar
		{
			get { return toolBar; }
			set
			{
				toolBar = value;
				var t = value.ControlObject as NSToolbar;
				var screenSize = UIScreen.MainScreen.Bounds.Size;
				var height = 44;
				var top = toolBar.Dock == ToolBarDock.Bottom ? screenSize.Height - height : 20; // 20px to avoid overlapping the status bar on iOS7. TODO: what about iOS6?
				t.Frame = new sd.RectangleF(0, top, screenSize.Width, height);
				this.Control.ContainerAddSubView(t);
			}
		}

		public abstract string Title { get; set; }

		public Screen Screen
		{
			get { return null; }
		}


		public MenuBar Menu { get { return null; } set { } }

		public Icon Icon { get { return null; } set { } }

		public bool Resizable { get { return false; } set { } }

		public bool Maximizable { get { return false; } set { } }

		public bool Minimizable { get { return false; } set { } }

		public bool ShowInTaskbar { get { return false; } set { } }

		public bool Topmost { get { return false; } set { } }

		public WindowState WindowState { get { return WindowState.Maximized; } set { } }

		public Rectangle? RestoreBounds
		{
			get { return null; }
		}

		public WindowStyle WindowStyle { get { return WindowStyle.Default; } set { } }

		public void BringToFront()
		{
		}

		public void SendToBack()
		{
		}

		public ContextMenu ContextMenu { get { return null; } set { } }
	}
}

