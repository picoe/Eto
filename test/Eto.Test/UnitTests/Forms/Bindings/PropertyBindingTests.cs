using System;
using NUnit.Framework;
using Eto.Forms;
using System.Data;
using System.ComponentModel;
using System.Collections.Generic;
using sc = System.ComponentModel;

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

		class MyCustomDescriptorClass : ICustomTypeDescriptor
		{
			Dictionary<object, object> _properties = new Dictionary<object, object> 
			{
				{ "FirstProperty", "Initial Value" },
				{ "SecondProperty", true }
			};
			
			PropertyDescriptorCollection _propertyDescriptorCollection;
			
			public string NonTypeDescriptorProperty { get; set; } = "Initial Other Value";
			

			AttributeCollection ICustomTypeDescriptor.GetAttributes() => sc.TypeDescriptor.GetAttributes(this);

			string ICustomTypeDescriptor.GetClassName() => sc.TypeDescriptor.GetClassName(this);

			string ICustomTypeDescriptor.GetComponentName() => sc.TypeDescriptor.GetComponentName(this);

			System.ComponentModel.TypeConverter ICustomTypeDescriptor.GetConverter() => sc.TypeDescriptor.GetConverter(this);

			EventDescriptor ICustomTypeDescriptor.GetDefaultEvent() => sc.TypeDescriptor.GetDefaultEvent(this);

			PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty() => sc.TypeDescriptor.GetDefaultProperty(this);

			object ICustomTypeDescriptor.GetEditor(Type editorBaseType) => sc.TypeDescriptor.GetEditor(this, editorBaseType);

			EventDescriptorCollection ICustomTypeDescriptor.GetEvents() => sc.TypeDescriptor.GetEvents(this);

			EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes) => sc.TypeDescriptor.GetEvents(this, attributes);

			PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties() => GetProperties(null);

			PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes) => GetProperties(attributes);

			private PropertyDescriptorCollection GetProperties(Attribute[] attributes)
			{
				if (_propertyDescriptorCollection != null)
					return _propertyDescriptorCollection;
				var props = new List<PropertyDescriptor>();
				props.Add(new MyPropertyDescriptor<string>("FirstProperty"));
				props.Add(new MyPropertyDescriptor<bool>("SecondProperty"));
				_propertyDescriptorCollection = new PropertyDescriptorCollection(props.ToArray());
				return _propertyDescriptorCollection;
			}

			object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd) => this;

			class MyPropertyDescriptor<T> : sc.PropertyDescriptor
			{
				public MyPropertyDescriptor(string name) : base(name, null)
				{
				}

				public override Type ComponentType => typeof(MyCustomDescriptorClass);

				public override bool IsReadOnly => false;

				public override Type PropertyType => typeof(T);

				public override bool CanResetValue(object component) => false;

				public override object GetValue(object component) => ((MyCustomDescriptorClass)component)._properties[Name];

				public override void ResetValue(object component)
				{
				}

				public override void SetValue(object component, object value) => ((MyCustomDescriptorClass)component)._properties[Name] = value;

				public override bool ShouldSerializeValue(object component) => false;
			}
		}

		[Test]
		public void PropertyBindingsShouldUseDescriptors()
		{
			var item = new MyCustomDescriptorClass();

			var stringBinding = Eto.Forms.Binding.Property<string>("FirstProperty");
			var boolBinding = Eto.Forms.Binding.Property<bool>("SecondProperty");
			var invalidBinding = Eto.Forms.Binding.Property<string>("ThirdInvalidProperty");
			var propertyInfoBinding = Eto.Forms.Binding.Property<string>("NonTypeDescriptorProperty");
			
			Assert.AreEqual("Initial Value", stringBinding.GetValue(item));
			stringBinding.SetValue(item, "Some Value");
			Assert.AreEqual("Some Value", stringBinding.GetValue(item));

			Assert.AreEqual(true, boolBinding.GetValue(item));
			boolBinding.SetValue(item, false);
			Assert.AreEqual(false, boolBinding.GetValue(item));
			
			Assert.IsNull(invalidBinding.GetValue(item));
			invalidBinding.SetValue(item, "something");
			Assert.IsNull(invalidBinding.GetValue(item));
			
			// ensure properties can also be accessed without descriptors
			Assert.AreEqual("Initial Other Value", propertyInfoBinding.GetValue(item));
			propertyInfoBinding.SetValue(item, "Some Other Value");
			Assert.AreEqual("Some Other Value", propertyInfoBinding.GetValue(item));
		}
	}
}
