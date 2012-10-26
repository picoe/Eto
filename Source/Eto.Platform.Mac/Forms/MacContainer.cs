using System;
using Eto.Forms;
using Eto.Drawing;
using MonoMac.AppKit;
using MonoMac.Foundation;
using SD = System.Drawing;
using System.Linq;

namespace Eto.Platform.Mac.Forms
{
	public interface IMacContainer : IMacAutoSizing
	{
		void SetContentSize (SD.SizeF contentSize);
		
		void LayoutChildren ();
	}
	
	public abstract class MacContainer<T, W> : MacView<T, W>, IContainer, IMacContainer
		where T: NSView
		where W: Container
	{
	

		public abstract object ContainerObject {
			get;
		}
		
		public virtual Size ClientSize {
			get { return Size; }
			set { Size = value; }
		}
		
		public override Size? MinimumSize {
			get;
			set;
		}
		
		public virtual void SetLayout (Layout layout)
		{
			var maclayout = layout.Handler as IMacLayout;
			if (maclayout == null)
				return;
			var control = maclayout.LayoutObject as NSView;
			if (control != null) {
				var container = ContainerObject as NSView;
				//control.AutoresizingMask = NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable;
				control.SetFrameSize (container.Frame.Size);
				container.AddSubview (control);
			}
		}
		
		protected override Size GetNaturalSize ()
		{
			var size = base.GetNaturalSize ();
			if (Widget.Layout != null && Widget.Layout.InnerLayout != null) {
				var layout = Widget.Layout.InnerLayout.Handler as IMacLayout;
				if (layout != null)
					size = layout.GetPreferredSize (Size.MaxValue);
			}
			return size;
		}
		
		public virtual void SetContentSize (SD.SizeF contentSize)
		{
			if (MinimumSize != null) {
				contentSize.Width = Math.Max (contentSize.Width, MinimumSize.Value.Width);
				contentSize.Height = Math.Max (contentSize.Height, MinimumSize.Value.Height);
			}
			if ((Control.AutoresizingMask & (NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable)) == (NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable)) {
				if (Widget.ParentLayout != null) {
					var layout = Widget.ParentLayout.InnerLayout.Handler as IMacLayout;
					if (layout != null)
						layout.SetContainerSize (contentSize);
				}
			}
		}
		
		public virtual void LayoutChildren ()
		{
			if (Widget.Layout != null && Widget.Layout.InnerLayout != null) {
				var childLayout = Widget.Layout.InnerLayout.Handler as IMacLayout;
				if (childLayout != null) {
					childLayout.LayoutChildren ();
				}
			}
		}
	}
}

