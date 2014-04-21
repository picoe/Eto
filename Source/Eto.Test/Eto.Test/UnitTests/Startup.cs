#if !PCL
using System;
using NUnit.Framework;
using Eto.Forms;
using System.Threading;
using Eto.Drawing;
using System.Configuration;

namespace Eto.Test.UnitTests
{
#if !iOS // We should not use the SetupFixture attribute on iOS. For now, this #ifdef allows iOS builds to work provided you build with the iOS symbol defined.
	[SetUpFixture]
#endif
	public class Startup
	{
		[SetUp]
		public void SetUp()
		{
			if (Application.Instance == null)
			{
				#if DESKTOP
				var generatorTypeName = ConfigurationManager.AppSettings["generator"];
				#else
				string generatorTypeName = null;
				#endif
				var ev = new ManualResetEvent(false);
				Exception exception = null;
				var thread = new Thread(() =>
				{
					try
					{
						Generator generator;
						if (string.IsNullOrEmpty(generatorTypeName))
							generator = Generator.Detect;
						else
							generator = Generator.GetGenerator(generatorTypeName);

						var app = new Application(generator);
						app.Initialized += (sender, e) => ev.Set();
						app.Run();
					}
					catch (Exception ex)
					{
						exception = ex;
					}
				});
				thread.SetApartmentState(ApartmentState.STA);
				thread.Start();
				if (!ev.WaitOne(10000))
					Assert.Fail("Could not initialize generator");
				if (exception != null)
					throw new Exception("Error initializing generator", exception);
			}
		}
	}
}

#endif