using System;
using Eto.Drawing;
using Eto.Forms;
using NUnit.Framework;

namespace Eto.Test.UnitTests.Forms.Bindings
{
	[TestFixture]
	public class ChildBindingTests : TestBase
	{
		class TestObject
		{
			public Color Color { get; set; }
		}

		[Test]
		public void IndirectBindingStructPropertiesShouldWork()
		{
			// child binding via expression
			var binding = Binding.Property((TestObject t) => t.Color.B);
			var item = new TestObject
			{
				Color = Colors.White
			};
			binding.SetValue(item, 0.5f);
			Assert.AreEqual(0.5f, item.Color.B, "Struct property value was not set");
		}

		[Test]
		public void DirectBindingStructPropertiesShouldWork()
		{
			var item = new TestObject
			{
				Color = Colors.White
			};
			var colorBinding = Binding.Property((TestObject t) => t.Color);
			var binding = new ObjectBinding<TestObject, Color>(item, colorBinding);

			var childBinding = binding.Child(Binding.Property((Color c) => c.B));
			childBinding.DataValue = 0.5f;
			Assert.AreEqual(0.5f, item.Color.B, "Struct property value was not set");
		}
	}
}
