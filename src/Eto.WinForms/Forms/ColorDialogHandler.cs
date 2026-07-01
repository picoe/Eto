namespace Eto.WinForms.Forms
{
	public class ColorDialogHandler : WidgetHandler<swf.ColorDialog, ColorDialog, ColorDialog.ICallback>, ColorDialog.IHandler, CommonDialog.ICancellableHandler
	{
		static int[] customColors;
		readonly Win32.CancellableModalDialog _cancellable = new Win32.CancellableModalDialog();

		public ColorDialogHandler()
		{
			Control = new swf.ColorDialog
			{
				AnyColor = true,
				AllowFullOpen = true,
				FullOpen = true
			};
		}

		public Color Color
		{
			get { return Control.Color.ToEto(); }
			set { Control.Color = value.ToSD(); }
		}

		public bool AllowAlpha { get; set; }

		public bool SupportsAllowAlpha => false;

		public DialogResult ShowDialog(Window parent)
		{
			if (customColors != null) Control.CustomColors = customColors;

			if (parent?.HasFocus == false)
				parent.Focus();

			var result = _cancellable.Show(() => parent != null
				? Control.ShowDialog(parent.GetContainerControl())
				: Control.ShowDialog());

			if (result == swf.DialogResult.OK)
			{
				Callback.OnColorChanged(Widget, EventArgs.Empty);
			}

			customColors = Control.CustomColors;

			return result.ToEto();
		}

		// A WH_CBT hook captured the native dialog's window handle when it was shown (see CancellableModalDialog),
		// so the async ShowDialogAsync can dismiss exactly this dialog when its cancellation token is signalled.
		public void CancelDialog() => _cancellable.Cancel();
	}
}

