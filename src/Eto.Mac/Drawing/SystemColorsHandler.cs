using System;
using Eto.Drawing;
using System.Threading;

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
		public Color ControlText => NSColor.ControlText.ToEtoWithAppearance(false);

		public Color HighlightText => NSColor.AlternateSelectedControlText.ToEtoWithAppearance(false);

		public Color Control => NSColor.Control.ToEtoWithAppearance(false);

		public Color Highlight => NSColor.AlternateSelectedControl.ToEtoWithAppearance(false);

		public Color WindowBackground => NSColor.WindowBackground.ToEtoWithAppearance(false);

		public Color DisabledText => NSColor.DisabledControlText.ToEtoWithAppearance(false);

		public Color ControlBackground => NSColor.ControlBackground.ToEtoWithAppearance(false);

		public Color SelectionText => NSColor.SelectedText.ToEtoWithAppearance(false);

		public Color Selection => NSColor.SelectedTextBackground.ToEtoWithAppearance(false);

		// todo: remove when CI supports NSColor.LinkColor
		static IntPtr s_classHandle = Class.GetHandle(typeof(NSColor));
		static IntPtr s_selLinkColorHandle = Selector.GetHandle("linkColor");
		static Lazy<bool> s_supportsLinkColor = new Lazy<bool>(() => ObjCExtensions.RespondsToSelector<NSColor>(s_selLinkColorHandle));

		public Color LinkText
		{
			get {
				if (s_supportsLinkColor.Value)
					return Runtime.GetNSObject<NSColor>(Messaging.IntPtr_objc_msgSend(s_classHandle, s_selLinkColorHandle)).ToEtoWithAppearance(false);

				return Highlight;
			}
		}
	}
}

