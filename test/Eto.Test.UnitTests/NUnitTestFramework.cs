#if MACOS
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Eto.Test.Sections;
using Microsoft.Testing.Extensions.TrxReport.Abstractions;
using Microsoft.Testing.Platform.CommandLine;
using Microsoft.Testing.Platform.Extensions;
using Microsoft.Testing.Platform.Extensions.CommandLine;
using Microsoft.Testing.Platform.Extensions.Messages;
using Microsoft.Testing.Platform.Extensions.TestFramework;
using Microsoft.Testing.Platform.Requests;
using Microsoft.Testing.Platform.Services;
using NUnit.Framework.Interfaces;

namespace Eto.Test.UnitTests;

/// <summary>
/// Runs the NUnit tests in-process, through NUnit's framework API, and reports them to the testing platform.
/// </summary>
/// <remarks>
/// Used instead of NUnit3TestAdapter's testing platform bridge (see <see cref="Program.Main"/>) on the
/// net*-macos build. That bridge goes through the NUnit engine, whose driver builds an
/// <see cref="System.Runtime.Loader.AssemblyDependencyResolver"/> for every test assembly it loads - which
/// requires hostpolicy to have been initialized by <c>corehost_main</c>. A macOS app bundle starts the
/// runtime from its own native launcher instead, so that call always fails with "Hostpolicy must be
/// initialized and corehost_main must have been called ..." and every test assembly fails to load.
///
/// Going through NUnit's framework API loads nothing by path - the tests are built from the assemblies that
/// are already loaded - so it works under the bundle's host. It's the same way the Eto.Test app's own unit
/// test section runs tests, and shares <see cref="UnitTestRunner"/> with it.
/// </remarks>
sealed class NUnitTestFramework : ITestFramework, IDataProducer
{
	readonly Func<IEnumerable<Assembly>> _getTestAssemblies;
	readonly ICommandLineOptions _commandLineOptions;

	public NUnitTestFramework(Func<IEnumerable<Assembly>> getTestAssemblies, IServiceProvider serviceProvider)
	{
		_getTestAssemblies = getTestAssemblies;
		_commandLineOptions = serviceProvider.GetCommandLineOptions();
	}

	public string Uid => nameof(NUnitTestFramework);

	public string Version => "1.0.0";

	public string DisplayName => "NUnit (in-process)";

	public string Description => "Runs NUnit tests in-process from the loaded test assemblies.";

	public Type[] DataTypesProduced => new[] { typeof(TestNodeUpdateMessage) };

	public Task<bool> IsEnabledAsync() => Task.FromResult(true);

	public Task<CreateTestSessionResult> CreateTestSessionAsync(CreateTestSessionContext context)
		=> Task.FromResult(new CreateTestSessionResult { IsSuccess = true });

	public Task<CloseTestSessionResult> CloseTestSessionAsync(CloseTestSessionContext context)
		=> Task.FromResult(new CloseTestSessionResult { IsSuccess = true });

	public async Task ExecuteRequestAsync(ExecuteRequestContext context)
	{
		try
		{
			switch (context.Request)
			{
				case DiscoverTestExecutionRequest discover:
					await DiscoverTestsAsync(context, discover);
					break;
				case RunTestExecutionRequest run:
					await RunTestsAsync(context, run);
					break;
			}
		}
		finally
		{
			context.Complete();
		}
	}

	async Task DiscoverTestsAsync(ExecuteRequestContext context, DiscoverTestExecutionRequest request)
	{
		var filter = CreateFilter(request.Filter);
		var runner = new UnitTestRunner(_getTestAssemblies());
		var sessionUid = request.Session.SessionUid;

		foreach (var test in runner.GetTests().SelectMany(GetTestCases).Where(filter.Pass))
		{
			var node = CreateNode(test, test.FullName, test.Name);
			node.Properties.Add(DiscoveredTestNodeStateProperty.CachedInstance);
			await context.MessageBus.PublishAsync(this, new TestNodeUpdateMessage(sessionUid, node));
		}
	}

	async Task RunTestsAsync(ExecuteRequestContext context, RunTestExecutionRequest request)
	{
		var filter = CreateFilter(request.Filter);
		var runner = new UnitTestRunner(_getTestAssemblies());
		var sessionUid = request.Session.SessionUid;

		// The results arrive on NUnit's own thread, so queue them up and publish from here instead of
		// blocking that thread on the message bus.
		var results = new ConcurrentQueue<TestNodeUpdateMessage>();
		runner.TestFinished += (sender, e) =>
		{
			foreach (var node in CreateResultNodes(e.Result))
				results.Enqueue(new TestNodeUpdateMessage(sessionUid, node));
		};

		using var stopOnCancel = context.CancellationToken.Register(runner.StopTests);

		var run = runner.RunTestsAsync(filter);
		while (!run.IsCompleted)
		{
			await PublishResultsAsync(context, results);
			await Task.WhenAny(run, Task.Delay(TimeSpan.FromMilliseconds(100)));
		}
		// observe any failure from the run itself, then publish whatever it produced before failing
		try
		{
			await run;
		}
		finally
		{
			await PublishResultsAsync(context, results);
		}
	}

	async Task PublishResultsAsync(ExecuteRequestContext context, ConcurrentQueue<TestNodeUpdateMessage> results)
	{
		while (results.TryDequeue(out var result))
			await context.MessageBus.PublishAsync(this, result);
	}

	/// <summary>
	/// Turns a finished NUnit result into the nodes to report, if any.
	/// </summary>
	/// <remarks>
	/// Only test cases are reported, with one exception: when a suite fails in its own OneTimeSetUp/TearDown
	/// there is no test case result to carry the failure, so report the suite itself to avoid losing it.
	/// </remarks>
	IEnumerable<TestNode> CreateResultNodes(ITestResult result)
	{
		if (!result.Test.IsSuite)
		{
			yield return CreateResultNode(result, result.FullName, result.Name);
			yield break;
		}

		if (result.ResultState.Status == TestStatus.Failed
			&& (result.ResultState.Site == FailureSite.SetUp || result.ResultState.Site == FailureSite.TearDown))
		{
			var what = result.ResultState.Site == FailureSite.SetUp ? "OneTimeSetUp" : "OneTimeTearDown";
			yield return CreateResultNode(result, $"{result.FullName}.{what}", $"{result.Name} ({what})");
		}
	}

	TestNode CreateResultNode(ITestResult result, string uid, string displayName)
	{
		var node = CreateNode(result.Test, uid, displayName);
		var properties = node.Properties;

		switch (result.ResultState.Status)
		{
			case TestStatus.Passed:
				properties.Add(PassedTestNodeStateProperty.CachedInstance);
				break;
			case TestStatus.Failed:
				properties.Add(new FailedTestNodeStateProperty(new NUnitFailureException(result), result.Message));
				properties.Add(new TrxExceptionProperty(result.Message, result.StackTrace));
				break;
			default:
				// NUnit's Skipped, Inconclusive and Warning all mean "didn't pass, didn't fail"
				properties.Add(new SkippedTestNodeStateProperty(result.Message ?? result.ResultState.Status.ToString()));
				break;
		}

		properties.Add(new TimingProperty(new TimingInfo(result.StartTime, result.EndTime, TimeSpan.FromSeconds(result.Duration))));
		return node;
	}

	static TestNode CreateNode(ITest test, string uid, string displayName)
	{
		var properties = new PropertyBag();

		var method = test.Method?.MethodInfo;
		if (method?.DeclaringType is Type declaringType)
		{
			properties.Add(new TestMethodIdentifierProperty(
				declaringType.Assembly.FullName ?? string.Empty,
				declaringType.Namespace ?? string.Empty,
				declaringType.Name,
				method.Name,
				method.GetGenericArguments().Length,
				method.GetParameters().Select(p => p.ParameterType.FullName ?? p.ParameterType.Name).ToArray(),
				method.ReturnType.FullName ?? method.ReturnType.Name));
		}

		properties.Add(new TrxFullyQualifiedTypeNameProperty(test.ClassName ?? test.FullName));

		var categories = GetCategories(test).ToArray();
		if (categories.Length > 0)
			properties.Add(new TrxCategoriesProperty(categories));

		return new TestNode { Uid = new TestNodeUid(uid), DisplayName = displayName, Properties = properties };
	}

	ITestFilter CreateFilter(ITestExecutionFilter requestFilter)
	{
		// a filter for specific tests (e.g. running a selection from an IDE) always wins over --filter
		if (requestFilter is TestNodeUidListFilter uidList)
		{
			var uids = new HashSet<string>(uidList.TestNodeUids.Select(uid => uid.Value), StringComparer.Ordinal);
			return new NUnitFilter(test => uids.Contains(test.FullName), isExplicitMatch: true);
		}

		if (_commandLineOptions.TryGetOptionArgumentList(FilterCommandLineOptionsProvider.FilterOptionName, out var arguments)
			&& arguments.Length > 0
			&& !string.IsNullOrWhiteSpace(arguments[0]))
		{
			return new NUnitFilter(FilterExpression.Parse(arguments[0]), isExplicitMatch: false);
		}

		return new NUnitFilter(test => true, isExplicitMatch: false);
	}

	internal static IEnumerable<ITest> GetTestCases(ITest test)
	{
		if (!test.IsSuite)
		{
			yield return test;
			yield break;
		}
		foreach (var child in test.Tests)
		{
			foreach (var testCase in GetTestCases(child))
				yield return testCase;
		}
	}

	/// <summary>
	/// Gets the categories of a test, including the ones it inherits from its fixture(s).
	/// </summary>
	internal static IEnumerable<string> GetCategories(ITest? test)
	{
		for (; test != null; test = test.Parent)
		{
			var categories = test.Properties["Category"];
			if (categories == null)
				continue;
			foreach (var category in categories.OfType<string>())
				yield return category;
		}
	}

	/// <summary>
	/// Carries an NUnit failure to the testing platform, which reports failures as exceptions.
	/// </summary>
	sealed class NUnitFailureException : Exception
	{
		readonly string? _stackTrace;

		public NUnitFailureException(ITestResult result)
			: base(result.Message)
		{
			_stackTrace = result.StackTrace;
		}

		public override string? StackTrace => _stackTrace;
	}
}

/// <summary>
/// An NUnit filter for a predicate on test cases. A suite passes when any of its test cases do, so that
/// NUnit keeps descending into it.
/// </summary>
sealed class NUnitFilter : ITestFilter
{
	readonly Func<ITest, bool> _matches;
	readonly bool _isExplicitMatch;

	public NUnitFilter(Func<ITest, bool> matches, bool isExplicitMatch)
	{
		_matches = matches;
		_isExplicitMatch = isExplicitMatch;
	}

	public bool Pass(ITest test)
		=> test.IsSuite ? NUnitTestFramework.GetTestCases(test).Any(_matches) : _matches(test);

	// only tests picked out by name are an explicit match, so that [Explicit] tests still need to be
	// selected directly rather than being run by a category or name filter
	public bool IsExplicitMatch(ITest test) => _isExplicitMatch && !test.IsSuite && _matches(test);

	public TNode AddToXml(TNode parentNode, bool recursive) => throw new NotSupportedException();

	public TNode ToXml(bool recursive) => throw new NotSupportedException();
}

/// <summary>
/// Parses the --filter expressions used with `dotnet test`, e.g.
/// <c>FullyQualifiedName~Grid&amp;TestCategory!=ManualTest</c>.
/// </summary>
/// <remarks>
/// Supports <c>FullyQualifiedName</c>, <c>Name</c> and <c>TestCategory</c> with the <c>=</c>, <c>!=</c>,
/// <c>~</c> and <c>!~</c> operators, combined with <c>&amp;</c> and <c>|</c> (<c>&amp;</c> binds tighter).
/// A bare value with no operator, e.g. <c>BrushTests</c>, matches the fully qualified name by substring.
/// Parentheses are not supported.
/// </remarks>
static class FilterExpression
{
	public static Func<ITest, bool> Parse(string expression)
	{
		var alternatives = expression.Split('|')
			.Select(ParseConjunction)
			.ToArray();
		return test => alternatives.Any(matches => matches(test));
	}

	static Func<ITest, bool> ParseConjunction(string expression)
	{
		var terms = expression.Split('&')
			.Select(ParseTerm)
			.ToArray();
		return test => terms.All(matches => matches(test));
	}

	static Func<ITest, bool> ParseTerm(string term)
	{
		term = term.Trim();

		var (property, op, value) = SplitTerm(term);
		var negate = op is "!=" or "!~";
		var exact = op is "=" or "!=";

		Func<ITest, bool> matches = property.ToLowerInvariant() switch
		{
			"testcategory" or "category" => test => NUnitTestFramework.GetCategories(test).Any(category => Matches(category, value, exact)),
			"name" => test => Matches(test.Name, value, exact),
			_ => test => Matches(test.FullName, value, exact)
		};

		return negate ? test => !matches(test) : matches;
	}

	static (string Property, string Operator, string Value) SplitTerm(string term)
	{
		foreach (var op in new[] { "!=", "!~", "=", "~" })
		{
			var index = term.IndexOf(op, StringComparison.Ordinal);
			if (index >= 0)
				return (term.Substring(0, index).Trim(), op, term.Substring(index + op.Length).Trim());
		}
		// no operator: a name to look for anywhere in the fully qualified name
		return ("FullyQualifiedName", "~", term);
	}

	static bool Matches(string? candidate, string value, bool exact)
	{
		if (candidate == null)
			return false;
		return exact
			? string.Equals(candidate, value, StringComparison.Ordinal)
			: candidate.Contains(value, StringComparison.Ordinal);
	}
}

/// <summary>
/// Adds the --filter option, which the testing platform only defines for the VSTest bridge.
/// </summary>
sealed class FilterCommandLineOptionsProvider : ICommandLineOptionsProvider
{
	public const string FilterOptionName = "filter";

	public string Uid => nameof(FilterCommandLineOptionsProvider);

	public string Version => "1.0.0";

	public string DisplayName => "NUnit filter";

	public string Description => "Adds the --filter option for the in-process NUnit runner.";

	public Task<bool> IsEnabledAsync() => Task.FromResult(true);

	public IReadOnlyCollection<CommandLineOption> GetCommandLineOptions() => new[]
	{
		new CommandLineOption(
			FilterOptionName,
			"Only run the tests matching the expression, e.g. \"FullyQualifiedName~Grid&TestCategory!=ManualTest\"",
			ArgumentArity.ExactlyOne,
			isHidden: false)
	};

	public Task<ValidationResult> ValidateOptionArgumentsAsync(CommandLineOption commandOption, string[] arguments)
		=> ValidationResult.ValidTask;

	public Task<ValidationResult> ValidateCommandLineOptionsAsync(ICommandLineOptions commandLineOptions)
		=> ValidationResult.ValidTask;
}

/// <summary>
/// Reports that <see cref="NUnitTestFramework"/> publishes the properties the trx report needs.
/// </summary>
sealed class TrxReportCapability : ITrxReportCapability
{
	public bool IsSupported => true;

	public void Enable()
	{
	}
}
#endif
