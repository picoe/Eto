using NUnit.Framework;
using System.Data;
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

			Assert.That(changed, Is.EqualTo(0));
			Assert.That(binding.GetValue(item), Is.EqualTo(0));

			item.IntProperty = 2;
			Assert.That(changed, Is.EqualTo(1));
			Assert.That(binding.GetValue(item), Is.EqualTo(2));

			item.IntProperty = 4;
			Assert.That(changed, Is.EqualTo(2));
			Assert.That(binding.GetValue(item), Is.EqualTo(4));
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
			Assert.That(changed, Is.EqualTo(1));
			Assert.That(binding.GetValue(item), Is.EqualTo(0));

			item.ChildBindObject.IntProperty = 2;
			Assert.That(changed, Is.EqualTo(2));
			Assert.That(binding.GetValue(item), Is.EqualTo(2));

			item.ChildBindObject = new BindObject { IntProperty = 3 };
			Assert.That(changed, Is.EqualTo(3));
			Assert.That(binding.GetValue(item), Is.EqualTo(3));

			oldChild.IntProperty = 4; // we should not be hooked into change events of the old child, since we have a new one now!
			Assert.That(changed, Is.EqualTo(3));
			Assert.That(binding.GetValue(item), Is.EqualTo(3));
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
			Assert.That(changed, Is.EqualTo(1));
			Assert.That(binding.GetValue(item), Is.EqualTo(0));

			item.ChildBindObject.IntProperty = 2;
			Assert.That(changed, Is.EqualTo(2));
			Assert.That(binding.GetValue(item), Is.EqualTo(2));

			item.ChildBindObject = new BindObject { IntProperty = 3 };
			Assert.That(changed, Is.EqualTo(3));
			Assert.That(binding.GetValue(item), Is.EqualTo(3));

			oldChild.IntProperty = 4; // we should not be hooked into change events of the old child, since we have a new one now!
			Assert.That(changed, Is.EqualTo(3));
			Assert.That(binding.GetValue(item), Is.EqualTo(3));
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
			Assert.That(changed, Is.EqualTo(1));
			Assert.That(binding.GetValue(item), Is.EqualTo(0));

			item.ChildBindObject.IntProperty = 2;
			Assert.That(changed, Is.EqualTo(2));
			Assert.That(binding.GetValue(item), Is.EqualTo(2));

			item.ChildBindObject = new BindObject { IntProperty = 3 };
			Assert.That(changed, Is.EqualTo(3));
			Assert.That(binding.GetValue(item), Is.EqualTo(3));

			oldChild.IntProperty = 4; // we should not be hooked into change events of the old child, since we have a new one now!
			Assert.That(changed, Is.EqualTo(3));
			Assert.That(binding.GetValue(item), Is.EqualTo(3));
		}

		[Test]
		public void NonExistantPropertyShouldNotCrash()
		{
			var item = new BindObject();
			var binding = Eto.Forms.Binding.Property<int?>("SomePropertyThatDoesntExist");

			int changed = 0;
			EventHandler<EventArgs> valueChanged = (sender, e) => changed++;
			var changeReference = binding.AddValueChangedHandler(item, valueChanged);

			Assert.That(changed, Is.EqualTo(0));
			Assert.That(binding.GetValue(item), Is.EqualTo(null));
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

			Assert.That(changed, Is.EqualTo(0));
			Assert.That(binding.GetValue(item), Is.EqualTo("some value"));
			Assert.DoesNotThrow(() => binding.SetValue(item, "some other value"));
			Assert.That(changed, Is.EqualTo(1));
			Assert.That(binding.GetValue(item), Is.EqualTo("some other value"));
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
			
			Assert.That(stringBinding.GetValue(item), Is.EqualTo("Initial Value"));
			stringBinding.SetValue(item, "Some Value");
			Assert.That(stringBinding.GetValue(item), Is.EqualTo("Some Value"));

			Assert.That(boolBinding.GetValue(item), Is.EqualTo(true));
			boolBinding.SetValue(item, false);
			Assert.That(boolBinding.GetValue(item), Is.EqualTo(false));
			
			Assert.That(invalidBinding.GetValue(item), Is.Null);
			invalidBinding.SetValue(item, "something");
			Assert.That(invalidBinding.GetValue(item), Is.Null);
			
			// ensure properties can also be accessed without descriptors
			Assert.That(propertyInfoBinding.GetValue(item), Is.EqualTo("Initial Other Value"));
			propertyInfoBinding.SetValue(item, "Some Other Value");
			Assert.That(propertyInfoBinding.GetValue(item), Is.EqualTo("Some Other Value"));
		}
	}
}
