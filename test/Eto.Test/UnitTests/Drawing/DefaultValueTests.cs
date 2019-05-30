using NUnit.Framework;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using Eto.Drawing;
using System.Linq.Expressions;

namespace Eto.Test.UnitTests.Drawing
{
	[TestFixture]
	public class DefaultValueTests : TestBase
	{
		static IEnumerable<PropertyTestInfo> GetTests()
		{ 
			yield return PropertyTest(() => new Eto.Drawing.LinearGradientBrush(Colors.Black, Colors.White, PointF.Empty, new PointF(10, 10)), r => r.Wrap);
			yield return PropertyTest(() => new Eto.Drawing.RadialGradientBrush(Colors.Black, Colors.White, PointF.Empty, new PointF(1, 1), new SizeF(10, 10)), r => r.Wrap);
		}

		[Test]
		[TestCaseSource(nameof(GetTests))]
		public void DefaultPropertyValuesShouldBeCorrect(PropertyTestInfo test)
		{
			Invoke(test.Run);
		}
	}
}
