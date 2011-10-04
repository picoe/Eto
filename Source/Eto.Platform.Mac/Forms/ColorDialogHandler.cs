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
		
		[Export("modalClosed:")]
		public void modalClosed(NSNotification notification)
		{
			NSColorPanel.SharedColorPanel.PerformClose (this);
			NSNotificationCenter.DefaultCenter.RemoveObserver (this);
		}
	}
	
	public class ColorDialogHandler : MacObject<NSColorPanel, ColorDialog>, IColorDialog, IColorDialogHandler
	{
		
		public ColorDialogHandler()
		{
			Control = NSColorPanel.SharedColorPanel;
		}
		
		public Color Color { get; set; }

		#region IDialog implementation
		
		public DialogResult ShowDialog (Window parent)
		{
			//Control = new NSColorPanel();
			NSWindow parentWindow;
			//Console.WriteLine ("Parent: {0}. {1}, {2}", parent, parent.ControlObject, NSApplication.SharedApplication.ModalWindow);
			if (parent != null) {
				if (parent.ControlObject is NSWindow) parentWindow = (NSWindow)parent.ControlObject;
				else if (parent.ControlObject is NSView) parentWindow = ((NSView)parent.ControlObject).Window;
				else parentWindow = NSApplication.SharedApplication.KeyWindow;
			}
			else parentWindow = NSApplication.SharedApplication.KeyWindow;
			
			ColorHandler.Instance = new ColorHandler{ Handler = this };
			Control.Delegate = ColorHandler.Instance;
			Control.SetTarget (null);
			Control.SetAction (null);
			Control.Color = Generator.ConvertNS (this.Color);
			
			Control.SetTarget (ColorHandler.Instance);
			Control.SetAction (new Selector("changeColor:"));

			//Control.Continuous = false;
			bool isModal = false;
			if (parentWindow != null) {
				if (parentWindow == NSApplication.SharedApplication.ModalWindow)
				{
					//Control.WorksWhenModal = true;
					//Control.ParentWindow = parentWindow;
					NSNotificationCenter.DefaultCenter.AddObserver(ColorHandler.Instance, new Selector("modalClosed:"), new NSString("NSWindowWillCloseNotification"), parentWindow);
					isModal = true;
				}
			}
			
			
			// work around for modal dialogs wanting to show the color panel.. only works when the panel is key
			
			//if (isModal) Control.MakeKeyAndOrderFront (parentWindow);
			//else Control.OrderFront (parentWindow);
			NSApplication.SharedApplication.OrderFrontColorPanel (parentWindow);
			if (isModal) Control.MakeKeyWindow();
			//Control.OrderFront (parentWindow);
			
			
			return DialogResult.None; // signal that we are returning right away!
		}
		
		#endregion
	}
}

