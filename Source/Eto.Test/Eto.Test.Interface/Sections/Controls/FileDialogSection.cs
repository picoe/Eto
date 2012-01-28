using System;
using Eto.Drawing;
using Eto.Forms;
using Eto.Test.Interface.Controls;
using System.Collections.Generic;

namespace Eto.Test.Interface.Sections.Controls
{
	public class FileDialogSection : SectionBase
	{
		public FileDialogSection ()
		{
			var layout = new DynamicLayout (this, new Size (20, 20));

			//layout.SetColumnScale(0);

			layout.AddRow (null, OpenFile (), OpenFileWithFilters (), null);
			layout.AddRow (null, SaveFile (), SaveFileWithFilters (), null);

			layout.Add (null);
		}

		Control OpenFile ()
		{
			var button = new Button { Text = "Open File" };
			button.Click += delegate {
				var dialog = new OpenFileDialog ();
				var result = dialog.ShowDialog (this.ParentWindow);
				if (result == DialogResult.Ok) {
					Log (dialog, "Result: {0}, FileName: {1}", result, dialog.FileName);
				}
				else
					Log (dialog, "Result: {0}", result);
			};
			return button;
		}

		Control OpenFileWithFilters ()
		{
			var button = new Button { Text = "Open File With Filters" };
			button.Click += delegate {
				var dialog = new OpenFileDialog ();

				dialog.Filters = GetFilters ("All Formats");

				var result = dialog.ShowDialog (this.ParentWindow);
				if (result == DialogResult.Ok) {
					Log (dialog, "Result: {0}, CurrentFilter: {1}, FileName: {2}", result, dialog.CurrentFilter, dialog.FileName);
				}
				else
					Log (dialog, "Result: {0}", result);
			};
			return button;
		}

		IEnumerable<IFileDialogFilter> GetFilters (string allFormatDescription)
		{
			var filters = new List<FileDialogFilter> ();
			filters.Add (new FileDialogFilter (allFormatDescription, "png", "jpg", "jpeg", "gif", "tiff"));
			filters.Add (new FileDialogFilter ("PNG Files", "png"));
			filters.Add (new FileDialogFilter ("JPeg Files", "jpg", "jpeg"));
			filters.Add (new FileDialogFilter ("GIF Files", "gif"));
			filters.Add (new FileDialogFilter ("TIFF Files", "tiff"));
			return filters;
		}

		Control SaveFileWithFilters ()
		{
			var button = new Button { Text = "Save File With Filters" };
			button.Click += delegate {
				var dialog = new SaveFileDialog ();

				dialog.Filters = GetFilters ("Auto Detect");

				var result = dialog.ShowDialog (this.ParentWindow);
				if (result == DialogResult.Ok) {
					Log (dialog, "Result: {0}, CurrentFilter: {1}, FileName: {2}", result, dialog.CurrentFilter, dialog.FileName);
				}
				else
					Log (dialog, "Result: {0}", result);
			};
			return button;
		}

		Control SaveFile ()
		{
			var button = new Button { Text = "Save File" };
			button.Click += delegate {
				var dialog = new SaveFileDialog ();
				var result = dialog.ShowDialog (this.ParentWindow);
				if (result == DialogResult.Ok) {
					Log (dialog, "Result: {0}, FileName: {1}", result, dialog.FileName);
				}
				else
					Log (dialog, "Result: {0}", result);
			};
			return button;
		}
	}
}

