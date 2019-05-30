using System;
using NUnit.Framework;
using System.ComponentModel;
using Eto.Forms;

namespace Eto.Test.UnitTests.Forms.Bindings
{
	class BindObject : INotifyPropertyChanged
	{

		bool boolProperty;
		public bool BoolProperty
		{
			get { return boolProperty; }
			set
			{
				boolProperty = value;
				OnPropertyChanged(nameof(BoolProperty));
			}
		}

		int intProperty;
		public int IntProperty
		{
			get { return intProperty; }
			set
			{
				intProperty = value;
				OnPropertyChanged(nameof(IntProperty));
			}
		}

		double doubleProperty;
		public double DoubleProperty
		{
			get { return doubleProperty; }
			set
			{
				doubleProperty = value;
				OnPropertyChanged(nameof(DoubleProperty));
			}
		}

		string stringProperty;
		public string StringProperty
		{
			get { return stringProperty; }
			set
			{
				stringProperty = value;
				OnPropertyChanged(nameof(StringProperty));
			}
		}

		bool? nullableBoolProperty;
		public bool? NullableBoolProperty
		{
			get { return nullableBoolProperty; }
			set
			{
				nullableBoolProperty = value;
				OnPropertyChanged(nameof(NullableBoolProperty));
			}
		}

		public int? NullableIntProperty { get; set; }

		double? nullableDoubleProperty;
		public double? NullableDoubleProperty
		{
			get { return nullableDoubleProperty; }
			set
			{
				nullableDoubleProperty = value;
				OnPropertyChanged(nameof(NullableDoubleProperty));
			}
		}

		BindObject childBindObject;
		public BindObject ChildBindObject
		{
			get { return childBindObject; }
			set
			{
				if (!ReferenceEquals(childBindObject, value))
				{
					childBindObject = value;
					OnPropertyChanged(nameof(ChildBindObject));
				}
			}
		}

		string internalStringProperty;
		internal string InternalStringProperty
		{
			get { return internalStringProperty; }
			set
			{
				internalStringProperty = value;
				OnPropertyChanged(nameof(InternalStringProperty));
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}

	[TestFixture]
	public class ObjectBindingChangedTests
	{
		[Test]
		public void BoolPropertyShouldUpdate()
		{
			var bindObject = new BindObject { BoolProperty = true };
			var binding = new ObjectBinding<bool>(bindObject, "BoolProperty");
			Assert.AreEqual(binding.DataValue, bindObject.BoolProperty, "Data value should equal object value");
			binding.DataValue = false;
			Assert.AreEqual(binding.DataValue, bindObject.BoolProperty, "Data value should equal object value");
			binding.DataValue = true;
			Assert.AreEqual(binding.DataValue, bindObject.BoolProperty, "Data value should equal object value");
		}

		[Test]
		public void IntPropertyShouldUpdate()
		{
			var bindObject = new BindObject { IntProperty = 0 };
			var binding = new ObjectBinding<int>(bindObject, "IntProperty");
			Assert.AreEqual(binding.DataValue, bindObject.IntProperty, "Data value should equal object value");
			binding.DataValue = 1;
			Assert.AreEqual(binding.DataValue, bindObject.IntProperty, "Data value should equal object value");
		}

		[Test]
		public void DoublePropertyShouldUpdate()
		{
			var bindObject = new BindObject { DoubleProperty = 0 };
			var binding = new ObjectBinding<double>(bindObject, "DoubleProperty");
			Assert.AreEqual(binding.DataValue, bindObject.DoubleProperty, "Data value should equal object value");
			binding.DataValue = 1.2;
			Assert.AreEqual(binding.DataValue, bindObject.DoubleProperty, "Data value should equal object value");
		}

		[Test]
		public void StringPropertyShouldUpdate()
		{
			var bindObject = new BindObject { StringProperty = "Initial Value" };
			var binding = new ObjectBinding<string>(bindObject, "StringProperty");
			Assert.AreEqual(binding.DataValue, bindObject.StringProperty, "Data value should equal object value");
			binding.DataValue = "Other Value";
			Assert.AreEqual(binding.DataValue, bindObject.StringProperty, "Data value should equal object value");
			binding.DataValue = null;
			Assert.AreEqual(binding.DataValue, bindObject.StringProperty, "Data value should equal object value");
		}
	}
}

