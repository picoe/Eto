using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
#if NET
using System.Runtime.Loader;
#endif
using System.Threading.Tasks;
using Microsoft.Testing.Extensions;
using Microsoft.Testing.Platform.Builder;
using NUnit.Framework;
using NUnit.VisualStudio.TestAdapter.TestingPlatformAdapter;

namespace Eto.Test.UnitTests;


internal static class Program
{
	static IEnumerable<Assembly> GetTestAssemblies()
	{
		// When more than one assembly is listed, the VSTest bridge requires the test application's
		// own assembly to be among them.
		yield return typeof(Program).Assembly;

		yield return typeof(Eto.Test.MainForm).Assembly;
#if WINDOWS || NETFRAMEWORK
		if (Platform.Instance.IsWpf)
		{
			// WPF-specific fixtures from test/Eto.Test.Wpf/UnitTests
			yield return typeof(Eto.Test.Wpf.UnitTests.BitmapTests).Assembly;
		}
		if (Platform.Instance.IsWinForms)
		{
			// WinForms-specific fixtures from test/Eto.Test.WinForms/UnitTests
			yield return typeof(Eto.Test.WinForms.UnitTests.NativeTests).Assembly;
		}
#elif NET
		if (Platform.Instance.IsGtk)
		{
			// GTK-specific fixtures from test/Eto.Test.Gtk/UnitTests
			yield return typeof(Eto.Test.Gtk.UnitTests.NativeParentWindowTests).Assembly;
		}
		if (Platform.Instance.IsMac)
		{
			// Mac-specific fixtures from test/Eto.Test.Mac/UnitTests
			yield return typeof(Eto.Test.Mac.UnitTests.BitmapTests).Assembly;
		}
#endif
	}

	/// <summary>
	/// Registers the platform's <see cref="ITestInput"/> implementation, which lives in the
	/// Eto.Test.&lt;Platform&gt; app assembly alongside its other platform-specific test code. The apps
	/// do this in their own Startup, so it has to be done here as well for `dotnet test`.
	/// </summary>
	static void RegisterTestInput(Platform platform)
	{
#if WINDOWS || NETFRAMEWORK
		if (platform.IsWpf)
			platform.Add<ITestInput>(() => new Eto.Test.Wpf.TestInput());
		else if (platform.IsWinForms)
			platform.Add<ITestInput>(() => new Eto.Test.WinForms.TestInput());
#elif NET
		if (platform.IsMac)
			platform.Add<ITestInput>(() => new Eto.Test.Mac.TestInput());
		else if (platform.IsGtk)
			platform.Add<ITestInput>(() => new Eto.Test.Gtk.TestInput());
#endif
	}

	[STAThread]
	public static int Main(string[] args)
	{
		AvoidThemeSatelliteResolverCrash();

		List<string> testArgs = new();
		// check any of the args for platform override
		Platform? platform = null;
		foreach (var arg in args)
		{
			if (arg.StartsWith("--platform=", StringComparison.OrdinalIgnoreCase))
			{
				Console.WriteLine($"Overriding platform with '{arg}'");
				var platformName = arg.Substring("--platform=".Length);
				if (!string.IsNullOrEmpty(platformName))
				{
					switch (platformName.ToLowerInvariant())
					{
#if WINDOWS || NETFRAMEWORK
						case "wpf":
							platform = new Eto.Wpf.Platform();
							break;
						case "winforms":
							platform = new Eto.WinForms.Platform();
							break;
#elif NET
						case "gtk":
							platform = new Eto.GtkSharp.Platform();
							break;
						case "mac":
							platform = new Eto.Mac.Platform();
							break;
#endif
						default:
							throw new ArgumentException($"Unknown platform '{platformName}'");
					}
				}
			}
			else
			{
				testArgs.Add(arg);
			}
		}
		
		if (platform == null)
			platform = Platform.Detect;

		RegisterTestInput(platform);

		using var app = new Application(platform);

#if !WINDOWS && !NETFRAMEWORK
		// Focusing a control needs the app to be active, and macOS hands activation over only when no
		// other app is holding it - so with anything else frontmost (Finder, on a CI runner) the test
		// windows never become key and no control in them can get focus. An automated test run is
		// exactly the case where taking activation from whatever is frontmost is the right thing to do.
		if (app.Handler is Eto.Mac.Forms.ApplicationHandler macApplication)
		{
			macApplication.ActivateOnStartup = true;
			macApplication.ActivateIgnoringOtherApps = true;
		}
#endif

		var exitCodeSource = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);

		app.Initialized += (sender, e) =>
		{
			var context = SynchronizationContext.Current;
			_ = Task.Run(async () =>
			{
				SynchronizationContext.SetSynchronizationContext(context);
				await RunTestsAndQuitAsync(app, testArgs.ToArray(), exitCodeSource);
			});
		};
		app.Run();

		// Only reached when the app quits normally (see RunTestsAndQuitAsync); the task is always complete
		// by then, and a failure to even run the tests was already reported there.
		return GetExitCode(exitCodeSource);
	}

	static readonly string[] WpfThemeSuffixes = { ".Aero2", ".Aero", ".AeroLite", ".Classic", ".Luna", ".Royale", ".Generic" };

	// When a themed Eto.Wpf control (e.g. TreeGridView) is laid out, WPF probes for an external per-OS-theme
	// satellite assembly (e.g. "Eto.Wpf.Aero2") before falling back to the embedded themes/generic.xaml. That
	// satellite never exists; in a normal app the probe just yields "not found" and WPF falls back. But under the
	// NUnit test host, its assembly Resolving handler throws a NullReferenceException instead of returning null,
	// which surfaces as a fatal FileLoadException during layout and crashes the test. Registering our own handler
	// FIRST (before NUnit's) lets us short-circuit those Eto theme probes with the normal FileNotFoundException so
	// WPF falls back cleanly and NUnit's broken handler never runs.
	static void AvoidThemeSatelliteResolverCrash()
	{
#if NET
		AssemblyLoadContext.Default.Resolving += (context, name) =>
		{
			var n = name.Name;
			if (n != null && n.StartsWith("Eto.", StringComparison.Ordinal) && WpfThemeSuffixes.Any(s => n.EndsWith(s, StringComparison.Ordinal)))
				throw new FileNotFoundException($"Eto theme satellite '{n}' does not exist; using generic theme.");
			return null;
		};
#endif
	}

	private static async Task RunTestsAndQuitAsync(Application app, string[] args, TaskCompletionSource<int> exitCodeSource)
	{
		try
		{
			var options = new TestApplicationOptions();
			ITestApplicationBuilder builder = await Microsoft.Testing.Platform.Builder.TestApplication.CreateBuilderAsync(args);
			builder.AddNUnit(GetTestAssemblies);
			// registered explicitly (adds the trx report options) as we don't use the generated entry point
			builder.AddTrxReportProvider();

			// ITestApplication is IDisposable only, there's no async disposal to await here.
			using ITestApplication testApplication = await builder.BuildAsync();
			int exitCode = await testApplication.RunAsync();
			exitCodeSource.TrySetResult(exitCode);
		}
		catch (Exception ex)
		{
			// log it here as well, the exception from Main is not always reported before we exit
			Console.Error.WriteLine($"Error running tests: {ex}");
			exitCodeSource.TrySetException(ex);
		}
		finally
		{
			// Quit the app once tests complete, exiting with the test result so a failing run actually
			// fails the process. Task.IsCompletedSuccessfully isn't available on .NET Framework, so check
			// the status directly.
			var exitCode = GetExitCode(exitCodeSource);

			// On Mac NSApplication.Terminate() ends the process itself with exit(0) and never returns from
			// Application.Run(), so both Main's return value and Environment.ExitCode are discarded there -
			// Environment.Exit is the only way to report the result. Everywhere else quit normally and let
			// Main return it: Environment.Exit terminates the process immediately, which can kill the test
			// host before the testing platform finishes shutting down and writes its trx report (seen on
			// Gtk, where a test that leaves the loop dirty makes NUnit's engine shutdown time out first).
			if (Platform.Instance.IsMac)
			{
				app.Invoke(() => Environment.Exit(exitCode));
			}
			else
			{
				app.Invoke(() => app.Quit());
				// A test that left the UI loop in a bad state can keep it from quitting, so force the
				// process down if that happens rather than letting the run sit until the CI step timeout.
				// The report is already written by this point, so nothing is lost by exiting here.
				_ = Task.Delay(TimeSpan.FromSeconds(30)).ContinueWith(_ => Environment.Exit(exitCode));
			}
		}
	}

	static int GetExitCode(TaskCompletionSource<int> exitCodeSource)
	{
		return exitCodeSource.Task.Status == TaskStatus.RanToCompletion ? exitCodeSource.Task.Result : 1;
	}
}