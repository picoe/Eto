// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt
// modified to remove use of CodeBase which is unsupported in single file applications

using System.Security;
using NUnit;
using NUnit.Framework;
using NUnit.Framework.Api;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;

namespace Eto.Test.UnitTests
{
	class PreFilter : IPreFilter
	{
		public bool IsMatch(Type type) => true;
		public bool IsMatch(Type type, MethodInfo method) => true;
	}

	/// <summary>
	/// DefaultTestAssemblyBuilder loads a single assembly and builds a TestSuite
	/// containing test fixtures present in the assembly.
	/// </summary>
	public class SingleFileDefaultTestAssemblyBuilder : ITestAssemblyBuilder
    {
        static readonly Logger log = InternalTrace.GetLogger(typeof(SingleFileDefaultTestAssemblyBuilder));

        #region Instance Fields

        /// <summary>
        /// The default suite builder used by the test assembly builder.
        /// </summary>
        readonly ISuiteBuilder _defaultSuiteBuilder;

        private IPreFilter _filter;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SingleFileDefaultTestAssemblyBuilder"/> class.
        /// </summary>
        public SingleFileDefaultTestAssemblyBuilder()
        {
            _defaultSuiteBuilder = new DefaultSuiteBuilder();
        }

        #endregion

        #region Build Methods

        /// <summary>
        /// Build a suite of tests from a provided assembly
        /// </summary>
        /// <param name="assembly">The assembly from which tests are to be built</param>
        /// <param name="options">A dictionary of options to use in building the suite</param>
        /// <returns>
        /// A TestSuite containing the tests found in the assembly
        /// </returns>
        public ITest Build(Assembly assembly, IDictionary<string, object> options)
        {
            log.Debug("Loading {0} in AppDomain {1}", assembly.FullName, AppDomain.CurrentDomain.FriendlyName);

            string suiteName = AssemblyHelper.GetAssemblyName(assembly).FullName;
            // string assemblyPath = AssemblyHelper.GetAssemblyPath(assembly);
            // string suiteName = assemblyPath.Equals("<Unknown>")
            //     ? AssemblyHelper.GetAssemblyName(assembly).FullName
            //     : assemblyPath;

            return Build(assembly, suiteName, options);
        }

        /// <summary>
        /// Build a suite of tests given the name or the location of an assembly
        /// </summary>
        /// <param name="assemblyNameOrPath">The name or the location of the assembly.</param>
        /// <param name="options">A dictionary of options to use in building the suite</param>
        /// <returns>
        /// A TestSuite containing the tests found in the assembly
        /// </returns>
        public ITest Build(string assemblyNameOrPath, IDictionary<string, object> options)
        {
            log.Debug("Loading {0} in AppDomain {1}", assemblyNameOrPath, AppDomain.CurrentDomain.FriendlyName);

            TestSuite testAssembly = null;

            try
            {
                var assembly = AssemblyHelper.Load(assemblyNameOrPath);
                testAssembly = Build(assembly, assemblyNameOrPath, options);
            }
            catch (Exception ex)
            {
                testAssembly = new TestAssembly(assemblyNameOrPath);
                testAssembly.MakeInvalid(ExceptionHelper.BuildMessage(ex, true));
            }

            return testAssembly;
        }

        private TestSuite Build(Assembly assembly, string assemblyNameOrPath, IDictionary<string, object> options)
        {
            TestSuite testAssembly = null;

            try
            {
                // if (options.ContainsKey(FrameworkPackageSettings.DefaultTestNamePattern))
                //     TestNameGenerator.DefaultTestNamePattern = options[FrameworkPackageSettings.DefaultTestNamePattern] as string;

                // if (options.ContainsKey(FrameworkPackageSettings.WorkDirectory))
                //     TestContext.DefaultWorkDirectory = options[FrameworkPackageSettings.WorkDirectory] as string;
                // else
                //     TestContext.DefaultWorkDirectory = Directory.GetCurrentDirectory();

                // if (options.ContainsKey(FrameworkPackageSettings.TestParametersDictionary))
                // {
                //     var testParametersDictionary = options[FrameworkPackageSettings.TestParametersDictionary] as IDictionary<string, string>;
                //     if (testParametersDictionary != null)
                //     {
                //         foreach (var parameter in testParametersDictionary)
                //             TestContext.Parameters.Add(parameter.Key, parameter.Value);
                //     }
                // }
                // else
                // {
                //     // This cannot be changed without breaking backwards compatibility with old runners.
                //     // Deserializes the way old runners understand.

                //     if (options.ContainsKey(FrameworkPackageSettings.TestParameters))
                //     {
                //         string parameters = options[FrameworkPackageSettings.TestParameters] as string;
                //         if (!string.IsNullOrEmpty(parameters))
                //             foreach (string param in parameters.Split(new[] { ';' }))
                //             {
                //                 int eq = param.IndexOf('=');

                //                 if (eq > 0 && eq < param.Length - 1)
                //                 {
                //                     var name = param.Substring(0, eq);
                //                     var val = param.Substring(eq + 1);

                //                     TestContext.Parameters.Add(name, val);
                //                 }
                //             }
                //     }
                // }

                _filter = new PreFilter();
                // if (options.ContainsKey(FrameworkPackageSettings.LOAD))
                //     foreach (string filterText in (IList)options[FrameworkPackageSettings.LOAD])
                //         _filter.Add(filterText);

                var fixtures = GetFixtures(assembly);

                testAssembly = BuildTestAssembly(assembly, assemblyNameOrPath, fixtures);
            }
            catch (Exception ex)
            {
                testAssembly = new TestAssembly(assemblyNameOrPath);
                testAssembly.MakeInvalid(ExceptionHelper.BuildMessage(ex, true));
            }

            return testAssembly;
        }

        #endregion

        #region Helper Methods

        private IList<NUnit.Framework.Internal.Test> GetFixtures(Assembly assembly)
        {
            var fixtures = new List<NUnit.Framework.Internal.Test>();
            log.Debug("Examining assembly for test fixtures");

            var testTypes = GetCandidateFixtureTypes(assembly);

            log.Debug("Found {0} classes to examine", testTypes.Count);
#if LOAD_TIMING
            System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
            timer.Start();
#endif
            int testcases = 0;
            foreach (Type testType in testTypes)
            {
                var typeInfo = new TypeWrapper(testType);

                // Any exceptions from this call are fatal problems in NUnit itself,
                // since this is always DefaultSuiteBuilder and the current implementation
                // of DefaultSuiteBuilder.CanBuildFrom cannot invoke any user code.
                if (_defaultSuiteBuilder.CanBuildFrom(typeInfo))
                {
                    // We pass the filter for use in selecting methods of the type.
                    // Any exceptions from this call are fatal problems in NUnit itself,
                    // since this is always DefaultSuiteBuilder and the current implementation
                    // of DefaultSuiteBuilder.BuildFrom handles all exceptions from user code.
                    NUnit.Framework.Internal.Test fixture = _defaultSuiteBuilder.BuildFrom(typeInfo, _filter);
                    fixtures.Add(fixture);
                    testcases += fixture.TestCaseCount;
                }
            }

#if LOAD_TIMING
            log.Debug("Found {0} fixtures with {1} test cases in {2} seconds", fixtures.Count, testcases, timer.Elapsed);
#else
            log.Debug("Found {0} fixtures with {1} test cases", fixtures.Count, testcases);
#endif

            return fixtures;
        }

        private IList<Type> GetCandidateFixtureTypes(Assembly assembly)
        {
            var result = new List<Type>();

            foreach (Type type in assembly.GetTypes())
                if (_filter.IsMatch(type))
                    result.Add(type);

            return result;
        }

        // This method invokes members on the 'System.Diagnostics.Process' class and must satisfy the link demand of
        // the full-trust 'PermissionSetAttribute' on this class. Callers of this method have no influence on how the
        // Process class is used, so we can safely satisfy the link demand with a 'SecuritySafeCriticalAttribute' rather
        // than a 'SecurityCriticalAttribute' and allow use by security transparent callers.
        [SecuritySafeCritical]
        private TestSuite BuildTestAssembly(Assembly assembly, string assemblyNameOrPath, IList<NUnit.Framework.Internal.Test> fixtures)
        {
            TestSuite testAssembly = new TestAssembly(assembly, assemblyNameOrPath);

            if (fixtures.Count == 0)
            {
                testAssembly.MakeInvalid("No test fixtures were found.");
            }
            else
            {
                NamespaceTreeBuilder treeBuilder =
                    new NamespaceTreeBuilder(testAssembly);
                treeBuilder.Add(fixtures);
                testAssembly = treeBuilder.RootSuite;
            }

            testAssembly.ApplyAttributesToTest(assembly);

            try
            {
                testAssembly.Properties.Set(PropertyNames.ProcessId, System.Diagnostics.Process.GetCurrentProcess().Id);
            }
            catch (PlatformNotSupportedException)
            { }
            testAssembly.Properties.Set(PropertyNames.AppDomain, AppDomain.CurrentDomain.FriendlyName);

            // TODO: Make this an option? Add Option to sort assemblies as well?
            testAssembly.Sort();

            return testAssembly;
        }
        #endregion
    }
}
