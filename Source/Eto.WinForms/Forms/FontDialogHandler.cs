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
		Font font;

		public FontDialogHandler()
		{
			Control = new swf.FontDialog
			{
				ShowColor = true,
				ShowEffects = false
			};
		}

		public override void AttachEvent (string id)
		{
			switch (id) {
			case FontDialog.FontChangedEvent:
				// handled in ShowDialog
				break;
			default:
				base.AttachEvent (id);
				break;
			}
		}

		public Font Font
		{
			get {
				if (font == null)
					font = new Font(new FontHandler(Control.Font));
				return font;
			}
			set {
				font = value;
				Control.Font = font.ToSD ();
			}
		}

		public DialogResult ShowDialog (Window parent)
		{
			var result = Control.ShowDialog();
			if (result == swf.DialogResult.OK)
			{
				font = Control.Font.ToEto();
				Callback.OnFontChanged(Widget, EventArgs.Empty);
				return DialogResult.Ok;
			}
			return DialogResult.Cancel;
		}
	}
}
