using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Eto.Windows.Build
{
	static class WindowsPackageHelper
	{
		// namespace for the GUIDs derived from names, so the same app always gets the same ones
		static readonly Guid EtoNamespace = new Guid("2f1c5e9a-7b3d-4e8f-9a61-0c5d8b7e2f43");

		/// <summary>Windows Installer versions are major.minor.build, with major and minor up to 255 and build up to 65535.</summary>
		public static string ProductVersion(string version, TaskLoggingHelper log)
		{
			var match = Regex.Match(version ?? string.Empty, @"^\s*(\d+)(?:\.(\d+))?(?:\.(\d+))?");
			if (!match.Success)
			{
				log.LogWarning("'{0}' isn't a version number, so the installer uses 1.0.0.", version);
				return "1.0.0";
			}
			int Part(int index) => match.Groups[index].Success ? int.Parse(match.Groups[index].Value) : 0;
			int major = Part(1), minor = Part(2), build = Part(3);
			if (major > 255 || minor > 255 || build > 65535)
				log.LogError("Version {0} can't be used by Windows Installer, which allows at most 255.255.65535. Set WindowsPackageVersion.", version);
			return $"{major}.{minor}.{build}";
		}

		public static Guid NameGuid(string name)
		{
			// RFC 4122 version 5, with the GUID fields in network byte order
			var ns = EtoNamespace.ToByteArray();
			SwapByteOrder(ns);
			var bytes = ns.Concat(Encoding.UTF8.GetBytes(name)).ToArray();
			byte[] hash;
			using (var sha = SHA1.Create())
				hash = sha.ComputeHash(bytes);
			var guid = new byte[16];
			Array.Copy(hash, guid, 16);
			guid[6] = (byte)((guid[6] & 0x0F) | 0x50);
			guid[8] = (byte)((guid[8] & 0x3F) | 0x80);
			SwapByteOrder(guid);
			return new Guid(guid);
		}

		static void SwapByteOrder(byte[] guid)
		{
			void Swap(int a, int b)
			{
				var t = guid[a];
				guid[a] = guid[b];
				guid[b] = t;
			}
			Swap(0, 3);
			Swap(1, 2);
			Swap(4, 5);
			Swap(6, 7);
		}

		public static string Format(Guid guid) => guid.ToString("B").ToUpperInvariant();
	}

	/// <summary>
	/// Reads the shared frameworks a framework-dependent app needs from its runtimeconfig.json.
	/// </summary>
	public class GetRuntimeFrameworks : Task
	{
		[Required]
		public string RuntimeConfigFile { get; set; }

		/// <summary>Framework names, with the minimum version in the Version metadata.</summary>
		[Output]
		public ITaskItem[] Frameworks { get; set; }

		public override bool Execute()
		{
			var frameworks = new List<ITaskItem>();
			if (File.Exists(RuntimeConfigFile))
			{
				var json = File.ReadAllText(RuntimeConfigFile);
				// includedFrameworks lists what a self-contained app carries itself, so only these are needed from the system
				var section = Regex.Match(json, "\"frameworks?\"\\s*:\\s*(\\[[^\\]]*\\]|\\{[^}]*\\})");
				if (section.Success)
				{
					foreach (Match framework in Regex.Matches(section.Groups[1].Value, "\\{[^{}]*\\}"))
					{
						var name = Regex.Match(framework.Value, "\"name\"\\s*:\\s*\"([^\"]+)\"");
						var version = Regex.Match(framework.Value, "\"version\"\\s*:\\s*\"([^\"]+)\"");
						if (name.Success && version.Success)
						{
							var item = new TaskItem(name.Groups[1].Value);
							item.SetMetadata("Version", version.Groups[1].Value);
							frameworks.Add(item);
						}
					}
				}
			}
			// the desktop runtime's installer includes the .NET runtime it needs
			if (frameworks.Any(r => r.ItemSpec == "Microsoft.WindowsDesktop.App"))
				frameworks.RemoveAll(r => r.ItemSpec == "Microsoft.NETCore.App");
			Frameworks = frameworks.ToArray();
			return true;
		}
	}

	/// <summary>
	/// Writes the generated part of the setup bootstrapper's source: its version info and what it installs.
	/// </summary>
	public class WriteSetupInfo : Task
	{
		[Required]
		public string OutputFile { get; set; }
		[Required]
		public string Title { get; set; }
		public string Manufacturer { get; set; }
		public string Description { get; set; }
		[Required]
		public string Version { get; set; }
		public string InformationalVersion { get; set; }
		[Required]
		public string Architecture { get; set; }
		[Required]
		public string MsiName { get; set; }
		public ITaskItem[] Frameworks { get; set; }

		static string Literal(string value) => "\"" + (value ?? string.Empty)
			.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\r", "\\r").Replace("\n", "\\n") + "\"";

		public override bool Execute()
		{
			var version = WindowsPackageHelper.ProductVersion(Version, Log) + ".0";
			var frameworks = (Frameworks ?? new ITaskItem[0]).Select(r => Literal(r.ItemSpec + "|" + r.GetMetadata("Version")));
			var source = new StringBuilder();
			source.AppendLine("using System.Reflection;");
			source.AppendLine($"[assembly: AssemblyTitle({Literal(Title + " Setup")})]");
			source.AppendLine($"[assembly: AssemblyProduct({Literal(Title)})]");
			source.AppendLine($"[assembly: AssemblyCompany({Literal(Manufacturer)})]");
			source.AppendLine($"[assembly: AssemblyDescription({Literal(Description)})]");
			source.AppendLine($"[assembly: AssemblyVersion({Literal(version)})]");
			source.AppendLine($"[assembly: AssemblyFileVersion({Literal(version)})]");
			source.AppendLine($"[assembly: AssemblyInformationalVersion({Literal(string.IsNullOrEmpty(InformationalVersion) ? Version : InformationalVersion)})]");
			source.AppendLine("namespace Eto.Setup");
			source.AppendLine("{");
			source.AppendLine("\tstatic class SetupInfo");
			source.AppendLine("\t{");
			source.AppendLine($"\t\tpublic const string Title = {Literal(Title)};");
			source.AppendLine($"\t\tpublic const string Architecture = {Literal(Architecture.ToLowerInvariant())};");
			source.AppendLine($"\t\tpublic const string MsiName = {Literal(MsiName)};");
			source.AppendLine($"\t\tpublic static readonly string[] Frameworks = {{ {string.Join(", ", frameworks)} }};");
			source.AppendLine("\t}");
			source.AppendLine("}");
			Directory.CreateDirectory(Path.GetDirectoryName(OutputFile));
			File.WriteAllText(OutputFile, source.ToString(), Encoding.UTF8);
			return !Log.HasLoggedErrors;
		}
	}

	/// <summary>
	/// Creates a Windows Installer package with msi.dll and makecab.exe, which come with Windows.
	/// </summary>
	public class WriteMsiPackage : Task
	{
		/// <summary>Files to install, with TargetPath relative to the install folder.</summary>
		[Required]
		public ITaskItem[] Files { get; set; }
		/// <summary>Path of the app's executable relative to the install folder; the shortcuts start it.</summary>
		[Required]
		public string MainExecutable { get; set; }
		[Required]
		public string ProductName { get; set; }
		[Required]
		public string Version { get; set; }
		public string Manufacturer { get; set; }
		public string Description { get; set; }
		public string Homepage { get; set; }
		/// <summary>Defaults to a GUID derived from AppId.</summary>
		public string UpgradeCode { get; set; }
		[Required]
		public string AppId { get; set; }
		public bool PerMachine { get; set; }
		/// <summary>Folder under Programs or Program Files, with \ for nested folders.</summary>
		[Required]
		public string InstallFolder { get; set; }
		/// <summary>x86, x64 or arm64.</summary>
		[Required]
		public string Architecture { get; set; }
		/// <summary>.ico for Installed apps; the executable's own icon is used without one.</summary>
		public string IconFile { get; set; }
		public bool DesktopShortcut { get; set; }
		/// <summary>Shared frameworks the app needs (from GetRuntimeFrameworks); installing stops when one is missing.</summary>
		public ITaskItem[] Frameworks { get; set; }
		[Required]
		public string OutputFile { get; set; }
		[Required]
		public string WorkDirectory { get; set; }

		[Output]
		public string ProductCode { get; set; }

		class TableDef
		{
			public string Name;
			public string Columns;
			public string Keys;
		}

		// the standard Windows Installer table schemas this package uses
		static readonly TableDef[] Tables =
		{
			new TableDef { Name = "Property", Columns = "`Property` CHAR(72) NOT NULL, `Value` LONGCHAR NOT NULL LOCALIZABLE", Keys = "`Property`" },
			new TableDef { Name = "Directory", Columns = "`Directory` CHAR(72) NOT NULL, `Directory_Parent` CHAR(72), `DefaultDir` CHAR(255) NOT NULL LOCALIZABLE", Keys = "`Directory`" },
			new TableDef { Name = "Component", Columns = "`Component` CHAR(72) NOT NULL, `ComponentId` CHAR(38), `Directory_` CHAR(72) NOT NULL, `Attributes` SHORT NOT NULL, `Condition` CHAR(255), `KeyPath` CHAR(72)", Keys = "`Component`" },
			new TableDef { Name = "Feature", Columns = "`Feature` CHAR(38) NOT NULL, `Feature_Parent` CHAR(38), `Title` CHAR(64) LOCALIZABLE, `Description` CHAR(255) LOCALIZABLE, `Display` SHORT, `Level` SHORT NOT NULL, `Directory_` CHAR(72), `Attributes` SHORT NOT NULL", Keys = "`Feature`" },
			new TableDef { Name = "FeatureComponents", Columns = "`Feature_` CHAR(38) NOT NULL, `Component_` CHAR(72) NOT NULL", Keys = "`Feature_`, `Component_`" },
			new TableDef { Name = "File", Columns = "`File` CHAR(72) NOT NULL, `Component_` CHAR(72) NOT NULL, `FileName` CHAR(255) NOT NULL LOCALIZABLE, `FileSize` LONG NOT NULL, `Version` CHAR(72), `Language` CHAR(20), `Attributes` SHORT, `Sequence` LONG NOT NULL", Keys = "`File`" },
			new TableDef { Name = "Media", Columns = "`DiskId` SHORT NOT NULL, `LastSequence` LONG NOT NULL, `DiskPrompt` CHAR(64) LOCALIZABLE, `Cabinet` CHAR(255), `VolumeLabel` CHAR(32), `Source` CHAR(72)", Keys = "`DiskId`" },
			new TableDef { Name = "Shortcut", Columns = "`Shortcut` CHAR(72) NOT NULL, `Directory_` CHAR(72) NOT NULL, `Name` CHAR(128) NOT NULL LOCALIZABLE, `Component_` CHAR(72) NOT NULL, `Target` CHAR(72) NOT NULL, `Arguments` CHAR(255), `Description` CHAR(255) LOCALIZABLE, `Hotkey` SHORT, `Icon_` CHAR(72), `IconIndex` SHORT, `ShowCmd` SHORT, `WkDir` CHAR(72), `DisplayResourceDLL` CHAR(255), `DisplayResourceId` SHORT, `DescriptionResourceDLL` CHAR(255), `DescriptionResourceId` SHORT", Keys = "`Shortcut`" },
			new TableDef { Name = "Icon", Columns = "`Name` CHAR(72) NOT NULL, `Data` OBJECT NOT NULL", Keys = "`Name`" },
			new TableDef { Name = "Upgrade", Columns = "`UpgradeCode` CHAR(38) NOT NULL, `VersionMin` CHAR(20), `VersionMax` CHAR(20), `Language` CHAR(255), `Attributes` LONG NOT NULL, `Remove` CHAR(255), `ActionProperty` CHAR(72) NOT NULL", Keys = "`UpgradeCode`, `VersionMin`, `VersionMax`, `Language`, `Attributes`" },
			new TableDef { Name = "LaunchCondition", Columns = "`Condition` CHAR(255) NOT NULL, `Description` CHAR(255) NOT NULL LOCALIZABLE", Keys = "`Condition`" },
			new TableDef { Name = "AppSearch", Columns = "`Property` CHAR(72) NOT NULL, `Signature_` CHAR(72) NOT NULL", Keys = "`Property`, `Signature_`" },
			new TableDef { Name = "RegLocator", Columns = "`Signature_` CHAR(72) NOT NULL, `Root` SHORT NOT NULL, `Key` CHAR(255) NOT NULL, `Name` CHAR(255), `Type` SHORT", Keys = "`Signature_`" },
			new TableDef { Name = "DrLocator",Columns = "`Signature_` CHAR(72) NOT NULL, `Parent` CHAR(72), `Path` CHAR(255), `Depth` SHORT", Keys = "`Signature_`, `Parent`, `Path`" },
			new TableDef { Name = "Signature", Columns = "`Signature` CHAR(72) NOT NULL, `FileName` CHAR(255) NOT NULL, `MinVersion` CHAR(20), `MaxVersion` CHAR(20), `MinSize` LONG, `MaxSize` LONG, `MinDate` LONG, `MaxDate` LONG, `Languages` CHAR(255)", Keys = "`Signature`" },
			new TableDef { Name = "CustomAction", Columns = "`Action` CHAR(72) NOT NULL, `Type` SHORT NOT NULL, `Source` CHAR(72), `Target` CHAR(255), `ExtendedType` LONG", Keys = "`Action`" },
			new TableDef { Name = "InstallExecuteSequence", Columns = "`Action` CHAR(72) NOT NULL, `Condition` CHAR(255), `Sequence` SHORT", Keys = "`Action`" },
			new TableDef { Name = "InstallUISequence", Columns = "`Action` CHAR(72) NOT NULL, `Condition` CHAR(255), `Sequence` SHORT", Keys = "`Action`" },
		};

		// the file each framework always has, which AppSearch looks for to tell whether it's installed
		static readonly Dictionary<string, string> FrameworkProbeFiles = new Dictionary<string, string>
		{
			{ "Microsoft.NETCore.App", "System.Private.CoreLib.dll" },
			{ "Microsoft.WindowsDesktop.App", "PresentationFramework.dll" },
			{ "Microsoft.AspNetCore.App", "Microsoft.AspNetCore.dll" },
		};

		static readonly Dictionary<string, string> FrameworkNames = new Dictionary<string, string>
		{
			{ "Microsoft.NETCore.App", ".NET Runtime" },
			{ "Microsoft.WindowsDesktop.App", ".NET Desktop Runtime" },
			{ "Microsoft.AspNetCore.App", "ASP.NET Core Runtime" },
		};

		class StreamValue
		{
			public string Path;
		}

		class InstallFile
		{
			public string Source;
			public string TargetPath;
			public string FileKey;
			public string ComponentKey;
			public string DirectoryKey;
		}

		readonly List<string> strings = new List<string>();
		readonly Dictionary<string, List<object[]>> rows = new Dictionary<string, List<object[]>>();
		readonly Dictionary<string, HashSet<string>> shortNames = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);

		void Add(string table, params object[] values)
		{
			if (!rows.TryGetValue(table, out var list))
				rows[table] = list = new List<object[]>();
			list.Add(values);
			strings.AddRange(values.OfType<string>());
		}

		public override bool Execute()
		{
			try
			{
				Write();
			}
			catch (Exception ex)
			{
				Log.LogError("Creating {0} failed: {1}", OutputFile, ex.Message);
			}
			if (Log.HasLoggedErrors && File.Exists(OutputFile))
				File.Delete(OutputFile);
			return !Log.HasLoggedErrors;
		}

		void Write()
		{
			var version = WindowsPackageHelper.ProductVersion(Version, Log);
			if (Log.HasLoggedErrors)
				return;
			var architecture = Architecture.ToLowerInvariant();
			var is64Bit = architecture != "x86";
			var platform = architecture == "x64" ? "x64" : architecture == "arm64" ? "Arm64" : "Intel";
			var upgradeCode = string.IsNullOrEmpty(UpgradeCode) ? WindowsPackageHelper.NameGuid("upgrade:" + AppId) : new Guid(UpgradeCode);
			var scope = PerMachine ? "perMachine" : "perUser";
			var productCode = Guid.NewGuid();
			ProductCode = WindowsPackageHelper.Format(productCode);
			var manufacturer = string.IsNullOrEmpty(Manufacturer) ? ProductName : Manufacturer;
			var summary = Regex.Replace((Description ?? string.Empty).Trim(), @"(?s)[\r\n].*", "");

			// directories
			Add("Directory", "TARGETDIR", null, "SourceDir");
			string parent;
			if (PerMachine)
			{
				parent = is64Bit ? "ProgramFiles64Folder" : "ProgramFilesFolder";
				Add("Directory", parent, "TARGETDIR", is64Bit ? "PFiles64" : "PFiles");
			}
			else
			{
				// where per-user apps go, like %ProgramFiles% is for everyone
				Add("Directory", "LocalAppDataFolder", "TARGETDIR", "LocalApp");
				Add("Directory", "UserProgramsFolder", "LocalAppDataFolder", "Programs");
				parent = "UserProgramsFolder";
			}
			var folders = InstallFolder.Split(new[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries);
			if (folders.Length == 0)
				folders = new[] { ProductName };
			for (int i = 0; i < folders.Length; i++)
			{
				var key = i == folders.Length - 1 ? "INSTALLFOLDER" : "InstallParent" + i;
				Add("Directory", key, parent, FileNameColumn(parent, folders[i]));
				parent = key;
			}
			Add("Directory", "ProgramMenuFolder", "TARGETDIR", "PMenu");
			if (DesktopShortcut)
				Add("Directory", "DesktopFolder", "TARGETDIR", "Desktop");

			// files, with one component each so each can be tracked on its own
			var files = new Dictionary<string, InstallFile>(StringComparer.OrdinalIgnoreCase);
			foreach (var item in Files)
			{
				var target = item.GetMetadata("TargetPath");
				if (string.IsNullOrEmpty(target))
					target = Path.GetFileName(item.ItemSpec);
				target = target.Replace('/', '\\').TrimStart('\\');
				// later items replace earlier ones, so extra content can override what was published
				files[target] = new InstallFile { Source = item.ItemSpec, TargetPath = target };
			}
			var mainExecutable = MainExecutable.Replace('/', '\\');
			if (!files.ContainsKey(mainExecutable))
			{
				Log.LogError("The app's executable {0} wasn't published, so there's nothing for the shortcuts to start. Windows packages need UseAppHost to be on.", mainExecutable);
				return;
			}

			var directories = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { { string.Empty, "INSTALLFOLDER" } };
			string GetDirectory(string relative)
			{
				if (directories.TryGetValue(relative, out var key))
					return key;
				var parentKey = GetDirectory(Path.GetDirectoryName(relative) ?? string.Empty);
				key = "dir" + directories.Count;
				Add("Directory", key, parentKey, FileNameColumn(parentKey, Path.GetFileName(relative)));
				directories[relative] = key;
				return key;
			}

			var componentAttributes = is64Bit ? 256 : 0; // msidbComponentAttributes64bit
			var sorted = files.Values.OrderBy(r => r.TargetPath, StringComparer.OrdinalIgnoreCase).ToList();
			for (int i = 0; i < sorted.Count; i++)
			{
				var file = sorted[i];
				var sequence = i + 1;
				file.FileKey = string.Equals(file.TargetPath, mainExecutable, StringComparison.OrdinalIgnoreCase) ? "MainExecutable" : "file" + sequence;
				file.ComponentKey = "cmp" + sequence;
				file.DirectoryKey = GetDirectory(Path.GetDirectoryName(file.TargetPath) ?? string.Empty);
				// stable across builds, so Windows Installer knows it's the same file in each version
				var componentId = WindowsPackageHelper.NameGuid($"component:{upgradeCode}:{scope}:{architecture}:{file.TargetPath.ToLowerInvariant()}");
				var info = new FileInfo(file.Source);
				if (info.Length > int.MaxValue)
					throw new InvalidOperationException($"{file.Source} is too large for Windows Installer.");
				Add("Component", file.ComponentKey, WindowsPackageHelper.Format(componentId), file.DirectoryKey, componentAttributes, null, file.FileKey);
				Add("FeatureComponents", "ProductFeature", file.ComponentKey);
				Add("File", file.FileKey, file.ComponentKey, FileNameColumn(file.DirectoryKey, Path.GetFileName(file.TargetPath)), (int)info.Length, GetFileVersion(file.Source), null, 512 /* vital */, sequence);
			}
			Add("Feature", "ProductFeature", null, ProductName, null, 0, 1, "INSTALLFOLDER", 0);
			Add("Media", 1, sorted.Count, null, "#app.cab", null, null);

			// shortcuts to the app, with its own icon
			var main = sorted.First(r => r.FileKey == "MainExecutable");
			Add("Shortcut", "StartMenuShortcut", "ProgramMenuFolder", FileNameColumn("ProgramMenuFolder", ProductName), main.ComponentKey, "[#MainExecutable]", null, string.IsNullOrEmpty(summary) ? null : summary, null, null, null, null, main.DirectoryKey, null, null, null, null);
			if (DesktopShortcut)
				Add("Shortcut", "DesktopShortcut", "DesktopFolder", FileNameColumn("DesktopFolder", ProductName), main.ComponentKey, "[#MainExecutable]", null, string.IsNullOrEmpty(summary) ? null : summary, null, null, null, null, main.DirectoryKey, null, null, null, null);

			var iconFile = !string.IsNullOrEmpty(IconFile) && File.Exists(IconFile) ? IconFile : main.Source;
			var iconName = "AppIcon" + Path.GetExtension(iconFile).ToLowerInvariant();
			Add("Icon", iconName, new StreamValue { Path = iconFile });

			// properties
			Add("Property", "ProductCode", ProductCode);
			Add("Property", "ProductName", ProductName);
			Add("Property", "ProductVersion", version);
			Add("Property", "Manufacturer", manufacturer);
			Add("Property", "ProductLanguage", "1033");
			Add("Property", "UpgradeCode", WindowsPackageHelper.Format(upgradeCode));
			Add("Property", "ARPPRODUCTICON", iconName);
			// there's no UI to change anything with
			Add("Property", "ARPNOMODIFY", "1");
			if (!string.IsNullOrEmpty(Homepage))
				Add("Property", "ARPURLINFOABOUT", Homepage);
			if (!string.IsNullOrEmpty(summary))
				Add("Property", "ARPCOMMENTS", summary);
			if (PerMachine)
				Add("Property", "ALLUSERS", "1");

			// a newer build replaces any older one (or another build of this version), but not a newer one
			Add("Upgrade", WindowsPackageHelper.Format(upgradeCode), null, version, null, 512 /* VersionMaxInclusive */, null, "OLDERVERSIONFOUND");
			Add("Upgrade", WindowsPackageHelper.Format(upgradeCode), version, null, null, 2 /* OnlyDetect */, null, "NEWERVERSIONFOUND");
			Add("LaunchCondition", "Installed OR NOT NEWERVERSIONFOUND", "A newer version of [ProductName] is already installed.");
			var secureProperties = new List<string> { "OLDERVERSIONFOUND", "NEWERVERSIONFOUND" };

			// stop when a runtime the app needs is missing, instead of installing an app that can't start
			var frameworkIndex = 0;
			foreach (var framework in Frameworks ?? new ITaskItem[0])
			{
				if (!FrameworkProbeFiles.TryGetValue(framework.ItemSpec, out var probeFile))
				{
					Log.LogMessage(MessageImportance.Normal, "Not checking for {0} when installing, as it can't be detected.", framework.ItemSpec);
					continue;
				}
				var frameworkVersion = System.Version.TryParse(Regex.Match(framework.GetMetadata("Version"), @"^\d+(\.\d+)*").Value, out var v) ? v : new System.Version(0, 0);
				var property = "DOTNETRUNTIME" + ++frameworkIndex;
				var programFiles = is64Bit ? "[ProgramFiles64Folder]" : "[ProgramFilesFolder]";
				// file versions start with the runtime's major.minor, and apps roll forward to any newer minor version
				void Search(string searchProperty, string path)
				{
					var signature = "Sig" + searchProperty;
					secureProperties.Add(searchProperty);
					Add("AppSearch", searchProperty, signature);
					Add("Signature", signature, FileNameColumn(signature, probeFile), $"{frameworkVersion.Major}.{Math.Max(0, frameworkVersion.Minor)}.0.0", $"{frameworkVersion.Major}.65535.65535.65535", null, null, null, null, null);
					Add("DrLocator", signature, null, path, 1);
				}
				Search(property, $"{programFiles}dotnet\\shared\\{framework.ItemSpec}");
				var condition = $"Installed OR {property}";
				if (architecture == "x64")
				{
					// on Arm64 Windows that folder has the Arm64 runtime, and x64 runtimes go in their own folder
					Search(property + "X64", $"{programFiles}dotnet\\x64\\shared\\{framework.ItemSpec}");
					if (!rows.ContainsKey("RegLocator"))
					{
						Add("AppSearch", "NATIVEARCHITECTURE", "SigNativeArchitecture");
						Add("RegLocator", "SigNativeArchitecture", 2 /* HKLM */, @"SYSTEM\CurrentControlSet\Control\Session Manager\Environment", "PROCESSOR_ARCHITECTURE", 2 | 16 /* raw value, 64 bit */);
						secureProperties.Add("NATIVEARCHITECTURE");
					}
					condition = $"Installed OR {property}X64 OR ({property} AND NOT (NATIVEARCHITECTURE ~= \"ARM64\"))";
				}
				var name = FrameworkNames.TryGetValue(framework.ItemSpec, out var n) ? n : framework.ItemSpec;
				Add("LaunchCondition", condition,
					EscapeFormatted($"{ProductName} needs the {name} {frameworkVersion.Major}.{Math.Max(0, frameworkVersion.Minor)} ({architecture}). Install it from https://dotnet.microsoft.com/download/dotnet/{frameworkVersion.Major}.{Math.Max(0, frameworkVersion.Minor)} and run this installer again."));
			}
			Add("Property", "SecureCustomProperties", string.Join(";", secureProperties));

			// show where it's installed in Installed apps
			Add("CustomAction", "SetARPINSTALLLOCATION", 51, "ARPINSTALLLOCATION", "[INSTALLFOLDER]", null);

			void Execute(string action, int sequence, string condition = null) => Add("InstallExecuteSequence", action, condition, sequence);
			Execute("FindRelatedProducts", 25);
			Execute("AppSearch", 50);
			Execute("LaunchConditions", 100);
			Execute("ValidateProductID", 700);
			Execute("CostInitialize", 800);
			Execute("FileCost", 900);
			Execute("CostFinalize", 1000);
			Execute("SetARPINSTALLLOCATION", 1001, "NOT Installed");
			Execute("MigrateFeatureStates", 1200);
			Execute("InstallValidate", 1400);
			// removing the old version completely first means nothing depends on what changed between them
			Execute("RemoveExistingProducts", 1401);
			Execute("InstallInitialize", 1500);
			Execute("ProcessComponents", 1600);
			Execute("UnpublishFeatures", 1800);
			Execute("RemoveShortcuts", 3200);
			Execute("RemoveFiles", 3500);
			Execute("RemoveFolders", 3600);
			Execute("CreateFolders", 3700);
			Execute("InstallFiles", 4000);
			Execute("CreateShortcuts", 4500);
			Execute("RegisterUser", 6000);
			Execute("RegisterProduct", 6100);
			Execute("PublishFeatures", 6300);
			Execute("PublishProduct", 6400);
			Execute("InstallFinalize", 6600);

			void UI(string action, int sequence) => Add("InstallUISequence", action, null, sequence);
			UI("FindRelatedProducts", 25);
			UI("AppSearch", 50);
			UI("LaunchConditions", 100);
			UI("ValidateProductID", 700);
			UI("CostInitialize", 800);
			UI("FileCost", 900);
			UI("CostFinalize", 1000);
			UI("MigrateFeatureStates", 1200);
			UI("ExecuteAction", 1300);

			Directory.CreateDirectory(WorkDirectory);
			var cabinet = CreateCabinet(sorted);
			if (cabinet == null)
				return;

			Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(OutputFile)));
			if (File.Exists(OutputFile))
				File.Delete(OutputFile);

			var codepage = ChooseCodepage();
			using (var db = Database.Create(OutputFile))
			{
				db.SetCodepage(codepage, WorkDirectory);
				foreach (var table in Tables)
				{
					db.Execute($"CREATE TABLE `{table.Name}` ({table.Columns} PRIMARY KEY {table.Keys})");
					if (!rows.TryGetValue(table.Name, out var tableRows))
						continue;
					var columns = Regex.Matches(table.Columns, @"`(\w+)` [A-Z]").Cast<Match>().Select(r => "`" + r.Groups[1].Value + "`").ToList();
					var query = $"INSERT INTO `{table.Name}` ({string.Join(", ", columns)}) VALUES ({string.Join(", ", columns.Select(r => "?"))})";
					foreach (var row in tableRows)
					{
						if (row.Length != columns.Count)
							throw new InvalidOperationException($"{table.Name} row has {row.Length} values instead of {columns.Count}.");
						db.Execute(query, row);
					}
				}
				db.Execute("INSERT INTO `_Streams` (`Name`, `Data`) VALUES (?, ?)", "app.cab", new StreamValue { Path = cabinet });

				var now = DateTime.Now;
				db.SetSummary(new Dictionary<int, object>
				{
					{ 1, (short)codepage },
					{ 2, "Installation Database" },
					{ 3, ProductName },
					{ 4, manufacturer },
					{ 5, "Installer" },
					{ 6, string.IsNullOrEmpty(summary) ? $"This installer database contains the logic and data required to install {ProductName}." : summary },
					{ 7, platform + ";1033" },
					{ 9, WindowsPackageHelper.Format(Guid.NewGuid()) },
					{ 12, now },
					{ 13, now },
					// Windows Installer 5, which Arm64 packages need
					{ 14, 500 },
					// compressed with long file names, and per-user packages don't need admin rights
					{ 15, PerMachine ? 2 : 2 | 8 },
					{ 18, "Eto.Forms" },
					{ 19, 2 },
				});
				db.Commit();
			}
		}

		string CreateCabinet(List<InstallFile> files)
		{
			var cabinet = Path.Combine(WorkDirectory, "app.cab");
			var ddf = Path.Combine(WorkDirectory, "app.ddf");
			if (File.Exists(cabinet))
				File.Delete(cabinet);
			var lines = new List<string>
			{
				".OPTION EXPLICIT",
				".Set CabinetNameTemplate=app.cab",
				$".Set DiskDirectoryTemplate=\"{WorkDirectory.TrimEnd('\\')}\"",
				".Set Cabinet=on",
				".Set Compress=on",
				".Set CompressionType=LZX",
				".Set CompressionMemory=21",
				// no limits, so everything goes in a single cabinet
				".Set MaxDiskSize=0",
				".Set MaxCabinetSize=0",
				".Set CabinetFileCountThreshold=0",
				".Set FolderFileCountThreshold=0",
				".Set FolderSizeThreshold=0",
				".Set RptFileName=NUL",
				".Set InfFileName=NUL",
			};
			// files are stored by their File key, in sequence order
			lines.AddRange(files.Select(r => $"\"{Path.GetFullPath(r.Source)}\" {r.FileKey}"));
			File.WriteAllLines(ddf, lines);

			var output = new StringBuilder();
			var start = new ProcessStartInfo(Path.Combine(Environment.SystemDirectory, "makecab.exe"), $"/F \"{ddf}\"")
			{
				UseShellExecute = false,
				CreateNoWindow = true,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				WorkingDirectory = WorkDirectory,
			};
			using (var process = new Process { StartInfo = start })
			{
				process.OutputDataReceived += (s, e) => { if (e.Data != null) lock (output) output.AppendLine(e.Data); };
				process.ErrorDataReceived += (s, e) => { if (e.Data != null) lock (output) output.AppendLine(e.Data); };
				process.Start();
				process.BeginOutputReadLine();
				process.BeginErrorReadLine();
				process.WaitForExit();
				if (process.ExitCode != 0 || !File.Exists(cabinet))
				{
					Log.LogError("makecab failed to compress the files: {0}", output.ToString().Trim());
					return null;
				}
			}
			return cabinet;
		}

		static string GetFileVersion(string path)
		{
			var info = FileVersionInfo.GetVersionInfo(path);
			if (info.FileMajorPart == 0 && info.FileMinorPart == 0 && info.FileBuildPart == 0 && info.FilePrivatePart == 0)
				return null;
			return $"{info.FileMajorPart}.{info.FileMinorPart}.{info.FileBuildPart}.{info.FilePrivatePart}";
		}

		// brackets have a meaning in formatted text, so show them as they are
		static string EscapeFormatted(string text) => Regex.Replace(text, @"[\[\]]", m => m.Value == "[" ? "[\\[]" : "[\\]]");

		/// <summary>Names are given as short|long, as Windows Installer needs an 8.3 name for each.</summary>
		string FileNameColumn(string directoryKey, string name)
		{
			if (!shortNames.TryGetValue(directoryKey, out var used))
				shortNames[directoryKey] = used = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			if (Regex.IsMatch(name, @"^[A-Za-z0-9_\-]{1,8}(\.[A-Za-z0-9_\-]{1,3})?$") && used.Add(name))
				return name;
			var extension = Regex.Replace(Path.GetExtension(name).TrimStart('.'), @"[^A-Za-z0-9_\-]", "");
			if (extension.Length > 3)
				extension = extension.Substring(0, 3);
			if (extension.Length > 0)
				extension = "." + extension.ToUpperInvariant();
			string hash;
			using (var sha = SHA1.Create())
				hash = BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(name.ToLowerInvariant()))).Replace("-", "");
			for (int i = 1; ; i++)
			{
				var suffix = "~" + i;
				var shortName = hash.Substring(0, 8 - suffix.Length) + suffix + extension;
				if (used.Add(shortName))
					return shortName + "|" + name;
			}
		}

		int ChooseCodepage()
		{
			// strings are stored in the database's codepage, so pick one that has every character the package uses
			try
			{
				var provider = Type.GetType("System.Text.CodePagesEncodingProvider, System.Text.Encoding.CodePages")?.GetProperty("Instance")?.GetValue(null) as EncodingProvider;
				if (provider != null)
					Encoding.RegisterProvider(provider);
			}
			catch
			{
			}
			var text = string.Concat(strings.Where(r => r != null));
			foreach (var codepage in new[] { 1252, 1250, 1251, 1253, 1254, 1255, 1256, 1257, 1258, 874, 932, 936, 949, 950 })
			{
				try
				{
					var encoding = Encoding.GetEncoding(codepage, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback);
					encoding.GetBytes(text);
					return codepage;
				}
				catch
				{
				}
			}
			Log.LogWarning("The product name, file names or other text use characters that no single Windows codepage has, so some may not show correctly.");
			return 1252;
		}

		class Database : IDisposable
		{
			uint handle;

			public static Database Create(string path)
			{
				Check(MsiOpenDatabaseW(path, (IntPtr)3 /* MSIDBOPEN_CREATE */, out var handle), "creating the database");
				return new Database { handle = handle };
			}

			public void SetCodepage(int codepage, string folder)
			{
				var file = "codepage.idt";
				File.WriteAllText(Path.Combine(folder, file), $"\r\n\r\n{codepage}\t_ForceCodepage\r\n");
				Check(MsiDatabaseImportW(handle, folder, file), "setting the codepage");
			}

			public void Execute(string query, params object[] values)
			{
				Check(MsiDatabaseOpenViewW(handle, query, out var view), query);
				uint record = 0;
				try
				{
					if (values.Length > 0)
					{
						record = MsiCreateRecord((uint)values.Length);
						for (uint i = 0; i < values.Length; i++)
						{
							switch (values[i])
							{
								case null:
									break;
								case string s:
									Check(MsiRecordSetStringW(record, i + 1, s), query);
									break;
								case int n:
									Check(MsiRecordSetInteger(record, i + 1, n), query);
									break;
								case StreamValue stream:
									Check(MsiRecordSetStreamW(record, i + 1, stream.Path), query);
									break;
								default:
									throw new ArgumentException($"Unsupported value {values[i]}");
							}
						}
					}
					Check(MsiViewExecute(view, record), $"{query} ({string.Join(", ", values)})");
				}
				finally
				{
					if (record != 0)
						MsiCloseHandle(record);
					MsiViewClose(view);
					MsiCloseHandle(view);
				}
			}

			public void SetSummary(Dictionary<int, object> properties)
			{
				Check(MsiGetSummaryInformationW(handle, null, (uint)properties.Count, out var summary), "opening the summary information");
				try
				{
					foreach (var property in properties)
					{
						var time = new FILETIME();
						switch (property.Value)
						{
							case short n:
								Check(MsiSummaryInfoSetPropertyW(summary, (uint)property.Key, 2 /* VT_I2 */, n, ref time, null), "setting the summary information");
								break;
							case int n:
								Check(MsiSummaryInfoSetPropertyW(summary, (uint)property.Key, 3 /* VT_I4 */, n, ref time, null), "setting the summary information");
								break;
							case DateTime date:
								var value = date.ToFileTime();
								time.Low = (uint)value;
								time.High = (uint)(value >> 32);
								Check(MsiSummaryInfoSetPropertyW(summary, (uint)property.Key, 64 /* VT_FILETIME */, 0, ref time, null), "setting the summary information");
								break;
							case string s:
								Check(MsiSummaryInfoSetPropertyW(summary, (uint)property.Key, 30 /* VT_LPSTR */, 0, ref time, s), "setting the summary information");
								break;
						}
					}
					Check(MsiSummaryInfoPersist(summary), "saving the summary information");
				}
				finally
				{
					MsiCloseHandle(summary);
				}
			}

			public void Commit() => Check(MsiDatabaseCommit(handle), "saving the database");

			public void Dispose()
			{
				if (handle != 0)
					MsiCloseHandle(handle);
				handle = 0;
			}

			static void Check(uint result, string context)
			{
				if (result == 0)
					return;
				var message = $"error {result}";
				var record = MsiGetLastErrorRecord();
				if (record != 0)
				{
					uint size = 1024;
					var buffer = new StringBuilder((int)size);
					if (MsiFormatRecordW(0, record, buffer, ref size) == 0)
						message += ": " + buffer;
					MsiCloseHandle(record);
				}
				throw new InvalidOperationException($"Windows Installer reported {message} while {context}");
			}

			[StructLayout(LayoutKind.Sequential)]
			struct FILETIME
			{
				public uint Low;
				public uint High;
			}

			// MSIHANDLE is 32 bits even in 64 bit processes
			[DllImport("msi.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
			static extern uint MsiOpenDatabaseW(string szDatabasePath, IntPtr szPersist, out uint phDatabase);
			[DllImport("msi.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
			static extern uint MsiDatabaseOpenViewW(uint hDatabase, string szQuery, out uint phView);
			[DllImport("msi.dll", ExactSpelling = true)]
			static extern uint MsiViewExecute(uint hView, uint hRecord);
			[DllImport("msi.dll", ExactSpelling = true)]
			static extern uint MsiViewClose(uint hView);
			[DllImport("msi.dll", ExactSpelling = true)]
			static extern uint MsiCloseHandle(uint hAny);
			[DllImport("msi.dll", ExactSpelling = true)]
			static extern uint MsiCreateRecord(uint cParams);
			[DllImport("msi.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
			static extern uint MsiRecordSetStringW(uint hRecord, uint iField, string szValue);
			[DllImport("msi.dll", ExactSpelling = true)]
			static extern uint MsiRecordSetInteger(uint hRecord, uint iField, int iValue);
			[DllImport("msi.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
			static extern uint MsiRecordSetStreamW(uint hRecord, uint iField, string szFilePath);
			[DllImport("msi.dll", ExactSpelling = true)]
			static extern uint MsiDatabaseCommit(uint hDatabase);
			[DllImport("msi.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
			static extern uint MsiDatabaseImportW(uint hDatabase, string szFolderPath, string szFileName);
			[DllImport("msi.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
			static extern uint MsiGetSummaryInformationW(uint hDatabase, string szDatabasePath, uint uiUpdateCount, out uint phSummaryInfo);
			[DllImport("msi.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
			static extern uint MsiSummaryInfoSetPropertyW(uint hSummaryInfo, uint uiProperty, uint uiDataType, int iValue, ref FILETIME pftValue, string szValue);
			[DllImport("msi.dll", ExactSpelling = true)]
			static extern uint MsiSummaryInfoPersist(uint hSummaryInfo);
			[DllImport("msi.dll", ExactSpelling = true)]
			static extern uint MsiGetLastErrorRecord();
			[DllImport("msi.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
			static extern uint MsiFormatRecordW(uint hInstall, uint hRecord, StringBuilder szResultBuf, ref uint pcchResultBuf);
		}
	}
}
