using System;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System.Reflection;
#if XAMMAC2
using AppKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using CoreAnimation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.CoreGraphics;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
#endif

namespace Eto.Mac
{
	public class EtoEnvironmentHandler : WidgetHandler<Widget>, EtoEnvironment.IHandler
	{
		static void Convert(EtoSpecialFolder folder, out NSSearchPathDirectory dir, out NSSearchPathDomain domain)
		{
			switch (folder)
			{
				case EtoSpecialFolder.ApplicationSettings:
					dir = NSSearchPathDirectory.ApplicationSupportDirectory;
					domain = NSSearchPathDomain.User;
					break;
				case EtoSpecialFolder.Documents:
					dir = NSSearchPathDirectory.DocumentDirectory;
					domain = NSSearchPathDomain.User;
					break;
				default:
					throw new NotSupportedException();
			}
		}

		public string GetFolderPath(EtoSpecialFolder folder)
		{
			NSSearchPathDirectory dir;
			NSSearchPathDomain domain;
			switch (folder)
			{
				case EtoSpecialFolder.ApplicationResources:
					return NSBundle.MainBundle.ResourcePath;
				case EtoSpecialFolder.EntryExecutable:
					{
						var path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
						if (string.IsNullOrEmpty(path))
							path = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
						return path;
					}
				case EtoSpecialFolder.ApplicationSettings:
					{
						Convert(folder, out dir, out domain);
						var path = NSSearchPath.GetDirectories(dir, domain).FirstOrDefault();
						path = Path.Combine(path, NSBundle.MainBundle.BundleIdentifier);
						if (!Directory.Exists(path))
							Directory.CreateDirectory(path);
						return path;
					}
				default:
					Convert(folder, out dir, out domain);
					return NSSearchPath.GetDirectories(dir, domain).FirstOrDefault();
			}
		}
	}
}

