using System;
using System.IO;
using System.Threading.Tasks;
using Eto.Drawing;
using Eto.Forms;
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
		where T: FileDialog, new()
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
	}
}
