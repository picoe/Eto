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
		static bool contextCreationSucceeded = false;

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
				if (!contextCreationSucceeded)
				{
					// Pull manifest from the .NET Framework install
					// directory

					string assemblyLoc = null;

					FileIOPermission fiop = new FileIOPermission(PermissionState.None);
					fiop.AllFiles = FileIOPermissionAccess.PathDiscovery;
					fiop.Assert();
					try
					{
						assemblyLoc = typeof(Object).Assembly.Location;
					}
					finally
					{
						CodeAccessPermission.RevertAssert();
					}

					string manifestLoc = null;
					string installDir = null;
					if (assemblyLoc != null)
					{
						installDir = Path.GetDirectoryName(assemblyLoc);
						const string manifestName = "XPThemes.manifest";
						manifestLoc = Path.Combine(installDir, manifestName);
					}

					if (manifestLoc != null && installDir != null)
					{
						enableThemingActivationContext = new ACTCTX();
						enableThemingActivationContext.cbSize = Marshal.SizeOf(typeof(ACTCTX));
						enableThemingActivationContext.lpSource = manifestLoc;

						// Set the lpAssemblyDirectory to the install
						// directory to prevent Win32 Side by Side from
						// looking for comctl32 in the application
						// directory, which could cause a bogus dll to be
						// placed there and open a security hole.
						enableThemingActivationContext.lpAssemblyDirectory = installDir;
						enableThemingActivationContext.dwFlags = ACTCTX_FLAG_ASSEMBLY_DIRECTORY_VALID;

						// Note this will fail gracefully if file specified
						// by manifestLoc doesn't exist.
						hActCtx = CreateActCtx(ref enableThemingActivationContext);
						contextCreationSucceeded = (hActCtx != new IntPtr(-1));
					}
				}

				// If we return false, we'll try again on the next call into
				// EnsureActivateContextCreated(), which is fine.
				return contextCreationSucceeded;
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
