using Eto.Drawing;
using Eto.Forms;
using System.IO;
using System;

namespace Eto.Test.Sections.Dialogs
{
	[Section("Dialogs", typeof(SelectFolderDialog))]
	public class SelectFolderSection : Panel
	{
		public SelectFolderSection()
		{
			var layout = new DynamicLayout { Spacing = new Size(20, 20), DefaultSpacing = new Size(5, 5), Padding = new Padding(10) };

			layout.AddRow(null, SelectFolder(), null);
			layout.AddRow(null, SelectFolderWithStartupPath(), null);

			layout.Add(null);

			Content = layout;
		}

		Control SelectFolder()
		{
			var button = new Button { Text = "Select Folder" };
			button.Click += delegate
			{
				var dialog = new SelectFolderDialog();
				var result = dialog.ShowDialog(ParentWindow);
				if (result == DialogResult.Ok)
				{
					Log.Write(dialog, "Result: {0}, Folder: {1}", result, dialog.Directory);
				}
				else
					Log.Write(dialog, "Result: {0}", result);
			};
			return button;
		}

		Control SelectFolderWithStartupPath()
		{
			var button = new Button { Text = "Select Folder With Initial Folder" };
			button.Click += delegate
			{
				var dialog = new SelectFolderDialog();
#if PCL
				dialog.Directory = EtoEnvironment.GetFolderPath(EtoSpecialFolder.Documents);
#else
				dialog.Directory = Directory.GetCurrentDirectory();
#endif

				var result = dialog.ShowDialog(ParentWindow);
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

