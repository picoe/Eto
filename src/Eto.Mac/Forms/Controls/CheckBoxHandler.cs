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
using nfloat = System.Double;
using nint = System.Int64;
using nuint = System.UInt64;
#else
using nfloat = System.Single;
using nint = System.Int32;
using nuint = System.UInt32;
#endif
#if SDCOMPAT
using CGSize = System.Drawing.SizeF;
using CGRect = System.Drawing.RectangleF;
using CGPoint = System.Drawing.PointF;
#endif
#endif

namespace Eto.Mac.Forms.Controls
{
	public class EtoCenteredButton : NSButtonCell
	{
		nfloat defaultHeight;
		public EtoCenteredButton(nfloat defaultHeight)
		{
			this.defaultHeight = defaultHeight;
		}

		public override CGRect DrawingRectForBounds(CGRect theRect)
		{
			var rect = base.DrawingRectForBounds(theRect);
			var titleSize = AttributedTitle.Size;
			rect.Y += (nfloat)Math.Max(0, (titleSize.Height - defaultHeight) / 2);
			return rect;
		}

		public override CGRect TitleRectForBounds(CGRect theRect)
		{
			var titleSize = AttributedTitle.Size;
			var rect = base.TitleRectForBounds(theRect);
			rect.Y -= (nfloat)Math.Max(0, (titleSize.Height - defaultHeight) / 2);
			return rect;
		}
	}

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

			static nfloat defaultHeight;
			static EtoCheckBoxButton()
			{
				var b = new EtoCheckBoxButton();
				b.SizeToFit();
				defaultHeight = b.Frame.Height;
			}

			public EtoCheckBoxButton()
			{
				Cell = new EtoCenteredButton(defaultHeight);
				Title = string.Empty;
				SetButtonType(NSButtonType.Switch);
			}
		}

		protected override NSButton CreateControl()
		{
			return new EtoCheckBoxButton();
		}

		protected override void Initialize()
		{
			Control.Activated += HandleActivated;

			base.Initialize();
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
