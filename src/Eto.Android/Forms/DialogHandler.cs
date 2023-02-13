using System;
using Eto.Forms;
using Eto.Drawing;
using a = Android;
using av = Android.Views;
using aw = Android.Widget;
using aa = Android.App;
using System.Threading.Tasks;
using Android.Content;
using System.Linq;

namespace Eto.Android.Forms
{
	/// <summary>
	/// Handler for <see cref="Form"/>
	/// </summary>
	/// <copyright>(c) 2013 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class DialogHandler : AndroidWindow<Dialog, Dialog.ICallback>, Dialog.IHandler
	{
		public DialogHandler()
		{
		}

		private aa.AlertDialog dialog;

		public bool ShowActivated { get; set; }

		public bool CanFocus { get; set; }

		public DialogDisplayMode DisplayMode { get; set; }

		public void ShowModal() => throw new NotSupportedException("Android platform only supports async dialogs");

		public Task ShowModalAsync()
		{
			var Completer = new DialogCompleter(this);

			var ParentActivity = (Activity ?? ApplicationHandler.Instance.TopActivity);
			Activity = ParentActivity;

			var InnerView = ContainerControl;

			if(Widget.PositiveButtons.Any() || Widget.NegativeButtons.Any())
			{
				var ButtonPanel = new aw.LinearLayout(Platform.AppContextThemed)
				{
					Orientation = aw.Orientation.Horizontal,
				};

				foreach(var button in Widget.PositiveButtons.Concat(Widget.NegativeButtons))
					ButtonPanel.AddView(button.GetContainerView());

				var InnerStack = new aw.LinearLayout(Platform.AppContextThemed)
				{
					Orientation = aw.Orientation.Vertical
				};

				InnerStack.AddView(InnerView, new aw.LinearLayout.LayoutParams(av.ViewGroup.LayoutParams.MatchParent, 0, 1));
				InnerStack.AddView(ButtonPanel, new aw.LinearLayout.LayoutParams(av.ViewGroup.LayoutParams.MatchParent, av.ViewGroup.LayoutParams.WrapContent, 0));
				
				InnerView = InnerStack;
			}

			var Builder = new aa.AlertDialog.Builder(ParentActivity)
				.SetTitle(Title)
				.SetView(InnerView)
				.SetOnDismissListener(Completer)
				.SetOnCancelListener(Completer);

			dialog = Builder.Create();
			dialog.SetOnShowListener(Completer);
			dialog.Show();

			return Completer.Task;
		}

		public override void Close()
		{
			dialog?.Dismiss();
		}

		private class DialogCompleter : Java.Lang.Object, a.Content.IDialogInterfaceOnDismissListener, a.Content.IDialogInterfaceOnCancelListener, a.Content.IDialogInterfaceOnShowListener
		{
			private readonly TaskCompletionSource<DialogResult> _Source;
			private readonly DialogHandler _Handler;

			public Task Task => _Source.Task;

			public DialogCompleter(DialogHandler handler)
			{
				_Handler = handler;
				_Source = new TaskCompletionSource<DialogResult>();
			}

			void IDialogInterfaceOnDismissListener.OnDismiss(IDialogInterface dialog)
			{
				_Source.SetResult(DialogResult.Cancel);
			}

			void IDialogInterfaceOnCancelListener.OnCancel(IDialogInterface dialog)
			{
				_Handler.AbortButton?.PerformClick();
			}

			void IDialogInterfaceOnShowListener.OnShow(IDialogInterface dialog)
			{
				_Handler.OnShown();
			}
		}

		private void OnShown()
		{
			ContainerControl.RequestFocus();

			Callback.OnShown(Widget, EventArgs.Empty);
		}

		public Button DefaultButton { get; set; }

		public Button AbortButton { get; set; }

		public void InsertDialogButton(Boolean positive, Int32 index, Button item)
		{
			// Just get this from widget later.
		}

		public void RemoveDialogButton(Boolean positive, Int32 index, Button item)
		{
			// Just get this from widget later.
		}
	}
}
