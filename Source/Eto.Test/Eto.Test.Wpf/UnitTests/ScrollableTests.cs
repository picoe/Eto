using Eto.Drawing;
using Eto.Forms;
using Eto.Test.UnitTests;
using NUnit.Framework;

namespace Eto.Test.Wpf.UnitTests
{
    [TestFixture]
    public class ScrollableTests : TestBase
    {
        [Test]
        public void ScrollableShouldSetScrollSize()
        {
            Invoke(() =>
            {
                var form = new Form();

                var scrollable = new Scrollable();
                form.Content = scrollable;

                scrollable.ScrollSize = new Size(1000, 1000);
            });
        }
    }
}
