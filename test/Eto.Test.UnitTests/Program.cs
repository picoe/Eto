using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
#if NET
using System.Runtime.Loader;
#endif
using System.Threading.Tasks;
using Microsoft.Testing.Platform.Builder;
using NUnit.Framework;
using NUnit.VisualStudio.TestAdapter.TestingPlatformAdapter;

namespace Eto.Test.UnitTests;


internal static class Program
{
	static IEnumerable<Assembly> GetTestAssemblies()
	{
		yield return typeof(Eto.Test.MainForm).Assembly;
	}

	[STAThread]
	public static int Main(string[] args)
	{
		AvoidThemeSatelliteResolverCrash();

		using var app = new Application();

		var exitCodeSource = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);

		app.Initialized += (sender, e) =>
		{
			var context = SynchronizationContext.Current;
			_ = Task.Run(async () =>
			{
				SynchronizationContext.SetSynchronizationContext(context);
				await RunTestsAndQuitAsync(app, args, exitCodeSource);
			});
		};
		app.Run();

		return exitCodeSource.Task.Result;
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

			using ITestApplication testApplication = await builder.BuildAsync();
			int exitCode = await testApplication.RunAsync();
			exitCodeSource.TrySetResult(exitCode);
		}
		catch (Exception ex)
		{
			exitCodeSource.TrySetException(ex);
		}
		finally
		{
			app.Invoke(() => app.Quit()); // quit app after tests complete
		}
	}
}