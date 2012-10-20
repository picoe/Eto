using System;
using Eto.Forms;
using MonoTouch.UIKit;
using System.Linq;
using MonoTouch.Foundation;

namespace Eto.Platform.iOS.Forms.Controls
{
	public class SplitterHandler : iosControl<UIView, Splitter>, ISplitter, IiosViewController
	{
		public UIViewController Controller { get { return SplitController; } }
		
		public MG.MGSplitViewController SplitController { get; set; }
		UIViewController[] children;
		Control panel1;
		Control panel2;
		int? position;

		public class Delegate : MG.MGSplitViewControllerDelegate
		{
		}
		
		public SplitterHandler()
		{
			SplitController = new MG.MGSplitViewController();
			SplitController.Delegate = new MG.MGSplitViewControllerDelegate();
			Control = Controller.View;
			Control.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
			
			children = new UIViewController[2];
			children[0] = new UIViewController();
			children[1] = new UIViewController();
			SplitController.ViewControllers = children;

			SplitController.AllowsDraggingDivider = true;
			SplitController.DividerStyle = MG.MGSplitViewDividerStyle.PaneSplitter;
			SplitController.SplitWidth = 10;
		}
		
		public int Position
		{
			get { return (int)SplitController.SplitPosition; }
			set { 
				//SplitController.SplitPosition = value;
			}
		}
		
		public SplitterOrientation Orientation
		{
			get { return SplitController.Vertical ? SplitterOrientation.Vertical : SplitterOrientation.Horizontal; }
			set { SplitController.Vertical = value == SplitterOrientation.Vertical; }
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
					children[0] = value.GetViewController() ?? new RotatableViewController();
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
					children[1] = value.GetViewController() ?? new RotatableViewController();
					SplitController.ViewControllers = children;
					panel2 = value;
				}
			}
		}
	}
}
