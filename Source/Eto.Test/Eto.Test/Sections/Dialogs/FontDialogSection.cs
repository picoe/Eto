using System;
using Eto.Drawing;
using Eto.Forms;
using System.Collections.Generic;

namespace Eto.Test.Sections.Dialogs
{
	public class FontDialogSection : Panel
	{
		Font selectedFont = new Font(FontFamily.Serif, 20, FontStyle.Bold);

		public FontDialogSection ()
		{
			var layout = new DynamicLayout (this, new Size (20, 20));

			layout.AddRow (null, PickFont (), null);
			layout.AddRow (null, PickFontWithStartingFont (), null);

			layout.Add (null);
		}

		Control PickFont ()
		{
			var button = new Button { Text = "Pick Font" };
			button.Click += delegate {
				var dialog = new FontDialog ();
				dialog.FontChanged += delegate {
					// you need to handle this event for OS X, where the dialog is a floating window
					Log.Write (dialog, "FontChanged, Font: {0}", dialog.Font);
				};
				var result = dialog.ShowDialog (this.ParentWindow);
				if (result == DialogResult.Ok) {
					Log.Write (dialog, "Result: {0}, Font: {1}", result, dialog.Font);
				}
				else
					Log.Write (dialog, "Result: {0}", result);
			};
			return button;
		}

		Control PickFontWithStartingFont ()
		{
			var button = new Button { Text = "Pick Font with initial starting font" };
			button.Click += delegate {
				var dialog = new FontDialog {
					Font = selectedFont
				};
				dialog.FontChanged += delegate {
					// need to handle this event for OS X, where the dialog is a floating window
					selectedFont = dialog.Font;
					Log.Write (dialog, "FontChanged, Font: {0}", dialog.Font);
				};
				var result = dialog.ShowDialog (this.ParentWindow);
				// do not get the font here, it may return immediately with a result of DialogResult.None on certain platforms
				Log.Write (dialog, "Result: {0}", result);
			};
			return button;
		}
	}
}

