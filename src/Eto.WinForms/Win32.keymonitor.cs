using System;
using System.Collections.Generic;

namespace Eto
{
	static partial class Win32
	{
		/// <summary>
		/// Reports key presses going to a window or any of its child windows, including child HWNDs
		/// that Eto knows nothing about. Used to implement <see cref="Eto.Forms.Window.PreviewKeyDown"/>.
		/// </summary>
		/// <remarks>
		/// Keys are posted straight to the focused HWND, so a window never sees keys aimed at hosted
		/// native content. A thread wide WH_KEYBOARD hook does see them. WH_KEYBOARD is used in
		/// preference to WH_GETMESSAGE because it only runs for keyboard messages instead of for every
		/// message on the thread.
		///
		/// The hook carries no target window, so the destination is inferred from the focused window of
		/// the thread at the time the key is dispatched. That is accurate for ordinary input but can be
		/// wrong while a platform modal or menu tracking loop is running.
		///
		/// One hook is shared by every registration on a thread and is removed again once the last
		/// registration goes away. Targets are held weakly, so forgetting to dispose a registration
		/// costs nothing beyond the next key press, which prunes it. This is not thread safe: register,
		/// dispose, and the hook itself all run on the UI thread.
		/// </remarks>
		public static class KeyMonitor
		{
			const int HC_ACTION = 0;
			// bit 31 of lParam is the transition state, set when the key is being released
			const long TransitionState = 1L << 31;

			/// <summary>
			/// Implemented by a window handler that wants to be told about key presses going to it.
			/// </summary>
			public interface ITarget
			{
				/// <summary>
				/// The window to watch, or IntPtr.Zero when it doesn't exist yet. Read for each key
				/// rather than cached, since a registration can outlive a particular native window.
				/// </summary>
				IntPtr KeyMonitorHandle { get; }

				/// <summary>
				/// Called for each key press going to that window or a child of it, with the Win32
				/// virtual key code and whether the key went down.
				/// </summary>
				void OnKeyMonitorKey(int virtualKey, bool isKeyDown);
			}

			class Registration : IDisposable
			{
				public WeakReference<ITarget> Target;

				public void Dispose() => Unregister(this);
			}

			static readonly List<Registration> s_registrations = new List<Registration>();
			// the hook needs a strong reference for as long as it is installed
			static HookProc s_hookProc;
			static IntPtr s_hook;

			/// <summary>
			/// Starts reporting keys going to <paramref name="target"/>'s window.
			/// </summary>
			/// <param name="target">Target to report keys to. Held weakly.</param>
			/// <returns>An object that stops the reporting when disposed.</returns>
			public static IDisposable Register(ITarget target)
			{
				if (target == null)
					throw new ArgumentNullException(nameof(target));

				var registration = new Registration { Target = new WeakReference<ITarget>(target) };
				s_registrations.Add(registration);
				if (s_hook == IntPtr.Zero)
				{
					s_hookProc = HookProcedure;
					s_hook = SetHook(WH.KEYBOARD, s_hookProc, GetCurrentThreadId());
				}
				return registration;
			}

			static void Unregister(Registration registration)
			{
				if (!s_registrations.Remove(registration))
					return;
				if (s_registrations.Count == 0 && s_hook != IntPtr.Zero)
				{
					UnhookWindowsHookEx(s_hook);
					s_hook = IntPtr.Zero;
					s_hookProc = null;
				}
			}

			static IntPtr HookProcedure(int code, IntPtr wParam, IntPtr lParam)
			{
				// the hook can be removed while dispatching below, so pass on the handle we came in with
				var hook = s_hook;

				// HC_NOREMOVE means the message was only peeked at and will come through again,
				// ignore it so a key isn't counted twice
				if (code == HC_ACTION && s_registrations.Count > 0)
				{
					var focused = GetThreadFocusWindow();
					var virtualKey = wParam.ToInt32();
					var isKeyDown = (lParam.ToInt64() & TransitionState) == 0;

					// iterate a snapshot, a target is free to dispose its registration while being called
					foreach (var registration in s_registrations.ToArray())
					{
						if (!registration.Target.TryGetTarget(out var target))
						{
							// collected without being disposed, drop it and let the hook go with the last one
							Unregister(registration);
							continue;
						}
						if (focused == IntPtr.Zero)
							continue;
						var hwnd = target.KeyMonitorHandle;
						if (hwnd != IntPtr.Zero && (hwnd == focused || IsChild(hwnd, focused)))
							target.OnKeyMonitorKey(virtualKey, isKeyDown);
					}
				}
				return CallNextHookEx(hook, code, wParam, lParam);
			}
		}
	}
}
