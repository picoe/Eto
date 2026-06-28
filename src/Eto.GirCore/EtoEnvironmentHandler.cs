namespace Eto.GirCore;

public class EtoEnvironmentHandler : WidgetHandler<Widget>, EtoEnvironment.IHandler
{
	public static bool UseXDG = true;

	public string GetFolderPath(EtoSpecialFolder folder)
	{
		string homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
		switch (folder)
		{
			case EtoSpecialFolder.ApplicationSettings:
				if (UseXDG && EtoEnvironment.Platform.IsMac)
					return Path.Combine(homeDir, "Library", "Application Support");
				return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

			case EtoSpecialFolder.ApplicationResources:
				return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

			case EtoSpecialFolder.EntryExecutable:
				var path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
				if (string.IsNullOrEmpty(path))
					path = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
				return path;

			case EtoSpecialFolder.Documents:
				if (UseXDG && EtoEnvironment.Platform.IsMac)
					return Path.Combine(homeDir, "Documents");
				if (UseXDG && EtoEnvironment.Platform.IsLinux)
					return GetXdgUserDirectory(homeDir, "XDG_DOCUMENTS_DIR", "Documents");
				return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

			case EtoSpecialFolder.Downloads:
				if (UseXDG && EtoEnvironment.Platform.IsLinux)
					return GetXdgUserDirectory(homeDir, "XDG_DOWNLOAD_DIR", "Downloads");
				return Path.Combine(homeDir, "Downloads");

			default:
				throw new NotSupportedException();
		}
	}

	static string GetXdgUserDirectory(string homeDir, string xdgUserDir, string fallback)
	{
		string xdgEnvVar = Environment.GetEnvironmentVariable(xdgUserDir);
		if (!string.IsNullOrEmpty(xdgEnvVar))
			return xdgEnvVar;

		string userDirsPath = Path.Combine(GetXdgBaseDirectory(homeDir, "XDG_CONFIG_HOME"), "user-dirs.dirs");
		try
		{
			string lineContent = File.ReadAllLines(userDirsPath).First(l => l.Contains(xdgUserDir) && l[0] != '#');
			int firstIndex = lineContent.IndexOf('"') + 1;
			int secondIndex = lineContent.IndexOf('"', firstIndex);
			lineContent = lineContent.Substring(firstIndex, secondIndex - firstIndex);
			lineContent = lineContent.Replace("$HOME", homeDir);

			if (lineContent == homeDir)
				return Path.Combine(homeDir, fallback);

			return lineContent;
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"Error occured while trying to get XDG-USER-DIR \"{xdgUserDir}\": {ex}");
		}
		return Path.Combine(homeDir, fallback);
	}

	static string GetXdgBaseDirectory(string homeDir, string xdgBaseDir)
	{
		string xdgEnvVar = Environment.GetEnvironmentVariable(xdgBaseDir);
		if (!string.IsNullOrEmpty(xdgEnvVar))
			return xdgEnvVar;

		return xdgBaseDir switch
		{
			"XDG_CONFIG_HOME" => Path.Combine(homeDir, ".config"),
			_ => homeDir
		};
	}
}
