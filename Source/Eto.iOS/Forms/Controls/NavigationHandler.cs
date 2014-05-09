using System;
using MonoTouch.UIKit;
using Eto.Forms;
using MonoTouch.ObjCRuntime;
using Eto.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace Eto.iOS.Forms.Controls
{
	internal class RotatableNavigationController : UINavigationController
	{
		[Obsolete]
		public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
		{
			return true; 
		}

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations()
		{
			return UIInterfaceOrientationMask.All;
		}
	}

	public class NavigationHandler : IosControl<UIView, Navigation, Navigation.ICallback>, Navigation.IHandler
	{
		readonly List<INavigationItem> items = new List<INavigationItem>();

		public UINavigationController Navigation
		{ 
			get { return (UINavigationController)base.Controller; }
			set { base.Controller = value; }
		}

		class Delegate : UINavigationControllerDelegate
		{
			public override void DidShowViewController(UINavigationController navigationController, UIViewController viewController, bool animated)
			{
				Handler.Widget.OnItemShown(EventArgs.Empty);
				// need to get the view controllers to reset the references to the popped controllers
				// this is due to how xamarin.ios keeps the controllers in an array
				// and this resets that array
				var controllers = Handler.Navigation.ViewControllers;
				Handler.items.RemoveAll(r => !controllers.Contains(r.Content.GetViewController()));
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
			Navigation = new UINavigationController
			{
				WeakDelegate = new Delegate { Handler = this }
			};
			Control = Navigation.View;
		}

		public void Push(INavigationItem item)
		{
			items.Add(item);
			var view = item.Content.GetViewController();
			view.NavigationItem.Title = item.Text;
			//
			if (!(view.View is UIScrollView) && view.RespondsToSelector(new Selector("setEdgesForExtendedLayout:")))
				view.EdgesForExtendedLayout = UIRectEdge.None;
			view.View.Frame = new System.Drawing.RectangleF(0, 0, 0, 0);
			view.View.AutoresizingMask = UIViewAutoresizing.All;
			Navigation.PushViewController(view, true);
		}

		public void Pop()
		{
			Navigation.PopViewControllerAnimated(true);
		}

		public virtual Size ClientSize
		{
			get { return Size; }
			set { Size = value; }
		}

		public bool RecurseToChildren { get { return true; } }
	}
}

