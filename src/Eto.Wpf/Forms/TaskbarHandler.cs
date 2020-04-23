using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Eto.Forms;

#if WPF
namespace Eto.Wpf.Forms
#elif WINFORMS
namespace Eto.WinForms.Forms
#endif
{
	public class TaskbarHandler : Taskbar.IHandler
	{
		[ComImport()]
		[Guid("ea1afb91-9e28-4b86-90e9-9e9f8a5eefaf")]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		private interface ITaskbarList3
		{
			// ITaskbarList
			[PreserveSig]
			void HrInit();
			[PreserveSig]
			void AddTab(IntPtr hwnd);
			[PreserveSig]
			void DeleteTab(IntPtr hwnd);
			[PreserveSig]
			void ActivateTab(IntPtr hwnd);
			[PreserveSig]
			void SetActiveAlt(IntPtr hwnd);

			// ITaskbarList2
			[PreserveSig]
			void MarkFullscreenWindow(IntPtr hwnd, [MarshalAs(UnmanagedType.Bool)] bool fFullscreen);

			// ITaskbarList3
			[PreserveSig]
			void SetProgressValue(IntPtr hwnd, UInt64 ullCompleted, UInt64 ullTotal);
			[PreserveSig]
			void SetProgressState(IntPtr hwnd, int state);
		}

		[ComImport()]
		[Guid("56fdf344-fd6d-11d0-958a-006097c9a090")]
		[ClassInterface(ClassInterfaceType.None)]
		private class TaskbarInstance
		{
		}

		private static ITaskbarList3 instance;
		private static bool supported;

		static TaskbarHandler()
		{
			supported = Environment.OSVersion.Version >= new Version(6, 1);

			if (supported)
				instance = (ITaskbarList3)new TaskbarInstance();
		}

		public void SetProgress(TaskbarProgressState state, float progress)
		{
			if (!supported)
				return;

			var winhandle = Process.GetCurrentProcess().MainWindowHandle;
			var taskstate = 0;

			switch (state)
			{
				case TaskbarProgressState.Indeterminate:
					taskstate = 0x1;
					break;
				case TaskbarProgressState.Progress:
					taskstate = 0x2;
					break;
				case TaskbarProgressState.Error:
					taskstate = 0x4;
					break;
				case TaskbarProgressState.Paused:
					taskstate = 0x8;
					break;
			}

			instance.SetProgressState(winhandle, taskstate);
			instance.SetProgressValue(winhandle, (ulong)(progress * 1000), 1000);
		}
	}
}
