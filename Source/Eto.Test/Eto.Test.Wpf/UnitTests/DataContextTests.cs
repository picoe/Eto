using System;
using System.Collections.Generic;
using Eto.Forms;
using Eto.Drawing;
using Eto.Test.UnitTests;
using NUnit.Framework;
using static Eto.Test.UnitTests.Forms.DataContextTests;

namespace Eto.Test.Wpf.UnitTests
{
    [TestFixture]
    public class DataContextTests : TestBase
    {
		static DataContextTests()
		{
			Platform.Instance.Add<CustomExpander.IHandler>(() => new CustomExpanderHandler());
		}

		[Test]
        public void DataContextInNativeControlShouldBeSet()
        {
			int dataContextChanged = 0;
			Shown(form =>
			{
				var c = new Panel();
				c.DataContextChanged += (sender, e) => dataContextChanged++;
				var expander = new CustomExpander { Content = c };

				var content = new Panel { Content = expander };

				Assert.AreEqual(0, dataContextChanged);

				// embed the expander natively, so it is 'disconnected' from eto
				var holder = new Panel();
				var holderWpf = holder.ToNative() as System.Windows.Controls.Decorator;
				holderWpf.Child = content.ToNative(true);

				form.Content = holder;
				content.DataContext = new MyViewModel();

				Assert.AreEqual(1, dataContextChanged);
			}, () =>
			{
				Assert.AreEqual(1, dataContextChanged);
			});
		}
	}
}
