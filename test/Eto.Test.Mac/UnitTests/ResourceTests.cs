using NUnit.Framework;

namespace Eto.Test.Mac64.UnitTests
{
	[TestFixture]
	public class ResourceTests
	{
		[Test]
		public void ContentShouldBeInResources()
		{
			var path = EtoEnvironment.GetFolderPath(EtoSpecialFolder.ApplicationResources);
			var file = Path.Combine(path, "Assets", "TestContent.txt");
			Assert.IsTrue(File.Exists(file));
		}

		[Test]
		public void BundleResourceShouldBeInResources()
		{
			var path = EtoEnvironment.GetFolderPath(EtoSpecialFolder.ApplicationResources);
			var file = Path.Combine(path, "Assets", "TestBundleResource.txt");
			Assert.IsTrue(File.Exists(file));
		}

		[Test]
		public void CopyToOutputShouldBeInExecutablePath()
		{
			if (Platform.Instance.ID == "XamMac2")
			{
				Assert.Ignore("Xamarin.Mac projects do not copy these files");
			}

			// getting the location of the assembly can be null when using mkbundle, so we use this instead.
			var path = EtoEnvironment.GetFolderPath(EtoSpecialFolder.EntryExecutable);

			Assert.IsNotEmpty(path, "#1");
			var file = Path.Combine(path, "Assets", "TestCopyToOutput.txt");
			Console.WriteLine($"Looking for file '{file}'");
			Assert.IsTrue(File.Exists(file), "#2");
		}
	}
}
