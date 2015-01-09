using System;
using System.Linq;
using UIKit;
using Eto.Forms;
using System.Threading.Tasks;

namespace Eto.iOS.Forms
{
	public class DialogHandler<TWidget, TCallback> : IosWindow<UIView, TWidget, TCallback>, Dialog.IHandler, Form.IHandler
		where TWidget: Window
		where TCallback: Window.ICallback
	{
		bool inNav;
		TaskCompletionSource<bool> closedcs;
		TaskCompletionSource<bool> opencs;

		protected override UIViewController CreateController()
		{
			return new RotatableViewController { View = Control };
		}

		public DialogHandler()
		{
			Control = new UIView();
			Control.Frame = UIScreen.MainScreen.Bounds;
		}

		protected override void Initialize()
		{
			base.Initialize();
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
			Close(true);
		}

		async void Close(bool closing)
		{
			if (inNav)
			{
				var viewControllers = Controller.NavigationController.ViewControllers.ToList();
				int index = viewControllers.IndexOf(Controller);
				if (index > 1)
					Controller.NavigationController.PopToViewController(viewControllers[index - 1], true);
				if (closing)
				{
					Callback.OnClosed(Widget, EventArgs.Empty);
					closedcs.SetResult(true);
				}
			}
			else
			{
				if (opencs != null)
					await opencs.Task;
				var tcs = new TaskCompletionSource<bool>();
				Controller.DismissViewController(animated: true, completionHandler: () => 
				{
					tcs.SetResult(true);
					if (closing)
					{
						Callback.OnClosed(Widget, EventArgs.Empty);
						closedcs.SetResult(true);
					}
				});
				await tcs.Task;
			}
		}

		public async void ShowModal(Control parent)
		{
			await ShowModalAsync(parent);
		}

		public async Task ShowModalAsync(Control parent)
		{
			await ShowModalAsync(parent, true);
		}

		async Task ShowModalAsync(Eto.Forms.Control parent, bool opening)
		{
			closedcs = new TaskCompletionSource<bool>();
			inNav = false;
			if (parent != null && (DisplayMode.HasFlag(DialogDisplayMode.Navigation) || DisplayMode == DialogDisplayMode.Default))
			{
				var iosView = parent.Handler as IIosViewControllerSource;
				if (iosView != null && iosView.Controller != null)
				{
					var nav = iosView.Controller.NavigationController;
					if (nav != null)
					{
						nav.PushViewController(Controller, true);
						inNav = true;
					}
				}
			}
			if (!inNav)
			{
				Controller.ModalPresentationStyle = WindowState == WindowState.Maximized ? UIModalPresentationStyle.FullScreen : UIModalPresentationStyle.FormSheet;

				var top = UIApplication.SharedApplication.KeyWindow.TopMostController();
				opencs = new TaskCompletionSource<bool>();
				top.PresentViewController(Controller, animated: true, completionHandler: () => opencs.SetResult(true));
				await opencs.Task;
				opencs = null;
			}
			if (opening)
				await closedcs.Task;
		}

		public void Show()
		{
			ShowModal(null);
		}

		public override void SendToBack()
		{
			base.SendToBack();

			Close(false);
		}

		public override async void BringToFront()
		{
			base.BringToFront();
			await ShowModalAsync(null, false);
		}
	}
}

