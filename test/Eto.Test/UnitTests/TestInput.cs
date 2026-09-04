using NUnit.Framework;
using System.Threading.Tasks;
using Eto.Forms;

namespace Eto.Test.UnitTests
{
	/// <summary>
	/// Platform hooks for driving keyboard input through a shown window and for inspecting the native
	/// keyboard focus, so tests about input routing and focus can be written once and run on every
	/// platform.
	/// </summary>
	/// <remarks>
	/// Implemented per platform in test/Eto.Test.&lt;Platform&gt;/TestInput.cs and registered on the
	/// platform with <see cref="Platform.Add{T}(System.Func{T})"/>. See <see cref="TestInput"/> for
	/// the entry point tests should use.
	/// </remarks>
	public interface ITestInput
	{
		/// <summary>
		/// Sends a key press to whatever currently has keyboard focus in the specified window, going
		/// through the platform's real input routing rather than raising Eto's events directly.
		/// </summary>
		/// <remarks>
		/// The returned task completes once the platform has delivered the key, which on some
		/// platforms means it has to go through the message loop first.
		/// </remarks>
		Task SendKeyDownAsync(Window window, Keys key);

		/// <inheritdoc cref="SendKeyDownAsync"/>
		Task SendKeyUpAsync(Window window, Keys key);

		/// <summary>
		/// Gets whether the window itself has keyboard focus, as opposed to a control inside it.
		/// </summary>
		bool IsWindowFocusedItself(Window window);

		/// <summary>
		/// Gets whether the window or any control inside it has keyboard focus.
		/// </summary>
		bool IsFocusWithinWindow(Window window);
	}

	/// <summary>
	/// Entry point for the <see cref="ITestInput"/> platform hooks. Tests that need them should call
	/// <see cref="EnsureSupported"/> (or use one of the methods here, which call it) so they get
	/// ignored rather than failing on a platform that has no implementation yet.
	/// </summary>
	public static class TestInput
	{
		static ITestInput Handler => Platform.Instance.CreateShared<ITestInput>();

		/// <summary>
		/// Gets whether the current platform has an <see cref="ITestInput"/> implementation registered.
		/// </summary>
		public static bool IsSupported => Platform.Instance.Supports<ITestInput>();

		/// <summary>
		/// Ignores the current test when the platform has no <see cref="ITestInput"/> implementation.
		/// </summary>
		public static void EnsureSupported()
		{
			if (!IsSupported)
				Assert.Ignore($"{Platform.Instance.ID} does not implement ITestInput");
		}

		/// <inheritdoc cref="ITestInput.SendKeyDownAsync"/>
		public static Task SendKeyDownAsync(Window window, Keys key)
		{
			EnsureSupported();
			return Handler.SendKeyDownAsync(window, key);
		}

		/// <inheritdoc cref="ITestInput.SendKeyUpAsync"/>
		public static Task SendKeyUpAsync(Window window, Keys key)
		{
			EnsureSupported();
			return Handler.SendKeyUpAsync(window, key);
		}

		/// <inheritdoc cref="ITestInput.IsWindowFocusedItself"/>
		public static bool IsWindowFocusedItself(Window window)
		{
			EnsureSupported();
			return Handler.IsWindowFocusedItself(window);
		}

		/// <inheritdoc cref="ITestInput.IsFocusWithinWindow"/>
		public static bool IsFocusWithinWindow(Window window)
		{
			EnsureSupported();
			return Handler.IsFocusWithinWindow(window);
		}
	}
}
