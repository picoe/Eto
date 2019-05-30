#if TODO_XAML
using Eto.Drawing;
using Eto.Forms;
using Eto.WinRT.Drawing;
using System;

using sw = Windows.UI.Xaml;
namespace Eto.WinRT.Forms
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
				var fontHandler = (FontHandler)Font.Handler;
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
#endif