namespace Eto.Test.Sections.Dialogs
{
	[Section("Dialogs", typeof(OpenWithDialog))]
	public class OpenWithDialogSection : Panel
    {
		readonly AsyncDialogOptions asyncOptions = new AsyncDialogOptions();

        public OpenWithDialogSection()
        {
			var layout = new DynamicLayout { Spacing = new Size(20, 20), DefaultSpacing = new Size(5, 5), Padding = 10 };

			var filepicker1 = new FilePicker { FileAction = FileAction.OpenFile, Width = 300 };
			var button1 = new Button { Text = "Show Dialog" };

			layout.AddSeparateRow(null, new Label { Text = "File to open:", VerticalAlignment = VerticalAlignment.Center }, filepicker1, null);
			layout.AddSeparateRow(null, asyncOptions, null);
			layout.AddSeparateRow(null, button1, null);

			layout.Add(null);

			Content = layout;

			button1.Click += delegate {
				var dialog = new OpenWithDialog(filepicker1.FilePath);
				asyncOptions.Run(dialog,
					() => dialog.ShowDialog(this),
					token => dialog.ShowDialogAsync(this, token),
					result => Log.Write(dialog, "Result: {0}", result));
			};
        }
    }
}
