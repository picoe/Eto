using System;
using Eto.Drawing;
using Eto.Forms;
using Eto.Test.Interface.Controls;
using System.Collections.Generic;

namespace Eto.Test.Interface.Sections.Dialogs
{
	public class ColorDialogSection : SectionBase
	{
		public ColorDialogSection ()
		{
			var layout = new DynamicLayout (this, new Size (20, 20));

			layout.AddRow (null, PickColor (), null);
			layout.AddRow (null, PickColorWithStartingColor (), null);

			layout.Add (null);
		}

		Control PickColor ()
		{
			var button = new Button { Text = "Pick Color" };
			button.Click += delegate {
				var dialog = new ColorDialog ();
				var result = dialog.ShowDialog (this.ParentWindow);
				if (result == DialogResult.Ok) {
					Log (dialog, "Result: {0}, Color: {1}", result, dialog.Color);
				}
				else
					Log (dialog, "Result: {0}", result);
			};
			return button;
		}

		Control PickColorWithStartingColor ()
		{
			var button = new Button { Text = "Pick Color with initial starting color (green)" };
			button.Click += delegate {
				var dialog = new ColorDialog {
					Color = Color.Green
				};
				var result = dialog.ShowDialog (this.ParentWindow);
				if (result == DialogResult.Ok) {
					Log (dialog, "Result: {0}, Color: {1}", result, dialog.Color);
				}
				else
					Log (dialog, "Result: {0}", result);
			};
			return button;
		}
	}
}

