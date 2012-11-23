using Eto.Drawing;
using Eto.Forms;
using Eto.Platform.Wpf.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sw = System.Windows;

namespace Eto.Platform.Wpf.Forms
{
	public class FontDialogHandler : WidgetHandler<CustomControls.FontDialog.FontChooser, FontDialog>, IFontDialog
	{
		public override CustomControls.FontDialog.FontChooser CreateControl ()
		{
			return new CustomControls.FontDialog.FontChooser ();
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
			get; set;
		}

		public DialogResult ShowDialog (Window parent)
		{
			if (parent != null) {
				var owner = parent.ControlObject as sw.Window;
				Control.Owner = owner;
				Control.WindowStartupLocation = sw.WindowStartupLocation.CenterOwner;
			}
			if (Font != null) {
				var fontHandler = Font.Handler as FontHandler;
				Control.SelectedFontFamily = fontHandler.WpfFamily;
				Control.SelectedFontPointSize = fontHandler.Size;
				Control.SelectedFontStyle = fontHandler.WpfFontStyle;
				Control.SelectedFontWeight = fontHandler.WpfFontWeight;
			}
			var result = Control.ShowDialog ();

			if (result == true) {
				var fontHandler = new FontHandler (Widget.Generator, Control.SelectedFontFamily, Control.SelectedFontPointSize, Control.SelectedFontStyle, Control.SelectedFontWeight);
				Font = new Font (Widget.Generator, fontHandler);
				Widget.OnFontChanged (EventArgs.Empty);
			}

			return result != null && result.Value ? DialogResult.Ok : DialogResult.Cancel;
		}
	}
}
