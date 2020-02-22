using System;
using Eto.Forms;
using NUnit.Framework;

namespace Eto.Test.UnitTests.Forms
{
	[TestFixture]
	public class ApplicationTests : TestBase
	{
		[Test, InvokeOnUI]
		public void ReinitializingWithNewPlatformShouldThrowException()
		{
			Assert.Throws<InvalidOperationException>(() =>
			{
				_ = new Application(Platform.Instance.GetType().AssemblyQualifiedName);
			});
		}

		[Test, InvokeOnUI]
		public void ReinitializingWithCurrentPlatformShouldThrowException()
		{
			Assert.Throws<InvalidOperationException>(() =>
			{
				_ = new Application(Platform.Instance);
			});
		}
	}
}
