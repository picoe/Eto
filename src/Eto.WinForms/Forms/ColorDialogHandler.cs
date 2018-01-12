using System;
using swf = System.Windows.Forms;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.WinForms.Forms
{
	public class ColorDialogHandler : WidgetHandler<swf.ColorDialog, ColorDialog, ColorDialog.ICallback>, ColorDialog.IHandler
	{
		static int[] customColors;
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
			swf.DialogResult result;
			if (customColors != null) Control.CustomColors = customColors;

			if (parent != null)
				result = Control.ShowDialog(parent.GetContainerControl());
			else
				result = Control.ShowDialog();

			if (result == swf.DialogResult.OK)
			{
				Callback.OnColorChanged(Widget, EventArgs.Empty);
			}

			customColors = Control.CustomColors;

			return result.ToEto();
		}
	}
}

