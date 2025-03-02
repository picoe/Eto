using NUnit.Framework;

namespace Eto.Test.UnitTests.Forms.Bindings;

[TestFixture]
public class DelegateBindingTests
{
	[Test]
	public void SubscribingToPropertyChangesShouldWork()
	{
		int propertyValueChanged = 0;
		void Handler(object sender, EventArgs e) => propertyValueChanged++;

		var bindObject = new BindObject { BoolProperty = true };
		
		var binding = new DelegateBinding<BindObject, bool>(o => o.BoolProperty, (o,v) => o.BoolProperty = v, nameof(BindObject.BoolProperty));
		
		// wire up handler
		var bindingReference = binding.AddValueChangedHandler(bindObject, Handler);

		bindObject.BoolProperty = true;

		Assert.That(propertyValueChanged, Is.EqualTo(1), "Handler should have been fired");

		// Remove the handler binding
		binding.RemoveValueChangedHandler(bindingReference, Handler);

		bindObject.BoolProperty = false;

		// Handler should no longer be triggered
		Assert.That(propertyValueChanged, Is.EqualTo(1), "Handler should not fire after being removed");
	}
}
