using Eto.Forms;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eto.Test.UnitTests
{
#if FIX_ON_IOS
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class InvokeAttribute : Attribute, ITestAction
	{
		public void AfterTest(TestDetails testDetails)
		{
		}

		public void BeforeTest(TestDetails testDetails)
		{
			if (testDetails.Method != null)
			{
				Exception exception = null;
				Application.Instance.Invoke(() =>
				{
					try
					{
						testDetails.Method.Invoke(testDetails.Fixture, null);
					}
					catch (Exception ex)
					{
						exception = ex;
					}
				});
				if (exception != null)
					throw new Exception("Invoke failed", exception);
				Assert.Pass();
			}
		}

		public ActionTargets Targets
		{
			get { return ActionTargets.Default; }
		}
	}

	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
	public class UseTestGeneratorAttribute : Attribute, ITestAction
	{
		IDisposable context;

		public virtual Generator CreateGenerator()
		{
			return new Handlers.Generator();
		}

		public void AfterTest(TestDetails testDetails)
		{
			if (context != null)
			{
				context.Dispose();
				context = null;
			}
		}

		public void BeforeTest(TestDetails testDetails)
		{
			context = CreateGenerator().Context;
		}

		public ActionTargets Targets
		{
			get { return ActionTargets.Default; }
		}
	}
#endif
}
