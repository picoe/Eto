using System;
using NUnit.Framework;
using Eto.Forms;

namespace Eto.Test.UnitTests.Forms.Bindings
{
	[TestFixture]
	public class PropertyBindingTests : TestBase
	{
		[Test]
		public void BindingWithNameShouldUpdateProperly()
		{
			var item = new BindObject();
			var binding = Eto.Forms.Binding.Property<int>("IntProperty");

			int changed = 0;
			binding.AddValueChangedHandler(item, (sender, e) => changed++);

			Assert.AreEqual(0, changed);
			Assert.AreEqual(0, binding.GetValue(item));

			item.IntProperty = 2;
			Assert.AreEqual(1, changed);
			Assert.AreEqual(2, binding.GetValue(item));

			item.IntProperty = 4;
			Assert.AreEqual(2, changed);
			Assert.AreEqual(4, binding.GetValue(item));
		}

		[Test]
		public void ChildBindingShouldUpdateProperly()
		{
			var item = new BindObject();
			var binding = Eto.Forms.Binding.Property((BindObject m) => m.ChildBindObject).Child(child => child.IntProperty);

			int changed = 0;
			binding.AddValueChangedHandler(item, (sender, e) => changed++);

			BindObject oldChild;

			item.ChildBindObject = oldChild = new BindObject();
			Assert.AreEqual(1, changed);
			Assert.AreEqual(0, binding.GetValue(item));

			item.ChildBindObject.IntProperty = 2;
			Assert.AreEqual(2, changed);
			Assert.AreEqual(2, binding.GetValue(item));

			item.ChildBindObject = new BindObject { IntProperty = 3 };
			Assert.AreEqual(3, changed);
			Assert.AreEqual(3, binding.GetValue(item));

			oldChild.IntProperty = 4; // we should not be hooked into change events of the old child, since we have a new one now!
			Assert.AreEqual(3, changed);
			Assert.AreEqual(3, binding.GetValue(item));
		}

		[Test]
		public void ChildBindingWithExpressionShouldUpdateProperly()
		{
			var item = new BindObject();
			var binding = Eto.Forms.Binding.Property((BindObject m) => m.ChildBindObject.IntProperty);

			int changed = 0;
			binding.AddValueChangedHandler(item, (sender, e) => changed++);

			BindObject oldChild;

			item.ChildBindObject = oldChild = new BindObject();
			Assert.AreEqual(1, changed);
			Assert.AreEqual(0, binding.GetValue(item));

			item.ChildBindObject.IntProperty = 2;
			Assert.AreEqual(2, changed);
			Assert.AreEqual(2, binding.GetValue(item));

			item.ChildBindObject = new BindObject { IntProperty = 3 };
			Assert.AreEqual(3, changed);
			Assert.AreEqual(3, binding.GetValue(item));

			oldChild.IntProperty = 4; // we should not be hooked into change events of the old child, since we have a new one now!
			Assert.AreEqual(3, changed);
			Assert.AreEqual(3, binding.GetValue(item));
		}

		[Test]
		public void ChildBindingWithNameShouldUpdateProperly()
		{
			var item = new BindObject();
			var binding = Eto.Forms.Binding.Property<int>("ChildBindObject.IntProperty");

			int changed = 0;
			binding.AddValueChangedHandler(item, (sender, e) => changed++);

			BindObject oldChild;

			item.ChildBindObject = oldChild = new BindObject();
			Assert.AreEqual(1, changed);
			Assert.AreEqual(0, binding.GetValue(item));

			item.ChildBindObject.IntProperty = 2;
			Assert.AreEqual(2, changed);
			Assert.AreEqual(2, binding.GetValue(item));

			item.ChildBindObject = new BindObject { IntProperty = 3 };
			Assert.AreEqual(3, changed);
			Assert.AreEqual(3, binding.GetValue(item));

			oldChild.IntProperty = 4; // we should not be hooked into change events of the old child, since we have a new one now!
			Assert.AreEqual(3, changed);
			Assert.AreEqual(3, binding.GetValue(item));
		}

		[Test]
		public void NonExistantPropertyShouldNotCrash()
		{
			var item = new BindObject();
			var binding = Eto.Forms.Binding.Property<int?>("SomePropertyThatDoesntExist");

			int changed = 0;
			EventHandler<EventArgs> valueChanged = (sender, e) => changed++;
			var changeReference = binding.AddValueChangedHandler(item, valueChanged);

			Assert.AreEqual(0, changed);
			Assert.AreEqual(null, binding.GetValue(item));
			Assert.DoesNotThrow(() => binding.SetValue(item, 123));
			binding.RemoveValueChangedHandler(changeReference, valueChanged);
		}

		[Test]
		public void InternalPropertyShouldBeAccessible()
		{
			var item = new BindObject { InternalStringProperty = "some value" };
			var binding = Eto.Forms.Binding.Property<string>("InternalStringProperty");

			int changed = 0;
			EventHandler<EventArgs> valueChanged = (sender, e) => changed++;
			var changeReference = binding.AddValueChangedHandler(item, valueChanged);

			Assert.AreEqual(0, changed);
			Assert.AreEqual("some value", binding.GetValue(item));
			Assert.DoesNotThrow(() => binding.SetValue(item, "some other value"));
			Assert.AreEqual(1, changed);
			Assert.AreEqual("some other value", binding.GetValue(item));
			binding.RemoveValueChangedHandler(changeReference, valueChanged);
		}
	}
}
