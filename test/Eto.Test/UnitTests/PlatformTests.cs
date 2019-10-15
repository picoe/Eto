using System;
using NUnit.Framework;

namespace Eto.Test.UnitTests
{
	[TestFixture]
	public class PlatformTests : TestBase
	{
		[Test]
		public void ReinitializingPlatformShouldThrowException()
		{
			Assert.Throws<InvalidOperationException>(() =>
			{
				Platform.Initialize(Platform.Instance.GetType().AssemblyQualifiedName);
			});
		}

		[Test]
		public void ReinitializingPlatformWithCurrentInstanceShouldNotThrowException()
		{
			Platform.Initialize(Platform.Instance);
		}
	}
}
