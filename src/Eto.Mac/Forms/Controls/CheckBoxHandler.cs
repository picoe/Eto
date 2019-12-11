using System;
using Eto.Forms;
using Eto.Drawing;

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

	public abstract class EtoCenteredButtonCell : NSButtonCell
	{
		nfloat defaultHeight;

		protected abstract nfloat GetDefaultHeight();

		protected virtual nfloat Offset => 0f;

		public override void SetButtonType(NSButtonType aType)
		{
			base.SetButtonType(aType);
			defaultHeight = GetDefaultHeight();
		}

		public override NSControlSize ControlSize
		{
			get => base.ControlSize;
			set
			{
				base.ControlSize = value;
				defaultHeight = GetDefaultHeight();
			}
		}

		nfloat ButtonOffset => (nfloat)Math.Max(0, Math.Ceiling((AttributedTitle.Size.Height - defaultHeight) / 2 - 1));

		public override CGRect DrawingRectForBounds(CGRect theRect)
		{
			var rect = base.DrawingRectForBounds(theRect);
			rect.Y += ButtonOffset;
			rect.Y += Offset;
			return rect;
		}

		public override CGRect TitleRectForBounds(CGRect theRect)
		{
			var rect = base.TitleRectForBounds(theRect);
			rect.Y -= ButtonOffset;
			rect.Y -= Offset;
			return rect;
		}
	}

	public class CheckBoxHandler : MacButton<NSButton, CheckBox, CheckBox.ICallback>, CheckBox.IHandler
	{
		public class EtoCheckCenteredButtonCell : EtoCenteredButtonCell
		{
			// check boxes get clipped at the top in mini mode using the alignment rects. macOS 10.14.6
			// see Eto.Test.Mac.UnitTests.CheckBoxTests.ButtonShouldNotBeClipped()
			protected override nfloat Offset => ControlSize == NSControlSize.Mini ? 0.5f : 0;

			protected override nfloat GetDefaultHeight()
			{
				switch (ControlSize)
				{
					default:
					case NSControlSize.Regular:
						return 14;
					case NSControlSize.Small:
						return 12;
					case NSControlSize.Mini:
						return 10;
				}
			}
		}

		public class EtoCheckBoxButton : NSButton, IMacControl
		{
			public WeakReference WeakHandler { get; set; }

			public object Handler
			{ 
				get { return WeakHandler.Target; }
				set { WeakHandler = new WeakReference(value); } 
			}

			public EtoCheckBoxButton()
			{
				Cell = new EtoCheckCenteredButtonCell();
				Title = string.Empty;
				SetButtonType(NSButtonType.Switch);
			}
		}

		protected override bool DefaultUseAlignmentFrame => true;

		protected override NSButton CreateControl() => new EtoCheckBoxButton();

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

		protected override void SetBackgroundColor(Color? color)
		{
			base.SetBackgroundColor(color);
			InvalidateMeasure();
		}
	}
}
