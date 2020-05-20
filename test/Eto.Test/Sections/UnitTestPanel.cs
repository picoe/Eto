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
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Text;
using System.Collections.Concurrent;
using System.Collections;
using System.ComponentModel;
using System.Threading;

namespace Eto.Test.Sections
{
	static class TestHelpers
	{
		public static IEnumerable<ITest> GetChildren(this ITest test) => GetChildren(test, true);

		public static IEnumerable<ITest> GetChildren(this ITest test, bool recursive)
		{
			if (test.HasChildren)
			{
				foreach (var child in test.Tests)
				{
					yield return child;
					if (recursive)
					{
						foreach (var childTest in GetChildren(child, recursive))
							yield return childTest;
					}
				}
			}
		}

		public static IEnumerable<ITest> GetParents(this ITest test)
		{
			while (test != null)
			{
				yield return test;
				test = test.Parent;
			}
		}

		public static IEnumerable<string> GetCategories(this ITest test)
		{
			var categories = test.Properties["Category"];
			if (categories == null || categories.Count == 0)
				return Enumerable.Empty<string>();
			return categories.OfType<string>();
		}

		public static BindableBinding<T, IEnumerable<TTo>> CastItems<T, TFrom, TTo>(this BindableBinding<T, IEnumerable<TFrom>> binding, TTo to)
			where T : Eto.Forms.IBindable
		{
			return binding.Convert(source => source.Cast<TTo>(), list => list.Cast<TFrom>());
		}
	}

	abstract class BaseFilter : ITestFilter
	{
		public bool IsExplicitMatch(ITest test) => Matches(test);

		public bool ChildCanMatch { get; set; } = true;
		public bool ParentCanMatch { get; set; } = true;

		public bool Pass(ITest test)
		{
			var matches = Matches(test);

			if (test.IsSuite)
			{
				if (ChildCanMatch)
					matches |= test.GetChildren().Any(Matches);
			}
			else if (ParentCanMatch)
				matches |= test.GetParents().Any(ParentMatch);

			return matches;
		}

		protected bool ParentMatch(ITest test) => Matches(test, true);

		protected bool Matches(ITest test) => Matches(test, false);

		protected abstract bool Matches(ITest test, bool parent);

		public TNode AddToXml(TNode parentNode, bool recursive) => throw new NotImplementedException();

		public TNode ToXml(bool recursive) => throw new NotImplementedException();
	}

	class NotFilter : ITestFilter
	{
		public ITestFilter Filter { get; set; }

		public NotFilter(ITestFilter filter)
		{
			Filter = filter;
		}

		public NotFilter()
		{
		}

		public bool IsExplicitMatch(ITest test) => Filter?.IsExplicitMatch(test) != true;

		public bool Pass(ITest test) => Filter?.Pass(test) != true;

		public TNode AddToXml(TNode parentNode, bool recursive) => throw new NotImplementedException();

		public TNode ToXml(bool recursive) => throw new NotImplementedException();
	}


	class AndFilter : ITestFilter
	{
		public List<ITestFilter> Filters { get; }

		public AndFilter()
		{
			Filters = new List<ITestFilter>();
		}

		public AndFilter(params ITestFilter[] filters)
		{
			Filters = filters.ToList();
		}

		public AndFilter(IEnumerable<ITestFilter> filters)
		{
			Filters = filters.ToList();
		}

		public TNode AddToXml(TNode parentNode, bool recursive) => throw new NotImplementedException();

		public bool IsExplicitMatch(ITest test)
		{
			for (int i = 0; i < Filters.Count; i++)
			{
				if (!Filters[i].IsExplicitMatch(test))
					return false;
			}
			return true;
		}

		public bool Pass(ITest test)
		{
			for (int i = 0; i < Filters.Count; i++)
			{
				if (!Filters[i].Pass(test))
					return false;
			}
			return true;
		}

		public TNode ToXml(bool recursive) => throw new NotImplementedException();
	}

	class CategoryFilter : BaseFilter
	{
		public List<string> Categories { get; }

		public CategoryFilter()
		{
			Categories = new List<string>();
		}

		public CategoryFilter(IEnumerable<string> categories)
		{
			Categories = categories.ToList();
		}

		public bool MatchAll { get; set; }

		public bool AllowNone { get; set; }

		protected override bool Matches(ITest test, bool parent)
		{
			var categories = test.Properties["Category"];
			if (categories == null || categories.Count == 0)
				return Categories.Count == 0;

			return MatchAll ? Categories.All(categories.Contains) : Categories.Any(categories.Contains);
		}
	}


	class OrFilter : ITestFilter
	{
		public List<ITestFilter> Filters { get; }

		public OrFilter()
		{
			Filters = new List<ITestFilter>();
		}

		public OrFilter(params ITestFilter[] filters)
		{
			Filters = filters.ToList();
		}

		public OrFilter(IEnumerable<ITestFilter> filters)
		{
			Filters = filters.ToList();
		}
		public TNode AddToXml(TNode parentNode, bool recursive) => throw new NotImplementedException();

		public bool IsExplicitMatch(ITest test)
		{
			for (int i = 0; i < Filters.Count; i++)
			{
				if (Filters[i].IsExplicitMatch(test))
					return true;
			}
			return false;
		}

		public bool Pass(ITest test)
		{
			for (int i = 0; i < Filters.Count; i++)
			{
				if (Filters[i].Pass(test))
					return true;
			}
			return false;
		}

		public TNode ToXml(bool recursive) => throw new NotImplementedException();
	}

	class SingleTestFilter : ITestFilter
	{
		public ITest Test { get; set; }

		public Assembly Assembly { get; set; }

		public bool IsExplicitMatch(ITest test)
		{
			if (Assembly != null)
			{
				if (!test.IsSuite && test.TypeInfo?.Assembly != Assembly)
					return false;
				if (test is TestAssembly testAssembly && testAssembly.Assembly != Assembly)
					return false;
			}
			return test.FullName == Test.FullName;
		}

		public bool Pass(ITest test)
		{
			if (Assembly != null)
			{
				if (!test.IsSuite && test.TypeInfo?.Assembly != Assembly)
					return false;

				if (test is TestAssembly testAssembly && testAssembly.Assembly != Assembly)
					return false;
			}


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

	class KeywordFilter : BaseFilter
	{
		string keywords;
		string[][] keywordTokens;

		string[] SplitMatches(string value, string regex) => Regex.Matches(value, regex).OfType<Match>().Select(r => r.Value).ToArray();

		/// <summary>
		/// Gets or sets the keyword string to search for.
		/// </summary>
		/// <remarks>
		/// Supports:
		///	  - '-' prefix to exclude keyword
		///   - Quotes for literal matches e.g. "my test"
		///   - Multiple keywords separated by whitespace
		/// </remarks>
		/// <value>The keywords.</value>
		public string Keywords
		{
			get => keywords;
			set
			{
				keywords = value;
				if (string.IsNullOrWhiteSpace(value))
					keywordTokens = null;
				else
				{
					var searches = SplitMatches(value, @"([-]?""[^""]*"")|((?<=[\s]|^)[^""\s]+(?=[\s]|$))");
					keywordTokens = searches
						.Select(s => SplitMatches(s, @"(^-)|((?<=^-?"").+(?=""$))|([A-Z][^A-Z]*[^A-Z""]?)|((?<!^-"")[^A-Z""][^A-Z]*[^A-Z""])|\w+"))
						.ToArray();
					if (keywordTokens.Length == 1 && keywordTokens[0].Length == 1 && keywordTokens[0][0] == keywords)
						keywordTokens = null;
				}
			}
		}

		protected override bool Matches(ITest test, bool parent)
		{
			if (string.IsNullOrEmpty(keywords))
				return true;

			var name = test.FullName;
			if (keywordTokens != null)
			{
				// poor man's search algo
				bool lastIsUpper = false;
				for (int i = 0; i < keywordTokens.Length; i++)
				{
					var search = keywordTokens[i];
					int index = 0;
					bool inverse = false;

					for (int j = 0; j < search.Length; j++)
					{
						var kw = search[j];
						if (!inverse && j == 0 && kw.Length == 1 && kw[0] == '-')
						{
							if (search.Length == 1) // just a '-', which is invalid
								break;
							if (parent) // only match inverse expressions on test itself or its children.
								return false;
							inverse = true;
							continue;
						}

						var isUpper = kw.Length == 1 && char.IsUpper(kw[0]);
						var idx = name.IndexOf(kw, index, isUpper ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);
						if (idx == -1)
						{
							if (lastIsUpper && isUpper && char.ToUpper(name[index]) == kw[0])
								index++;
							else if (inverse)
							{
								inverse = false;
								break;
							}
							else
								return !parent && inverse;
						}
						else
							index = idx + kw.Length;

						lastIsUpper = isUpper;
					}
					if (inverse)
						return false;
				}
				return true;
			}
			return name.IndexOf(keywords, StringComparison.OrdinalIgnoreCase) >= 0;
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

	public sealed class UnitTestResultEventArgs : EventArgs
	{
		public ITestResult Result { get; private set; }

		public UnitTestResultEventArgs(ITestResult result)
		{
			Result = result;
		}
	}

	public sealed class UnitTestTestEventArgs : EventArgs
	{
		public ITest Test { get; private set; }

		public UnitTestTestEventArgs(ITest test)
		{
			Test = test;
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

	public class UnitTestSource
	{
		public string AssemblyName { get; }
		public Assembly Assembly { get; }
		public UnitTestSource(string assemblyName)
		{
			AssemblyName = assemblyName;
		}

		public UnitTestSource(Assembly assembly)
		{
			Assembly = assembly;
		}

		public static implicit operator UnitTestSource(Assembly assembly) => new UnitTestSource(assembly);

		public static implicit operator UnitTestSource(string assemblyName) => new UnitTestSource(assemblyName);
	}

	public class UnitTestRunner : ITestListener
	{
		MultipleTestResult allresults;
		ObservableCollection<UnitTestSource> sources = new ObservableCollection<UnitTestSource>();
		Queue<ITestAssemblyRunner> runnersToTest;
		ITestFilter testFilter;
		bool isRunning;
		ITestAssemblyRunner currentRunner;
		TaskCompletionSource<ITestResult> tcs;
		UnitTestProgressEventArgs progressArgs;
		List<ITestAssemblyRunner> runnerCache;
		ITestAssemblyBuilder builder = new DefaultTestAssemblyBuilder();

		public IList<UnitTestSource> Sources => sources;

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

		public bool ShowOutput { get; set; }

		public event EventHandler<UnitTestLogEventArgs> Log;

		public event EventHandler<UnitTestProgressEventArgs> Progress;

		public event EventHandler<UnitTestResultEventArgs> TestFinished;

		public event EventHandler<UnitTestTestEventArgs> TestStarted;

		public event EventHandler<EventArgs> IsRunningChanged;

		public UnitTestRunner()
		{
			sources.CollectionChanged += (sender, e) => runnerCache = null;
		}

		public UnitTestRunner(IEnumerable<Assembly> assemblies)
			: this()
		{
			foreach (var assembly in assemblies)
				sources.Add(assembly);
		}

		public UnitTestRunner(IEnumerable<UnitTestSource> sources)
			: this()
		{
			foreach (var source in sources)
				this.sources.Add(source);
		}

		public void WriteLog(string message)
		{
			Log?.Invoke(this, new UnitTestLogEventArgs(message));
		}

		IEnumerable<ITestAssemblyRunner> GetRunners()
		{
			if (runnerCache != null)
				return runnerCache;
			runnerCache = new List<ITestAssemblyRunner>();
			var settings = new Dictionary<string, object>();

			foreach (var source in Sources)
			{
				var runner = new NUnitTestAssemblyRunner(builder);
				if (source.Assembly != null)
					runner.Load(source.Assembly, settings);
				else if (!string.IsNullOrEmpty(source.AssemblyName))
					runner.Load(source.AssemblyName, settings);
				runnerCache.Add(runner);
			}
			return runnerCache;
		}

		public IEnumerable<TestAssembly> GetTests()
		{
			foreach (var runner in GetRunners())
			{
				yield return (TestAssembly)runner.ExploreTests(TestFilter.Empty);
			}
		}

		public IEnumerable<string> GetCategories(ITestFilter filter)
		{
			return GetTests().SelectMany(TestHelpers.GetChildren).Where(filter.Pass).SelectMany(TestHelpers.GetCategories).Distinct();
		}

		public int GetTestCount(ITestFilter filter)
		{
			return GetTests().SelectMany(TestHelpers.GetChildren).Count(r => !r.IsSuite && filter.Pass(r));
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
			runnersToTest = new Queue<ITestAssemblyRunner>(GetRunners().Where(r => testFilter.Pass(r.LoadedTest)));

			var totalTestCount = GetTestCount(testFilter);
			progressArgs = new UnitTestProgressEventArgs(totalTestCount);

			WriteLog($"Total test count: {totalTestCount}");
			TestNextAssembly();
			return tcs.Task;
		}

		class CustomSynchronizationContext : SynchronizationContext
		{
			public override void Post(SendOrPostCallback d, object state)
			{
				Application.Instance.AsyncInvoke(() => d(state));
			}
			public override void Send(SendOrPostCallback d, object state)
			{
				Application.Instance.Invoke(() => d(state));
			}
		}


		void TestNextAssembly()
		{
			lock (this)
			{
				ITestAssemblyRunner nextRunner = null;
				if (runnersToTest?.Count > 0)
					nextRunner = runnersToTest.Dequeue();
				if (nextRunner == null)
				{
					WriteLog(allresults.FailCount > 0 ? "FAILED" : "PASSED");
					WriteLog($"\tPassed: {allresults.PassCount}, Failed: {allresults.FailCount}, Skipped: {allresults.SkipCount}, Inconclusive: {allresults.InconclusiveCount}, Warnings: {allresults.WarningCount}");
					WriteLog($"\tDuration: {allresults.Duration}");
					currentRunner = null;
					IsRunning = false;
					tcs.SetResult(allresults);
					return;
				}
				var lastSync = SynchronizationContext.Current;
				try
				{
					// prevent nunit from trying to use the WPF or WinForms context in a bad way..
					SynchronizationContext.SetSynchronizationContext(new CustomSynchronizationContext());
					new TestExecutionContext.AdhocContext().EstablishExecutionEnvironment();
					currentRunner = nextRunner;
					nextRunner.RunAsync(this, testFilter);
				}
				catch (Exception ex)
				{
					WriteLog($"Error running tests: {ex}");
					tcs.SetException(ex);
					currentRunner = null;
					IsRunning = false;
				}
				finally
				{
					SynchronizationContext.SetSynchronizationContext(lastSync);
				}
			}
		}

		public void StopTests()
		{
			lock (this)
			{
				if (IsRunning)
				{
					WriteLog("Stopping tests...");
					runnersToTest?.Clear();
					currentRunner?.StopRun(true);
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
			TestStarted?.Invoke(this, new UnitTestTestEventArgs(test));
			if (!test.IsSuite)
				WriteLog(test.FullName);
		}

		void ITestListener.TestFinished(ITestResult result)
		{
			TestFinished?.Invoke(this, new UnitTestResultEventArgs(result));
			if (!result.Test.IsSuite)
			{
				progressArgs.AddResult(result);
				Progress?.Invoke(this, progressArgs);

				// ITestListener.ShowOutput is not currently called, we need to redirect console and trace output
				if (ShowOutput && !string.IsNullOrEmpty(result.Output))
					WriteLog(result.Output);

				if (result.AssertionResults.Count > 0)
				{
					foreach (var assertion in result.AssertionResults)
					{
						if (assertion.Status == AssertionStatus.Passed)
							continue;
						if (!string.IsNullOrEmpty(result.StackTrace))
							WriteLog($"{assertion.Status}: {assertion.Message}\n{assertion.StackTrace}");
						else
							WriteLog($"{assertion.Status}: {assertion.Message}");
					}
				}
				else if (result.ResultState.Status != TestStatus.Passed && result.ResultState.Status != TestStatus.Skipped)
				{
					if (!string.IsNullOrEmpty(result.StackTrace))
						WriteLog($"{result.ResultState.Status}: {result.Message}\n{result.StackTrace}");
					else
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
			if (ShowOutput)
				WriteLog(output.ToString());
		}

		void ITestListener.SendMessage(TestMessage message)
		{
			if (ShowOutput)
				WriteLog(message.ToString());
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

	public class AsyncQueue
	{
		List<Action> actions = new List<Action>();
		Dictionary<string, Action> namedActions = new Dictionary<string, Action>();
		UITimer timer;
		double delay = 0.2;
		bool isQueued;

		public double Delay
		{
			get => delay;
			set
			{
				delay = value;
				if (timer != null)
					timer.Interval = delay;
			}
		}

		public void Add(string name, Action action)
		{
			lock (this)
			{
				namedActions[name] = action;
				Start();
			}
		}

		public void Add(Action action)
		{
			lock (this)
			{
				actions.Add(action);
				Start();
			}
		}

		void Start()
		{
			if (!isQueued)
			{
				isQueued = true;
				/**
				Application.Instance.AsyncInvoke(FlushQueue);
				/**/
				Application.Instance.AsyncInvoke(StartTimer);
				/**/
			}
		}

		void StartTimer()
		{
			if (timer == null)
			{
				timer = new UITimer { Interval = delay };
				timer.Elapsed += Timer_Elapsed;
			}
			timer.Start();
		}

		void Timer_Elapsed(object sender, EventArgs e) => FlushQueue();

		void FlushQueue()
		{
			List<Action> actionList;
			lock (this)
			{
				actionList = actions;
				actionList.AddRange(namedActions.Values);
				namedActions.Clear();
				actions = new List<Action>();
				isQueued = false;
				timer?.Stop();
			}

			foreach (var action in actionList)
			{
				action();
			}
		}
	}

	public class UnitTestPanel : Panel, INotifyPropertyChanged
	{
		TreeGridView tree;
		Button startButton;
		Button stopButton;
		Control filterControls;
		SearchBox search;
		TextArea log;
		UnitTestProgressBar progress;
		Label testCountLabel;
		bool hasLogged;
		UITimer timer;
		Panel customFilterControls;
		ITestFilter customFilter;
		Dictionary<object, UnitTestItem> testMap;
		Dictionary<TestStatus, Image> stateImages = new Dictionary<TestStatus, Image>();
		Image notRunStateImage;
		Image runningStateImage;
		ConcurrentDictionary<ITest, ITestResult> lastResultMap = new ConcurrentDictionary<ITest, ITestResult>();
		ConcurrentDictionary<ITest, IList<ITestResult>> allResultsMap = new ConcurrentDictionary<ITest, IList<ITestResult>>();
		AsyncQueue asyncQueue = new AsyncQueue();
		IEnumerable<ITestFilter> statusFilters = Enumerable.Empty<ITestFilter>();
		IEnumerable<string> includeCategories = Enumerable.Empty<string>();
		IEnumerable<string> excludeCategories = Enumerable.Empty<string>();
		IList<string> availableCategories;

		public event EventHandler<UnitTestLogEventArgs> Log;
		public event PropertyChangedEventHandler PropertyChanged;

		public new Control Content
		{
			get => customFilterControls.Content;
			set => customFilterControls.Content = value;
		}

		public ITestFilter CustomFilter
		{
			get => customFilter;
			set
			{
				customFilter = value;
				if (Loaded)
					PopulateTree();
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether to merge nodes with only a single child into its parent.
		/// </summary>
		/// <value><c>true</c> to merge single nodes; otherwise, <c>false</c>.</value>
		public bool MergeSingleNodes { get; set; } = true;

		public UnitTestRunner Runner { get; }

		public UnitTestPanel(bool showLog = true)
			: this(new UnitTestRunner(), showLog)
		{
		}

		class NotRunFilter : ITestFilter
		{
			Func<ITest, ITestResult> LookupResult { get; }

			public NotRunFilter(Func<ITest, ITestResult> lookupResult)
			{
				LookupResult = lookupResult;
			}

			public override string ToString() => "Not Run";

			public bool Pass(ITest test)
			{
				if (test.IsSuite)
					return test.GetChildren(false).Any(Pass);
				return IsExplicitMatch(test);
			}

			public bool IsExplicitMatch(ITest test) => LookupResult(test) == null;

			public TNode ToXml(bool recursive) => throw new NotImplementedException();

			public TNode AddToXml(TNode parentNode, bool recursive) => throw new NotImplementedException();
		}

		class StatusFilter : ITestFilter
		{
			Func<ITest, ITestResult> _lookupResult;

			public string Name => Status.ToString();

			public TestStatus Status { get; }

			public StatusFilter(Func<ITest, ITestResult> lookupResult, TestStatus status)
			{
				Status = status;
				_lookupResult = lookupResult;
			}

			public override string ToString() => Name;

			public bool Pass(ITest test)
			{
				if (test.IsSuite)
					return test.GetChildren(false).Any(Pass);
				return IsExplicitMatch(test);
			}

			public bool IsExplicitMatch(ITest test) => _lookupResult(test)?.ResultState.Status == Status;

			public TNode ToXml(bool recursive) => throw new NotImplementedException();

			public TNode AddToXml(TNode parentNode, bool recursive) => throw new NotImplementedException();
		}

		IEnumerable<ITestFilter> GetOptionalFilters()
		{
			foreach (var value in Enum.GetValues(typeof(TestStatus)).Cast<TestStatus>())
			{
				yield return new StatusFilter(LookupResult, value);
			}

			yield return new NotRunFilter(LookupResult);
		}

		ITestResult LookupResult(ITest test)
		{
			if (!lastResultMap.TryGetValue(test, out var result))
				return null;
			return result;
		}

		IEnumerable<ITestFilter> StatusFilters
		{
			get => statusFilters;
			set
			{
				statusFilters = value.ToList();
				PopulateTree();
			}
		}

		IEnumerable<string> IncludeCategories
		{
			get => includeCategories;
			set
			{
				includeCategories = value.ToList();
				PopulateTree();
			}
		}

		IEnumerable<string> ExcludeCategories
		{
			get => excludeCategories;
			set
			{
				excludeCategories = value.ToList();
				PopulateTree();
			}
		}

		IEnumerable<string> AvailableCategories
		{
			get => availableCategories;
			set
			{
				if (ReferenceEquals(value, availableCategories))
					return;
				var newCategories = value.ToList();
				if (availableCategories?.SequenceEqual(newCategories) != true)
				{
					availableCategories = newCategories;
					OnPropertyChanged(nameof(AvailableCategories));
					OnPropertyChanged(nameof(HasCategories));
				}
			}
		}

		bool HasCategories => availableCategories?.Count > 0;

		void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));


		public UnitTestPanel(UnitTestRunner runner, bool showLog = true)
		{
			this.Runner = runner;

			customFilterControls = new Panel();

			progress = new UnitTestProgressBar();

			runner.Log += Runner_Log;
			runner.Progress += Runner_Progress;
			runner.TestFinished += Runner_TestFinished;
			runner.TestStarted += Runner_TestStarted;
			runner.IsRunningChanged += Runner_IsRunningChanged;

			timer = new UITimer();
			timer.Interval = 0.5;
			timer.Elapsed += (sender, e) => PerformSearch();

			testCountLabel = new Label {
				VerticalAlignment = VerticalAlignment.Center
			};

			startButton = new Button { Text = "Start" };
			startButton.Click += (s, e) => RunTests();

			stopButton = new Button { Text = "Stop", Enabled = false };
			stopButton.Click += (s, e) => runner?.StopTests();

			search = new SearchBox();
			search.Text = TestApplication.Settings.LastUnitTestFilter;
			search.PlaceholderText = "Filter(s)";
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
				TestApplication.Settings.LastUnitTestFilter = search.Text;
				timer.Start();
			};


			tree = new TreeGridView { ShowHeader = false, Size = new Size(400, -1) };
			tree.Columns.Add(new GridColumn
			{
				DataCell = new ImageTextCell
				{
					TextBinding = Binding.Property((UnitTestItem m) => m.Text),
					ImageBinding = Binding.Property((UnitTestItem m) => m.Image)
				}
			});

			tree.Activated += (sender, e) =>
			{
				if (runner.IsRunning)
					return;
				var item = (UnitTestItem)tree.SelectedItem;
				if (item != null)
				{
					var filter = item.Filter;
					if (filter != null)
					{
						RunTests(filter);
					}
				}
			};

			var showOuputCheckBox = new CheckBox { Text = "Show Output" };
			showOuputCheckBox.CheckedBinding.Bind(runner, r => r.ShowOutput);

			var buttons = new TableLayout
			{
				Padding = new Padding(10, 0),
				Spacing = new Size(5, 5),
				Rows = { new TableRow(startButton, stopButton, showOuputCheckBox, null, testCountLabel) }
			};

			var statusChecks = new CheckBoxList
			{
				Spacing = new Size(2, 2),
				Orientation = Orientation.Horizontal,
				DataStore = GetOptionalFilters()
			};
			statusChecks.SelectedValuesBinding.CastItems((ITestFilter)null).Bind(this, c => c.StatusFilters);

			var includeChecks = new CheckBoxList
			{
				Spacing = new Size(2, 2),
			};
			includeChecks.Bind(c => c.DataStore, this, c => c.AvailableCategories);
			includeChecks.SelectedValuesBinding.CastItems((string)null).Bind(this, c => c.IncludeCategories);
			includeChecks.Bind(c => c.Visible, this, c => c.HasCategories);

			var includeLabel = new Label { Text = "Include" };
			includeLabel.Bind(c => c.Visible, this, c => c.HasCategories);

			var excludeChecks = new CheckBoxList
			{
				Spacing = new Size(2, 2),
			};
			excludeChecks.Bind(c => c.DataStore, this, c => c.AvailableCategories);
			excludeChecks.SelectedValuesBinding.CastItems((string)null).Bind(this, c => c.ExcludeCategories);
			excludeChecks.Bind(c => c.Visible, this, c => c.HasCategories);

			var excludeLabel = new Label { Text = "Exclude" };
			excludeLabel.Bind(c => c.Visible, this, c => c.HasCategories);

			filterControls = new TableLayout
			{
				Spacing = new Size(5, 5),
				Rows = {
					new TableRow("Show", statusChecks, null),
					new TableRow(includeLabel, includeChecks),
					new TableRow(excludeLabel, excludeChecks)
				}
			};

			var allFilters = new Panel
			{
				Padding = new Padding(10, 0),
				Content = new Scrollable
				{
					Border = BorderType.None,
					Content = new TableLayout { Rows = { filterControls, customFilterControls } }
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
						Padding = new Padding(0, 10, 0, 0),
						Spacing = new Size(5, 5),
						Rows = { allFilters, search, tree }
					},

					Panel2 = new TableLayout
					{
						Padding = new Padding(0, 10, 0, 0),
						Spacing = new Size(5, 5),
						Rows = { buttons, progress, log }
					}
				};
			}
			else
			{
				Size = new Size(400, 400);
				base.Content = new TableLayout
				{
					Padding = new Padding(0, 10, 0, 0),
					Spacing = new Size(5, 5),
					Rows = { buttons, allFilters, search, progress, tree }
				};
			}
		}

		private void PerformSearch()
		{
			timer.Stop();
			PopulateTree();
		}

		List<UnitTestLogEventArgs> logQueue = new List<UnitTestLogEventArgs>();

		void Runner_IsRunningChanged(object sender, EventArgs e)
		{
			var running = Runner.IsRunning;
			Application.Instance.Invoke(() =>
			{
				startButton.Enabled = !running;
				stopButton.Enabled = running;
				search.ReadOnly = running;
				filterControls.Enabled = !running;

				if (!running && StatusFilters.Any())
					PopulateTree();
			});
		}

		void Runner_Log(object sender, UnitTestLogEventArgs e)
		{
			lock (logQueue)
			{
				logQueue.Add(e);
			}
			asyncQueue.Add("log", () =>
			{
				List<UnitTestLogEventArgs> logQueueCopy;
				lock (logQueue)
				{
					logQueueCopy = logQueue;
					logQueue = new List<UnitTestLogEventArgs>();
				}
				var sb = new StringBuilder();

				foreach (var logEvent in logQueueCopy)
				{
					if (log != null)
					{
						if (hasLogged)
							sb.AppendLine();
						else
							hasLogged = true;
						sb.Append(logEvent.Message);
					}
					Log?.Invoke(this, logEvent);
				}

				log?.Append(sb.ToString(), true);
			});
		}

		void Runner_Progress(object sender, UnitTestProgressEventArgs e)
		{
			var progressAmount = e.TestCaseCount > 0 ? (float)e.CompletedCount / e.TestCaseCount : 0;
			var color = e.FailCount > 0 ? Colors.Red : e.WarningCount > 0 ? Colors.Yellow : Colors.Green;
			asyncQueue.Add("progress", () =>
			{
				progress.Progress = progressAmount;
				progress.Color = color;
			});
		}


		void Runner_TestStarted(object sender, UnitTestTestEventArgs e)
		{
			var test = e.Test;
			if (testMap.TryGetValue(test, out var treeItem))
			{
				if (lastResultMap.ContainsKey(test))
					lastResultMap.TryRemove(test, out var result);
				asyncQueue.Add(() =>
				{
					treeItem.Image = RunningStateImage;
					tree.ReloadItem(treeItem, false);
				});
			}
		}

		IList<ITestResult> GetAllResults(ITest test)
		{
			if (allResultsMap.TryGetValue(test, out var list))
				return list;

			list = new List<ITestResult>();
			if (allResultsMap.TryAdd(test, list))
				return list;

			if (allResultsMap.TryGetValue(test, out list))
				return list;

			throw new InvalidOperationException($"All results does not have an entry for {test.FullName}");
		}

		void Runner_TestFinished(object sender, UnitTestResultEventArgs e)
		{
			var test = e.Result.Test;
			var result = e.Result;
			if (testMap.TryGetValue(test, out var treeItem))
			{
				lastResultMap[test] = result;
				GetAllResults(test).Add(result);

				asyncQueue.Add(() =>
				{
					treeItem.Image = GetStateImage(result);
					tree.ReloadItem(treeItem, false);
				});
			}
		}

		Image NotRunStateImage => notRunStateImage ?? (notRunStateImage = CreateImage(Colors.Silver, Colors.Black, null));

		Image RunningStateImage => runningStateImage ?? (runningStateImage = CreateImage(Colors.Blue, Colors.White, "↻"));

		Image GetStateImage(ITestResult result) => result != null ? GetStateImage(result.ResultState.Status) : NotRunStateImage;

		Image GetStateImage(TestStatus status)
		{
			if (stateImages.TryGetValue(status, out var image))
				return image;
			image = CreateImage(status);
			stateImages[status] = image;
			return image;
		}


		Image CreateImage(TestStatus status)
		{
			switch (status)
			{
				case TestStatus.Warning:
					return CreateImage(Colors.Yellow, Colors.Black, "!");
				case TestStatus.Failed:
					return CreateImage(Colors.Red, (g, b) =>
					{
						var offset = 10;
						var pen = new Pen(Colors.White, 4);
						g.DrawLine(pen, offset, offset, b.Width - offset, b.Height - offset);
						g.DrawLine(pen, b.Width - offset, offset, offset, b.Height - offset);
					});
				case TestStatus.Inconclusive:
					return CreateImage(Colors.Yellow, Colors.Black, "?");
				case TestStatus.Skipped:
					return CreateImage(Colors.Yellow, Colors.Black, "»");
				case TestStatus.Passed:
					return CreateImage(Colors.Green, Colors.White, "✓");
				default:
					throw new NotSupportedException();
			}
		}

		static Image CreateImage(Color color, Action<Graphics, Bitmap> draw)
		{
			var bmp = new Bitmap(32, 32, PixelFormat.Format32bppRgba);
			using (var g = new Graphics(bmp))
			{
				var r = new RectangleF(Point.Empty, bmp.Size);
				r.Inflate(-1, -1);
				g.FillEllipse(color, r);
				draw?.Invoke(g, bmp);
			}
			return bmp.WithSize(16, 16);
		}

		static Image CreateImage(Color color, Color textcolor, string text)
		{
			return CreateImage(color, (g, b) =>
			{
				var r = new RectangleF(Point.Empty, b.Size);
				r.Inflate(-1, -1);
				if (text != null)
				{
					var font = SystemFonts.Default(SystemFonts.Default().Size * 2);
					var size = g.MeasureString(font, text);
					g.DrawText(font, textcolor, r.Location + (PointF)(r.Size - size) / 2, text);
				}
			});
		}

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

		ITestFilter CreateFilter(ITestFilter testFilter = null)
		{
			var filters = GetFilters(testFilter).ToList();
			if (filters.Count > 1)
				return new AndFilter(filters);

			if (filters.Count == 0)
				return TestFilter.Empty;

			return filters[0];
		}

		IEnumerable<ITestFilter> GetFilters(ITestFilter testFilter)
		{
			if (customFilter != null)
				yield return customFilter;

			if (IncludeCategories.Any())
				yield return new CategoryFilter(IncludeCategories);

			if (ExcludeCategories.Any())
				yield return new NotFilter(new CategoryFilter(ExcludeCategories) { ChildCanMatch = false });

			if (testFilter != null)
				yield return testFilter;

			if (StatusFilters.Any())
				yield return new OrFilter(StatusFilters.OfType<ITestFilter>());

			if (!string.IsNullOrWhiteSpace(search.Text))
				yield return new KeywordFilter { Keywords = search.Text };
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
			Task.Run(() =>
			{
				var map = new Dictionary<object, UnitTestItem>();
				var tests = Runner.GetTests();
		  // always show all categories
		  var categories = AvailableCategories ?? Runner.GetCategories(TestFilter.Empty).OrderBy(r => r);
				var totalTestCount = Runner.GetTestCount(filter);
				var testSuites = tests.Select(suite => ToTree(suite.Assembly, suite, filter, map)).Where(r => r != null);
				var treeData = new TreeGridItem(testSuites);
				Application.Instance.AsyncInvoke(() =>
		  {
				  AvailableCategories = categories;
				  testCountLabel.Text = $"{totalTestCount} Tests";
				  testMap = map;
				  tree.DataStore = treeData;
			  });
			});
		}

		class UnitTestItem : TreeGridItem
		{
			public string Text { get; set; }
			public ITest Test { get; set; }
			public Image Image { get; set; }
			public ITestFilter Filter { get; set; }
		}

		TreeGridItem ToTree(Assembly assembly, ITest test, ITestFilter filter, IDictionary<object, UnitTestItem> map)
		{
			// add a test
			var name = test.Name;
			var isTestAssembly = test is TestAssembly;
			if (isTestAssembly)
			{
				var an = new AssemblyName(Path.GetFileNameWithoutExtension(test.Name));
				name = an.Name;
			}

			if (!filter.Pass(test))
				return null;

			lastResultMap.TryGetValue(test, out var result);
			var worstChildResult = test.GetChildren()
				.Select(t => lastResultMap.TryGetValue(t, out var r) ? r : null)
				.Where(r => r != null)
				.OrderByDescending(r => r.ResultState.Status)
				.FirstOrDefault();
			if (worstChildResult?.ResultState.Status > result?.ResultState.Status)
				result = worstChildResult;

			var item = new UnitTestItem
			{
				Text = name,
				Test = test,
				Image = GetStateImage(result),
				Filter = new SingleTestFilter { Test = test, Assembly = assembly }
			};
			map.Add(test, item);
			if (test.HasChildren)
			{
				item.Expanded = !(test is ParameterizedMethodSuite);
				foreach (var child in test.Tests)
				{
					var treeItem = ToTree(assembly, child, filter, map);
					if (treeItem != null)
						item.Children.Add(treeItem);
				}
				if (MergeSingleNodes)
				{
					while (item.Children.Count == 1)
					{
						// collapse test nodes
						var child = item.Children[0] as UnitTestItem;
						if (child.Children.Count == 0)
							break;
						if (!child.Text.StartsWith(item.Text, StringComparison.Ordinal))
						{
							var separator = isTestAssembly ? ":" : ".";

							child.Text = $"{item.Text}{separator}{child.Text}";
						}
						child.Expanded |= (test is TestAssembly);
						item = child;
					}
				}
				if (item.Children.Count == 0)
					return null;
			}
			return item;
		}
	}
}