using NUnit.Framework;

namespace Eto.Test.Mac64.UnitTests
{
	[TestFixture]
	public class ResourceTests
	{
		/// <summary>
		/// Content and BundleResource items are only copied into the Resources folder when the app bundle
		/// itself is built, so these tests can only run from the bundled Eto.Test.Mac64.app. The shared unit
		/// test runner is a plain console app where NSBundle.MainBundle just points at the output directory.
		/// </summary>
		static void RequireAppBundle()
		{
			if (!NSBundle.MainBundle.BundlePath.EndsWith(".app", StringComparison.Ordinal))
				Assert.Ignore("Only applies when running from within the application bundle");
		}

		[Test]
		public void ContentShouldBeInResources()
		{
			RequireAppBundle();
			var path = EtoEnvironment.GetFolderPath(EtoSpecialFolder.ApplicationResources);
			var file = Path.Combine(path, "Assets", "TestContent.txt");
			Assert.That(File.Exists(file), Is.True);
		}

		[Test]
		public void BundleResourceShouldBeInResources()
		{
			RequireAppBundle();
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
