using System;
using MonoTouch.UIKit;
using Eto.Forms;
using Eto.Platform.iOS.Forms.Controls;
using Eto.Drawing;

namespace Eto.Platform.iOS.Forms
{
	public abstract class iosWindow<T, W> : iosContainer<T, W>, IWindow
		where T: UIView
		where W: Window
	{

		public iosWindow ()
		{
		}

		public Point Location {
			get {
				return this.Control.Frame.Location.ToEtoPoint ();
			}
			set {
				var frame = this.Control.Frame;
				frame.Location = value.ToSDPointF ();
				this.Control.Frame = frame;
			}
		}
		
		public double Opacity {
			get { return Control.Alpha; }
			set { Control.Alpha = (float)value; }
		}

		public override void AttachEvent (string handler)
		{
			switch (handler) {
			case Window.ClosedEvent:
			case Window.ClosingEvent:
				// TODO
				break;
			default:
				base.AttachEvent (handler);
				break;
			}
		}
		
		public virtual void Close ()
		{
		}

		public ToolBar ToolBar {
			get {
				throw new NotImplementedException ();
			}
			set {
				throw new NotImplementedException ();
			}
		}

		public abstract string Title { get; set; }

	}
}

