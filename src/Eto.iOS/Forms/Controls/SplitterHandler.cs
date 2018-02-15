using System;
using Eto.Forms;
using UIKit;
using System.Linq;
using Foundation;
using System.Diagnostics;
using Eto.Mac.Forms;

namespace Eto.iOS.Forms.Controls
{
	public class SplitterHandler : MacContainer<UIView, Splitter, Splitter.ICallback>, Splitter.IHandler
	{
		protected override UIViewController CreateController()
		{
			return /*(UIViewController)MGSplitController ??*/ SplitController;
		}

		public override UIView ContainerControl { get { return Control; } }
		
		public UISplitViewController SplitController { get; set; }

		//public MG.MGSplitViewController MGSplitController { get; set; }

		UIViewController[] children;
		Control panel1;
		Control panel2;

		public bool UseMGSplitViewController { get; set; }

		class SplitControllerDelegate : UISplitViewControllerDelegate
		{
			public override bool ShouldHideViewController(UISplitViewController svc, UIViewController viewController, UIInterfaceOrientation inOrientation)
			{
				return false;
			}			
		}
		
		public SplitterHandler ()
		{
			UseMGSplitViewController = false;
		}

		protected override void Initialize ()
		{
			base.Initialize ();

			/*if (UseMGSplitViewController) {
				MGSplitController = new MG.MGSplitViewController ();
				Control = MGSplitController.View;
			} else*/ {
				SplitController = new UISplitViewController ();
				SplitController.Delegate = new SplitControllerDelegate();
				Control = SplitController.View;
			}
			//SplitController.Delegate = new MG.MGSplitViewControllerDelegate();
			Control.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
			
			children = new UIViewController[2];
			children [0] = new UIViewController ();
			children [1] = new UIViewController ();
			SetViewControllers ();

			/*if (UseMGSplitViewController) {
				MGSplitController.AllowsDraggingDivider = true;
				MGSplitController.DividerStyle = MG.MGSplitViewDividerStyle.PaneSplitter;
				MGSplitController.SplitWidth = 10;
			}*/
		}

		/**/
		public int Position {
			get { 
				/*if (UseMGSplitViewController)
					return (int)MGSplitController.SplitPosition;
				else*/
					return 0;
			}
			set { 
				//SplitController.SplitPosition = value;
			}
		}
		
		public Orientation Orientation {
			get {
				/*if (UseMGSplitViewController)
					return MGSplitController.Vertical ? Orientation.Vertical : Orientation.Horizontal;
				else*/
				return Orientation.Horizontal;
			}
			set { 
				/*if (UseMGSplitViewController)
					MGSplitController.Vertical = value == Orientation.Vertical;
				else if (value == Orientation.Vertical)*/
					Debug.WriteLine ("UISplitViewController cannot set orientation to vertical");
			}
		}

		public SplitterFixedPanel FixedPanel {
			get {
				return SplitterFixedPanel.None;
			}
			set {
				
			}
		}

		void SetViewControllers ()
		{
			/*if (UseMGSplitViewController)
				MGSplitController.ViewControllers = children;
			else*/
				SplitController.ViewControllers = children;
		}

		public Control Panel1 {
			get { return panel1; }
			set {
				if (panel1 != value) {
					children [0] = value.GetViewController () ?? new RotatableViewController ();
					SetViewControllers ();
					panel1 = value;
				}
			}
		}

		public Control Panel2 {
			get { return panel2; }
			set {
				if (panel2 != value) {
					children [1] = value.GetViewController () ?? new RotatableViewController ();
					SetViewControllers ();
					panel2 = value;
				}
			}
		}

		public double RelativePosition
		{
			get
			{
				return 0;
			}
			set
			{
				
			}
		}

		public int SplitterWidth
		{
			get
			{
				return 1;
			}
			set
			{
				
			}
		}
	}
}
