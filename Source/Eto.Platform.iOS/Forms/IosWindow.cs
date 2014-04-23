using System;
using MonoTouch.UIKit;
using Eto.Forms;
using Eto.Platform.iOS.Forms.Controls;
using Eto.Drawing;
using Eto.Platform.Mac.Forms;
using NSToolbar = MonoTouch.UIKit.UIToolbar;
using NSToolbarItem = MonoTouch.UIKit.UIBarButtonItem;
using sd = System.Drawing;

namespace Eto.Platform.iOS.Forms
{
	public abstract class IosWindow<TControl, TWidget> : MacPanel<TControl, TWidget>, IWindow
		where TControl: UIView
		where TWidget: Window
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


		public MenuBar Menu
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

		public Icon Icon { get; set; }

		public bool Resizable
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

		public bool Maximizable
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

		public bool Minimizable
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

		public bool ShowInTaskbar
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

		public bool Topmost
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

		public WindowState WindowState
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

		public Rectangle? RestoreBounds
		{
			get { throw new NotImplementedException(); }
		}

		public WindowStyle WindowStyle
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

		public void BringToFront()
		{
			throw new NotImplementedException();
		}

		public void SendToBack()
		{
			throw new NotImplementedException();
		}

		public ContextMenu ContextMenu
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
	}
}

