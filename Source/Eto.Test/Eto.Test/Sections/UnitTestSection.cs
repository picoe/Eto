using System;
using System.IO;
using System.Reflection;
using Eto.Forms;
using Eto.Threading;
using System.Collections.Generic;
using NUnit.Framework.Internal;
using NUnit.Framework.Api;
using System.Threading.Tasks;
using Eto.Drawing;
using System.Linq;
using Eto.Test.UnitTests.Handlers;
using NUnit;
using NUnit.Framework.Interfaces;
using System.Collections;

namespace Eto.Test.Sections
{
	public class TestListener : ITestListener
	{
		public Application Application { get; set; }

		public void TestFinished(ITestResult result)
		{
			if (!result.HasChildren)
			{
				if (!string.IsNullOrEmpty(result.Output))
					Application.Instance.Invoke(() => Log.Write(null, result.Output));
				if (result.FailCount > 0)
				{
					Application.Invoke(() => Log.Write(null, "Failed: {0}\n{1}", result.Message, result.StackTrace));
				}
			}
		}

		public void TestStarted(ITest test)
		{
			if (!test.HasChildren)
				Application.Invoke(() => Log.Write(null, test.FullName));
		}
	}

	class SingleTestFilter : CategoryFilter
	{
		public ITest Test { get; set; }

		public override bool Pass(ITest test)
		{
			var parent = Test;
			// check if it is a parent of the test
			while (parent != null)
			{
				if (test.FullName == parent.FullName)
					return base.Pass(test);
				parent = parent.Parent;
			}
			// execute all children of the test
			parent = test;
			while (parent != null)
			{
				if (parent.FullName == Test.FullName)
					return base.Pass(test);
				parent = parent.Parent;
			}
			return false;
		}
	}

	class CategoryFilter : ITestFilter
	{
		public Application Application { get; set; }

		public Assembly Assembly { get; set; }

		public Assembly ExecutingAssembly { get; set; }

		public List<string> IncludeCategories { get; private set; }

		public List<string> ExcludeCategories { get; private set; }

		public CategoryFilter()
		{
			IncludeCategories = new List<string>();
			ExcludeCategories = new List<string>();
		}

		public virtual bool Pass(ITest test)
		{
			if (Assembly != null && ExecutingAssembly != Assembly)
				return false;

			var categoryList = test.Properties["Category"];

			bool pass = true;
			if (categoryList != null)
			{
				var categories = categoryList.OfType<string>().ToList();
				if (IncludeCategories.Count > 0)
					pass = categories.Any(IncludeCategories.Contains);

				if (ExcludeCategories.Count > 0)
					pass &= !categories.Any(ExcludeCategories.Contains);
			}
			if (!pass)
				Application.Invoke(() => Log.Write(null, "Skipping {0} (excluded)", test.FullName));
			return pass;
		}

		public virtual bool IsEmpty { get { return false; } }
	}

	[Section("Tests", "Unit Tests")]
	public class UnitTestSection : Panel
	{
		TreeView tree;
		Button startButton;
		CheckBox useTestPlatform;

		public UnitTestSection()
		{
			startButton = new Button { Text = "Start Tests", Size = new Size(200, 80) };
			useTestPlatform = new CheckBox { Text = "Use Test Platform" };
			var buttons = new StackLayout
			{
				Padding = new Padding(10),
				Spacing = 5,
				HorizontalContentAlignment = HorizontalAlignment.Center,
				Items = { startButton, useTestPlatform }
			};

			if (Platform.Supports<TreeView>())
			{
				tree = new TreeView();

				tree.Activated += (sender, e) =>
				{
					var item = (TreeItem)tree.SelectedItem;
					if (item != null)
					{
						RunTests(item.Tag as CategoryFilter);
					}
				};

				Content = new StackLayout
				{
					Spacing = 5,
					HorizontalContentAlignment = HorizontalAlignment.Stretch,
					Items = { buttons, new StackLayoutItem(tree, expand: true) }
				};
			}
			else
				Content = buttons;

			startButton.Click += (s, e) => RunTests();
		}

		class MultipleTestResult : ITestResult
		{
			public List<ITestResult> Results { get; private set; }

			public MultipleTestResult()
			{
				Results = new List<ITestResult>();
			}

			public int AssertCount { get { return Results.Sum(r => r.AssertCount); } }

			public IList<ITestResult> Children { get { return Results.SelectMany(r => r.Children).ToList(); } }

			public double Duration { get { return Results.Sum(r => r.Duration); } }

			public DateTime EndTime { get { return Results.Max(r => r.EndTime); } }

			public int FailCount { get { return Results.Sum(r => r.FailCount); } }

			public string FullName { get { return string.Empty; } }

			public bool HasChildren { get { return Results.Any(r => r.HasChildren); } }

			public int InconclusiveCount { get { return Results.Sum(r => r.InconclusiveCount); } }

			public string Message { get { return string.Join("\n", Results.Select(r => r.Message)); } }

			public string Name { get { return string.Join(", ", Results.Select(r => r.Name)); } }

			public string Output { get { return string.Join("\n", Results.Select(r => r.Output)); } }

			public int PassCount { get { return Results.Sum(r => r.PassCount); } }

			public ResultState ResultState { get { throw new NotImplementedException(); } }

			public int SkipCount { get { return Results.Sum(r => r.SkipCount); } }

			public string StackTrace { get { throw new NotImplementedException(); } }

			public DateTime StartTime { get { return Results.Min(r => r.StartTime); } }

			public ITest Test { get { return Results.Select(r => r.Test).FirstOrDefault(); } }

			public XmlNode AddToXml(XmlNode parentNode, bool recursive)
			{
				throw new NotImplementedException();
			}

			public XmlNode ToXml(bool recursive)
			{
				return null;
			}
		}

		async void RunTests(CategoryFilter filter = null)
		{
			if (!startButton.Enabled)
				return;
			startButton.Enabled = false;
			Log.Write(null, "Starting tests...");
			var testPlatform = useTestPlatform.Checked == true ? new TestPlatform() : Platform;
			try
			{
				await Task.Run(() =>
				{
					using (Platform.ThreadStart())
					{
						try
						{
							var listener = new TestListener { Application = Application.Instance }; // use running application for logging

							var builder = new DefaultTestAssemblyBuilder();
							var runner = new NUnitTestAssemblyRunner(builder);
							var settings = new Dictionary<string, object>();
							var result = new MultipleTestResult();
							foreach (var assembly in ((TestApplication)TestApplication.Instance).TestAssemblies)
							{
								runner.Load(assembly, settings);

								filter = filter ?? new CategoryFilter();
								filter.Application = Application.Instance;
								filter.ExecutingAssembly = assembly;
								if (testPlatform is TestPlatform)
									filter.ExcludeCategories.Add(UnitTests.TestUtils.NoTestPlatformCategory);
								else
									filter.ExcludeCategories.RemoveAll(r => r == UnitTests.TestUtils.NoTestPlatformCategory);
								using (testPlatform.Context)
								{
									result.Results.Add(runner.Run(listener, filter));
								}
							}
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

		IEnumerable<TestAssembly> GetTestSuites()
		{
			var builder = new DefaultTestAssemblyBuilder();
			var settings = new Dictionary<string, object>();
			foreach (var assembly in ((TestApplication)TestApplication.Instance).TestAssemblies)
			{
				yield return builder.Build(assembly, settings) as TestAssembly;
			}
		}

		void PopulateTree()
		{
			var testSuites = GetTestSuites().Select(suite => ToTree(suite.Assembly, suite)).ToList();
			TreeItem treeData;
			if (testSuites.Count > 1)
			{
				treeData = new TreeItem();
				treeData.Children.AddRange(testSuites);
			}
			else treeData = testSuites.FirstOrDefault();
			Application.Instance.AsyncInvoke(() => tree.DataStore = treeData);
		}

		TreeItem ToTree(Assembly assembly, ITest test)
		{
			// add a test
			var item = new TreeItem { Text = test.Name, Tag = new SingleTestFilter { Test = test, Assembly = assembly } };
			if (test.HasChildren)
			{
				item.Expanded = !(test is ParameterizedMethodSuite);
				foreach (var child in test.Tests)
				{
					item.Children.Add(ToTree(assembly, child));
				}
			}
			return item;
		}
	}
}