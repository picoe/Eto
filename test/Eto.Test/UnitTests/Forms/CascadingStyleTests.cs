using Eto.Forms;
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
            Assert.IsTrue(child.Visible);

            container.AttachNative(); // trigger load to apply styles

            Assert.IsFalse(child.Visible);
        }

        [Test, InvokeOnUI]
        public void StyleShouldApplyFromContainer()
        {
            var container = new Panel();

            container.Styles.Add<Label>("style", l => l.Visible = false);

            var child = new Label();
            container.Content = child;
            Assert.IsTrue(child.Visible);

            container.AttachNative(); // trigger load to apply styles
            Assert.IsTrue(child.Visible);

            child.Style = "style";

            Assert.IsFalse(child.Visible);
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
            Assert.AreEqual(VerticalAlignment.Top, child.VerticalAlignment);

            container1.AttachNative(); // trigger load to apply styles

            // container1 style applies
            Assert.AreEqual(VerticalAlignment.Bottom, child.VerticalAlignment);

            child.Style = "style";

            // container2 style now applies
            Assert.AreEqual(VerticalAlignment.Center, child.VerticalAlignment);

            child.Style = null;

            // container1 style now applies again
            Assert.AreEqual(VerticalAlignment.Bottom, child.VerticalAlignment);
        }

		[Test, InvokeOnUI]
		public void StyleShouldApplyWhenControlDynamicallyAdded()
		{
			var container = new Panel();

			container.Styles.Add<Label>("style", l => l.Visible = false);

			container.AttachNative(); // trigger load to apply styles

			var child = new Label();
			child.Style = "style";
			Assert.IsTrue(child.Visible);

			container.Content = child; // styles apply now that it is a child of the container

			Assert.IsFalse(child.Visible);
		}

		[Test, InvokeOnUI]
		public void DefaultStyleShouldApplyWhenControlDynamicallyAdded()
		{
			var container = new Panel();

			container.Styles.Add<Label>(null, l => l.Visible = false);

			container.AttachNative();

			var child = new Label();
			Assert.IsTrue(child.Visible);

			container.Content = child; // styles apply now that it is a child of the container

			Assert.IsFalse(child.Visible);
		}
	}
}
