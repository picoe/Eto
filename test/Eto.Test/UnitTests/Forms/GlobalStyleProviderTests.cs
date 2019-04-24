using NUnit.Framework;

namespace Eto.Test.UnitTests.Forms
{
    [TestFixture]
    public class GlobalStyleProviderTests : TestBase
    {
        [Test, InvokeOnUI]
        public void WidgetShouldApplyDefault()
        {
            var style = new DefaultStyleProvider();
            style.Add<StyledWidget>(null, h => h.SomeProperty = true);

            var oldProvider = Style.Provider;
            Style.Provider = style;
            try
            {
                var styledWidget = new StyledWidget();
                Assert.IsTrue(styledWidget.SomeProperty);
            }
            finally
            {
                Style.Provider = oldProvider;
            }
        }

        [Test, InvokeOnUI]
        public void WidgetWithStyleShouldApply()
        {
            var style = new DefaultStyleProvider();
            style.Add<StyledWidget>("style", h => h.SomeProperty = true);

            var oldProvider = Style.Provider;
            Style.Provider = style;
            try
            {
                var styledWidget = new StyledWidget();
                Assert.IsFalse(styledWidget.SomeProperty);
                styledWidget.Style = "style";
                Assert.IsTrue(styledWidget.SomeProperty);
            }
            finally
            {
                Style.Provider = oldProvider;
            }
        }

        [Test, InvokeOnUI]
        public void HandlerShouldApplyDefault()
        {
            var style = new DefaultStyleProvider();
            style.Add<StyledWidgetHandler>(null, h => h.SomeProperty = true);

            var oldProvider = Style.Provider;
            Style.Provider = style;
            try
            {
                var styledWidget = new StyledWidget();
                Assert.IsTrue(styledWidget.SomeProperty);
            }
            finally
            {
                Style.Provider = oldProvider;
            }
        }

        [Test, InvokeOnUI]
        public void HandlerWithStyleShouldApply()
        {
            var style = new DefaultStyleProvider();
            style.Add<StyledWidgetHandler>("style", h => h.SomeProperty = true);

            var oldProvider = Style.Provider;
            Style.Provider = style;
            try
            {
                var styledWidget = new StyledWidget();
                Assert.IsFalse(styledWidget.SomeProperty);
                styledWidget.Style = "style";
                Assert.IsTrue(styledWidget.SomeProperty);
            }
            finally
            {
                Style.Provider = oldProvider;
            }
        }
    }
}
