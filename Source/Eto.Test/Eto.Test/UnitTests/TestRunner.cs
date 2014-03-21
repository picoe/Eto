using System;
using System.Reflection;

namespace Eto.Test.UnitTests
{
	public class EtoUnitTestAttribute : Attribute
	{
	}

	public class EtoAssert
	{
		public bool Succeeded { get; set; }

		public EtoAssert()
		{
			Succeeded = true;
		}
		internal void AreEqual(object sender, object expected, object actual)
		{
			if (expected == null && actual == null)
			{
			}
			else if (
				(expected == null && actual != null) ||
				(expected != null && actual == null) ||
				!expected.Equals(actual))
			{
				Log.Write(sender, "Assert.Equals failure. Expected={0} Actual={1}", expected ?? "null", actual ?? "null");
				Succeeded = false;
			}
		}
	}

	public class TestContext
	{
	}

	public class TestRunner
	{
		public void RunTests<T>(Func<T> factory) where T : EtoTestFixture
		{
			foreach (var method in typeof(T).GetMethods(BindingFlags.Instance | BindingFlags.Public))
			{
				// if the [UnitTest] attribute is defined, run the test
				var attrs = method.GetCustomAttributes(typeof(EtoUnitTestAttribute), inherit: false);
				if (attrs != null && attrs.Length > 0)
				{
					var fixture = factory(); // create a new instance of the fixture
					fixture.TestContext = new TestContext();
					fixture.Assert = new EtoAssert();
					// run the test method
					method.Invoke(fixture, null);
					Log.Write(this, "{0}: {1}.{2}", fixture.Assert.Succeeded ? "PASSED" : "FAILED", typeof(T), method.Name);
				}
			}
		}
	}

	public class EtoTestFixture
	{
		public TestContext TestContext { get; set; }
		public EtoAssert Assert { get; set; }
	}
}
