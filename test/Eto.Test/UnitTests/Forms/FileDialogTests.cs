using NUnit.Framework;

namespace Eto.Test.UnitTests.Forms
{
	[TestFixture]
	public class OpenFileDialogTests : FileDialogTests<OpenFileDialog>
	{
	}

	[TestFixture]
	public class SaveFileDialogTests : FileDialogTests<SaveFileDialog>
	{
		[Test, ManualTest, InvokeOnUI]
		public void FileNameShouldNotHaveDoubleExtension()
		{
			// this can only be replicated on macOS when the extension is unknown
			// and we set the filename first
			var fd = new SaveFileDialog();
			fd.FileName = "NoDoubleExt.eto";
			fd.Directory = new Uri(EtoEnvironment.GetFolderPath(EtoSpecialFolder.Downloads));
			fd.Filters.Clear();
			fd.Filters.Add(new FileFilter("ETO Files", ".eto"));
			fd.Filters.Add(new FileFilter("Text Files", ".txt"));
			fd.ShowDialog(null);
		}
	}

	public class FileDialogTests<T> : TestBase
		where T : FileDialog, new()
	{
		[Test, InvokeOnUI, ManualTest]
		public void FileNameShouldHaveConsistentValues()
		{
			var fd = new T();
			fd.Filters.Add(new FileFilter("Text Files (*.txt)", ".txt"));

			Assert.That(fd.FileName, Is.Null.Or.Empty.Or.EqualTo("Untitled"), "#1");

			fd.FileName = null;
			Assert.That(fd.FileName, Is.Null.Or.Empty.Or.EqualTo("Untitled"), "#2");

			fd.FileName = "SomeFile.txt";

			Assert.That(Path.GetFileName(fd.FileName), Is.EqualTo("SomeFile.txt"), "#3");

			var result = fd.ShowDialog(null);

			if (result == DialogResult.Cancel || typeof(T) == typeof(SaveFileDialog))
				Assert.That(Path.GetFileName(fd.FileName), Is.EqualTo("SomeFile.txt"), "#4");

			if (result == DialogResult.Ok)
			{
				var directoryName = Path.GetDirectoryName(fd.FileName);
				Assert.That(directoryName, Is.Not.Null, "#5.1");
				Assert.That(directoryName, Is.Not.Empty, "#5.2");
				Console.WriteLine($"Directory: {directoryName}");
			}

			Console.WriteLine($"FileName: {fd.FileName}");

		}

		[Test, ManualTest, InvokeOnUI]
		public void OpenFromSecondaryDialogShouldNotChangeItsOrder()
		{
			var btn1 = new Button { Text = "Click Me" };
			btn1.Click += (s, e) =>
			{
				var btn2 = new Button { Text = "File Dialog" };
				btn2.Click += (s1, e1) =>
				{
					var fileDialog = new T();
					fileDialog.Filters.Add(new FileFilter("Text Files (*.txt)", ".txt"));
					fileDialog.CurrentFilterIndex = 0;
					
					// Close the file dialog and the dialog with the "File Dialog" button should stay on top.
					fileDialog.ShowDialog(btn2.ParentWindow);
				};
				var dlg2 = new Dialog
				{
					Title = "Test FileDialog",
					ClientSize = new Size(200, 200),
					Content = new TableLayout
					{
						Rows = {
							null,
							"This dialog should remain on top\nafter the file dialog closes",
							TableLayout.AutoSized(btn2, centered: true),
							null
						}
					}
				};
				dlg2.ShowModal(btn1.ParentWindow);
			};
			var dlg1 = new Dialog
			{
				Title = "TestEtoFileDialog",
				ClientSize = new Size(200, 200),
				Content = TableLayout.AutoSized(btn1, centered: true)
			};
			dlg1.ShowModal(Application.Instance.MainForm);
		}
	}
}
