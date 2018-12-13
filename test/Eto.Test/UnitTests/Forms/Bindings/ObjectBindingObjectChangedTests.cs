using System;
using NUnit.Framework;
using Eto.Forms;

namespace Eto.Test.UnitTests.Forms.Bindings
{
	[TestFixture]
	public class ObjectBindingObjectChangedTests
	{
		[Test]
		public void BoolPropertyShouldUpdate()
		{
			var bindObject = new BindObject { BoolProperty = true };
			var binding = new ObjectBinding<bool>(bindObject, "BoolProperty");
			Assert.AreEqual(binding.DataValue, bindObject.BoolProperty, "Data value should equal object value");
			bindObject.BoolProperty = false;
			Assert.AreEqual(binding.DataValue, bindObject.BoolProperty, "Data value should equal object value");
			bindObject.BoolProperty = true;
			Assert.AreEqual(binding.DataValue, bindObject.BoolProperty, "Data value should equal object value");
		}

		[Test]
		public void IntPropertyShouldUpdate()
		{
			var bindObject = new BindObject { IntProperty = 0 };
			var binding = new ObjectBinding<int>(bindObject, "IntProperty");
			Assert.AreEqual(binding.DataValue, bindObject.IntProperty, "Data value should equal object value");
			bindObject.IntProperty = 1;
			Assert.AreEqual(binding.DataValue, bindObject.IntProperty, "Data value should equal object value");
		}

		[Test]
		public void DoublePropertyShouldUpdate()
		{
			var bindObject = new BindObject { DoubleProperty = 0 };
			var binding = new ObjectBinding<double>(bindObject, "DoubleProperty");
			Assert.AreEqual(binding.DataValue, bindObject.DoubleProperty, "Data value should equal object value");
			bindObject.DoubleProperty = 1.2;
			Assert.AreEqual(binding.DataValue, bindObject.DoubleProperty, "Data value should equal object value");
		}

		[Test]
		public void StringPropertyShouldUpdate()
		{
			var bindObject = new BindObject { StringProperty = "Initial Value" };
			var binding = new ObjectBinding<string>(bindObject, "StringProperty");
			Assert.AreEqual(binding.DataValue, bindObject.StringProperty, "Data value should equal object value");
			bindObject.StringProperty = "Other Value";
			Assert.AreEqual(binding.DataValue, bindObject.StringProperty, "Data value should equal object value");
			bindObject.StringProperty = null;
			Assert.AreEqual(binding.DataValue, bindObject.StringProperty, "Data value should equal object value");
		}
	}
}

