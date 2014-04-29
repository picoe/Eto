using System;
using System.IO;
using System.Reflection;
using Eto.Forms;
using Eto.Threading;
using System.Collections.Generic;
using NUnit.Framework.Internal;
using NUnitLite.Runner;
using NUnit.Framework.Api;
using System.Threading.Tasks;
using Eto.Drawing;

namespace Eto.Test.Sections
{
	public class TestListener : ITestListener
	{
		public void TestFinished(ITestResult result)
		{
			if (!result.HasChildren)
			{
				if (result.FailCount > 0)
				{
					Application.Instance.Invoke(() => Log.Write(null, "Failed: {0}\n{1}", result.Message, result.StackTrace));
				}
			}
		}

		public void TestOutput(TestOutput testOutput)
		{
			Application.Instance.Invoke(() => Log.Write(null, testOutput.Text));
		}

		public void TestStarted(ITest test)
		{
			if (!test.HasChildren)
				Application.Instance.Invoke(() => Log.Write(null, test.FullName));
		}
	}

	public class UnitTestSection : Panel
	{
		public UnitTestSection()
		{
			var layout = new DynamicLayout();
			var button = new Button { Text = "Start Tests", Size = new Size(200, 100) };
			layout.AddCentered(button);

			Content = layout;

			button.Click += (s, e) =>
			{
				button.Enabled = false;
				Log.Write(null, "Starting tests...");
				Task.Factory.StartNew(() =>
				{
					using (Platform.ThreadStart())
					{
						try
						{
							#if PCL
							var assembly = GetType().GetTypeInfo().Assembly;
							#else
							var assembly = GetType().Assembly;
							#endif

							var runner = new NUnitLiteTestAssemblyRunner(new NUnitLiteTestAssemblyBuilder());
							if (!runner.Load(assembly, new Dictionary<string, object>()))
							{
								Log.Write(null, "Failed to load test assembly");
								return;
							}

							var listener = new TestListener();
							var result = runner.Run(listener, TestFilter.Empty);
							var writer = new StringWriter();
							writer.WriteLine(result.FailCount > 0 ? "FAILED" : "PASSED");
							writer.WriteLine("\tPass: {0}, Fail: {1}, Skipped: {2}, Inconclusive: {3}", result.PassCount, result.FailCount, result.SkipCount, result.InconclusiveCount);
							writer.Write("\tDuration: {0}", result.Duration);
							Application.Instance.Invoke(() => Log.Write(null, writer.ToString()));
						}
						catch (Exception ex)
						{
							Application.Instance.Invoke(() => Log.Write(null, "Error running tests: {0}", ex));
						}
						finally
						{
							Application.Instance.Invoke(() => button.Enabled = true);
						}
					}
				});
			};
		}
	}
}
