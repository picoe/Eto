using System;
using MonoMac.AppKit;
using Eto.Forms;
using MonoMac.ObjCRuntime;

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
	}
}

