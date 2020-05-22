using System;
using swf = System.Windows.Forms;
using sd = System.Drawing;
using Eto.Forms;
using Eto.Drawing;
using Eto.WinForms.Drawing;

namespace Eto.WinForms.Forms
{
	public class FontDialogHandler : WidgetHandler<swf.FontDialog, FontDialog, FontDialog.ICallback>, FontDialog.IHandler
	{
		Font _font;

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
			var result = Control.ShowDialog();
			if (result == swf.DialogResult.OK)
			{
				_font = null;
				Callback.OnFontChanged(Widget, EventArgs.Empty);
				return DialogResult.Ok;
			}
			return DialogResult.Cancel;
		}
	}
}
