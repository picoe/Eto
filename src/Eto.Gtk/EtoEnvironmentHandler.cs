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
		public string GetFolderPath(EtoSpecialFolder folder)
		{
			string homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
			switch (folder)
			{
				// Currently has a bug on dotnet where it returns the wrong path for Mac
				// https://github.com/dotnet/runtime/issues/63214#issuecomment-1003414322
				case EtoSpecialFolder.ApplicationSettings:
					{
						if (EtoEnvironment.Platform.IsMac)
							return Path.Combine(homeDir, "Library", "Preferences");
						else 
							return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
					}
				case EtoSpecialFolder.ApplicationResources:
					return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
				case EtoSpecialFolder.EntryExecutable:
					{
						var path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
						if (string.IsNullOrEmpty(path))
							path = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
						return path;
					}
				// Curently has a bug on dotnet, where it returns the wrong path for Linux+Mac
				// See link above
				case EtoSpecialFolder.Documents:
					{
						if (EtoEnvironment.Platform.IsLinux)
							return GetXdgUserDirectory(homeDir, "XDG_DOCUMENTS_DIR", "Documents");
						// Technically Mac would be solved by the Linux path as well, but it's cleaner
						// and faster to just combine the paths on Mac rather than trying to resolve the xdg path
						else if (EtoEnvironment.Platform.IsMac)
							return Path.Combine(homeDir, "Documents");
						else
							return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
					}
				case EtoSpecialFolder.Downloads:
					{
						if (EtoEnvironment.Platform.IsLinux)
							return GetXdgUserDirectory(homeDir, "XDG_DOWNLOAD_DIR", "Downloads");
						else
							return Path.Combine(homeDir, "Downloads");
					}
				default:
					throw new NotSupportedException();
			}
		}

		/// <summary>
		/// Tries to resolve <paramref name="xdgUserDir"/> and get its value.
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
				string lineContent = File.ReadAllLines(userDirsPath).Where(l => l.Contains(xdgUserDir) && l[0] != '#').First();
				// Get the path which is enclosed in '"'
				int firstIndex = lineContent.IndexOf('"') + 1;
				int secondIndex = lineContent.IndexOf('"', firstIndex);
				lineContent = lineContent.Substring(firstIndex, secondIndex - firstIndex);

				// If HOME is there, replace it
				lineContent = lineContent.Replace("$HOME", homeDir);

				// We have the Path now
				return lineContent;
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error occured while trying to get XDG-USER-DIR \"{xdgUserDir}\": {ex}");
			}

			// Something failed while trying 
			return Path.Combine(homeDir, fallback);
		}

		/// <summary>
		/// Tries to resolve <paramref name="xdgBaseDir"/> and get its value.
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
					{
						Debug.WriteLine($"XDG-BASE-DIRECTORY {xdgBaseDir} not found!");
						return homeDir;
					}
			}
		}
	}
}

