using Eto.Drawing;
using Eto.Forms;
using System.Collections.Generic;

namespace Eto.Test.Sections.Dialogs
{
	public class FileDialogSection : Panel
	{
		public FileDialogSection()
		{
			var layout = new DynamicLayout(new Size(20, 20));

			layout.AddRow(null, OpenFile(), OpenFileWithFilters(), null);
			layout.AddRow(null, SaveFile(), SaveFileWithFilters(), null);

			layout.Add(null);

			Content = layout;
		}

		IEnumerable<IFileDialogFilter> GetFilters(string allFormatDescription)
		{
			yield return new FileDialogFilter(allFormatDescription, "png", "jpg", "jpeg", "gif", "tiff");
			yield return new FileDialogFilter("PNG Files", "png");
			yield return new FileDialogFilter("JPeg Files", "jpg", "jpeg");
			yield return new FileDialogFilter("GIF Files", "gif");
			yield return new FileDialogFilter("TIFF Files", "tiff");
		}

		Control OpenFile()
		{
			var button = new Button { Text = "Open File" };
			button.Click += delegate
			{
				var dialog = new OpenFileDialog();
				var result = dialog.ShowDialog(this.ParentWindow);
				if (result == DialogResult.Ok)
				{
					Log.Write(dialog, "Result: {0}, FileName: {1}", result, dialog.FileName);
				}
				else
					Log.Write(dialog, "Result: {0}", result);
			};
			return button;
		}

		Control OpenFileWithFilters()
		{
			var button = new Button { Text = "Open File With Filters" };
			button.Click += delegate
			{
				var dialog = new OpenFileDialog();

				dialog.Filters = GetFilters("All Formats");

				var result = dialog.ShowDialog(this.ParentWindow);
				if (result == DialogResult.Ok)
				{
					Log.Write(dialog, "Result: {0}, CurrentFilter: {1}, FileName: {2}", result, dialog.CurrentFilter, dialog.FileName);
				}
				else
					Log.Write(dialog, "Result: {0}", result);
			};
			return button;
		}

		Control SaveFileWithFilters()
		{
			var button = new Button { Text = "Save File With Filters" };
			button.Click += delegate
			{
				var dialog = new SaveFileDialog();

				dialog.Filters = GetFilters("Auto Detect");

				var result = dialog.ShowDialog(this.ParentWindow);
				if (result == DialogResult.Ok)
				{
					Log.Write(dialog, "Result: {0}, CurrentFilter: {1}, FileName: {2}", result, dialog.CurrentFilter, dialog.FileName);
				}
				else
					Log.Write(dialog, "Result: {0}", result);
			};
			return button;
		}

		Control SaveFile()
		{
			var button = new Button { Text = "Save File" };
			button.Click += delegate
			{
				var dialog = new SaveFileDialog();
				var result = dialog.ShowDialog(this.ParentWindow);
				if (result == DialogResult.Ok)
				{
					Log.Write(dialog, "Result: {0}, FileName: {1}", result, dialog.FileName);
				}
				else
					Log.Write(dialog, "Result: {0}", result);
			};
			return button;
		}
	}
}

