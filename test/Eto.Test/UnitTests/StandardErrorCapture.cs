namespace Eto.Test.UnitTests
{
	/// <summary>
	/// Helper to capture what is written to the process' standard error stream while running an action.
	/// </summary>
	/// <remarks>
	/// GTK/GLib criticals and warnings are written directly to the native stderr file descriptor,
	/// bypassing <see cref="Console.Error"/>, so on unix we redirect fd 2 to a temp file to catch them.
	/// On other platforms (where there is no GTK to warn) we simply capture <see cref="Console.Error"/>.
	/// </remarks>
	public static class StandardErrorCapture
	{
		[DllImport("libc", SetLastError = true)]
		static extern int dup(int oldfd);

		[DllImport("libc", SetLastError = true)]
		static extern int dup2(int oldfd, int newfd);

		[DllImport("libc", SetLastError = true)]
		static extern int close(int fd);

		[DllImport("libc")]
		static extern int fflush(IntPtr stream);

		/// <summary>
		/// Runs <paramref name="action"/> while capturing everything written to the process' standard
		/// error stream and returns the captured text.
		/// </summary>
		public static string Capture(Action action)
		{
			if (!(RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX)))
			{
				var writer = new StringWriter();
				var previous = Console.Error;
				Console.SetError(writer);
				try
				{
					action();
				}
				finally
				{
					Console.SetError(previous);
				}
				return writer.ToString();
			}

			var tempFile = Path.GetTempFileName();
			var savedStderr = dup(2); // keep a copy of the real stderr so we can restore it
			try
			{
				using (var fs = new FileStream(tempFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
				{
					// on unix a SafeFileHandle wraps the file descriptor
					var fd = fs.SafeFileHandle.DangerousGetHandle().ToInt32();
					dup2(fd, 2); // point fd 2 (stderr) at the temp file
					try
					{
						action();
						fflush(IntPtr.Zero); // flush all C stdio streams so GLib output lands in the file
					}
					finally
					{
						dup2(savedStderr, 2); // restore the real stderr
					}
				}
				return File.ReadAllText(tempFile);
			}
			finally
			{
				close(savedStderr);
				try { File.Delete(tempFile); } catch { }
			}
		}
	}
}
