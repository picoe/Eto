using System;
using System.Collections.Generic;
using System.Linq;
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