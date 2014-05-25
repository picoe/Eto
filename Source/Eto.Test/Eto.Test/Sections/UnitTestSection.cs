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
using System.Linq;

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

	class SingleTestFilter : ITestFilter
	{
		public ITest Test { get; set; }

		public bool Pass(ITest test)
		{
			var parent = Test;
			// check if it is a parent of the test
			while (parent != null)
			{
				if (test.FullName == parent.FullName)
					return true;
				parent = parent.Parent;
			}
			// execute all children of the test
			while (test != null)
			{
				if (test.FullName == Test.FullName)
					return true;
				test = test.Parent;
			}
			return false;
		}

		public bool IsEmpty { get { return false; } }
	}

	class NamespaceTestFilter : ITestFilter
	{
		public string Namespace { get; set; }

		public bool Pass(ITest test)
		{
			if (test.FixtureType == null && test.Parent != null)
				return Pass(test.Parent);
			var ns = test.FixtureType.Namespace;
			return ns == Namespace || ns.StartsWith(Namespace + ".", StringComparison.Ordinal);
		}

		public bool IsEmpty { get { return false; } }
	}

	public class UnitTestSection : Panel
	{
		TreeView tree;
		Button startButton;

		public UnitTestSection()
		{
			startButton = new Button { Text = "Start Tests", Size = new Size(200, 100) };
			var buttons = new TableLayout(new TableRow(null, startButton, null));

			if (Platform.Supports<TreeView>())
			{
				tree = new TreeView();

				tree.Activated += (sender, e) =>
				{
					var item = (TreeItem)tree.SelectedItem;
					if (item != null)
					{
						RunTests(item.Tag as ITestFilter);
					}
				};

				Content = new TableLayout(
					buttons,
					tree
				);
			}
			else
				Content = new TableLayout(null, buttons, null);

			startButton.Click += (s, e) => RunTests();
		}

		async void RunTests(ITestFilter filter = null)
		{
			if (!startButton.Enabled)
				return;
			startButton.Enabled = false;
			Log.Write(null, "Starting tests...");
			try
			{
				await Task.Run(() =>
				{
					using (Platform.ThreadStart())
					{
						try
						{
							var assembly = GetType().GetTypeInfo().Assembly;
							var runner = new NUnitLiteTestAssemblyRunner(new NUnitLiteTestAssemblyBuilder());
							if (!runner.Load(assembly, new Dictionary<string, object>()))
							{
								Log.Write(null, "Failed to load test assembly");
								return;
							}
							var listener = new TestListener();
							var result = runner.Run(listener, filter ?? TestFilter.Empty);
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
							Application.Instance.Invoke(() => startButton.Enabled = true);
						}
					}
				});
			}
			catch (Exception ex)
			{
				Log.Write(null, "Error running tests\n{0}", ex);
			}
		}

		protected override async void OnLoadComplete(EventArgs e)
		{
			base.OnLoadComplete(e);
			try
			{
				if (tree != null)
					await Task.Factory.StartNew(PopulateTree);
			}
			catch (Exception ex)
			{
				Log.Write(null, "Error populating tree\n{0}", ex);
			}
		}

		TestSuite GetTestSuite()
		{
			var assembly = GetType().GetTypeInfo().Assembly;
			var builder = new NUnitLiteTestAssemblyBuilder();
			return builder.Build(assembly, new Dictionary<string, object>());
		}

		void PopulateTree()
		{
			var testSuite = GetTestSuite();
			var treeData = ToTree(testSuite);
			Application.Instance.AsyncInvoke(() => tree.DataStore = treeData);
		}

		TreeItem ToTree(ITest test, int startIndex = 0)
		{
			// add a test
			var item = new TreeItem { Text = test.Name, Tag = new SingleTestFilter { Test = test } };
			if (test.HasChildren)
			{
				item.Children.AddRange(ToTree(test.Tests, startIndex));
			}
			return item;
		}

		IEnumerable<TreeItem> ToTree(IEnumerable<ITest> tests, int startIndex)
		{
			if (startIndex == -1)
			{
				foreach (var test in tests)
				{
					yield return ToTree(test, -1);
				}
				yield break;
			}

			// split namespaces and group them to the startIndex
			var namespaces = from s in
			                     (
			                         from r in tests
			                         group r by r.FixtureType.Namespace into g
			                         orderby g.Key
			                         select g.Key.Split('.').Take(startIndex + 1)
			                     )
			                 group s by s.Last() into gg // group by last namespace
			                 select gg.First();

			foreach (var ns in namespaces)
			{
				var fullns = string.Join(".", ns);
				var childtests = ToTree(from t in tests
				                        where t.FixtureType.Namespace.StartsWith(fullns + ".", StringComparison.Ordinal)
				                        select t, startIndex + 1);

				var nstests = ToTree(from t in tests
				                     where t.FixtureType.Namespace == fullns
				                     select t, -1);

				// add a namespace
				var ti = new TreeItem
				{
					Text = ns.Last(),
					Expanded = true,
					Tag = new NamespaceTestFilter { Namespace = fullns }
				};
				ti.Children.AddRange(from t in nstests.Concat(childtests)
				                     orderby t.Text
				                     select t);
				yield return ti;
			}
		}
	}
}