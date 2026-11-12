using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;
using Microsoft.Win32;

// Built for each app by WindowsPackage.targets with the C# 5 compiler that comes with .NET Framework,
// so it runs on any Windows without installing anything first. Keep to C# 5 features.
namespace Eto.Setup
{
	/// <summary>
	/// Installs the .NET runtimes the app needs when they're missing, then runs the app's msi embedded in this exe.
	/// Arguments are passed on to msiexec, e.g. /quiet or INSTALLFOLDER="C:\Apps\MyApp".
	/// </summary>
	static class Program
	{
		const int ErrorCancelled = 1602;
		const int ErrorFailed = 1603;

		internal static string Caption { get { return T("SetupTitle"); } }

		static Dictionary<string, string> strings;

		/// <summary>Text in Windows' language, or the msi's own when it doesn't have that one.</summary>
		internal static string T(string id)
		{
			if (strings == null)
			{
				strings = new Dictionary<string, string>();
				var language = FindLanguage(CultureInfo.CurrentUICulture) ?? SetupInfo.Languages[0];
				for (int i = 1; i + 1 < language.Length; i += 2)
					strings[language[i]] = language[i + 1];
			}
			string value;
			if (!strings.TryGetValue(id, out value))
				value = id;
			return value.Replace("[ProductName]", SetupInfo.Title);
		}

		static string[] FindLanguage(CultureInfo ui)
		{
			// the culture itself, then its parents, so de-AT uses de-DE and zh-HK uses zh-TW
			for (var culture = ui; !string.IsNullOrEmpty(culture.Name); culture = culture.Parent)
			{
				foreach (var language in SetupInfo.Languages)
				{
					CultureInfo candidate;
					try
					{
						candidate = new CultureInfo(language[0]);
					}
					catch (ArgumentException)
					{
						continue;
					}
					for (; !string.IsNullOrEmpty(candidate.Name); candidate = candidate.Parent)
					{
						if (string.Equals(candidate.Name, culture.Name, StringComparison.OrdinalIgnoreCase))
							return language;
					}
				}
			}
			return null;
		}

		[STAThread]
		static int Main(string[] args)
		{
			bool quiet = false, passive = false;
			foreach (var arg in args)
			{
				var option = arg.TrimStart('/', '-').ToLowerInvariant();
				if (option == "quiet" || option == "q" || option.StartsWith("qn"))
					quiet = true;
				else if (option == "passive" || option.StartsWith("qb") || option.StartsWith("qr"))
					passive = true;
			}

			Application.EnableVisualStyles();
			try
			{
				var missing = GetMissingFrameworks();
				if (missing.Count > 0 && !InstallFrameworks(missing, quiet, passive))
					return ErrorCancelled;
				return RunMsi();
			}
			catch (Exception ex)
			{
				if (!quiet)
					MessageBox.Show(ex.Message, Caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return ErrorFailed;
			}
		}

		static List<Framework> GetMissingFrameworks()
		{
			var root = DotNetRoot(SetupInfo.Architecture);
			var missing = new List<Framework>();
			foreach (var entry in SetupInfo.Frameworks)
			{
				var parts = entry.Split('|');
				var framework = new Framework { Name = parts[0], Version = ParseVersion(parts[1]) ?? new Version(0, 0) };
				if (!IsInstalled(root, framework))
					missing.Add(framework);
			}
			return missing;
		}

		static bool InstallFrameworks(List<Framework> missing, bool quiet, bool passive)
		{
			var arch = SetupInfo.Architecture;
			if (!quiet && !passive)
			{
				var names = string.Empty;
				foreach (var framework in missing)
					names += "\n" + framework.GetDisplayName(arch);
				var message = T("SetupRuntimeDownload") + "\n" + names;
				if (MessageBox.Show(message, Caption, MessageBoxButtons.OKCancel, MessageBoxIcon.Information) != DialogResult.OK)
					return false;
			}

			// TLS 1.2 isn't on by default for .NET Framework on older Windows
			ServicePointManager.SecurityProtocol |= (SecurityProtocolType)3072;
			foreach (var framework in missing)
			{
				var url = framework.GetDownloadUrl(arch);
				if (url == null)
					throw new InvalidOperationException(string.Format(T("SetupRuntimeUnsupported"), framework.GetDisplayName(arch)));

				var file = Path.Combine(Path.GetTempPath(), "EtoSetup-" + Guid.NewGuid().ToString("N") + ".exe");
				try
				{
					if (quiet)
					{
						using (var client = new WebClient())
							client.DownloadFile(url, file);
					}
					else if (!DownloadForm.Download(url, file, string.Format(T("SetupDownloading"), framework.GetDisplayName(arch))))
						return false;

					VerifyMicrosoftSignature(file, framework.GetDisplayName(arch));

					var start = new ProcessStartInfo(file, quiet ? "/install /quiet /norestart" : "/install /passive /norestart") { UseShellExecute = true };
					Process process;
					try
					{
						process = Process.Start(start);
					}
					catch (Win32Exception ex)
					{
						// the user said no to installing it as an administrator
						if (ex.NativeErrorCode == 1223)
							return false;
						throw;
					}
					using (process)
					{
						process.WaitForExit();
						var code = process.ExitCode;
						if (code == ErrorCancelled)
							return false;
						// 3010 and 1641 ask for a restart, which can wait until the app is installed too
						if (code != 0 && code != 3010 && code != 1641 && code != 1638)
							throw new InvalidOperationException(string.Format(T("SetupRuntimeFailed"), framework.GetDisplayName(arch), code));
					}
				}
				finally
				{
					try
					{
						File.Delete(file);
					}
					catch
					{
					}
				}
			}

			missing = GetMissingFrameworks();
			if (missing.Count > 0)
				throw new InvalidOperationException(string.Format(T("SetupRuntimeNotFound"), missing[0].GetDisplayName(arch)));
			return true;
		}

		static int RunMsi()
		{
			var folder = Path.Combine(Path.GetTempPath(), "EtoSetup-" + Guid.NewGuid().ToString("N"));
			Directory.CreateDirectory(folder);
			try
			{
				var msi = Path.Combine(folder, SetupInfo.MsiName);
				using (var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream("Setup.msi"))
				using (var file = File.Create(msi))
					resource.CopyTo(file);

				// Windows Installer applies the msi's transform for Windows' language itself, and fails if it's also given
				var start = new ProcessStartInfo(Path.Combine(Environment.SystemDirectory, "msiexec.exe"), "/i \"" + msi + "\" " + GetArguments()) { UseShellExecute = false };
				using (var process = Process.Start(start))
				{
					process.WaitForExit();
					return process.ExitCode;
				}
			}
			finally
			{
				try
				{
					Directory.Delete(folder, true);
				}
				catch
				{
				}
			}
		}

		// the arguments exactly as given, so quoted property values reach msiexec unchanged
		static string GetArguments()
		{
			var line = Environment.CommandLine.TrimStart();
			var end = line.StartsWith("\"") ? line.IndexOf('"', 1) + 1 : line.IndexOf(' ');
			if (end <= 0 || end >= line.Length)
				return string.Empty;
			return line.Substring(end).Trim();
		}

		static bool IsInstalled(string root, Framework framework)
		{
			var folder = Path.Combine(Path.Combine(root, "shared"), framework.Name);
			if (!Directory.Exists(folder))
				return false;
			foreach (var versionFolder in Directory.GetDirectories(folder))
			{
				// apps roll forward to newer minor and patch versions of the same major version
				var version = ParseVersion(Path.GetFileName(versionFolder));
				if (version != null && version.Major == framework.Version.Major && version >= framework.Version)
					return true;
			}
			return false;
		}

		static Version ParseVersion(string text)
		{
			var dash = text.IndexOf('-');
			if (dash >= 0)
				text = text.Substring(0, dash);
			Version version;
			return Version.TryParse(text, out version) ? version : null;
		}

		static string DotNetRoot(string arch)
		{
			// where the .NET installer put this architecture's runtimes
			foreach (var view in new[] { RegistryView.Registry32, RegistryView.Registry64 })
			{
				using (var hive = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, view))
				using (var key = hive.OpenSubKey(@"SOFTWARE\dotnet\Setup\InstalledVersions\" + arch))
				{
					var location = key != null ? key.GetValue("InstallLocation") as string : null;
					if (!string.IsNullOrEmpty(location))
						return location;
				}
			}

			var programFiles = Environment.GetEnvironmentVariable("ProgramW6432");
			if (string.IsNullOrEmpty(programFiles))
				programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
			var native = NativeArchitecture();
			if (arch == "x86" && native != "x86")
				return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "dotnet");
			if (arch == "x64" && native == "arm64")
				return Path.Combine(programFiles, @"dotnet\x64");
			return Path.Combine(programFiles, "dotnet");
		}

		static string NativeArchitecture()
		{
			try
			{
				ushort process, machine;
				if (IsWow64Process2(GetCurrentProcess(), out process, out machine))
				{
					switch (machine)
					{
						case 0xAA64:
							return "arm64";
						case 0x8664:
							return "x64";
						case 0x14C:
							return "x86";
					}
				}
			}
			catch (EntryPointNotFoundException)
			{
				// before Windows 10 1709, which can't be Arm64
			}
			var arch = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432");
			if (string.IsNullOrEmpty(arch))
				arch = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");
			return string.Equals(arch, "AMD64", StringComparison.OrdinalIgnoreCase) ? "x64" : string.Equals(arch, "ARM64", StringComparison.OrdinalIgnoreCase) ? "arm64" : "x86";
		}

		static void VerifyMicrosoftSignature(string file, string name)
		{
			var error = WinTrust.Verify(file);
			if (error != 0)
				throw new InvalidOperationException(string.Format(T("SetupNotSigned"), name, "0x" + error.ToString("X8")));
			var certificate = X509Certificate.CreateFromSignedFile(file);
			if (certificate.Subject.IndexOf("O=Microsoft Corporation", StringComparison.OrdinalIgnoreCase) < 0)
				throw new InvalidOperationException(string.Format(T("SetupNotMicrosoft"), name));
		}

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool IsWow64Process2(IntPtr process, out ushort processMachine, out ushort nativeMachine);

		[DllImport("kernel32.dll")]
		static extern IntPtr GetCurrentProcess();
	}

	class Framework
	{
		public string Name;
		public Version Version;

		public string GetDisplayName(string arch)
		{
			string name;
			switch (Name)
			{
				case "Microsoft.WindowsDesktop.App":
					name = ".NET Desktop Runtime";
					break;
				case "Microsoft.AspNetCore.App":
					name = "ASP.NET Core Runtime";
					break;
				case "Microsoft.NETCore.App":
					name = ".NET Runtime";
					break;
				default:
					name = Name;
					break;
			}
			return string.Format("{0} {1}.{2} ({3})", name, Version.Major, Math.Max(0, Version.Minor), arch);
		}

		/// <summary>Microsoft's link to the latest patch of this version's installer.</summary>
		public string GetDownloadUrl(string arch)
		{
			string product;
			switch (Name)
			{
				case "Microsoft.WindowsDesktop.App":
					product = "windowsdesktop-runtime";
					break;
				case "Microsoft.AspNetCore.App":
					product = "aspnetcore-runtime";
					break;
				case "Microsoft.NETCore.App":
					product = "dotnet-runtime";
					break;
				default:
					return null;
			}
			return string.Format("https://aka.ms/dotnet/{0}.{1}/{2}-win-{3}.exe", Version.Major, Math.Max(0, Version.Minor), product, arch);
		}
	}

	class DownloadForm : Form
	{
		readonly WebClient client = new WebClient();
		readonly ProgressBar progress;
		Exception error;
		bool completed;

		/// <summary>Downloads with a progress bar, returning false when cancelled.</summary>
		public static bool Download(string url, string file, string text)
		{
			using (var form = new DownloadForm(text))
			{
				form.Shown += delegate { form.client.DownloadFileAsync(new Uri(url), file); };
				form.ShowDialog();
				if (form.error != null)
					throw new InvalidOperationException(string.Format(Program.T("SetupDownloadFailed"), url, form.error.Message), form.error);
				return form.completed;
			}
		}

		DownloadForm(string text)
		{
			Text = Program.Caption;
			FormBorderStyle = FormBorderStyle.FixedDialog;
			MaximizeBox = false;
			MinimizeBox = false;
			StartPosition = FormStartPosition.CenterScreen;
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(400, 110);
			try
			{
				Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
			}
			catch
			{
			}

			var label = new Label { Text = text, Location = new Point(12, 12), AutoSize = true };
			progress = new ProgressBar { Location = new Point(12, 40), Size = new Size(376, 20) };
			var cancel = new Button { Text = Program.T("CancelButton"), Location = new Point(313, 74), Size = new Size(75, 24) };
			cancel.Click += delegate { Close(); };
			Controls.Add(label);
			Controls.Add(progress);
			Controls.Add(cancel);
			CancelButton = cancel;

			client.DownloadProgressChanged += (sender, e) =>
			{
				if (e.TotalBytesToReceive > 0)
					progress.Value = e.ProgressPercentage;
				else
					progress.Style = ProgressBarStyle.Marquee;
			};
			client.DownloadFileCompleted += (sender, e) =>
			{
				if (e.Error != null && !e.Cancelled)
					error = e.Error;
				completed = e.Error == null && !e.Cancelled;
				if (!IsDisposed)
					Close();
			};
			// closing waits for the download to stop, which then closes the form
			FormClosing += (sender, e) =>
			{
				if (client.IsBusy)
				{
					e.Cancel = true;
					client.CancelAsync();
				}
			};
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
				client.Dispose();
			base.Dispose(disposing);
		}
	}

	static class WinTrust
	{
		static readonly Guid GenericVerifyV2 = new Guid("00AAC56B-CD44-11d0-8CC2-00C04FC295EE");

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		class FileInfo
		{
			public int StructSize = Marshal.SizeOf(typeof(FileInfo));
			public string FilePath;
			public IntPtr File = IntPtr.Zero;
			public IntPtr KnownSubject = IntPtr.Zero;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		class TrustData
		{
			public int StructSize = Marshal.SizeOf(typeof(TrustData));
			public IntPtr PolicyCallbackData = IntPtr.Zero;
			public IntPtr SIPClientData = IntPtr.Zero;
			public int UIChoice = 2; // WTD_UI_NONE
			public int RevocationChecks = 0; // WTD_REVOKE_NONE
			public int UnionChoice = 1; // WTD_CHOICE_FILE
			public IntPtr FileInfo;
			public int StateAction = 0; // WTD_STATEACTION_IGNORE
			public IntPtr StateData = IntPtr.Zero;
			public string URLReference;
			public int ProvFlags = 0;
			public int UIContext = 0;
			public IntPtr SignatureSettings = IntPtr.Zero;
		}

		[DllImport("wintrust.dll", CharSet = CharSet.Unicode)]
		static extern int WinVerifyTrust(IntPtr hwnd, [MarshalAs(UnmanagedType.LPStruct)] Guid action, TrustData data);

		/// <summary>0 when the file's signature is valid and trusted, otherwise the error.</summary>
		public static int Verify(string path)
		{
			var file = new FileInfo { FilePath = path };
			var pointer = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(FileInfo)));
			try
			{
				Marshal.StructureToPtr(file, pointer, false);
				return WinVerifyTrust(new IntPtr(-1) /* no UI */, GenericVerifyV2, new TrustData { FileInfo = pointer });
			}
			finally
			{
				Marshal.DestroyStructure(pointer, typeof(FileInfo));
				Marshal.FreeHGlobal(pointer);
			}
		}
	}
}
