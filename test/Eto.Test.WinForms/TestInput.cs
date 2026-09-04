using Eto.Test.UnitTests;
using Eto.WinForms;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Eto.Test.WinForms
{
	public class TestInput : ITestInput
	{
		const int WM_KEYDOWN = 0x0100;
		const int WM_KEYUP = 0x0101;

		[DllImport("user32.dll")]
		static extern IntPtr GetFocus();

		[DllImport("user32.dll")]
		static extern bool PostMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

		static swf.Form GetForm(Window window) => (swf.Form)window.ControlObject;

		public Task SendKeyDownAsync(Window window, Keys key) => SendKeyAsync(window, key, WM_KEYDOWN);

		public Task SendKeyUpAsync(Window window, Keys key) => SendKeyAsync(window, key, WM_KEYUP);

		static async Task SendKeyAsync(Window window, Keys key, int msg)
		{
			var form = GetForm(window);

			// post to whatever has focus, falling back to the window when nothing in it does
			var target = GetFocus();
			if (target == IntPtr.Zero)
				target = form.Handle;

			PostMessage(target, msg, (IntPtr)(int)key.ToSWF(), IntPtr.Zero);

			// the message has to go around the message loop before WinForms delivers it, which is also
			// what gives Form.KeyPreview a chance to see it first
			await Task.Delay(50);
		}

		public bool IsWindowFocusedItself(Window window) => GetFocus() == GetForm(window).Handle;

		public bool IsFocusWithinWindow(Window window) => GetForm(window).ContainsFocus;
	}
}
