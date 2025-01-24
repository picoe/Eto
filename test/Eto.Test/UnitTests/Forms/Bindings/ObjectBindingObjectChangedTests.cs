using NUnit.Framework;
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
			Assert.That(bindObject.BoolProperty, Is.EqualTo(binding.DataValue), "Data value should equal object value");
			bindObject.BoolProperty = false;
			Assert.That(bindObject.BoolProperty, Is.EqualTo(binding.DataValue), "Data value should equal object value");
			bindObject.BoolProperty = true;
			Assert.That(bindObject.BoolProperty, Is.EqualTo(binding.DataValue), "Data value should equal object value");
		}

		[Test]
		public void IntPropertyShouldUpdate()
		{
			var bindObject = new BindObject { IntProperty = 0 };
			var binding = new ObjectBinding<int>(bindObject, "IntProperty");
			Assert.That(bindObject.IntProperty, Is.EqualTo(binding.DataValue), "Data value should equal object value");
			bindObject.IntProperty = 1;
			Assert.That(bindObject.IntProperty, Is.EqualTo(binding.DataValue), "Data value should equal object value");
		}

		[Test]
		public void DoublePropertyShouldUpdate()
		{
			var bindObject = new BindObject { DoubleProperty = 0 };
			var binding = new ObjectBinding<double>(bindObject, "DoubleProperty");
			Assert.That(bindObject.DoubleProperty, Is.EqualTo(binding.DataValue), "Data value should equal object value");
			bindObject.DoubleProperty = 1.2;
			Assert.That(bindObject.DoubleProperty, Is.EqualTo(binding.DataValue), "Data value should equal object value");
		}

		[Test]
		public void StringPropertyShouldUpdate()
		{
			var bindObject = new BindObject { StringProperty = "Initial Value" };
			var binding = new ObjectBinding<string>(bindObject, "StringProperty");
			Assert.That(bindObject.StringProperty, Is.EqualTo(binding.DataValue), "Data value should equal object value");
			bindObject.StringProperty = "Other Value";
			Assert.That(bindObject.StringProperty, Is.EqualTo(binding.DataValue), "Data value should equal object value");
			bindObject.StringProperty = null;
			Assert.That(bindObject.StringProperty, Is.EqualTo(binding.DataValue), "Data value should equal object value");
		}
	}
}

