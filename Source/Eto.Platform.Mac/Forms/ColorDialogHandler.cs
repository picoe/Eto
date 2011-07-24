using System;
using MonoMac.AppKit;
using Eto.Forms;
using MonoMac.ObjCRuntime;
using MonoMac.Foundation;
using Eto.Drawing;

namespace Eto.Platform.Mac.Forms
{
	interface IColorDialogHandler
	{
		Color Color { get; set; }
		ColorDialog Widget { get; }
		NSColorPanel Control { get; }
	}
	
	class ColorHandler : NSWindowDelegate
	{
		public static ColorHandler Instance { get; set; }
		public IColorDialogHandler Handler { get; set; }
		
		[Export("changeColor:")]
		public void changeColor(NSColorPanel panel)
		{
			Handler.Color = Generator.Convert(panel.Color.UsingColorSpace (NSColorSpace.DeviceRGB));
			Handler.Widget.OnColorChanged(EventArgs.Empty);
		}
		
		public override void WillClose (NSNotification notification)
		{
			Handler.Control.SetTarget (null);
			Handler.Control.SetAction (null);
			ColorHandler.Instance = null;
		}
	}
	
	public class ColorDialogHandler : WidgetHandler<NSColorPanel, ColorDialog>, IColorDialog, IColorDialogHandler
	{
		
		public ColorDialogHandler()
		{
			Control = NSColorPanel.SharedColorPanel;
		}
		
		public Color Color { get; set; }

		#region IDialog implementation
		
		public DialogResult ShowDialog (Window parent)
		{
			NSWindow parentWindow;
			if (parent != null) {
				if (parent.ControlObject is NSWindow) parentWindow = (NSWindow)parent.ControlObject;
				else if (parent.ControlObject is NSView) parentWindow = ((NSView)parent.ControlObject).Window;
				else parentWindow = NSApplication.SharedApplication.KeyWindow;
			}
			else parentWindow = NSApplication.SharedApplication.KeyWindow;
			
			if (ColorHandler.Instance == null) ColorHandler.Instance = new ColorHandler();
			ColorHandler.Instance.Handler = this;
			Control.Color = Generator.ConvertNS (this.Color);
			//Control.Continuous = false;
			Control.Delegate = ColorHandler.Instance;
			
			Control.SetTarget (ColorHandler.Instance);
			Control.SetAction (new Selector("changeColor:"));
			
			NSApplication.SharedApplication.OrderFrontColorPanel (parentWindow);
			
			
			return DialogResult.None; // signal that we are returning right away!
		}
		
		#endregion
	}
}

