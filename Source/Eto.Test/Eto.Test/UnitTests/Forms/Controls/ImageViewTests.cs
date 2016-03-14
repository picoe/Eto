using System;
using NUnit.Framework;
using Eto.Forms;
using Eto.Drawing;

namespace Eto.Test.UnitTests.Forms.Controls
{
	[TestFixture]
	public class ImageViewTests
	{
		[Test]
		public void NoImageShouldNotCrash()
		{
			TestBase.Form(form =>
			{
				form.Content = new ImageView();
				form.Shown += (sender, e) => Application.Instance.AsyncInvoke(form.Close);
			});
		}

		[Test]
		public void NullImageShouldNotCrash()
		{
			TestBase.Form(form =>
			{
				form.Content = new ImageView { Image = null };
				form.Shown += (sender, e) => Application.Instance.AsyncInvoke(form.Close);
			});
		}

		[Test]
		public void ImageShouldNotCrash()
		{
			TestBase.Form(form =>
			{
				form.Content = new ImageView { Image = TestIcons.TestImage };
				form.Shown += (sender, e) => Application.Instance.AsyncInvoke(form.Close);
			});
		}
	}
}

