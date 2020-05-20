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
			Color _color;
			TestObject _child;
			public Color Color
			{
				get => _color;
				set
				{
					if (_color != value)
					{
						_color = value;
						ColorChanged?.Invoke(this, EventArgs.Empty);
					}
				}
			}

			public event EventHandler<EventArgs> ColorChanged;

			public TestObject Child
			{
				get => _child;
				set {
					if (_child != value)
					{
						_child = value;
						ChildChanged?.Invoke(this, EventArgs.Empty);
					}
				}
			}
			public event EventHandler<EventArgs> ChildChanged;
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

		[Test]
		public void AddingMultipleChangeHandlersForIndirectBindingsShouldWork()
		{
			var item = new TestObject { Color = Colors.Blue, Child = new TestObject { Color = Colors.Blue } };
			var binding = Binding.Property((TestObject t) => t.Child);
			var childBinding = binding.Child(c => c.Color);

			int valueChange1Count = 0;
			int valueChange2Count = 0;
			void ValueChanged1(object sender, EventArgs e) => valueChange1Count++;
			void ValueChanged2(object sender, EventArgs e) => valueChange2Count++;

			var changed1 = childBinding.AddValueChangedHandler(item, ValueChanged1);
			var changed2 = childBinding.AddValueChangedHandler(item, ValueChanged2);

			item.Child.Color = Colors.Green;

			Assert.AreEqual(1, valueChange1Count, "#1.1");
			Assert.AreEqual(1, valueChange2Count, "#1.2");

			// setting the child to something else should re-wire all the value changed bindings to the new instance
			// .. and trigger the value changed
			item.Child = new TestObject { Color = Colors.Blue };

			Assert.AreEqual(2, valueChange1Count, "#2.1");
			Assert.AreEqual(2, valueChange2Count, "#2.2");

			// remove the first handler and ensure the second one is still active
			childBinding.RemoveValueChangedHandler(changed1, ValueChanged1);

			item.Child.Color = Colors.Yellow;

			// only hooked up to the second handler
			Assert.AreEqual(2, valueChange1Count, "#3.1");
			Assert.AreEqual(3, valueChange2Count, "#3.2");

			item.Child = new TestObject { Color = Colors.White };

			Assert.AreEqual(2, valueChange1Count, "#4.1");
			Assert.AreEqual(4, valueChange2Count, "#4.2");

			childBinding.RemoveValueChangedHandler(changed2, ValueChanged2);

			item.Child.Color = Colors.Tomato;
			item.Child = new TestObject { Color = Colors.Wheat };

			// no changes as nothing should be hooked up
			Assert.AreEqual(2, valueChange1Count, "#5.1");
			Assert.AreEqual(4, valueChange2Count, "#5.2");

		}

		[Test]
		public void AddingMultipleChangeHandlersForDirectBindingsShouldWork()
		{
			var item = new TestObject { Color = Colors.Blue, Child = new TestObject { Color = Colors.Blue } };
			var binding = Binding.Property(item, t => t.Child);
			var childBinding = binding.Child(c => c.Color);

			int valueChange1Count = 0;
			int valueChange2Count = 0;
			void ValueChanged1(object sender, EventArgs e) => valueChange1Count++;
			void ValueChanged2(object sender, EventArgs e) => valueChange2Count++;

			childBinding.DataValueChanged += ValueChanged1;
			childBinding.DataValueChanged += ValueChanged2;

			item.Child.Color = Colors.Green;

			Assert.AreEqual(1, valueChange1Count, "#1.1");
			Assert.AreEqual(1, valueChange2Count, "#1.2");

			// setting the child to something else should re-wire all the value changed bindings to the new instance
			// .. and trigger the value changed
			item.Child = new TestObject { Color = Colors.Blue };

			Assert.AreEqual(2, valueChange1Count, "#2.1");
			Assert.AreEqual(2, valueChange2Count, "#2.2");

			// remove the first handler and ensure the second one is still active
			childBinding.DataValueChanged -= ValueChanged1;

			item.Child.Color = Colors.Yellow;

			// only hooked up to the second handler
			Assert.AreEqual(2, valueChange1Count, "#3.1");
			Assert.AreEqual(3, valueChange2Count, "#3.2");

			item.Child = new TestObject { Color = Colors.White };

			Assert.AreEqual(2, valueChange1Count, "#4.1");
			Assert.AreEqual(4, valueChange2Count, "#4.2");

			childBinding.DataValueChanged -= ValueChanged2;

			item.Child.Color = Colors.Tomato;
			item.Child = new TestObject { Color = Colors.Wheat };

			// no changes as nothing should be hooked up
			Assert.AreEqual(2, valueChange1Count, "#5.1");
			Assert.AreEqual(4, valueChange2Count, "#5.2");

		}
	}
}
