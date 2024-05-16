using NUnit.Framework;

namespace Eto.Test.UnitTests.Forms.Bindings;

[TestFixture]
public class BindingHelpersTests
{
	[Test]
	public void AddingAPropertyEventByNameShouldWork()
	{
		var propertyValueChanged = false;
		void Handler(object sender, EventArgs e) => propertyValueChanged = true;

		var bindObject = new BindObject { BoolProperty = true };
		Binding.AddPropertyEvent(bindObject, "BoolProperty", Handler);

		// Act
		bindObject.BoolProperty = false;

		// Assert
		Assert.That(propertyValueChanged, Is.True);
	}

	[Test]
	public void AddingAPropertyEventByExpressionShouldWork()
	{
		var propertyValueChanged = false;
		void Handler(object sender, EventArgs e) => propertyValueChanged = true;

		var bindObject = new BindObject { BoolProperty = true };
		Binding.AddPropertyEvent(bindObject, obj => obj.BoolProperty, Handler);

		// Act
		bindObject.BoolProperty = false;

		// Assert
		Assert.That(propertyValueChanged, Is.True);
	}

	[Test]
	public void AddingMultiplePropertyEventsShouldWork()
	{
		var boolPropertyValueChanged = false;
		var intPropertyValueChanged = false;
		var stringPropertyValueChanged = false;
		void BoolHandler(object sender, EventArgs e) => boolPropertyValueChanged = true;
		void IntHandler(object sender, EventArgs e) => intPropertyValueChanged = true;
		void StringHandler(object sender, EventArgs e) => stringPropertyValueChanged = true;

		var bindObject = new BindObject { BoolProperty = true, IntProperty = 3, StringProperty = "Test1" };
		Binding.AddPropertyEvent(bindObject, obj => obj.BoolProperty, BoolHandler);
		Binding.AddPropertyEvent(bindObject, obj => obj.IntProperty, IntHandler);
		Binding.AddPropertyEvent(bindObject, obj => obj.StringProperty, StringHandler);

		// Act
		bindObject.BoolProperty = false;
		bindObject.IntProperty = 4;
		bindObject.StringProperty = "Test2";

		// Assert
		Assert.That(boolPropertyValueChanged, Is.True);
		Assert.That(intPropertyValueChanged, Is.True);
		Assert.That(stringPropertyValueChanged, Is.True);
	}

	[Test]
	public void APropertyEventShouldNotRespondToADifferentProperty()
	{
		var propertyValueChanged = false;
		void Handler(object sender, EventArgs e) => propertyValueChanged = true;

		var bindObject = new BindObject { BoolProperty = true, IntProperty = 1 };
		Binding.AddPropertyEvent(bindObject, obj => obj.BoolProperty, Handler);

		// Act
		bindObject.IntProperty = 2;

		// Assert
		Assert.That(propertyValueChanged, Is.False);
	}

	[Test]
	public void RemovingAPropertyEventShouldWork()
	{
		var propertyValueChanged = false;
		void Handler(object sender, EventArgs e) => propertyValueChanged = true;

		var bindObject = new BindObject { BoolProperty = true };
		var obj = Binding.AddPropertyEvent(bindObject, obj => obj.BoolProperty, Handler);

		// Act
		Binding.RemovePropertyEvent(bindObject, Handler);
		bindObject.BoolProperty = false;

		// Assert
		Assert.That(propertyValueChanged, Is.False);
	}

	[Test]
	public void RemovingAPropertyEventShouldKeepOtherEvents()
	{
		var boolPropertyValueChanged = false;
		var intPropertyValueChanged = false;
		var stringPropertyValueChanged = false;
		void BoolHandler(object sender, EventArgs e) => boolPropertyValueChanged = true;
		void IntHandler(object sender, EventArgs e) => intPropertyValueChanged = true;
		void StringHandler(object sender, EventArgs e) => stringPropertyValueChanged = true;

		var bindObject = new BindObject { BoolProperty = true, IntProperty = 3, StringProperty = "Test1" };
		var boolObj = Binding.AddPropertyEvent(bindObject, obj => obj.BoolProperty, BoolHandler);
		var intObj = Binding.AddPropertyEvent(bindObject, obj => obj.IntProperty, IntHandler);
		var stringObj = Binding.AddPropertyEvent(bindObject, obj => obj.StringProperty, StringHandler);

		// Act
		Binding.RemovePropertyEvent(bindObject, IntHandler);
		bindObject.BoolProperty = false;
		bindObject.IntProperty = 4;
		bindObject.StringProperty = "Test2";

		// Assert
		Assert.That(boolPropertyValueChanged, Is.True);
		Assert.That(intPropertyValueChanged, Is.False);
		Assert.That(stringPropertyValueChanged, Is.True);
	}

	[Test]
	public void RemovingAllPropertyEventsShouldWork()
	{
		var boolPropertyValueChanged = false;
		var intPropertyValueChanged = false;
		var stringPropertyValueChanged = false;
		void BoolHandler(object sender, EventArgs e) => boolPropertyValueChanged = true;
		void IntHandler(object sender, EventArgs e) => intPropertyValueChanged = true;
		void StringHandler(object sender, EventArgs e) => stringPropertyValueChanged = true;

		var bindObject = new BindObject { BoolProperty = true, IntProperty = 3, StringProperty = "Test1" };
		var boolObj = Binding.AddPropertyEvent(bindObject, obj => obj.BoolProperty, BoolHandler);
		var intObj = Binding.AddPropertyEvent(bindObject, obj => obj.IntProperty, IntHandler);
		var stringObj = Binding.AddPropertyEvent(bindObject, obj => obj.StringProperty, StringHandler);

		// Act
		Binding.RemovePropertyEvent(bindObject, StringHandler);
		Binding.RemovePropertyEvent(bindObject, BoolHandler);
		Binding.RemovePropertyEvent(bindObject, IntHandler);
		bindObject.BoolProperty = false;
		bindObject.IntProperty = 4;
		bindObject.StringProperty = "Test2";

		// Assert
		Assert.That(boolPropertyValueChanged, Is.False);
		Assert.That(intPropertyValueChanged, Is.False);
		Assert.That(stringPropertyValueChanged, Is.False);
	}
	
	[Test]
	public void UnsubscribingToSameEventShouldntUnsubscribeAllWhenPassingProperty()
	{
		var numchanged = 0;
		void Handler(object sender, EventArgs e) => numchanged++;

		var bindObject = new BindObject { BoolProperty = true, IntProperty = 3, StringProperty = "Test1" };
		Binding.AddPropertyEvent(bindObject, obj => obj.BoolProperty, Handler);
		Binding.AddPropertyEvent(bindObject, obj => obj.IntProperty, Handler);
		Binding.AddPropertyEvent(bindObject, obj => obj.StringProperty, Handler);

		// Act
		Binding.RemovePropertyEvent(bindObject, obj => obj.BoolProperty, Handler);
		bindObject.BoolProperty = false;
		bindObject.IntProperty = 4;
		bindObject.StringProperty = "Test2";

		// Assert
		Assert.That(numchanged, Is.EqualTo(2));
	}

	[Test]
	public void UnsubscribingToSameEventWillUnsubscribeAll()
	{
		var numchanged = 0;
		void Handler(object sender, EventArgs e) => numchanged++;

		var bindObject = new BindObject { BoolProperty = true, IntProperty = 3, StringProperty = "Test1" };
		Binding.AddPropertyEvent(bindObject, obj => obj.BoolProperty, Handler);
		Binding.AddPropertyEvent(bindObject, obj => obj.IntProperty, Handler);
		Binding.AddPropertyEvent(bindObject, obj => obj.StringProperty, Handler);

		// Act
		Binding.RemovePropertyEvent(bindObject, Handler);
		bindObject.BoolProperty = false;
		bindObject.IntProperty = 4;
		bindObject.StringProperty = "Test2";

		// Assert
		// ambiguous which one to remove, so it removed all -- need to specify property or return value from AddPropertyEvent
		Assert.That(numchanged, Is.EqualTo(0));
	}

	[Test]
	public void UnsubscribingToAReferencedEventShouldWork()
	{
		var numchanged = 0;
		void Handler(object sender, EventArgs e) => numchanged++;

		var bindObject = new BindObject { BoolProperty = true, IntProperty = 3, StringProperty = "Test1" };
		var boolObj = Binding.AddPropertyEvent(bindObject, obj => obj.BoolProperty, Handler);
		Binding.AddPropertyEvent(bindObject, obj => obj.IntProperty, Handler);
		Binding.AddPropertyEvent(bindObject, obj => obj.StringProperty, Handler);

		// Act
		Binding.RemovePropertyEvent(boolObj, Handler);
		bindObject.BoolProperty = false;
		bindObject.IntProperty = 4;
		bindObject.StringProperty = "Test2";

		// Assert
		Assert.That(numchanged, Is.EqualTo(2));
	}
	
}
