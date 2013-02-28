using System;
using MonoMac.AppKit;
using Eto.Forms;
using MonoMac.ObjCRuntime;
using Eto.Drawing;
using System.Linq;

namespace Eto.Platform.Mac.Forms
{
	public class ScreenHandler : WidgetHandler<NSScreen, Screen>, IScreen
	{
		public ScreenHandler (NSScreen screen)
		{
			this.Control = screen;
		}

		static Selector selBackingScaleFactor = new Selector ("backingScaleFactor");

#pragma warning disable 612, 618
		public float RealScale
		{
			get
			{
				if (this.Control.RespondsToSelector (selBackingScaleFactor))
					return this.Control.BackingScaleFactor;
				else
					return this.Control.UserSpaceScaleFactor;
			}
		}
#pragma warning restore 612, 618

		public float Scale
		{
			get { return 1f; }
		}

		public RectangleF Bounds
		{
			get
			{ 
				var bounds = Control.Frame;
				var origin = NSScreen.Screens[0].Frame.Bottom;
				bounds.Y = origin - bounds.Height - bounds.Y;
				return bounds.ToEto ();
			}
		}

		public RectangleF WorkingArea
		{
			get
			{ 
				var workingArea = this.Control.VisibleFrame;
				var origin = NSScreen.Screens[0].Frame.Bottom;
				workingArea.Y = origin - workingArea.Height - workingArea.Y;
				return workingArea.ToEto ();
			}
		}

		public int BitsPerPixel
		{
			get { return NSGraphics.BitsPerPixelFromDepth (Control.Depth); }
		}

		public bool IsPrimary
		{
			get { return this.Control == NSScreen.Screens[0]; }
		}
	}
}

