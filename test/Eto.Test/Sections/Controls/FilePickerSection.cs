using Eto.Drawing;
using Eto.Forms;

namespace Eto.Test.Sections.Controls
{
	[Section("Controls", typeof(FilePicker))]
	public class FilePickerSection : Panel
	{
		public FilePickerSection()
		{
			var layout = new DynamicLayout();
			layout.DefaultPadding = new Padding(10);
			layout.DefaultSpacing = new Size(4, 4);
			layout.BeginVertical();

			layout.BeginHorizontal();
			layout.Add(new Label { Text = "Open File: ", VerticalAlignment = VerticalAlignment.Center });
			var filepicker1 = new FilePicker { FileAction = FileAction.OpenFile };
			LogEvents(filepicker1);
			layout.Add(filepicker1, true, false);
			layout.EndHorizontal();

			layout.BeginHorizontal();
			layout.Add(new Label { Text = "Save File: ", VerticalAlignment = VerticalAlignment.Center });
			var filepicker2 = new FilePicker { FileAction = FileAction.SaveFile };
			LogEvents(filepicker2);
			layout.Add(filepicker2, true, false);
			layout.EndHorizontal();

			layout.BeginHorizontal();
			layout.Add(new Label { Text = "Select Folder: ", VerticalAlignment = VerticalAlignment.Center });
			var filepicker3 = new FilePicker { FileAction = FileAction.SelectFolder };
			LogEvents(filepicker3);
			layout.Add(filepicker3, true, false);
			layout.EndHorizontal();

			layout.Add(null, false, true);

			layout.EndVertical();
			Content = layout;
		}

		void LogEvents(FilePicker control)
		{
			control.FilePathChanged += delegate
			{
				Log.Write(control, "FilePathChanged, FilePath: {0}", control.FilePath);
			};
		}
	}
}
