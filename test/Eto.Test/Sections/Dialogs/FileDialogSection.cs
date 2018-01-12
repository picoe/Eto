using Eto.Drawing;
using Eto.Forms;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Eto.Test.Sections.Dialogs
{
	[Section("Dialogs", "File Dialog")]
	public class FileDialogSection : Panel
	{
		public string FileName { get; set; }

		public string Directory { get; set; }

		public string Title { get; set; }

		public bool MultiSelect { get; set; }

		public FileDialogSection()
		{
			Content = new TableLayout
			{
				Padding = new Padding(10),
				Spacing = new Size(5, 5),
				Rows =
				{
					TableLayout.Horizontal(null, new Label { Text = "File Name" }, GetFileName(), null),
					TableLayout.Horizontal(null, new Label { Text = "Directory" }, GetDirectory(), null),
					TableLayout.Horizontal(null, GetMultiSelect(), null),
					TableLayout.Horizontal(null, new Label { Text = "Title" }, GetTitle(), null),
					TableLayout.Horizontal(null, OpenFile(), OpenFileWithFilters(), null),
					TableLayout.Horizontal(null, SaveFile(), SaveFileWithFilters(), null),
					null
				}
			};
		}

		Control GetMultiSelect()
		{
			var control = new CheckBox { Text = "MultiSelect" };
			control.CheckedBinding.Bind(this, r => r.MultiSelect);
			return control;
		}

		Control GetFileName()
		{
			var fileNameText = new TextBox { Size = new Size(200, -1) };
			fileNameText.TextBinding.Bind(this, r => r.FileName);
			return fileNameText;
		}

		Control GetDirectory()
		{
			var fileNameText = new TextBox { Size = new Size(200, -1) };
			fileNameText.TextBinding.Bind(this, r => r.Directory);
			return fileNameText;
		}

		Control GetTitle()
		{
			var titleText = new TextBox { Size = new Size(200, -1) };
			titleText.TextBinding.Bind(this, r => r.Title);
			return titleText;
		}

		void SetAttributes(OpenFileDialog openDialog)
		{
			openDialog.MultiSelect = MultiSelect;
			SetAttributes((FileDialog)openDialog);
		}

		void SetAttributes(FileDialog dialog)
		{
			if (!string.IsNullOrEmpty(FileName))
			{
				dialog.FileName = FileName;
			}
			if (!string.IsNullOrEmpty(Directory))
			{
				Uri dir;
				if (Uri.TryCreate(Directory, UriKind.Absolute, out dir))
					dialog.Directory = dir;
				else
					MessageBox.Show("You must specify an absolute directory");
			}
			if (!string.IsNullOrEmpty(Title))
				dialog.Title = Title;
		}

		Control OpenFile()
		{
			var button = new Button { Text = "Open File" };
			button.Click += delegate
			{
				var dialog = new OpenFileDialog();
				SetAttributes(dialog);
				var result = dialog.ShowDialog(ParentWindow);
				if (result == DialogResult.Ok)
				{
					Log.Write(dialog, "Result: {0}, FileName: {1}\nFiles: {2}", result, dialog.FileName, string.Join(", ", dialog.Filenames));
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
				var dialog = new OpenFileDialog
				{
					Filters =
					{
						new FileFilter("All Formats", "png", "jpg", "jpeg", "gif", "tiff"),
						new FileFilter("All Files", "*"),
						"PNG Files|png",
						new FileFilter("JPeg Files", "jpg", "jpeg"),
						new FileFilter("GIF Files", "gif"),
						new FileFilter("TIFF Files", "tiff"),
					}
				};
				SetAttributes(dialog);

				var result = dialog.ShowDialog(ParentWindow);
				if (result == DialogResult.Ok)
				{
					Log.Write(dialog, "Result: {0}, CurrentFilter: {1}, FileName: {2}\nFiles: {3}", result, dialog.CurrentFilter, dialog.FileName, string.Join(", ", dialog.Filenames));
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
				var dialog = new SaveFileDialog
				{
					Filters =
					{
						new FileFilter("Auto Detect", ".png", ".jpg", ".jpeg", ".gif", ".tiff"),
						new FileFilter("All Files", "*"),
						"PNG Files|.png",
						new FileFilter("JPeg Files", ".jpg", ".jpeg"),
						new FileFilter("GIF Files", ".gif"),
						new FileFilter("TIFF Files", ".tiff"),
					}
				};
				SetAttributes(dialog);

				var result = dialog.ShowDialog(ParentWindow);
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
				SetAttributes(dialog);
				var result = dialog.ShowDialog(ParentWindow);
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

