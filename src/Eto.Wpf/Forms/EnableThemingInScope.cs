using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Windows.Forms;

namespace Eto.Wpf.Forms
{
	/// <devdoc>
	///     This class is intended to use with the C# 'using' statement in
	///     to activate an activation context for turning on visual theming at
	///     the beginning of a scope, and have it automatically deactivated
	///     when the scope is exited.
	/// </devdoc>
	/// <summary>
	/// Enables visual styles for common system dialogs (e.g message box).
	/// Original source from: http://support.microsoft.com/kb/830033/en-us
	/// Curtis Wensley: Modified for 64-bit compatibility by using an IntPtr instead of uint for the cookie
	/// </summary>
	[SuppressUnmanagedCodeSecurity]
	internal class EnableThemingInScope : IDisposable
	{
		// Private data
		IntPtr cookie;
		static ACTCTX enableThemingActivationContext;
		static IntPtr hActCtx;
		static bool? contextCreationSucceeded;

		public EnableThemingInScope(bool enable)
		{
			cookie = IntPtr.Zero;
			if (enable && OSFeature.Feature.IsPresent(OSFeature.Themes))
			{
				if (EnsureActivateContextCreated())
				{
					if (!ActivateActCtx(hActCtx, out cookie))
					{
						// Be sure cookie always zero if activation failed
						cookie = IntPtr.Zero;
					}
				}
			}
		}

		~EnableThemingInScope()
		{
			Dispose(false);
		}

		void IDisposable.Dispose()
		{
			Dispose(true);
		}

		void Dispose(bool disposing)
		{
			if (cookie != IntPtr.Zero)
			{
				if (DeactivateActCtx(0, cookie))
				{
					// deactivation succeeded...
					cookie = IntPtr.Zero;
				}
			}
		}

		bool EnsureActivateContextCreated()
		{
			lock (typeof(EnableThemingInScope))
			{
				if (contextCreationSucceeded != null)
					return contextCreationSucceeded.Value;
					
				// Use a custom manifest from resources and write it to a temp file
				var manifestLoc = Path.GetTempFileName();
				try
				{
					var stream = typeof(EnableThemingInScope).Assembly.GetManifestResourceStream("Eto.Wpf.XPThemes.manifest");
					if (stream == null)
						return false;
						
					using (var fs = File.Create(manifestLoc))
					{
						stream.CopyTo(fs);
					}

					if (manifestLoc != null)
					{
						enableThemingActivationContext = new ACTCTX();
						enableThemingActivationContext.cbSize = Marshal.SizeOf(typeof(ACTCTX));
						enableThemingActivationContext.lpSource = manifestLoc;

						// Note this will fail gracefully if file specified
						// by manifestLoc doesn't exist.
						hActCtx = CreateActCtx(ref enableThemingActivationContext);
						contextCreationSucceeded = (hActCtx != new IntPtr(-1));
					}
				}
				finally
				{
					if (File.Exists(manifestLoc))
						File.Delete(manifestLoc);
				}

				return contextCreationSucceeded.Value;
			}
		}

		// All the pinvoke goo...
		[DllImport("Kernel32.dll")]
		extern static IntPtr CreateActCtx(ref ACTCTX actctx);
		[DllImport("Kernel32.dll")]
		extern static bool ActivateActCtx(IntPtr hActCtx, out IntPtr lpCookie);
		[DllImport("Kernel32.dll")]
		extern static bool DeactivateActCtx(uint dwFlags, IntPtr lpCookie);

		const int ACTCTX_FLAG_ASSEMBLY_DIRECTORY_VALID = 0x004;

		struct ACTCTX
		{
			public int cbSize;
			public uint dwFlags;
			public string lpSource;
			public ushort wProcessorArchitecture;
			public ushort wLangId;
			public string lpAssemblyDirectory;
			public string lpResourceName;
			public string lpApplicationName;
		}
	}
}
