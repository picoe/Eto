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

		[Test]
		public void IconShouldNotCrashInEventsOrAfterClosed()
		{
			Form(form =>
			{
				// shouldn't really be setting ImageView.Image after the form is closed (as you can't re-open the form),
				// but with complex code this may happen so we should protect from crashing.
				var imageView = new ImageView { Width = 64, Height = 64 };
				form.Content = new StackLayout { Items = { imageView } };
				imageView.Image = TestIcons.Logo;
				form.Load += (sender, e) => imageView.Image = TestIcons.TestIcon;
				form.LoadComplete += (sender, e) => imageView.Image = TestIcons.TestIcon;
				form.Closing += (sender, e) =>
				{
					imageView.Image = TestIcons.Logo;
				};
				form.Closed += (sender, e) =>
				{
					imageView.Image = TestIcons.Logo;
				};
				form.Shown += (sender, e) => Application.Instance.AsyncInvoke(() =>
				{
					form.Close();
					imageView.Image = TestIcons.Logo;
				});
			});
		}

		public class ImageSizeTestCase
		{
			public Func<Image> StartImage { get; set; }
			public Func<Image> UpdateImage { get; set; }
			public Func<Size> StartSize { get; set; }
			public Func<Size> UpdateSize { get; set; }
			public ImageSizeTestCase(Func<Image> start, Size startSize, Func<Image> update, Size updateSize)
			{
				StartImage = start;
				UpdateImage = update;
				StartSize = () => startSize;
				UpdateSize = () => updateSize;
			}
			public ImageSizeTestCase(Func<Image> start, Func<Size> startSize, Func<Image> update, Func<Size> updateSize)
			{
				StartImage = start;
				UpdateImage = update;
				StartSize = startSize;
				UpdateSize = updateSize;
			}
			public override string ToString() => $"{StartSize()},{UpdateSize()}";
		}

		static IEnumerable<ImageSizeTestCase> GetImageSizeTests()
		{
			Icon logoInstance = null;
			Icon logo() => logoInstance ?? (logoInstance = TestIcons.Logo);
			Bitmap imageInstance = null;
			Bitmap image() => imageInstance ?? (imageInstance = TestIcons.TestImage);
			var zeroSize = Platform.Instance.IsGtk ? new Size(1, 1) : new Size(0, 0); // gtk doesn't allow 0,0 size controls?
			yield return new ImageSizeTestCase(() => logo().WithSize(16, 16), new Size(16, 16), () => logo().WithSize(32, 32), new Size(32, 32));
			yield return new ImageSizeTestCase(() => logo().WithSize(32, 32), new Size(32, 32), null, zeroSize);
			yield return new ImageSizeTestCase(() => image(), () => image().Size, () => image().WithSize(32, 32), () => new Size(32, 32));
			if (Platform.Instance.IsWinForms)
				zeroSize = new Size(1, 1); // what the? Can't figure out how not to do this.. Not detrimental though.
			yield return new ImageSizeTestCase(null, zeroSize, () => logo().WithSize(32, 32), new Size(32, 32));
		}

		[Test, TestCaseSource(nameof(GetImageSizeTests))]
		public void ImageSizeShouldUpdateImageViewSize(ImageSizeTestCase imageInfo)
		{
			Exception exception = null;
			Form(form =>
			{
				var imageView = new ImageView();
				imageView.Image = imageInfo.StartImage?.Invoke();
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
								Assert.AreEqual(imageInfo.UpdateSize(), imageView.Size);
								form.Close();
							}
							catch (Exception ex)
							{
								exception = ex;
								form.Close();
							}
						};

						Assert.AreEqual(imageInfo.StartSize(), imageView.Size);
						imageView.Image = imageInfo.UpdateImage?.Invoke();
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

