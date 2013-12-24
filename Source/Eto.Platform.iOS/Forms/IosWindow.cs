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
	public abstract class IosWindow<TControl, TWidget> : MacDockContainer<TControl, TWidget>, IWindow
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
				var top = toolBar.Dock == ToolBarDock.Bottom ? screenSize.Height - height : 0;
				t.Frame = new sd.RectangleF(0, top, screenSize.Width, height);
				this.Control.AddSubview(t);
			}
		}

		public abstract string Title { get; set; }

		public Screen Screen
		{
			get { return null; }
		}
	}
}

