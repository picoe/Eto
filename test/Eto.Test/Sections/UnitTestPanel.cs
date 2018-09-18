using System;
using System.Reflection;
using Eto.Forms;
using System.Collections.Generic;
using NUnit.Framework.Internal;
using NUnit.Framework.Api;
using System.Threading.Tasks;
using Eto.Drawing;
using System.Linq;
using NUnit.Framework.Interfaces;
using System.IO;
using System.Collections.ObjectModel;

namespace Eto.Test.Sections
{
	class SingleTestFilter : ITestFilter
	{
		public ITest Test { get; set; }

		public Assembly Assembly { get; set; }

		public bool IsExplicitMatch(ITest test)
		{
			if (Assembly != null && !test.IsSuite && test.TypeInfo?.Assembly != Assembly)
				return false;
			return test.FullName == Test.FullName;
		}

		public bool Pass(ITest test)
		{
			if (Assembly != null && !test.IsSuite && test.TypeInfo?.Assembly != Assembly)
				return false;

			var parent = Test;
			// check if it is a parent of the test
			while (parent != null)
			{
				if (test.FullName == parent.FullName)
					return true;
				parent = parent.Parent;
			}
			// execute all children of the test
			parent = test;
			while (parent != null)
			{
				if (parent.FullName == Test.FullName)
					return true;
				parent = parent.Parent;
			}
			return false;
		}

		public TNode AddToXml(TNode parentNode, bool recursive) => throw new NotImplementedException();

		public TNode ToXml(bool recursive) => throw new NotImplementedException();
	}

	class CategoryFilter : ITestFilter
	{
		public Assembly Assembly { get; set; }

		public List<string> IncludeCategories { get; private set; }

		public List<string> ExcludeCategories { get; private set; }

		public string Keyword { get; set; }

		public int SkipCount { get; set; }

		public ITestFilter InnerFilter { get; set; }

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

		internal static IEnumerable<ITest> GetChildren(ITest test)
		{
			if (test.HasChildren)
			{
				foreach (var child in test.Tests)
				{
					yield return child;
					foreach (var childTest in GetChildren(child))
						yield return childTest;
				}
			}
		}

		bool Matches(ITest test) => MatchesKeyword(test) && MatchesIncludeCategory(test);

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
			if (Assembly != null && !test.IsSuite && test.TypeInfo?.Assembly != Assembly)
				return false;

			bool pass = InnerFilter?.Pass(test) ?? true;
			if (pass)
			{
				if (!test.IsSuite)
				{
					var parents = GetParents(test).ToList();
					pass &= parents.Any(Matches);
					pass &= !parents.Any(MatchesExcludeCategory);
				}
				else
				{
					pass &= !MatchesExcludeCategory(test);
					pass &= GetChildren(test).Any(Matches);
				}
			}

			if (!pass)
				SkipCount++;

			return pass;
		}

		public bool IsExplicitMatch(ITest test)
		{
			return MatchesKeyword(test) && MatchesIncludeCategory(test) && !MatchesExcludeCategory(test);
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

	class MultipleTestResult : ITestResult
	{
		public List<ITestResult> Results { get; private set; }

		public MultipleTestResult()
		{
			Results = new List<ITestResult>();
		}

		public int AssertCount => Results.Sum(r => r.AssertCount);

		public IEnumerable<ITestResult> Children => Results.SelectMany(r => r.Children);

		public double Duration => Results.Sum(r => r.Duration);

		public DateTime EndTime => Results.Max(r => r.EndTime);

		public int FailCount => Results.Sum(r => r.FailCount);

		public string FullName => string.Empty;

		public bool HasChildren => Results.Any(r => r.HasChildren);

		public int InconclusiveCount => Results.Sum(r => r.InconclusiveCount);

		public string Message => string.Join("\n", Results.Select(r => r.Message));

		public string Name => string.Join(", ", Results.Select(r => r.Name));

		public string Output => string.Join("\n", Results.Select(r => r.Output));

		public int PassCount => Results.Sum(r => r.PassCount);

		public ResultState ResultState => throw new NotImplementedException();

		public int SkipCount => Results.Sum(r => r.SkipCount);

		public string StackTrace => throw new NotImplementedException();

		public DateTime StartTime => Results.Min(r => r.StartTime);

		public ITest Test => Results.Select(r => r.Test).FirstOrDefault();

		public int WarningCount => Results.Sum(r => r.WarningCount);

		public IList<AssertionResult> AssertionResults => Results.SelectMany(r => r.AssertionResults).ToList();

		public ICollection<TestAttachment> TestAttachments => Results.SelectMany(r => r.TestAttachments).ToList();

		public TNode AddToXml(TNode parentNode, bool recursive) => throw new NotImplementedException();

		public TNode ToXml(bool recursive) => null;
	}

	public class UnitTestLogEventArgs : EventArgs
	{
		public string Message { get; }

		public UnitTestLogEventArgs(string message)
		{
			Message = message;
		}
	}

	public class UnitTestProgressEventArgs : EventArgs
	{
		public int TestCaseCount { get; private set; }
		public int CompletedCount { get; private set; }
		public int AssertCount { get; private set; }
		public int FailCount { get; private set; }
		public int PassCount { get; private set; }
		public int WarningCount { get; private set; }
		public int InconclusiveCount { get; private set; }
		public int SkipCount { get; private set; }
		public ITestResult CurrentResult { get; private set; }

		internal UnitTestProgressEventArgs(int testCaseCount)
		{
			TestCaseCount = testCaseCount;
		}

		internal void SetCount(int count)
		{
			CompletedCount = count;
		}

		internal void AddResult(ITestResult result)
		{
			CurrentResult = result;
			CompletedCount++;
			AssertCount += result.AssertCount;
			FailCount += result.FailCount;
			WarningCount += result.WarningCount;
			PassCount += result.PassCount;
			InconclusiveCount += result.InconclusiveCount;
			SkipCount += result.SkipCount;
		}
	}

	public class UnitTestRunner : ITestListener
	{
		MultipleTestResult allresults;
		ObservableCollection<Assembly> assemblies = new ObservableCollection<Assembly>();
		Queue<Assembly> assembliesToTest;
		ITestFilter testFilter;
		NUnitTestAssemblyRunner runner;
		bool isRunning;
		TaskCompletionSource<ITestResult> tcs;
		UnitTestProgressEventArgs progressArgs;
		List<TestAssembly> testSuiteCache;

		public IList<Assembly> Assemblies => assemblies;

		public bool IsRunning
		{
			get => isRunning;
			private set
			{
				if (isRunning != value)
				{
					isRunning = value;
					IsRunningChanged?.Invoke(this, EventArgs.Empty);
				}
			}
		}

		public event EventHandler<UnitTestLogEventArgs> Log;

		public event EventHandler<UnitTestProgressEventArgs> Progress;

		public event EventHandler<EventArgs> IsRunningChanged;

		public UnitTestRunner()
		{
			assemblies.CollectionChanged += (sender, e) => testSuiteCache = null;
		}

		public UnitTestRunner(IEnumerable<Assembly> assemblies)
			: this()
		{
			foreach (var assembly in assemblies)
				this.assemblies.Add(assembly);
		}

		public void WriteLog(string message)
		{
			Log?.Invoke(this, new UnitTestLogEventArgs(message));
		}

		public IEnumerable<TestAssembly> GetTestSuites() => testSuiteCache ?? (testSuiteCache = BuildTestSuites().ToList());

		IEnumerable<TestAssembly> BuildTestSuites()
		{
			var builder = new DefaultTestAssemblyBuilder();
			var settings = new Dictionary<string, object>();
			foreach (var assembly in Assemblies)
			{
				yield return builder.Build(assembly, settings) as TestAssembly;
			}
		}

		public ITestResult RunTests(ITestFilter filter = null) => RunTestsAsync(filter).GetAwaiter().GetResult();

		public Task<ITestResult> RunTestsAsync(ITestFilter filter = null)
		{
			if (IsRunning && tcs != null)
				return tcs.Task;

			tcs = new TaskCompletionSource<ITestResult>();
			IsRunning = true;
			WriteLog("Starting tests...");

			allresults = new MultipleTestResult();
			testFilter = filter ?? TestFilter.Empty;
			assembliesToTest = new Queue<Assembly>(assemblies);

			var totalTestCount = GetTestSuites().SelectMany(CategoryFilter.GetChildren).Count(r => !r.IsSuite && testFilter.Pass(r));
			progressArgs = new UnitTestProgressEventArgs(totalTestCount);

			WriteLog($"Total test count: {totalTestCount}");
			TestNextAssembly();
			return tcs.Task;
		}

		void TestNextAssembly()
		{
			lock (this)
			{
				Assembly nextAssembly = null;
				if (assembliesToTest?.Count > 0)
					nextAssembly = assembliesToTest.Dequeue();
				if (nextAssembly == null)
				{
					WriteLog(allresults.FailCount > 0 ? "FAILED" : "PASSED");
					WriteLog($"\tPassed: {allresults.PassCount}, Failed: {allresults.FailCount}, Skipped: {allresults.SkipCount}, Inconclusive: {allresults.InconclusiveCount}, Warnings: {allresults.WarningCount}");
					WriteLog($"\tDuration: {allresults.Duration}");
					IsRunning = false;
					tcs.SetResult(allresults);
					return;
				}
				try
				{
					runner = new NUnitTestAssemblyRunner(new DefaultTestAssemblyBuilder());
					var settings = new Dictionary<string, object>();
					runner.Load(nextAssembly, settings);
					runner.RunAsync(this, testFilter);
				}
				catch (Exception ex)
				{
					WriteLog($"Error running tests: {ex}");
					tcs.SetException(ex);
					IsRunning = false;
				}
			}
		}

		public void StopTests()
		{
			lock (this)
			{
				if (runner != null && IsRunning)
				{
					WriteLog("Stopping tests...");
					assembliesToTest?.Clear();
					runner.StopRun(true);
					IsRunning = false;
				}
			}
		}

		internal void FinishedAssembly(ITestResult result)
		{
			allresults.Results.Add(result);

			TestNextAssembly();
		}

		void ITestListener.TestStarted(ITest test)
		{
			if (!test.IsSuite)
				WriteLog(test.FullName);
		}

		void ITestListener.TestFinished(ITestResult result)
		{
			if (!result.Test.IsSuite)
			{
				progressArgs.AddResult(result);
				Progress?.Invoke(this, progressArgs);

				if (result.AssertionResults.Count > 0)
				{
					foreach (var assertion in result.AssertionResults)
					{
						if (assertion.Status == AssertionStatus.Passed)
							continue;
						WriteLog($"{assertion.Status}: {result.Message}\n{result.StackTrace}");
					}
				}
				else
				{
					if (result.ResultState.Status != TestStatus.Passed && result.ResultState.Status != TestStatus.Skipped)
						WriteLog($"{result.ResultState.Status}: {result.Message}");
				}
			}
			else if (result.Test is TestAssembly)
			{
				FinishedAssembly(result);
			}
		}

		void ITestListener.TestOutput(TestOutput output)
		{
			if (!string.IsNullOrEmpty(output.Text))
				WriteLog(output.Text);
		}
	}

	class UnitTestProgressBar : Drawable
	{
		float progress;
		Color color = Colors.Green;
		public float Progress
		{
			get => progress;
			set
			{
				if (progress != value)
				{
					progress = value;
					Invalidate();
				}
			}
		}

		public Color Color
		{
			get => color;
			set
			{
				if (color != value)
				{
					color = value;
					Invalidate();
				}
			}
		}

		public UnitTestProgressBar()
		{
			Size = new Size(200, 5);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			var size = new SizeF(Width * progress, Height);
			e.Graphics.FillRectangle(Color, 0, 0, size.Width, size.Height);
		}
	}

	static class BindingExtensions
	{
		public static IndirectBinding<T> Invoke<T>(this IndirectBinding<T> binding)
		{
			return new DelegateBinding<object, T>(
				m => Application.Instance.Invoke(() => binding.GetValue(m)),
				(m, val) => Application.Instance.Invoke(() => binding.SetValue(m, val)),
				addChangeEvent: (m, ev) => binding.AddValueChangedHandler(m, ev),
				removeChangeEvent: binding.RemoveValueChangedHandler
			);
		}
	}


	public class UnitTestPanel : Panel
	{
		TreeGridView tree;
		Button startButton;
		Button stopButton;
		SearchBox search;
		TextArea log;
		UnitTestProgressBar progress;
		bool hasLogged;
		UITimer timer;
		Panel customContent;
		List<string> includeCategories;
		List<string> excludeCategories;

		public new Control Content
		{
			get => customContent.Content;
			set => customContent.Content = value;
		}

		public UnitTestRunner Runner { get; }

		public UnitTestPanel(bool showLog = true)
			: this(new UnitTestRunner(), showLog)
		{
		}

		public UnitTestPanel(UnitTestRunner runner, bool showLog = true)
		{
			this.Runner = runner;
			runner.Log += (sender, e) => Application.Instance.Invoke(() =>
			{
				if (hasLogged)
					log?.Append("\n");
				else
					hasLogged = true;
				log?.Append(e.Message, true);
			});

			customContent = new Panel();

			progress = new UnitTestProgressBar();

			runner.Progress += (sender, e) =>
			{
				var progressAmount = e.TestCaseCount > 0 ? (float)e.CompletedCount / e.TestCaseCount : 0;
				var color = e.FailCount > 0 ? Colors.Red : e.WarningCount > 0 ? Colors.Yellow : Colors.Green;
				Application.Instance.AsyncInvoke(() =>
				{
					progress.Progress = progressAmount;
					progress.Color = color;
				});
			};

			timer = new UITimer();
			timer.Interval = 0.5;
			timer.Elapsed += (sender, e) => PerformSearch();

			var isRunningBinding = Binding.Property(runner, r => r.IsRunning);
			var enabledPropertyBinding = Binding.Property((Button c) => c.Enabled).Invoke();
			startButton = new Button { Text = "Start" };
			startButton.Click += (s, e) => RunTests();
			startButton.Bind(enabledPropertyBinding, isRunningBinding.Convert(r => !r));

			stopButton = new Button { Text = "Stop" };
			stopButton.Click += (s, e) => runner?.StopTests();
			stopButton.Bind(enabledPropertyBinding, isRunningBinding);

			var buttons = new TableLayout
			{
				Padding = new Padding(10),
				Spacing = new Size(5, 5),
				Rows = { new TableRow(startButton, stopButton, null) }
			};

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
			search.TextChanged += (sender, e) =>
			{
				if (timer.Started)
					timer.Stop();
				timer.Start();
			};


			tree = new TreeGridView { ShowHeader = false, Size = new Size(400, -1) };
			tree.Columns.Add(new GridColumn
			{
				DataCell = new TextBoxCell { Binding = Binding.Property((UnitTestItem m) => m.Text) }
			});

			tree.Activated += (sender, e) =>
			{
				if (runner.IsRunning)
					return;
				var item = (TreeGridItem)tree.SelectedItem;
				if (item != null)
				{
					var filter = item.Tag as ITestFilter;
					if (filter != null)
					{
						RunTests(filter);
					}
				}
			};


			if (showLog)
			{
				Size = new Size(950, 600);
				log = new TextArea { Size = new Size(400, 300), ReadOnly = true, Wrap = false };

				base.Content = new Splitter
				{
					FixedPanel = SplitterFixedPanel.None,

					Panel1 = new TableLayout
					{
						Spacing = new Size(5, 5),
						Rows = { search, tree }
					},

					Panel2 = new TableLayout
					{
						Spacing = new Size(5, 5),
						Rows = { buttons, customContent, progress, log }
					}
				};
			}
			else
			{
				Size = new Size(400, 400);
				base.Content = new TableLayout
				{
					Spacing = new Size(5, 5),
					Rows = { buttons, customContent, search, progress, tree }
				};
			}
		}

		private void PerformSearch()
		{
			timer.Stop();
			PopulateTree();
		}

		public IList<string> IncludeCategories => includeCategories ?? (includeCategories = new List<string>());
		public IList<string> ExcludeCategories => excludeCategories ?? (excludeCategories = new List<string>());

		void RunTests(ITestFilter filter = null)
		{
			if (!startButton.Enabled)
				return;
			startButton.Enabled = false;
			progress.Progress = 0;
			progress.Color = Colors.Green;
			if (log != null)
				log.Text = string.Empty;
			hasLogged = false;

			// run asynchronously so UI is responsive
			Runner.RunTestsAsync(CreateFilter(filter));
		}

		ITestFilter CreateFilter(ITestFilter filter = null)
		{
			var categoryFilter = new CategoryFilter();
			categoryFilter.InnerFilter = filter;

			if (includeCategories != null)
				categoryFilter.IncludeCategories.AddRange(includeCategories);
			if (excludeCategories != null)
				categoryFilter.ExcludeCategories.AddRange(excludeCategories);

			categoryFilter.Keyword = search.Text;
			return categoryFilter;
		}

		protected override void OnLoadComplete(EventArgs e)
		{
			base.OnLoadComplete(e);
			try
			{
				if (tree != null)
					PopulateTree();
			}
			catch (Exception ex)
			{
				log?.Append($"Error populating tree\n{ex}", true);
			}
		}

		public void Refresh()
		{
			PopulateTree();
		}

		void PopulateTree()
		{
			var filter = CreateFilter();
			Task.Factory.StartNew(() =>
			{
				var testSuites = Runner.GetTestSuites().Select(suite => ToTree(suite.Assembly, suite, filter)).Where(r => r != null).ToList();

				var treeData = new TreeGridItem(testSuites);
				Application.Instance.AsyncInvoke(() => tree.DataStore = treeData);
			});
		}

		class UnitTestItem : TreeGridItem
		{
			public string Text { get; set; }
		}

		TreeGridItem ToTree(Assembly assembly, ITest test, ITestFilter filter)
		{
			// add a test
			var name = test.Name;
			if (test.IsSuite)
			{
				var an = new AssemblyName(Path.GetFileNameWithoutExtension(test.Name));
				name = an.Name;
			}

			if (!filter.Pass(test))
				return null;

			var item = new UnitTestItem { Text = name, Tag = new SingleTestFilter { Test = test, Assembly = assembly } };
			if (test.HasChildren)
			{
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
					var child = item.Children[0] as UnitTestItem;
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
			return item;
		}
	}
}