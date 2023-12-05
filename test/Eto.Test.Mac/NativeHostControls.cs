namespace Eto.Test.Mac
{
	class NativeHostControls : INativeHostControls
	{
		public IEnumerable<NativeHostTest> GetNativeHostTests()
		{
			yield return new NativeHostTest("NSView", () => new NSButton { Title = "A NSButton"});
			yield return new NativeHostTest("IntPtr", () => new NSButton { Title = "An IntPtr handle button"}.Handle);
		}
	}
}

