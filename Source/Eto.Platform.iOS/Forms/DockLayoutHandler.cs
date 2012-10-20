using System;
using SD = System.Drawing;
using Eto.Forms;
using Eto.Drawing;
using MonoTouch.UIKit;

namespace Eto.Platform.iOS.Forms
{
	public class DockLayoutHandler : iosLayout<UIView, DockLayout>, IDockLayout
	{
		Control child;
		Padding padding;

		public DockLayoutHandler ()
		{
			DisposeControl = false;
		}
		
		public override UIView Control {
			get {
				return Widget.Container != null ? (UIView)Widget.Container.ContainerObject : null;
			}
			protected set {
				base.Control = value;
			}
		}
		
		public Eto.Drawing.Padding Padding {
			get { return padding; }
			set {
				padding = value;
				if (this.Widget.Loaded)
					Layout ();
			}
		}
		
		public override Eto.Drawing.Size GetPreferredSize ()
		{
			return child.GetPreferredSize ();
		}

		public override void LayoutChildren ()
		{
			if (child == null) return;
			
			UIView parent = this.Control;
			
			UIView childControl = (UIView)child.ControlObject;
			var frame = parent.Frame;

			frame.Y = padding.Top;
			frame.X = padding.Left;
			frame.Width -= padding.Horizontal;
			frame.Height -= padding.Vertical;
			
			childControl.Frame = frame;
		}

		public override void OnLoadComplete ()
		{
			base.OnLoadComplete ();
			if (!this.Control.Frame.IsEmpty)
				Layout ();
			Widget.Container.SizeChanged += delegate(object sender, EventArgs e) {
				Layout ();
			};
		}

		public Control Content {
			get {
				return child;
			}
			set {
				if (child != null) { 
					((UIView)child.ControlObject).RemoveFromSuperview(); child = null; 
				}
				if (value != null)
				{
					child = value;
					var childControl = (UIView)child.ControlObject;
					childControl.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
					if (this.Widget.Loaded)
						Layout ();
					UIView parent = this.Control;
					parent.AddSubview(childControl);
				}
				
			}
		}
	}
}
