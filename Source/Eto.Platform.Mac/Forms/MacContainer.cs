using System;
using Eto.Forms;
using Eto.Drawing;
using MonoMac.AppKit;
using MonoMac.Foundation;
namespace Eto.Platform.Mac
{
	
	public abstract class MacContainer<T, W> : MacView<T, W>, IContainer
		where T: NSView
		where W: Container
	{
	

		public abstract object ContainerObject {
			get;
		}
		
		public virtual Size ClientSize
		{
			get { return Size; }
			set { Size = value; }
		}
		
		public virtual void SetLayout(Layout layout)
		{
			var maclayout = layout.Handler as IMacLayout;
			if (maclayout == null) return;
			var control = maclayout.LayoutObject as NSView;
			if (control != null)
			{
				var container = ContainerObject as NSView;
				control.SetFrameSize(container.Frame.Size);
				container.AddSubview(control);
			}
		}
		
	}
}

