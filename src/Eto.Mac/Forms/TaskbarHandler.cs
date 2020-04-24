using System;
using System.Diagnostics;
using Eto.Forms;

#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
using CoreImage;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
using MonoMac.CoreImage;
using nfloat = System.Double;
using nint = System.Int64;
using nuint = System.UInt64;
#endif

namespace Eto.Mac.Forms
{
	public class TaskbarHandler : Taskbar.IHandler
	{
		public void SetProgress(TaskbarProgressState state, float progress)
		{
			switch (state)
			{
				case TaskbarProgressState.Indeterminate:
					SetProgressBar(true, true);
					break;
				case TaskbarProgressState.Progress:
					SetProgressBar(true, false, progress);
					if (progress >= 1f)
					{
						NSApplication.SharedApplication.RequestUserAttention(NSRequestUserAttentionType.InformationalRequest);
					}
					break;
				case TaskbarProgressState.Error:
					SetProgressBar(progress > 0, false, progress);
					NSApplication.SharedApplication.RequestUserAttention(NSRequestUserAttentionType.CriticalRequest);
					break;
				case TaskbarProgressState.Paused:
					SetProgressBar(true, false, progress);
					break;
				case TaskbarProgressState.None:
					SetProgressBar(false);
					break;
			}
		}

		public class ProgressImageView : NSImageView
		{
			public ProgressBar ProgressBar { get; }

			public ProgressImageView()
			{
				var app = NSApplication.SharedApplication;
				Image = app.ApplicationIconImage;
				SetFrameSize(Image.Size);

				ProgressBar = new ProgressBar
				{
					MaxValue = 1000
				};
				var pbview = ProgressBar.ToNative(true);
				pbview.AutoresizingMask = NSViewResizingMask.WidthSizable;
				pbview.SetFrameSize(new CGSize(Frame.Width, pbview.Frame.Height));
				AddSubview(pbview);
			}
		}

		private static void SetProgressBar(bool visible, bool indeterminate = false, float value = 0f)
		{
			var app = NSApplication.SharedApplication;
			if (visible)
			{
				var dockControl = app.DockTile.ContentView as ProgressImageView ?? new ProgressImageView();

				dockControl.ProgressBar.Indeterminate = indeterminate;
				if (!indeterminate)
				{
					dockControl.ProgressBar.Value = (int)(value * 1000);
				}
				app.DockTile.ContentView = dockControl;
			}
			else
			{
				// should allow null here, probably.. but bindings don't allow it..
				app.DockTile.ContentView = new NSImageView { Image = app.ApplicationIconImage };
			}
			app.DockTile.Display();
		}
	}
}
