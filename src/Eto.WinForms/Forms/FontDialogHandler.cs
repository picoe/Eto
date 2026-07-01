using Eto.WinForms.Drawing;

namespace Eto.WinForms.Forms
{
	public class FontDialogHandler : WidgetHandler<swf.FontDialog, FontDialog, FontDialog.ICallback>, FontDialog.IHandler, CommonDialog.ICancellableHandler
	{
		Font _font;
		readonly Win32.CancellableModalDialog _cancellable = new Win32.CancellableModalDialog();

		public FontDialogHandler()
		{
			Control = new swf.FontDialog
			{
				ShowColor = true,
				ShowEffects = false
			};
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case FontDialog.FontChangedEvent:
					Control.ShowApply = true;
					Control.Apply += Control_Apply;
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		private void Control_Apply(object sender, EventArgs e)
		{
			_font = null;
			Callback.OnFontChanged(Widget, EventArgs.Empty);
		}

		public Font Font
		{
			get
			{
				if (_font == null)
					_font = Control.Font.ToEto();
				return _font;
			}
			set
			{
				_font = value;
				Control.Font = _font.ToSD();
				Callback.OnFontChanged(Widget, EventArgs.Empty);
			}
		}

		public DialogResult ShowDialog(Window parent)
		{
			if (parent?.HasFocus == false)
				parent.Focus();

			var result = _cancellable.Show(() => Control.ShowDialog());
			if (result == swf.DialogResult.OK)
			{
				_font = null;
				Callback.OnFontChanged(Widget, EventArgs.Empty);
				return DialogResult.Ok;
			}
			return DialogResult.Cancel;
		}

		// A WH_CBT hook captured the native dialog's window handle when it was shown (see CancellableModalDialog),
		// so the async ShowDialogAsync can dismiss exactly this dialog when its cancellation token is signalled.
		public void CancelDialog() => _cancellable.Cancel();
	}
}
