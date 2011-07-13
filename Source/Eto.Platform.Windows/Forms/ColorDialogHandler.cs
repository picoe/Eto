using System;
using SWF = System.Windows.Forms;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Platform.Windows.Forms
{
	public class ColorDialogHandler : WidgetHandler<SWF.ColorDialog, ColorDialog>, IColorDialog
	{
		static bool fullOpen;
		static int[] customColors;
		public ColorDialogHandler ()
		{
			this.Control = new SWF.ColorDialog ();
			this.Control.AnyColor = true;
			this.Control.AllowFullOpen = true;
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
			this.Control.FullOpen = fullOpen;
			if (customColors != null) this.Control.CustomColors = customColors;
			
			if (parent != null)
				result = this.Control.ShowDialog (parent.ControlObject as SWF.Control);
			else
				result = this.Control.ShowDialog ();
			
			if (result == System.Windows.Forms.DialogResult.OK) {
				Widget.OnColorChanged (EventArgs.Empty);
			}
			
			customColors = this.Control.CustomColors;
			fullOpen = this.Control.FullOpen;
			
			return Generator.Convert (result);
		}
	}
}

