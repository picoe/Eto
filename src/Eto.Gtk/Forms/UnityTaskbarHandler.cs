using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Eto.Forms;

namespace Eto.GtkSharp
{
	public class UnityTaskbarHandler : Taskbar.IHandler
	{
		private const string libunity = "libunity.so.9";

		[DllImport(libunity, CallingConvention = CallingConvention.Cdecl)]
		protected extern static IntPtr unity_launcher_entry_get_for_desktop_id(string app_name);

		[DllImport(libunity, CallingConvention = CallingConvention.Cdecl)]
		protected extern static void unity_launcher_entry_set_progress_visible(IntPtr self, bool visible);

		[DllImport(libunity, CallingConvention = CallingConvention.Cdecl)]
		protected extern static void unity_launcher_entry_set_progress(IntPtr self, double progress);

		[DllImport(libunity, CallingConvention = CallingConvention.Cdecl)]
		protected extern static void unity_launcher_entry_set_urgent(IntPtr self, bool urgent);

		private static IntPtr _handle;
		private static TaskbarProgressState _state;

		static UnityTaskbarHandler()
		{
			var desktopEntry = Environment.GetEnvironmentVariable("DESKTOP_ENTRY");

			if (string.IsNullOrEmpty(desktopEntry))
				Console.WriteLine("Please set DESKTOP_ENTRY to point to your apps .desktop launcher in order to use application progressbar.");

			try
			{
				_handle = unity_launcher_entry_get_for_desktop_id(desktopEntry);
			}
			catch
			{
				Console.WriteLine("Failed to init Unity LauncherEntry API.");
			}

		}

		public void SetProgress(TaskbarProgressState state, float progress)
		{
			if (_handle == IntPtr.Zero)
				return;

			unity_launcher_entry_set_progress_visible(_handle, state != TaskbarProgressState.None);

			switch (state)
			{
				case TaskbarProgressState.None:
					unity_launcher_entry_set_urgent(_handle, false);
					unity_launcher_entry_set_progress(_handle, 0f);
					break;
				case TaskbarProgressState.Progress:
					unity_launcher_entry_set_urgent(_handle, (int)progress == 1);
					unity_launcher_entry_set_progress(_handle, progress);
					break;
				case TaskbarProgressState.Paused:
					unity_launcher_entry_set_urgent(_handle, false);
					unity_launcher_entry_set_progress(_handle, progress);
					break;
				case TaskbarProgressState.Indeterminate:
					unity_launcher_entry_set_urgent(_handle, false);

					if (state == TaskbarProgressState.Indeterminate && _state != TaskbarProgressState.Indeterminate)
					{
						_state = state;
						Task.Run(AnimateIndeterminate);
					}
					break;
				case TaskbarProgressState.Error:
					unity_launcher_entry_set_urgent(_handle, true);
					unity_launcher_entry_set_progress(_handle, progress);
					break;
			}

			_state = state;
		}

		private async Task AnimateIndeterminate()
		{
			var progress = 0f;

			while (true)
			{
				if (_state != TaskbarProgressState.Indeterminate)
				{
					break;
				}

				unity_launcher_entry_set_progress(_handle, progress);

				progress += 0.01f;

				if (progress > 1f)
				{
					progress = 0f;
				}

				await Task.Delay(10);
			}
		}
	}
}
