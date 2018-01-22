using Eto.Drawing;
using Eto.Forms;
using System.IO;
using System;

namespace Eto.Test.Sections.Dialogs
{
	[Section("Dialogs", typeof(SelectFolderDialog))]
	public class SelectFolderSection : Panel
	{
		string Title { get; set; }

		bool SetDialogParent { get; set; } = true;

		bool SetInitialFolder { get; set; }

		string InitialFolder { get; set; } = EtoEnvironment.GetFolderPath(EtoSpecialFolder.Documents);

		public SelectFolderSection()
		{
			var layout = new DynamicLayout { Spacing = new Size(20, 20), DefaultSpacing = new Size(5, 5), Padding = 10 };

			layout.AddSeparateRow(null, "Title:", TitleTextBox(), null);
			layout.AddSeparateRow(null, SetParentCheckBox(), SetInitialFolderCheckBox(), null);
			layout.AddSeparateRow(null, SelectFolder(), null);

			layout.Add(null);

			Content = layout;
		}

		Control SetInitialFolderCheckBox()
		{
			var control = new CheckBox { Text = "Set initial folder:" };
			control.CheckedBinding.Bind(this, c => c.SetInitialFolder);
			var folder = new TextBox();
			folder.TextBinding.Bind(this, c => c.InitialFolder);
			control.CheckedBinding.Bind(folder, f => f.Enabled, DualBindingMode.OneWayToSource);
			return TableLayout.Horizontal(2, control, folder);
		}

		Control TitleTextBox()
		{
			var control = new TextBox();
			control.TextBinding.Bind(this, c => c.Title);
			return control;
		}

		Control SetParentCheckBox()
		{
			var control = new CheckBox { Text = "Set parent" };
			control.CheckedBinding.Bind(this, c => c.SetDialogParent);
			return control;
		}

		Control SelectFolder()
		{
			var button = new Button { Text = "Select Folder" };
			button.Click += delegate
			{
				var dialog = new SelectFolderDialog();
				if (!string.IsNullOrEmpty(Title))
					dialog.Title = Title;

				if (SetInitialFolder)
					dialog.Directory = InitialFolder;
				
				var result = SetDialogParent ? dialog.ShowDialog(ParentWindow) : dialog.ShowDialog(null);
				if (result == DialogResult.Ok)
				{
					Log.Write(dialog, "Result: {0}, Folder: {1}", result, dialog.Directory);
				}
				else
					Log.Write(dialog, "Result: {0}", result);
			};
			return button;
		}

	}
}

