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

			Assert.AreEqual("SomeFile.txt", Path.GetFileName(fd.FileName), "#3");

			var result = fd.ShowDialog(null);

			if (result == DialogResult.Cancel || typeof(T) == typeof(SaveFileDialog))
				Assert.AreEqual("SomeFile.txt", Path.GetFileName(fd.FileName), "#4");

			if (result == DialogResult.Ok)
			{
				var directoryName = Path.GetDirectoryName(fd.FileName);
				Assert.IsNotNull(directoryName, "#5.1");
				Assert.IsNotEmpty(directoryName, "#5.2");
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
