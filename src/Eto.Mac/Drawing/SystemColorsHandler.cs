using System;
using Eto.Drawing;

#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
#endif

namespace Eto.Mac.Drawing
{
	public class SystemColorsHandler : SystemColors.IHandler
	{
		public Color ControlText
		{
			get { return NSColor.ControlText.ToEto(false); }
		}

		public Color HighlightText
		{
			get { return NSColor.AlternateSelectedControlText.ToEto(false); }
		}

		public Color Control
		{
			get { return NSColor.Control.ToEto(false); }
		}

		public Color Highlight
		{
			get { return NSColor.AlternateSelectedControl.ToEto(false); }
		}

		public Color WindowBackground
		{
			get { return NSColor.WindowBackground.ToEto(false); }
		}

		public Color DisabledText
		{
			get { return NSColor.DisabledControlText.ToEto(false); }
		}

		public Color ControlBackground
		{
			get { return NSColor.ControlBackground.ToEto(false); }
		}
	}
}

