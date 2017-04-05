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
using NUnit.Framework;

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
					Application.Instance.AsyncInvoke(() => Log.Write(null, result.Output));
				if (result.FailCount > 0)
				{
					Application.AsyncInvoke(() => Log.Write(null, "Failed: {0}\n{1}", result.Message, result.StackTrace));
				}
				if (result.InconclusiveCount > 0)
					Application.AsyncInvoke(() => Log.Write(null, "Inconclusive: {0}\n{1}", result.Message, result.StackTrace));
			}
		}

		public void TestOutput(TestOutput output)
		{
		}

		public void TestStarted(ITest test)
		{
			if (!test.HasChildren)
				Application.AsyncInvoke(() => Log.Write(null, test.FullName));
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

		public string Keyword { get; set; }

		public int SkipCount { get; set; }

		public CategoryFilter()
		{
			IncludeCategories = new List<string>();
			ExcludeCategories = new List<string>();
		}

		protected static IEnumerable<ITest> GetParents(ITest test)
		{
			while (test != null)
			{
				yield return test;
				test = test.Parent;
			}
		}

		bool MatchesKeyword(ITest test)
		{
			if (string.IsNullOrEmpty(Keyword))
				return true;
			return test.FullName.IndexOf(Keyword, StringComparison.OrdinalIgnoreCase) >= 0;
		}

		bool MatchesIncludeCategory(ITest test)
		{
			if (IncludeCategories.Count == 0)
				return true;

			var categoryList = test.Properties["Category"];
			if (categoryList == null || categoryList.Count == 0)
				return false;
			
			return categoryList.OfType<string>().Any(IncludeCategories.Contains);
		}

		bool MatchesExcludeCategory(ITest test)
		{
			if (ExcludeCategories.Count == 0)
				return false;

			var categoryList = test.Properties["Category"];
			if (categoryList == null || categoryList.Count == 0)
				return false;

			return categoryList.OfType<string>().Any(ExcludeCategories.Contains);
		}
		public virtual bool Pass(ITest test)
		{
			if (Assembly != null && ExecutingAssembly != Assembly)
				return false;

			bool pass = true;
			if (!test.IsSuite)
			{
				var parents = GetParents(test).ToList();
				pass &= parents.Any(MatchesKeyword);
				pass &= parents.Any(MatchesIncludeCategory);
				pass &= !parents.Any(MatchesExcludeCategory);
			}

			if (!pass)
				SkipCount++;
			return pass;
		}

		public bool IsExplicitMatch(ITest test)
		{
			return MatchesKeyword(test) && MatchesIncludeCategory(test) && MatchesIncludeCategory(test);
		}

		public TNode ToXml(bool recursive)
		{
			throw new NotImplementedException();
		}

		public TNode AddToXml(TNode parentNode, bool recursive)
		{
			throw new NotImplementedException();
		}
	}

	[Section("Automated Tests", "Unit Tests")]
	public class UnitTestSection : Panel
	{
		TreeView tree;
		Button startButton;
		CheckBox useTestPlatform;
		CheckBox includeManualTests;
		SearchBox search;

		public UnitTestSection()
		{
			startButton = new Button { Text = "Start Tests", Size = new Size(200, 80) };
			useTestPlatform = new CheckBox { Text = "Use Test Platform" };
			includeManualTests = new CheckBox { Text = "Manual Tests", Checked = true };
			var buttons = new StackLayout
			{
				Padding = new Padding(10),
				Spacing = 5,
				HorizontalContentAlignment = HorizontalAlignment.Center,
				Items =
				{
					startButton,
					TableLayout.Horizontal(useTestPlatform, includeManualTests)
				}
			};

			if (Platform.Supports<TreeView>())
			{

				search = new SearchBox();
				search.Focus();
				search.KeyDown += (sender, e) =>
				{
					if (e.KeyData == Keys.Enter)
					{	
						startButton.PerformClick();
						e.Handled = true;
					}
				};

				var timer = new UITimer();
				timer.Interval = 0.5;
				timer.Elapsed += (sender, e) =>
				{
					timer.Stop();
					var searchText = search.Text;
					Task.Factory.StartNew(() => PopulateTree(searchText));
				};
				search.TextChanged += (sender, e) => {
					if (timer.Started)
						timer.Stop();
					timer.Start();
				};

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
					Items = { buttons, search, new StackLayoutItem(tree, expand: true) }
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

			public IEnumerable<ITestResult> Children { get { return Results.SelectMany(r => r.Children); } }

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

			public TNode AddToXml(TNode parentNode, bool recursive)
			{
				throw new NotImplementedException();
			}

			public TNode ToXml(bool recursive)
			{
				return null;
			}
		}

		async void RunTests(CategoryFilter filter = null)
		{
			if (!startButton.Enabled)
				return;
			startButton.Enabled = false;
			var keywords = search.Text;
			Log.Write(null, "Starting tests...");
			var testPlatform = useTestPlatform.Checked == true ? new TestPlatform() : Platform;
			var runManualTests = includeManualTests.Checked == true;
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
							if (filter != null)
								filter.SkipCount = 0;
							foreach (var assembly in ((TestApplication)TestApplication.Instance).TestAssemblies)
							{
								runner.Load(assembly, settings);

								filter = filter ?? new CategoryFilter();
								filter.Application = Application.Instance;
								filter.ExecutingAssembly = assembly;
								if (!runManualTests)
									filter.ExcludeCategories.Add(UnitTests.TestBase.ManualTestCategory);
								if (testPlatform is TestPlatform)
									filter.IncludeCategories.Add(UnitTests.TestBase.TestPlatformCategory);
								else
									filter.IncludeCategories.RemoveAll(r => r == UnitTests.TestBase.TestPlatformCategory);
								filter.Keyword = keywords;
								using (testPlatform.Context)
								{
									result.Results.Add(runner.Run(listener, filter));
								}
							}
							var writer = new StringWriter();
							writer.WriteLine(result.FailCount > 0 ? "FAILED" : "PASSED");
							writer.WriteLine("\tPass: {0}, Fail: {1}, Skipped: {2}, Inconclusive: {3}", result.PassCount, result.FailCount, result.SkipCount + filter.SkipCount, result.InconclusiveCount);
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
					await Task.Factory.StartNew(() => PopulateTree());
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

		void PopulateTree(string filter = null)
		{
			var testSuites = GetTestSuites().Select(suite => ToTree(suite.Assembly, suite, filter)).Where(r => r != null).ToList();
			var treeData = new TreeItem(testSuites);
			Application.Instance.AsyncInvoke(() => tree.DataStore = treeData);
		}

		TreeItem ToTree(Assembly assembly, ITest test, string filter)
		{
			// add a test
			var name = test.Name;
			if (test.IsSuite)
			{
				var an = new AssemblyName(test.Name);
				name = an.Name;
			}

			var item = new TreeItem { Text = name, Tag = new SingleTestFilter { Test = test, Assembly = assembly } };
			var nameMatches = filter == null || test.FullName.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0;
			if (test.HasChildren)
			{
				if (nameMatches)
					filter = null;
				item.Expanded = !(test is ParameterizedMethodSuite);
				foreach (var child in test.Tests)
				{
					var treeItem = ToTree(assembly, child, filter);
					if (treeItem != null)
						item.Children.Add(treeItem);
				}
				while (item.Children.Count == 1)
				{
					// collapse test nodes
					var child = item.Children[0] as TreeItem;
					if (child.Children.Count == 0)
						break;
					if (!child.Text.StartsWith(item.Text, StringComparison.Ordinal))
						item.Text += "." + child.Text;
					else
						item.Text = child.Text;
					item.Children.Clear();
					item.Children.AddRange(child.Children);
				}
				if (item.Children.Count == 0)
					return null;
			}
			else if (!nameMatches)
			{
				return null;
			}
			return item;
		}
	}
}