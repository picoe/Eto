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
			Assert.That(File.Exists(file), Is.True);
		}

		[Test]
		public void BundleResourceShouldBeInResources()
		{
			var path = EtoEnvironment.GetFolderPath(EtoSpecialFolder.ApplicationResources);
			var file = Path.Combine(path, "Assets", "TestBundleResource.txt");
			Assert.That(File.Exists(file), Is.True);
		}

		[Test]
		public void CopyToOutputShouldBeInExecutablePath()
		{
			// getting the location of the assembly can be null when using mkbundle, so we use this instead.
			var path = EtoEnvironment.GetFolderPath(EtoSpecialFolder.EntryExecutable);

			Assert.That(path, Is.Not.Empty, "#1");
			var file = Path.Combine(path, "Assets", "TestCopyToOutput.txt");
			Console.WriteLine($"Looking for file '{file}'");
			Assert.That(File.Exists(file), Is.True, "#2");
		}
	}
}
