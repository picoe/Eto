using sd = System.Drawing;
using swf = System.Windows.Forms;
using Eto.Forms;
using System;
using System.Threading.Tasks;

namespace Eto.WinForms.Forms
{
	public class DialogHandler : WindowHandler<swf.Form, Dialog, Dialog.ICallback>, Dialog.IHandler
	{
		Button button;
		Button abortButton;

		
		public DialogHandler()
		{
			Control = new swf.Form
			{
				StartPosition = swf.FormStartPosition.CenterParent,
				AutoSize = true,
				Size = sd.Size.Empty,
				MinimumSize = sd.Size.Empty,
				ShowInTaskbar = false,
				ShowIcon = false,
				MaximizeBox = false,
				MinimizeBox = false
			};
		}

		protected override swf.FormBorderStyle DefaultWindowStyle
		{
			get { return swf.FormBorderStyle.FixedDialog; }
		}

		public Button AbortButton
		{
			get
			{
				return abortButton;
			}
			set
			{
				abortButton = value;
				if (abortButton != null)
				{
					var b = abortButton.ControlObject as swf.IButtonControl;
					Control.CancelButton = b;
				}
				else
					Control.CancelButton = null;
			}
		}

		public Button DefaultButton
		{
			get
			{
				return button;
			}
			set
			{
				button = value;
				if (button != null)
				{
					var b = button.ControlObject as swf.IButtonControl;
					Control.AcceptButton = b;
				}
				else
					Control.AcceptButton = null;
			}
		}

		public DialogDisplayMode DisplayMode { get; set; }

		public void ShowModal(Control parent)
		{
			if (parent != null)
			{
				var parentWindow = parent.ParentWindow;
				if (parentWindow != null)
				{
					var parentHandler = parentWindow.Handler as IWindowHandler;
					if (parentHandler != null)
					{
						Control.ShowDialog(parentHandler.Win32Window);
						return;
					}
				}
			}
			Control.ShowDialog();
		}

		public Task ShowModalAsync(Control parent)
		{
			var tcs = new TaskCompletionSource<bool>();
			Application.Instance.AsyncInvoke(() =>
			{
				if (parent != null)
					Control.ShowDialog((swf.Control)parent.ControlObject);
				else
					Control.ShowDialog();

				tcs.SetResult(true);
			});
			return tcs.Task;
		}
	}
}
