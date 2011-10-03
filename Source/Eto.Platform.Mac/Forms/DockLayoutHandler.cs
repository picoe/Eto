using System;
using SD = System.Drawing;
using Eto.Forms;
using Eto.Drawing;
using MonoMac.AppKit;
using MonoMac.Foundation;

namespace Eto.Platform.Mac
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
				SetChildFrame();
			}
		}
		
		public override void SizeToFit ()
		{
			if (child != null)
			{
				AutoSize (child);
				var c = child.ControlObject as NSView;
				if (c != null) {
					SetContainerSize (c.Frame.Size);
				}
			}
			else SetContainerSize (SD.SizeF.Empty);
		}
		
		void SetChildFrame()
		{
			if (child == null) return;
			
			NSView parent = (NSView)ControlObject;
			
			NSView childControl = (NSView)child.ControlObject;
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
			
			childControl.Frame = frame;
		}
				
		public Control Content {
			get {
				return this.child;
			}
			set {
				if (this.child != null) { 
					((NSView)this.child.ControlObject).RemoveFromSuperview(); 
				}
				if (value != null)
				{
					this.child = value;
					NSView childControl = (NSView)child.ControlObject;
					childControl.AutoresizingMask = NSViewResizingMask.WidthSizable | NSViewResizingMask.HeightSizable;
					if (Widget.Container.Loaded) SetChildFrame();
					
					NSView parent = (NSView)ControlObject;
					parent.AddSubview(childControl);
				}
				else this.child = null;
			}
		}
		
		public override void OnLoadComplete ()
		{
			base.OnLoadComplete ();
			SetChildFrame ();
			
			Control.PostsFrameChangedNotifications = true;
			this.AddObserver(NSView.NSViewFrameDidChangeNotification, delegate(ObserverActionArgs e) { 
				var handler = e.Widget.Handler as DockLayoutHandler;
				handler.SetChildFrame();
			});
		}

		public override void SetContainerSize (SD.SizeF size)
		{
			size += Generator.ConvertF (Padding.Size);
			
			base.SetContainerSize (size);
			SetChildFrame();
		}
	}
}
