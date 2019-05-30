using System;
using UIKit;
using Eto.Forms;
using ObjCRuntime;
using Eto.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace Eto.iOS.Forms.Controls
{
	internal class RotatableNavigationController : UINavigationController
	{
		public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
		{
			return true; 
		}

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations()
		{
			return UIInterfaceOrientationMask.All;
		}
	}

	public class NavigationHandler : IosView<UIView, Navigation, Navigation.ICallback>, Navigation.IHandler
	{
		readonly List<INavigationItem> items = new List<INavigationItem>();

		public UINavigationController Navigation
		{ 
			get { return (UINavigationController)base.Controller; }
			set { base.Controller = value; }
		}

		UIBarButtonItem[] mainToolBar;
		public UIBarButtonItem[] MainToolBar
		{
			get { return mainToolBar; }
			set
			{
				mainToolBar = value;
				if (Widget.Loaded)
				{
					var vc = Navigation.ViewControllers;
					if (vc.Length > 0)
					{
						vc.Last().ToolbarItems = mainToolBar;
					}
					Navigation.Toolbar.SetItems(mainToolBar, true);
					Navigation.SetToolbarHidden(mainToolBar == null || mainToolBar.Length == 0, true);
				}
			}
		}

		class Delegate : UINavigationControllerDelegate
		{
			public override void WillShowViewController(UINavigationController navigationController, UIViewController viewController, bool animated)
			{
				var items = viewController.ToolbarItems ?? (navigationController.ViewControllers.Length == 1 ? Handler.MainToolBar : null);
				navigationController.SetToolbarItems(items, true);
				navigationController.SetToolbarHidden(items == null || items.Length == 0, true);
			}

			public override void DidShowViewController(UINavigationController navigationController, UIViewController viewController, bool animated)
			{
				var shownItem = Handler.items.FirstOrDefault(r => r.Content.GetViewController(false) == viewController);
				if (shownItem != null)
					Handler.Callback.OnItemShown(Handler.Widget, new NavigationItemEventArgs(shownItem));
				// need to get the view controllers to reset the references to the popped controllers
				// this is due to how xamarin.ios keeps the controllers in an array
				// and this resets that array
				var controllers = Handler.Navigation.ViewControllers;
				var removedItems = Handler.items.Where(r => !controllers.Contains(r.Content.GetViewController(false))).ToList();
				foreach (var removedItem in removedItems)
				{
					Handler.Callback.OnItemRemoved(Handler.Widget, new NavigationItemEventArgs(removedItem));
				}
				Handler.items.RemoveAll(removedItems.Contains);

				/* for testing garbage collection after a view is popped
				#if DEBUG
				GC.Collect();
				GC.WaitForPendingFinalizers();
				#endif
				/**/
			}

			WeakReference handler;

			public NavigationHandler Handler { get { return (NavigationHandler)handler.Target; } set { handler = new WeakReference(value); } }
		}

		public NavigationHandler()
		{
			Navigation = new RotatableNavigationController
			{
				WeakDelegate = new Delegate { Handler = this },
				ToolbarHidden = false
			};
			Control = Navigation.View;
		}

		public void Push(INavigationItem item)
		{
			items.Add(item);
			var view = item.Content.GetViewController();
			view.NavigationItem.Title = item.Text ?? string.Empty;
			if (!(view.View is UIScrollView) && view.EdgesForExtendedLayoutIsSupported())
				view.EdgesForExtendedLayout = UIRectEdge.None;
			view.View.Frame = new CoreGraphics.CGRect(0, 0, 0, 0);
			view.View.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
			Navigation.PushViewController(view, true);
		}

		public void Pop()
		{
			Navigation.PopViewController(true);
		}

		public virtual Size ClientSize
		{
			get { return Size; }
			set { Size = value; }
		}

		public bool RecurseToChildren { get { return true; } }
	}
}

