using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using NUnit.Framework;

namespace Eto.Test.UnitTests.Forms
{
	[TestFixture]
	public class RangeTests
	{
		[TestCase(100, 200, 100, 101)]
		[TestCase(100, 100, 100, 1)]
		[TestCase(100, 99, 100, 0)]
		public void Range_FromStartEnd(int s, int e, int start, int length)
		{
			var a = Range.FromStartEnd(s, e);
			var b = new Range(start, length);
			Assert.AreEqual(a, b);
		}

		[TestCase(100, 200, 300, 400, 0, -1)] // nonintersecting
		[TestCase(100, 200, 150, 300, 150, 200)]
		[TestCase(150, 300, 100, 200, 150, 200)] // opposite order
		public void Range_Intersect_Intersects(int s1, int e1, int s2, int e2, int s, int e)
		{
			var r1 = Range.FromStartEnd(s1, e1);
			var r2 = Range.FromStartEnd(s2, e2);
			var r = Range.FromStartEnd(s, e);
			Assert.AreEqual(r, r1.Intersect(r2));
			Assert.AreEqual(r, r2.Intersect(r1));
		}
	}
}
