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
	public class ProgressBarHandler : MacView<NSProgressIndicator, ProgressBar, ProgressBar.ICallback>, ProgressBar.IHandler
	{
		public class EtoSlider : NSProgressIndicator, IMacControl
		{
			public WeakReference WeakHandler { get; set; }

			public object Handler
			{ 
				get { return WeakHandler.Target; }
				set { WeakHandler = new WeakReference(value); } 
			}

			public EtoSlider()
			{
				Indeterminate = false;
			}

		}

		public override NSView ContainerControl { get { return Control; } }

		protected override NSProgressIndicator CreateControl()
		{
			return new EtoSlider();
		}

		protected override void Initialize()
		{
			MinValue = 0;
			MaxValue = 100;
			base.Initialize();
		}

		protected override SizeF GetNaturalSize (SizeF availableSize)
		{
			return new Size (80, 30);
		}

		public bool Indeterminate {
			get { return Control.Indeterminate; }
			set { 
				Control.Indeterminate = value;
				if (value)
					Control.StartAnimation (Control);
				else
					Control.StopAnimation (Control);
			}
		}

		public int MaxValue {
			get { return (int)Control.MaxValue; }
			set { 
				Control.MaxValue = value;
			}
		}
		
		public int MinValue {
			get { return (int)Control.MinValue; }
			set {
				Control.MinValue = value;
			}
		}

		public int Value {
			get { return (int)Control.DoubleValue; }
			set { Control.DoubleValue = value; }
		}
	}
}

