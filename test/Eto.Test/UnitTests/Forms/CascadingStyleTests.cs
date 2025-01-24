using NUnit.Framework;

namespace Eto.Test.UnitTests.Forms
{
    [TestFixture]
    public class CascadingStyleTests : TestBase
    {
        [Test, InvokeOnUI]
        public void DefaultStyleShouldApplyFromContainer()
        {
            var container = new Panel();

            container.Styles.Add<Label>(null, l => l.Visible = false);

            var child = new Label();
            container.Content = child;
            Assert.That(child.Visible, Is.True);

            container.AttachNative(); // trigger load to apply styles

            Assert.That(child.Visible, Is.False);
        }

        [Test, InvokeOnUI]
        public void StyleShouldApplyFromContainer()
        {
            var container = new Panel();

            container.Styles.Add<Label>("style", l => l.Visible = false);

            var child = new Label();
            container.Content = child;
            Assert.That(child.Visible, Is.True);

            container.AttachNative(); // trigger load to apply styles
            Assert.That(child.Visible, Is.True);

            child.Style = "style";

            Assert.That(child.Visible, Is.False);
        }

        [Test, InvokeOnUI]
        public void StyleShouldApplyFromParentToChildOrder()
        {
            var container1 = new Panel();
            container1.Styles.Add<Label>(null, l => l.VerticalAlignment = VerticalAlignment.Bottom);

            var container2 = new Panel();
            container2.Styles.Add<Label>("style", l => l.VerticalAlignment = VerticalAlignment.Center);
            container1.Content = container2;

            var child = new Label();
            container2.Content = child;
            Assert.That(child.VerticalAlignment, Is.EqualTo(VerticalAlignment.Top));

            container1.AttachNative(); // trigger load to apply styles

            // container1 style applies
            Assert.That(child.VerticalAlignment, Is.EqualTo(VerticalAlignment.Bottom));

            child.Style = "style";

            // container2 style now applies
            Assert.That(child.VerticalAlignment, Is.EqualTo(VerticalAlignment.Center));

            child.Style = null;

            // container1 style now applies again
            Assert.That(child.VerticalAlignment, Is.EqualTo(VerticalAlignment.Bottom));
        }

		[Test, InvokeOnUI]
		public void StyleShouldApplyWhenControlDynamicallyAdded()
		{
			var container = new Panel();

			container.Styles.Add<Label>("style", l => l.Visible = false);

			container.AttachNative(); // trigger load to apply styles

			var child = new Label();
			child.Style = "style";
			Assert.That(child.Visible, Is.True);

			container.Content = child; // styles apply now that it is a child of the container

			Assert.That(child.Visible, Is.False);
		}

		[Test, InvokeOnUI]
		public void DefaultStyleShouldApplyWhenControlDynamicallyAdded()
		{
			var container = new Panel();

			container.Styles.Add<Label>(null, l => l.Visible = false);

			container.AttachNative();

			var child = new Label();
			Assert.That(child.Visible, Is.True);

			container.Content = child; // styles apply now that it is a child of the container

			Assert.That(child.Visible, Is.False);
		}
	}
}
