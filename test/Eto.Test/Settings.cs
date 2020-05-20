using System;
using System.Diagnostics;
using System.IO;

namespace Eto.Test
{
	public class Settings
	{
		static string AppSettingsFolder => EtoEnvironment.GetFolderPath(EtoSpecialFolder.ApplicationSettings);
		static string SettingsFile => Path.Combine(AppSettingsFolder, "mainform.json");

		public string InitialSection { get; set; }

		public bool SaveInitialSection { get; set; }

		public string LastUnitTestFilter { get; set; }

		public static Settings Load()
		{
			try
			{
				if (File.Exists(SettingsFile))
					return Newtonsoft.Json.JsonConvert.DeserializeObject<Settings>(File.ReadAllText(SettingsFile)) ?? new Settings();
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error reading settings: {ex}");
			}
			return new Settings();
		}

		public void Save()
		{
			try
			{
				var str = Newtonsoft.Json.JsonConvert.SerializeObject(this);
				File.WriteAllText(SettingsFile, str);
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Error writing settings: {ex}");
			}

		}
	}
}

