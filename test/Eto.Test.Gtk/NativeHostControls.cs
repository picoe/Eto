namespace Eto.Test.Gtk;

class NativeHostControls : INativeHostControls
{
	public IEnumerable<NativeHostTest> GetNativeHostTests()
	{
		yield return new NativeHostTest("Gtk.Button", () => new global::Gtk.Button { Child = new global::Gtk.Label { Text = "A Gtk.Button" } });
		yield return new NativeHostTest("IntPtr", () => new global::Gtk.Button { Child = new global::Gtk.Label { Text = "An IntPtr handle button" } }.Handle);
	}
}

