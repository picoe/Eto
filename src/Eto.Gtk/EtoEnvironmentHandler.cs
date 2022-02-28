using System;
using Eto;
using System.Linq;
using System.Reflection;
using System.IO;
using System.Diagnostics;

namespace Eto.GtkSharp
{
	public class EtoEnvironmentHandler : WidgetHandler<Widget>, EtoEnvironment.IHandler
	{

		/// <summary>
		/// Determines if <see cref="GetFolderPath"/> should return XDG Folders or the legacy ones.
		/// </summary>
		/// <remarks>This changes the return values of:
		/// <list type="bullet">
		/// <item><see cref="EtoSpecialFolder.ApplicationSettings"/> for Mac. If this is set to <c>true</c>,
		/// it will return "~/Library/Application Support". Otherwise it will return "~/.config/".</item>
		/// <item><see cref="EtoSpecialFolder.Documents"/> for Mac and Linux. If this is set to <c>true</c>,
		/// it will return the user's documents folder for Mac and Linux. For Linux, $XDG_DOCUMENTS_DIR
		/// is used for determining the folder, with "~/Documents" as a fallback should it not be set.<br/>
		/// If this is set to <c>false</c>, it will return the home folder for both Mac and Linux.</item>
		/// <item><see cref="EtoSpecialFolder.Downloads"/> for Mac and Linux. If this is set to <c>true</c>,
		/// it will return the user's download folder for Mac and Linux. For Linux, $XDG_DOWNLOADS_DIR
		/// is used for determining the folder, with "~/Downloads" as a fallback should it not be set. <br/>
		/// If this is set to <c>false</c>, it will return "~/Downloads" for both Mac and Linux.</item>
		/// </list></remarks>
		public static bool UseXDG = true;

		public string GetFolderPath(EtoSpecialFolder folder)
		{
			string homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
			switch (folder)
			{
				// Currently has a bug on dotnet where it returns the wrong path for Mac
				// https://github.com/dotnet/runtime/issues/63214#issuecomment-1003414322
				case EtoSpecialFolder.ApplicationSettings:
					if (UseXDG && EtoEnvironment.Platform.IsMac)
						return Path.Combine(homeDir, "Library", "Application Support");
					return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

				case EtoSpecialFolder.ApplicationResources:
					return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

				case EtoSpecialFolder.EntryExecutable:
					var path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
					if (String.IsNullOrEmpty(path))
						path = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
					return path;

				// Currently has a bug on dotnet, where it returns the wrong path for Linux+Mac
				// See link above
				case EtoSpecialFolder.Documents:
					if (UseXDG && EtoEnvironment.Platform.IsMac)
						return Path.Combine(homeDir, "Documents");
					if (UseXDG && EtoEnvironment.Platform.IsLinux)
						return GetXdgUserDirectory(homeDir, "XDG_DOCUMENTS_DIR", "Documents");
					return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

				case EtoSpecialFolder.Downloads:
					if (UseXDG && EtoEnvironment.Platform.IsLinux)
						return GetXdgUserDirectory(homeDir, "XDG_DOWNLOAD_DIR", "Downloads");

					// This technically doesn't return the proper download path for Windows on UseXDG, but it's not really a priority to implement.
					// If someone wants to implement this to call SHGetFolderPath on Windows, feel free to PR.
					return Path.Combine(homeDir, "Downloads");

				default:
					throw new NotSupportedException();
			}
		}

		/// <summary>
		/// Tries to resolve <paramref name="xdgUserDir"/> and get its value. <br/>
		/// See this for more documentation: https://www.freedesktop.org/wiki/Software/xdg-user-dirs/
		/// </summary>
		/// <param name="homeDir">Home directory of the user.</param>
		/// <param name="xdgUserDir">The XDG user directory to get the value for.</param>
		/// <param name="fallback">The fallback folder, which will be appended to <paramref name="homeDir"/>.</param>
		/// <returns>The value of <paramref name="xdgUserDir"/> if it can be resolved, otherwise
		/// the combined Path of <paramref name="homeDir"/> and <paramref name="xdgUserDir"/></returns>
		private static string GetXdgUserDirectory(string homeDir, string xdgUserDir, string fallback)
		{
			// If user has set a custom xdg env var, return that instead
			string xdgEnvVar = Environment.GetEnvironmentVariable(xdgUserDir);
			if (!String.IsNullOrEmpty(xdgEnvVar))
				return xdgEnvVar;

			// Try to read XDG_CONFIG_HOME/users-dirs.dirs and get the variable from there
			string userDirsPath = Path.Combine(GetXdgBaseDirectory(homeDir, "XDG_CONFIG_HOME"), "user-dirs.dirs");
			try
			{
				// Find the line where the xdg user dir is mentioned and it's not commented out
				string lineContent = File.ReadAllLines(userDirsPath).First(l => l.Contains(xdgUserDir) && (l[0] != '#'));

				// Get the path which is enclosed in '"'
				int firstIndex = lineContent.IndexOf('"') + 1;
				int secondIndex = lineContent.IndexOf('"', firstIndex);
				lineContent = lineContent.Substring(firstIndex, secondIndex - firstIndex);

				// If HOME is there, replace it
				lineContent = lineContent.Replace("$HOME", homeDir);

				// Per xdg-specification, if a user dir is set to home it's marked as disabled, which means we use the fallback.
				if (lineContent == homeDir)
					return Path.Combine(homeDir, fallback);

				// We have the path now
				return lineContent;
			}
			// If we get an error trying to read the file, if the user dir doesn't exist or anything else, we use the fallback.
			catch (Exception ex)
			{
				Debug.WriteLine($"Error occured while trying to get XDG-USER-DIR \"{xdgUserDir}\": {ex}");
			}
			return Path.Combine(homeDir, fallback);
		}

		/// <summary>
		/// Tries to resolve <paramref name="xdgBaseDir"/> and get its value. <br/>
		/// See this for more documentation: https://specifications.freedesktop.org/basedir-spec/basedir-spec-latest.html
		/// </summary>
		/// <param name="homeDir">Home directory of the user.</param>
		/// <param name="xdgBaseDir">The XDG base directory to get the value for.</param>
		/// <returns>The value of the Environment Variable <paramref name="xdgBaseDir"/>
		/// otherwise the appropriate fallback will used.</returns>
		private static string GetXdgBaseDirectory(string homeDir, string xdgBaseDir)
		{
			// If user has set a custom xdg env var, return that instead
			string xdgEnvVar = Environment.GetEnvironmentVariable(xdgBaseDir);
			if (!String.IsNullOrEmpty(xdgEnvVar))
				return xdgEnvVar;

			// Otherwise, return the fallback
			switch (xdgBaseDir)
			{
				// We currently only need CONFIG. Can expand this later if needed.
				case "XDG_CONFIG_HOME":
					return Path.Combine(homeDir, ".config");
				// xdg-user-dir returns HOME for everything that isn't defined
				default:
					Debug.WriteLine($"XDG-BASE-DIRECTORY {xdgBaseDir} not found!");
					return homeDir;
			}
		}
	}
}

