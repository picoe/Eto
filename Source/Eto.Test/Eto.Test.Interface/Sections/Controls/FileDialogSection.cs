using System;
using Eto.Drawing;
using Eto.Forms;
using Eto.Test.Interface.Controls;

namespace Eto.Test.Interface.Sections.Controls
{
	public class FileDialogSection : SectionBase
	{
		public FileDialogSection ()
		{
			var layout = new TableLayout (this, 1, 10);
			
			//layout.SetColumnScale(0);
			
			layout.Add (ScalingControl (OpenFile ()), 0, 0);
			layout.Add (ScalingControl (SaveFile ()), 0, 1);
		}

		Control OpenFile ()
		{
			var button = new Button{ Text = "Open File" };
			button.Click += delegate {
				var dialog = new OpenFileDialog ();
				var result = dialog.ShowDialog (this.ParentWindow);
				Log (dialog, "Result: {0}", result);
				if (result == DialogResult.Ok) {
					Log (dialog, "Selected File: {0}", dialog.FileName);
				}
			};
			return button;
		}

		Control SaveFile ()
		{
			var button = new Button{ Text = "Save File" };
			button.Click += delegate {
				var dialog = new SaveFileDialog ();
				var result = dialog.ShowDialog (this.ParentWindow);
				Log (dialog, "Result: {0}", result);
				if (result == DialogResult.Ok) {
					Log (dialog, "File to save: {0}", dialog.FileName);
				}
			};
			return button;
		}
		
		Control ScalingControl (Control control)
		{
			var layout = new TableLayout (new Panel{ }, 3, 1);
			layout.Padding = Padding.Empty;
			layout.SetColumnScale (0);
			layout.SetColumnScale (2);
			layout.Add (control, 1, 0);
			return layout.Container;
		}
	}
}

