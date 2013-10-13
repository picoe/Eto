using System;
using MonoTouch.UIKit;
using Eto.Forms;
using Eto.Platform.iOS.Forms.Controls;
using Eto.Drawing;
using Eto.Platform.Mac.Forms;

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

		public abstract string Title { get; set; }

		public Screen Screen
		{
			get { return null; }
		}
	}
}

