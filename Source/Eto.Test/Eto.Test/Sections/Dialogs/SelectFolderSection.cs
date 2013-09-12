using System;
using Eto.Drawing;
using Eto.Forms;
using System.Collections.Generic;
using System.IO;

namespace Eto.Test.Sections.Dialogs
{
	public class SelectFolderSection : Panel
	{
		public SelectFolderSection()
		{
			var layout = new DynamicLayout(new Size(20, 20));

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
				var result = dialog.ShowDialog(this.ParentWindow);
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
				dialog.Directory = Directory.GetCurrentDirectory();

				var result = dialog.ShowDialog(this.ParentWindow);
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

