using System;
using SWF = System.Windows.Forms;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Platform.Windows.Forms
{
	public class ColorDialogHandler : WidgetHandler<SWF.ColorDialog, ColorDialog>, IColorDialog
	{
		static int[] customColors;
		public ColorDialogHandler ()
		{
			this.Control = new SWF.ColorDialog ();
			this.Control.AnyColor = true;
			this.Control.AllowFullOpen = true;
			this.Control.FullOpen = true;
		}
		
		public Color Color {
			get { return Control.Color.ToEto (); }
			set { Control.Color = value.ToSD (); }
		}
		
		public DialogResult ShowDialog (Window parent)
		{
			SWF.DialogResult result;
			if (customColors != null) Control.CustomColors = customColors;
			
			if (parent != null)
                result = Control.ShowDialog(parent.GetContainerControl());
			else
				result = Control.ShowDialog ();
			
			if (result == System.Windows.Forms.DialogResult.OK) {
				Widget.OnColorChanged (EventArgs.Empty);
			}
			
			customColors = Control.CustomColors;
			
			return result.ToEto ();
		}
	}
}

