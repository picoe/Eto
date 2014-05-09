using System;
using System.Linq;
using MonoTouch.UIKit;
using Eto.Forms;
using System.Threading.Tasks;

namespace Eto.iOS.Forms
{
	internal class RotatableViewController : UIViewController
	{
		public RotatableViewController()
		{
			AutomaticallyAdjustsScrollViewInsets = true;
		}

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations()
		{
			return UIInterfaceOrientationMask.All;
		}

		[Obsolete]
		public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
		{
			return true;
		}
	}

	public class DialogHandler : IosWindow<UIView, Dialog, Dialog.ICallback>, IDialog
	{
		bool inNav;
		TaskCompletionSource<bool> completionSource;

		~DialogHandler()
		{
			Console.WriteLine("woo");
		}

		public DialogHandler()
		{
			Control = new UIView();
			Control.Frame = UIScreen.MainScreen.Bounds;
			Controller.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
		}

		public Button AbortButton { get; set; }

		public Button DefaultButton { get; set; }

		public DialogDisplayMode DisplayMode { get; set; }

		public override string Title
		{
			get { return Controller.Title; }
			set { Controller.Title = value; }
		}

		public override void Close()
		{
			if (inNav)
			{
				var viewControllers = Controller.NavigationController.ViewControllers.ToList();
				int index = viewControllers.IndexOf(Controller);
				if (index > 1)
					Controller.NavigationController.PopToViewController(viewControllers[index - 1], true);
				completionSource.SetResult(true);
			}
			else
			{
				Controller.DismissViewController(animated: true, completionHandler: () => completionSource.SetResult(true));
			}
		}

		public async void ShowModal(Control parent)
		{
			await ShowModalAsync(parent);
		}

		public Task ShowModalAsync(Control parent)
		{
			completionSource = new TaskCompletionSource<bool>();
			inNav = false;
			if (DisplayMode.HasFlag(DialogDisplayMode.Navigation) || DisplayMode == DialogDisplayMode.Default)
			{
				var controller = parent.Handler as IIosView;
				if (controller != null)
				{
					var nav = controller.Controller.NavigationController;
					if (nav != null)
					{
						nav.PushViewController(Controller, true);
						inNav = true;
					}
				}
			}

			if (!inNav)
			{
				var top = UIApplication.SharedApplication.KeyWindow.TopMostController();

				top.PresentViewController(Controller, animated: true, completionHandler: null);
			}
			return completionSource.Task;
		}
	}
}

