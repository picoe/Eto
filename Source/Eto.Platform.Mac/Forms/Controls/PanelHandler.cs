using System;
using Eto.Forms;
using MonoMac.AppKit;

namespace Eto.Platform.Mac
{
	public class PanelHandler : MacContainer<NSView, Panel>, IPanel
	{
		
		class MyView : NSView {
			
			public override bool IsFlipped {
				get {
					return true;
				}
			}
		}

		public PanelHandler()
		{
			Control = new MyView();
		}

		public override object ContainerObject {
			get {
				return Control;
			}
		}
		
		
	}
}
