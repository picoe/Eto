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
			get {
				return Generator.Convert (this.Control.Color);
			}
			set {
				this.Control.Color = Generator.Convert (value);
			}
		}
		
		public DialogResult ShowDialog (Window parent)
		{
			SWF.DialogResult result;
			if (customColors != null) this.Control.CustomColors = customColors;
			
			if (parent != null)
				result = this.Control.ShowDialog (parent.ControlObject as SWF.Control);
			else
				result = this.Control.ShowDialog ();
			
			if (result == System.Windows.Forms.DialogResult.OK) {
				Widget.OnColorChanged (EventArgs.Empty);
			}
			
			customColors = this.Control.CustomColors;
			
			return Generator.Convert (result);
		}
	}
}

