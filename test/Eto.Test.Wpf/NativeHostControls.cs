namespace Eto.Test.Wpf
{
	class NativeHostControls : INativeHostControls
	{
		public IEnumerable<NativeHostTest> GetNativeHostTests()
		{
			yield return new NativeHostTest("FrameworkElement", () => new System.Windows.Controls.Button { Content = "A WPF button"});
			yield return new NativeHostTest("HWND", () => new System.Windows.Forms.Button { Text = "A HWND button"}.Handle);
			yield return new NativeHostTest("WinForms", () => new System.Windows.Forms.Button { Text = "A WinForms button"});
		}
	}
}

