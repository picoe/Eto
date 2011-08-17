using System;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Test.Interface.Controls
{
	public class FileDialogSection : Panel
	{
		public FileDialogSection ()
		{
			var layout = new TableLayout(this, 1, 10);
			
			//layout.SetColumnScale(0);
			
			layout.Add (ScalingControl(OpenFile()), 0, 0);
			layout.Add (ScalingControl(SaveFile()), 0, 1);
		}

		Control OpenFile()
		{
			var button = new Button{ Text = "Open File" };
			button.Click += delegate {
				var dialog = new OpenFileDialog();
				dialog.ShowDialog(this.ParentWindow);
			};
			return button;
		}

		Control SaveFile()
		{
			var button = new Button{ Text = "Save File" };
			button.Click += delegate {
				var dialog = new SaveFileDialog();
				dialog.ShowDialog(this.ParentWindow);
			};
			return button;
		}
		
		Control ScalingControl(Control control)
		{
			var layout = new TableLayout(new Panel{ }, 3, 1);
			layout.Padding = Padding.Empty;
			layout.SetColumnScale(0);
			layout.SetColumnScale(2);
			layout.Add (control, 1, 0);
			return layout.Container;
		}
	}
}

