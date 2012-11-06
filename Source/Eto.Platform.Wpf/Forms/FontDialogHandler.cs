using Eto.Drawing;
using Eto.Forms;
using Eto.Platform.Wpf.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using swf = System.Windows.Forms;

namespace Eto.Platform.Wpf.Forms
{
	public class FontDialogHandler : WidgetHandler<swf.FontDialog, FontDialog>, IFontDialog
	{
		Font font;

		public override swf.FontDialog CreateControl ()
		{
			return new swf.FontDialog {
				ShowEffects = false,
				ShowColor = false
			};
		}

		public override void AttachEvent (string id)
		{
			switch (id) {
			case FontDialog.FontChangedEvent:
				// handled during showdialog
				break;
			default:
				base.AttachEvent (id);
				break;
			}
		}

		public Font Font
		{
			get { return font; }
			set
			{
				font = value;
				Control.Font = font.ToSD();
			}
		}

		public DialogResult ShowDialog (Window parent)
		{
			var dr = Control.ShowDialog ();
			if (dr == swf.DialogResult.OK) {
				font = Control.Font.ToEto(Widget.Generator);
				Widget.OnFontChanged (EventArgs.Empty);
				return DialogResult.Ok;
			}
			else
				return DialogResult.Cancel;
		}
	}
}
