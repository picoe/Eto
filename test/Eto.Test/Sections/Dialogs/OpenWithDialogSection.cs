using System;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Test.Sections.Dialogs
{
	[Section("Dialogs", typeof(OpenWithDialog))]
	public class OpenWithDialogSection : Panel
    {
        public OpenWithDialogSection()
        {
			var layout = new DynamicLayout();
			layout.DefaultPadding = new Padding(10);
			layout.DefaultSpacing = new Size(4, 4);
			layout.BeginVertical();

			layout.BeginHorizontal();
			layout.Add(new Label { Text = "File to open: ", VerticalAlignment = VerticalAlignment.Center });
			var filepicker1 = new FilePicker { FileAction = FileAction.OpenFile };
			layout.Add(filepicker1, true, false);
			layout.EndHorizontal();

			layout.BeginHorizontal();
			layout.Add(null);
			var button1 = new Button();
			button1.Text = "Show Dialog";
			layout.Add(button1, true, false);
			layout.EndHorizontal();

			layout.Add(null, false, true);

			layout.EndVertical();
			Content = layout;

			button1.Click += delegate {
				var dialog = new OpenWithDialog(filepicker1.FilePath);
				dialog.ShowDialog(this);
			};
        }
    }
}
