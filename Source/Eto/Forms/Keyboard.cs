using System;
using System.Collections.Generic;
namespace Eto.Forms
{
	/// <summary>
	/// General keyboard methods
	/// </summary>
	[Handler(typeof(IHandler))]
	public static class Keyboard
	{
		static IHandler Handler { get { return Platform.Instance.CreateShared<IHandler>(); } }

		/// <summary>
		/// Gets an enumeration of all keys supported by the <see cref="IsKeyLocked"/> method.
		/// </summary>
		/// <value>The supported lock keys.</value>
		public static IEnumerable<Keys> SupportedLockKeys { get { return Handler.SupportedLockKeys; } }

		/// <summary>
		/// Determines if the specified <paramref name="key"/> is in a locked state, such as the <see cref="Keys.CapsLock"/>, 
		/// <see cref="Keys.ScrollLock"/>, <see cref="Keys.NumberLock"/>, or <see cref="Keys.Insert"/> key.
		/// </summary>
		/// <returns><c>true</c> if the specified key is locked; otherwise, <c>false</c>.</returns>
		/// <param name="key">Key to determine the state.</param>
		public static bool IsKeyLocked(Keys key)
		{
			return Handler.IsKeyLocked(key);
		}

		/// <summary>
		/// Handler for platforms to implement the <see cref="Keyboard"/> functionality.
		/// </summary>
		public interface IHandler
		{
			/// <summary>
			/// Gets an enumeration of all keys supported by the <see cref="IsKeyLocked"/> method.
			/// </summary>
			/// <value>The supported lock keys.</value>
			IEnumerable<Keys> SupportedLockKeys { get; }

			/// <summary>
			/// Determines if the specified <paramref name="key"/> is in a locked state, such as the <see cref="Keys.CapsLock"/>, 
			/// <see cref="Keys.ScrollLock"/>, <see cref="Keys.NumberLock"/>, or <see cref="Keys.Insert"/> key.
			/// </summary>
			/// <returns><c>true</c> if the specified key is locked; otherwise, <c>false</c>.</returns>
			/// <param name="key">Key to determine the state.</param>
			bool IsKeyLocked(Keys key);
		}
	}
}

