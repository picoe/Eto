using System;
using SD = System.Drawing;
using Eto.Forms;
using Eto.Drawing;
using MonoMac.AppKit;
using MonoMac.Foundation;

namespace Eto.Platform.Mac.Forms
{
	
	
	public class DockLayoutHandler : MacLayout<NSView, DockLayout>, IDockLayout
	{
		Control child;
		Padding padding;
		
		public override NSView Control {
			get {
				return (NSView)Widget.Container.ContainerObject;
			}
			protected set {
				base.Control = value;
			}
		}
		
		public Eto.Drawing.Padding Padding {
			get { return padding; }
			set {
				padding = value;
				UpdateParentLayout ();
			}
		}
		
		public override Size GetPreferredSize ()
		{
			if (child != null)
			{
				return GetPreferredSize (child) + Padding.Size;
			}
			else return Size.Empty;
		}
		
		public override void LayoutChildren ()
		{
			if (child == null) return;
			
			NSView parent = (NSView)ControlObject;
			
			NSView childControl = child.GetContainerView ();
			var frame = parent.Frame;
			
			if (frame.Width > padding.Horizontal && frame.Height > padding.Vertical)
			{
				frame.X = padding.Left;
				frame.Width -= padding.Horizontal;
				frame.Y = padding.Bottom;
				frame.Height -= padding.Vertical;
			}
			else {
				frame.X = 0;
				frame.Y = 0;
			}
			
			if (childControl.Frame != frame)
				childControl.Frame = frame;
		}
				
		public Control Content {
			get {
				return this.child;
			}
			set {
				if (this.child != null) { 
					this.child.GetContainerView ().RemoveFromSuperview(); 
				}
				if (value != null)
				{
					this.child = value;
					NSView childControl = child.GetContainerView ();
					childControl.AutoresizingMask = NSViewResizingMask.WidthSizable | NSViewResizingMask.HeightSizable;
					
					NSView parent = (NSView)ControlObject;
					parent.AddSubview(childControl);
				}
				else
					this.child = null;
				if (Widget.Loaded || Widget.Container.Loaded) {
					UpdateParentLayout ();
				}
			}
		}
		
		public override void OnLoadComplete ()
		{
			base.OnLoadComplete ();
			
			Control.PostsFrameChangedNotifications = true;
			this.AddObserver(NSView.NSViewFrameDidChangeNotification, delegate(ObserverActionArgs e) { 
				var handler = e.Widget.Handler as DockLayoutHandler;
				handler.LayoutChildren();
			});
		}

		public override void SetContainerSize (SD.SizeF size)
		{
			size += Generator.ConvertF (Padding.Size);
			
			base.SetContainerSize (size);
		}
	}
}
