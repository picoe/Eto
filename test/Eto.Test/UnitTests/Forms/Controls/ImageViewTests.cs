using System;
using NUnit.Framework;
using Eto.Forms;
using Eto.Drawing;
using System.Runtime.ExceptionServices;
using System.Collections.Generic;

namespace Eto.Test.UnitTests.Forms.Controls
{
	[TestFixture]
	public class ImageViewTests : TestBase
	{
		[Test]
		public void NoImageShouldNotCrash()
		{
			Form(form =>
			{
				form.Content = new ImageView();
				form.Shown += (sender, e) => Application.Instance.AsyncInvoke(form.Close);
			});
		}

		[Test]
		public void NullImageShouldNotCrash()
		{
			Form(form =>
			{
				form.Content = new ImageView { Image = null };
				form.Shown += (sender, e) => Application.Instance.AsyncInvoke(form.Close);
			});
		}

		[Test]
		public void ImageShouldNotCrash()
		{
			Form(form =>
			{
				form.Content = new ImageView { Image = TestIcons.TestImage };
				form.Shown += (sender, e) => Application.Instance.AsyncInvoke(form.Close);
			});
		}

		static IEnumerable<object[]> GetImageSizeTests()
		{
			var logo = TestIcons.Logo;
			var image = TestIcons.TestImage;
			var zeroSize = Platform.Instance.IsGtk ? new Size(1, 1) : new Size(0, 0); // gtk doesn't allow 0,0 size controls?
			yield return new object[] { logo.WithSize(16, 16), new Size(16, 16), logo.WithSize(32, 32), new Size(32, 32) };
			yield return new object[] { logo.WithSize(32, 32), new Size(32, 32), null, zeroSize };
			yield return new object[] { image, image.Size, image.WithSize(32, 32), new Size(32, 32) };
			if (Platform.Instance.IsWinForms)
				zeroSize = new Size(1, 1); // what the? Can't figure out how not to do this.. Not detrimental though.
			yield return new object[] { null, zeroSize, logo.WithSize(32, 32), new Size(32, 32) };
		}

		[Test, TestCaseSource(nameof(GetImageSizeTests))]
		public void ImageSizeShouldUpdateImageViewSize(Image startingImage, Size startingSize, Image updateImage, Size updateSize)
		{
			Exception exception = null;
			Form(form =>
			{
				var imageView = new ImageView();
				imageView.Image = startingImage;
				Assert.AreEqual(new Size(-1, -1), imageView.Size, "#1");
				form.ClientSize = new Size(200, 200);
				form.Content = new StackLayout { Items = { imageView } };

				form.Shown += (sender, e) => 
				{
					try
					{
						imageView.SizeChanged += (sender2, e2) =>
						{
							try
							{
								Assert.AreEqual(updateSize, imageView.Size);
								form.Close();
							}
							catch (Exception ex)
							{
								exception = ex;
								form.Close();
							}
						};

						Assert.AreEqual(startingSize, imageView.Size);
						imageView.Image = updateImage;
					}
					catch (Exception ex)
					{
						exception = ex;
						form.Close();
					}
				};
			});
			if (exception != null)
				ExceptionDispatchInfo.Capture(exception).Throw();
		}
	}
}

