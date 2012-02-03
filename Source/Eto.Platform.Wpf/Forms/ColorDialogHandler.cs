using System;
using Eto.Forms;
using Eto.Drawing;
using sw = System.Windows;
using swc = System.Windows.Controls;
using msc = Microsoft.Samples.CustomControls;

namespace Eto.Platform.Wpf.Forms
{
	public class ColorDialogHandler : WidgetHandler<msc.ColorPickerDialog, ColorDialog>, IColorDialog
	{
		public ColorDialogHandler ()
		{
			this.Control = new msc.ColorPickerDialog {
				ShowAlpha = false
			};
		}
		
		public Color Color {
			get {
				return Generator.Convert (this.Control.SelectedColor);
			}
			set {
				this.Control.StartingColor = Generator.Convert (value);
			}
		}

		public DialogResult ShowDialog (Window parent)
		{
			bool? result = null;
			if (parent != null) {
				var owner = parent.ControlObject as sw.Window;
				Control.Owner = owner;
				Control.WindowStartupLocation = sw.WindowStartupLocation.CenterOwner;
				result = Control.ShowDialog ();
			}
			else {
				result = Control.ShowDialog ();
			}
			return result != null && result.Value ? DialogResult.Ok : DialogResult.Cancel;
		}
	}
}

