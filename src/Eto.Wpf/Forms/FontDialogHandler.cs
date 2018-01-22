using Eto.Drawing;
using Eto.Forms;
using Eto.Wpf.Drawing;
using System;

using sw = System.Windows;
namespace Eto.Wpf.Forms
{
	public class FontDialogHandler : WidgetHandler<CustomControls.FontDialog.FontChooser, FontDialog, FontDialog.ICallback>, FontDialog.IHandler
	{
		public FontDialogHandler()
		{
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case FontDialog.FontChangedEvent:
					// handled during showdialog
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}

		public Font Font
		{
			get;
			set;
		}

		public DialogResult ShowDialog(Window parent)
		{
			Control = new CustomControls.FontDialog.FontChooser();
			
			if (parent != null)
			{
				var owner = parent.ControlObject as sw.Window;
				Control.Owner = owner;
				Control.WindowStartupLocation = sw.WindowStartupLocation.CenterOwner;
			}
			if (Font != null)
			{
				var fontHandler = (FontHandler)Font.Handler;
				Control.SelectedFontFamily = fontHandler.WpfFamily;
				Control.SelectedFontPointSize = fontHandler.Size;
				Control.SelectedFontStyle = fontHandler.WpfFontStyle;
				Control.SelectedFontWeight = fontHandler.WpfFontWeight;
			}
			var result = Control.ShowDialog();

			if (result == true)
			{
				var fontHandler = new FontHandler(Control.SelectedFontFamily, Control.SelectedFontPointSize, Control.SelectedFontStyle, Control.SelectedFontWeight);
				Font = new Font(fontHandler);
				Callback.OnFontChanged(Widget, EventArgs.Empty);
			}

			return result != null && result.Value ? DialogResult.Ok : DialogResult.Cancel;
		}
	}
}
