using System;
using Eto.Forms;

#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
using CoreImage;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
using MonoMac.CoreImage;
#if Mac64
using CGSize = MonoMac.Foundation.NSSize;
using CGRect = MonoMac.Foundation.NSRect;
using CGPoint = MonoMac.Foundation.NSPoint;
using nfloat = System.Double;
using nint = System.Int64;
using nuint = System.UInt64;
#else
using CGSize = System.Drawing.SizeF;
using CGRect = System.Drawing.RectangleF;
using CGPoint = System.Drawing.PointF;
using nfloat = System.Single;
using nint = System.Int32;
using nuint = System.UInt32;
#endif
#endif

namespace Eto.Mac.Forms.Controls
{
	public class CheckBoxHandler : MacButton<NSButton, CheckBox, CheckBox.ICallback>, CheckBox.IHandler
	{
		public class EtoCheckBoxButton : NSButton, IMacControl
		{
			public WeakReference WeakHandler { get; set; }

			public object Handler
			{ 
				get { return WeakHandler.Target; }
				set { WeakHandler = new WeakReference(value); } 
			}
		}

		public CheckBoxHandler()
		{
			Control = new EtoCheckBoxButton { Handler = this, Title = string.Empty };
			Control.SetButtonType(NSButtonType.Switch);
			Control.Activated += HandleActivated;
		}

		static void HandleActivated(object sender, EventArgs e)
		{
			var handler = GetHandler(sender) as CheckBoxHandler;
			handler.TriggerMouseCallback();
			handler.Callback.OnCheckedChanged(handler.Widget, EventArgs.Empty);
		}

		public bool? Checked
		{
			get
			{ 
				switch (Control.State)
				{
					case NSCellStateValue.On:
						return true;
					case NSCellStateValue.Off:
						return false;
					default:
						return null;
				}
			}
			set
			{ 
				if (Checked != value)
				{
					if (value == null)
						Control.State = ThreeState ? NSCellStateValue.Mixed : NSCellStateValue.Off;
					else if (value.Value)
						Control.State = NSCellStateValue.On;
					else
						Control.State = NSCellStateValue.Off;
					Callback.OnCheckedChanged(Widget, EventArgs.Empty);
				}
			}
		}

		public bool ThreeState
		{
			get { return Control.AllowsMixedState; }
			set { Control.AllowsMixedState = value; }
		}
	}
}
