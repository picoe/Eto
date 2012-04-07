using System;
using Eto.Forms;
using MonoTouch.UIKit;
using System.Linq;

namespace Eto.Platform.iOS.Forms.Controls
{
	public class SplitterHandler : iosControl<UIView, Splitter>, ISplitter, IiosViewController
	{
		public UIViewController Controller { get { return SplitController; } }
		
		UISplitViewController SplitController { get; set; }
		UIViewController[] children;
		Control panel1;
		Control panel2;
		int? position;
		
		class RotatableSplitViewController : UISplitViewController
		{
			public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
			{
				return true; //return base.ShouldAutorotateToInterfaceOrientation (toInterfaceOrientation);
			}
		}
		
		public SplitterHandler()
		{
			SplitController = new RotatableSplitViewController();
			Control = Controller.View;
			
			children = new UIViewController[2];
			children[0] = new UIViewController();
			children[1] = new UIViewController();
			SplitController.ViewControllers = children;
		}
		
		public int Position
		{
			get { return position ?? 0; }
			set {
				position = value;
				//var adjview = control.Subviews[0];
				//adjview.SetFrameSize(new System.Drawing.SizeF(adjview.Frame.Height, (float)position));
				//control.AdjustSubviews();
			}
		}
		
		public SplitterOrientation Orientation
		{
			get
			{
				//if (control.IsVertical) return SplitterOrientation.Vertical;
				//else return SplitterOrientation.Horizontal;
				return SplitterOrientation.Horizontal;
			}
			set
			{
				/*switch (value)
				{
					default:
					case SplitterOrientation.Horizontal: control.IsVertical = false; break;
					case SplitterOrientation.Vertical: control.IsVertical = true; break;
				}*/
			}
		}
		
		public SplitterFixedPanel FixedPanel {
			get {
				return SplitterFixedPanel.None;
			}
			set {
				
			}
		}

		public Control Panel1
		{
			get { return panel1; }
			set
			{
				if (panel1 != value)
				{
					children[0] = (value != null) ? value.GetViewController() : new RotatableViewController();
					SplitController.ViewControllers = children;
					panel1 = value;
				}
			}
		}

		public Control Panel2
		{
			get { return panel2; }
			set
			{
				if (panel2 != value)
				{
					children[1] = (value != null) ? value.GetViewController() : new RotatableViewController();
					SplitController.ViewControllers = children;
					panel2 = value;
				}
			}
		}
	}
}
