namespace Eto.Test.WinForms
{
	class NativeHostControls : INativeHostControls
	{
		public IEnumerable<NativeHostTest> GetNativeHostTests()
		{
			yield return new NativeHostTest("HWND", () => new swf.UserControl { AutoScaleMode = swf.AutoScaleMode.Dpi, Controls = { new swf.Button { Text = "A HWND button" } } }.Handle);
			yield return new NativeHostTest("WinForms", () => new swf.Button { Text = "A WinForms button" });
		}
	}
}

