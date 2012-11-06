using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swf = System.Windows.Forms;
using sd = System.Drawing;
using Eto.Forms;
using Eto.Drawing;
using Eto.Platform.Windows.Drawing;

namespace Eto.Platform.Windows.Forms
{
	public class FontDialogHandler : WidgetHandler<swf.FontDialog, FontDialog>, IFontDialog
	{
		Font font;

		public override swf.FontDialog CreateControl ()
		{
			return new swf.FontDialog {
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
			get { return font;  }
			set
			{
				font = value;
				Control.Font = font.ToSD ();
			}
		}

		public DialogResult ShowDialog (Window parent)
		{
			var result = Control.ShowDialog ();
			if (result == swf.DialogResult.OK) {
				font = Control.Font.ToEto (Widget.Generator);
				Widget.OnFontChanged (EventArgs.Empty);
				return DialogResult.Ok;
			}
			else
				return DialogResult.Cancel;
		}
	}
}
