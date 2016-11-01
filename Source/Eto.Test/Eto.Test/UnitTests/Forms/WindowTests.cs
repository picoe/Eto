using System;
using NUnit.Framework;
using Eto.Forms;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Specialized;
using Eto.Drawing;

namespace Eto.Test.UnitTests.Forms
{
	[TestFixture]
	public class WindowTests : TestBase
	{
		[Test]
		public void WindowShouldReportInitialSize()
		{
			TestBase.Form(form =>
			{
				Size? size = null;
				form.Content = new Panel { Size = new Size(300, 300) };
				form.SizeChanged += (sender, e) => size = form.Size;

				form.Shown += (sender, e) => {
					Assert.IsNotNull(size, "#1");
					Assert.IsTrue(size.Value.Width >= 300, "#2");
					Assert.IsTrue(size.Value.Height>= 300, "#3");
					form.Close();
				};
			});
		}

		[Test]
		public void DefaultFormValuesShouldBeCorrect()
		{
			TestProperties(f => f,
				f => f.CanFocus,
				f => f.ShowActivated,
				f => f.Enabled
			);
		}
	}
}

