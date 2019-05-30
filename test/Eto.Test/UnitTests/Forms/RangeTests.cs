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
		[TestCase(100, 200, 300, 400, false)]
		[TestCase(100, 200, 201, 300, true)]
		[TestCase(100, 200, 200, 300, false)]
		public void TouchesShouldTouch(int s1, int e1, int s2, int e2, bool touches)
		{
			var r1 = new Range<int>(s1, e1);
			var r2 = new Range<int>(s2, e2);
			Assert.AreEqual(touches, r1.Touches(r2, i => i + 1));
			Assert.AreEqual(touches, r2.Touches(r1, i => i + 1));
		}

		[TestCase(100, 200, 300, 400, false)]
		[TestCase(100, 200, 150, 300, true)]
		[TestCase(100, 200, 200, 300, true)]
		[TestCase(100, 200, 201, 300, false)]
		public void IntersectsShouldBeValid(int s1, int e1, int s2, int e2, bool intersects)
		{
			var r1 = new Range<int>(s1, e1);
			var r2 = new Range<int>(s2, e2);
			Assert.AreEqual(intersects, r1.Intersects(r2));
			Assert.AreEqual(intersects, r2.Intersects(r1));
		}

		[TestCase(100, 200, 150, 300, 100, 300)]
		[TestCase(100, 200, 200, 300, 100, 300)]
		[TestCase(100, 200, 300, 400, 0, -1)]
		[TestCase(100, 200, 201, 300, 0, -1)]
		public void UnionShouldUnion(int s1, int e1, int s2, int e2, int s, int e)
		{
			var r1 = new Range<int>(s1, e1);
			var r2 = new Range<int>(s2, e2);
			var r = e >= 0 ? (Range<int>?)new Range<int>(s, e) : null;
			Assert.AreEqual(r, r1.Union(r2));
			Assert.AreEqual(r, r2.Union(r1));
		}

		[TestCase(100, 200, 150, 300, 100, 300)]
		[TestCase(100, 200, 200, 300, 100, 300)]
		[TestCase(100, 200, 201, 300, 100, 300)]
		[TestCase(100, 200, 202, 300, 0, -1)]
		[TestCase(100, 200, 300, 400, 0, -1)]
		public void UnionWithTouchShouldUnion(int s1, int e1, int s2, int e2, int s, int e)
		{
			var r1 = new Range<int>(s1, e1);
			var r2 = new Range<int>(s2, e2);
			var r = e >= 0 ? (Range<int>?)new Range<int>(s, e) : null;
			Assert.AreEqual(r, r1.Union(r2, i => i + 1));
			Assert.AreEqual(r, r2.Union(r1, i => i + 1));
		}

		[TestCase(100, 200, 150, 300, 150, 200)]
		[TestCase(100, 200, 200, 300, 200, 200)]
		[TestCase(100, 200, 300, 400, 0, -1)]
		[TestCase(100, 200, 201, 300, 0, -1)]
		public void IntersectShouldIntersect(int s1, int e1, int s2, int e2, int s, int e)
		{
			var r1 = new Range<int>(s1, e1);
			var r2 = new Range<int>(s2, e2);
			var r = e >= 0 ? (Range<int>?)new Range<int>(s, e) : null;
			Assert.AreEqual(r, r1.Intersect(r2));
			Assert.AreEqual(r, r2.Intersect(r1));
		}

		[TestCase(100, 200, 4, false)]
		[TestCase(100, 200, 99, false)]
		[TestCase(100, 200, 100, true)]
		[TestCase(100, 200, 150, true)]
		[TestCase(100, 200, 200, true)]
		[TestCase(100, 200, 201, false)]
		[TestCase(100, 200, 500, false)]
		public void ContainsShouldContain(int s, int e, int value, bool contains)
		{
			var r = new Range<int>(s, e);
			Assert.AreEqual(contains, r.Contains(value));
		}
	}
}
