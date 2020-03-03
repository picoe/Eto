using System;
using NUnit.Framework;

namespace Eto.Test.UnitTests
{
	[TestFixture]
	public class PlatformTests : TestBase
	{
		[Test, InvokeOnUI]
		public void ReinitializingPlatformShouldThrowException()
		{
			Assert.Throws<InvalidOperationException>(() =>
			{
				Platform.Initialize(Platform.Instance.GetType().AssemblyQualifiedName);
			});
		}

		[Test, InvokeOnUI]
		public void ReinitializingPlatformWithCurrentInstanceShouldNotThrowException()
		{
			Platform.Initialize(Platform.Instance);
		}
	}
}
