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

		[Test, ManualTest, InvokeOnUI]
		public void DotFileNameShouldNotHaveDoubleExtension()
		{
			// macOS doesn't consider a leading period to be an extension, so it would append the
			// filter's extension and show ".gitignore.gitignore"
			var fd = new SaveFileDialog();
			fd.Title = "Press OK to continue";
			fd.Filters.Add(new FileFilter("Git Ignore", ".gitignore"));
			fd.FileName = ".gitignore";
			fd.ShowDialog(null);
			Assert.That(Path.GetFileName(fd.FileName), Is.EqualTo(".gitignore"), "#1");
		}
	}

	public class FileDialogTests<T> : TestBase
		where T : FileDialog, new()
	{
		[Test, InvokeOnUI]
		public void FileNameShouldReturnFullPathThatWasSet()
		{
			var fd = new T();
			var fullPath = Path.Combine(Path.GetTempPath(), "SomeFile.txt");

			fd.FileName = fullPath;
			Assert.That(fd.FileName, Is.EqualTo(fullPath), "#1");

			fd.FileName = "OtherFile.txt";
			Assert.That(Path.GetFileName(fd.FileName), Is.EqualTo("OtherFile.txt"), "#2");

			fd.FileName = null;
			Assert.That(fd.FileName, Is.Null.Or.Empty.Or.EqualTo("Untitled"), "#3");
		}

		[Test, InvokeOnUI]
		public void DirectoryShouldNotThrowWhenNotSet()
		{
			var fd = new T();
			Assert.DoesNotThrow(() => _ = fd.Directory);
		}

		[Test, InvokeOnUI]
		public void FileNameAndDirectoryShouldUseLastOneSet()
		{
			var dir1 = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName())).FullName;
			var dir2 = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName())).FullName;
			try
			{
				var fd = new T();

				fd.FileName = Path.Combine(dir1, "SomeFile.txt");
				Assert.That(Path.GetFileName(fd.Directory?.LocalPath.TrimEnd('/', '\\')), Is.EqualTo(Path.GetFileName(dir1)), "#1 setting FileName should set Directory");

				fd.Directory = new Uri(dir2);
				Assert.That(fd.FileName, Is.EqualTo(Path.Combine(dir2, "SomeFile.txt")), "#2 setting Directory should move FileName");

				fd.FileName = Path.Combine(dir1, "OtherFile.txt");
				Assert.That(fd.FileName, Is.EqualTo(Path.Combine(dir1, "OtherFile.txt")), "#3.1");
				Assert.That(Path.GetFileName(fd.Directory?.LocalPath.TrimEnd('/', '\\')), Is.EqualTo(Path.GetFileName(dir1)), "#3.2 setting FileName again should set Directory");

				fd.FileName = "NameOnly.txt";
				fd.Directory = new Uri(dir2);
				Assert.That(Path.GetFileName(fd.FileName), Is.EqualTo("NameOnly.txt"), "#4 name only should keep the name");
			}
			finally
			{
				Directory.Delete(dir1);
				Directory.Delete(dir2);
			}
		}

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
