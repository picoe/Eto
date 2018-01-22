using Eto.Forms;
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

namespace Eto.Mac.Forms.Actions
{
	public class MacCommand : Command
	{
		public Selector Selector { get; private set; }
		
		public MacCommand(string id, string text, string selector)
		{
			ID = id;
			MenuText = ToolBarText = text;
			Selector = new Selector(selector);
		}
	}
}

