using Eto.Mac;
using Eto.Test.Mac.UnitTests;
using Eto.Test.UnitTests;
using System.Threading.Tasks;

namespace Eto.Test.Mac
{
	public class TestInput : ITestInput
	{
		static NSWindow GetWindow(Window window) => (NSWindow)window.ControlObject;

		public Task SendKeyDownAsync(Window window, Keys key) => SendKeyAsync(window, key, keyDown: true);

		public Task SendKeyUpAsync(Window window, Keys key) => SendKeyAsync(window, key, keyDown: false);

		static Task SendKeyAsync(Window window, Keys key, bool keyDown)
		{
			var keyEvent = KeyEvents.CreatePhysicalKeyEvent(GetKeyCode(key), keyDown, key.ModifierMask());

			// -[NSWindow sendEvent:] delivers the key to the first responder and on up the responder
			// chain, which is the routing Eto hooks its key events into.  Going through NSApplication
			// instead would need the event to carry a window number, which a synthesized one does not.
			GetWindow(window).SendEvent(keyEvent);
			return Task.CompletedTask;
		}

		public bool IsWindowFocusedItself(Window window)
		{
			var control = GetWindow(window);
			var responder = control.FirstResponder;
			return responder != null && responder.Handle == control.Handle;
		}

		public bool IsFocusWithinWindow(Window window)
		{
			var control = GetWindow(window);
			var responder = control.FirstResponder;
			if (responder == null)
				return false;
			// macOS keeps the focus per window, and makes the window itself the first responder when
			// nothing in its content can take focus
			return responder.Handle == control.Handle
				|| (responder is NSView view && view.Window?.Handle == control.Handle);
		}

		static readonly Dictionary<Keys, ushort> s_keyCodes = GetKeyCodes();

		/// <summary>
		/// Inverts Eto's physical keyCode to <see cref="Keys"/> map so a test can name a key rather
		/// than a macOS virtual key code.
		/// </summary>
		/// <remarks>
		/// Built in ascending keyCode order, so the main keyboard wins over the numeric pad for the
		/// keys that appear on both (Enter and Equal).
		/// </remarks>
		static Dictionary<Keys, ushort> GetKeyCodes()
		{
			var keyCodes = new Dictionary<Keys, ushort>();
			// 126 (Up) is the highest key code Eto maps
			for (ushort keyCode = 0; keyCode <= 126; keyCode++)
			{
				var key = KeyMap.MapKey(keyCode, 0);
				if (key != Keys.None && !keyCodes.ContainsKey(key))
					keyCodes.Add(key, keyCode);
			}
			return keyCodes;
		}

		static int GetKeyCode(Keys key)
		{
			if (!s_keyCodes.TryGetValue(key & Keys.KeyMask, out var keyCode))
				throw new NotSupportedException($"There is no macOS key code for {key & Keys.KeyMask}");
			return keyCode;
		}
	}
}
