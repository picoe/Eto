using Eto.Forms;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eto.Drawing;

namespace Eto.Test.UnitTests.Forms
{
	[TestFixture]
	public class ClipboardTests : TestBase
	{
		[Test]
		public void GettingAndSettingTextShouldNotCrash()
		{
			Invoke(() =>
			{
				for (int i = 0; i < 100; i++)
				{
					// this crashes on WPF on some machines.. don't know why as I can't repro the issue.
					var clipboard = new Clipboard();
					var val = "Hello" + i;
					clipboard.Text = val;
					Assert.AreEqual(val, clipboard.Text);
				}
			});
		}

		[Test]
		public void SettingMultipleFormatsShouldWork()
		{
			Invoke(() =>
			{
				var byteData = new byte[] { 10, 20, 30 };
				using (var clipboard = new Clipboard())
				{
					clipboard.Text = "Text";
					clipboard.Html = "<strong>Some Html</strong>";
					clipboard.SetString("woot", "eto-woot");
					clipboard.SetData(byteData, "eto-byte-data");

					Assert.AreEqual("Text", clipboard.Text);
					Assert.AreEqual("<strong>Some Html</strong>", clipboard.Html);
					Assert.AreEqual("woot", clipboard.GetString("eto-woot"));
					Assert.AreEqual(byteData, clipboard.GetData("eto-byte-data"));

					Assert.Contains("eto-woot", clipboard.Types);
					Assert.Contains("eto-byte-data", clipboard.Types);
				}

				using (var clipboard = new Clipboard())
				{
					Assert.AreEqual("Text", clipboard.Text);
					Assert.AreEqual("<strong>Some Html</strong>", clipboard.Html);
					Assert.AreEqual("woot", clipboard.GetString("eto-woot"));
					Assert.AreEqual(byteData, clipboard.GetData("eto-byte-data"));

					Assert.Contains("eto-woot", clipboard.Types);
					Assert.Contains("eto-byte-data", clipboard.Types);

					clipboard.Clear();
					CollectionAssert.DoesNotContain("eto-woot", clipboard.Types);
					CollectionAssert.DoesNotContain("eto-byte-data", clipboard.Types);
					Assert.AreEqual(null, clipboard.Text);
					Assert.AreEqual(null, clipboard.Html);
					Assert.AreEqual(null, clipboard.Image);
					Assert.AreEqual(null, clipboard.GetString("eto-woot"));
					Assert.AreEqual(null, clipboard.GetData("eto-byte-data"));
				}

				using (var clipboard = new Clipboard())
				{
					CollectionAssert.DoesNotContain("eto-woot", clipboard.Types);
					CollectionAssert.DoesNotContain("eto-byte-data", clipboard.Types);
					Assert.AreEqual(null, clipboard.Text);
					Assert.AreEqual(null, clipboard.Html);
					Assert.AreEqual(null, clipboard.Image);
					Assert.AreEqual(null, clipboard.GetString("eto-woot"));
					Assert.AreEqual(null, clipboard.GetData("eto-byte-data"));
				}
			});
		}

		[TestCase("Text")]
		[TestCase("Html")]
		//[TestCase("Image-Icon")] // TODO: not yet implemented fully, if even possible on all platforms
		[TestCase("Image-Bitmap")]
		[TestCase("String")]
		[TestCase("Data")]
		public void SettingIndividualValueShouldWork(string property)
		{
			Invoke(() =>
			{
				var byteData = new byte[] { 10, 20, 30 };
				using (var clipboard = new Clipboard())
				{
					switch (property)
					{
						case "Text":
							clipboard.Text = "Some Text";
							Assert.AreEqual("Some Text", clipboard.Text);
							Assert.AreEqual(null, clipboard.Html);
							Assert.AreEqual(null, clipboard.Image);
							Assert.AreEqual(null, clipboard.GetString("eto-woot"));
							Assert.AreEqual(null, clipboard.GetData("eto-byte-data"));
							break;
						case "Html":
							clipboard.Html = "<strong>Some Html</strong>";
							Assert.AreEqual(null, clipboard.Text);
							Assert.AreEqual("<strong>Some Html</strong>", clipboard.Html);
							Assert.AreEqual(null, clipboard.Image);
							Assert.AreEqual(null, clipboard.GetString("eto-woot"));
							Assert.AreEqual(null, clipboard.GetData("eto-byte-data"));
							break;
						case "Image-Icon":
							clipboard.Image = TestIcons.Logo;
							Assert.AreEqual(null, clipboard.Text);
							Assert.AreEqual(null, clipboard.Html);
							Assert.IsNotNull(clipboard.Image);
							Assert.IsInstanceOf<Icon>(clipboard.Image);
							Assert.AreEqual(null, clipboard.GetString("eto-woot"));
							Assert.AreEqual(null, clipboard.GetData("eto-byte-data"));
							break;
						case "Image-Bitmap":
							clipboard.Image = TestIcons.TestImage;
							Assert.AreEqual(null, clipboard.Text);
							Assert.AreEqual(null, clipboard.Html);
							Assert.IsNotNull(clipboard.Image);
							Assert.IsInstanceOf<Bitmap>(clipboard.Image);
							Assert.AreEqual(null, clipboard.GetString("eto-woot"));
							Assert.AreEqual(null, clipboard.GetData("eto-byte-data"));
							break;
						case "String":
							clipboard.SetString("Some string value", "eto-woot");
							Assert.AreEqual(null, clipboard.Text);
							Assert.AreEqual(null, clipboard.Html);
							Assert.AreEqual(null, clipboard.Image);
							Assert.AreEqual("Some string value", clipboard.GetString("eto-woot"));
							Assert.AreEqual(null, clipboard.GetData("eto-byte-data"));
							Assert.Contains("eto-woot", clipboard.Types);
							break;
						case "Data":
							clipboard.SetData(byteData, "eto-byte-data");
							Assert.Contains("eto-byte-data", clipboard.Types);
							Assert.AreEqual(null, clipboard.Text);
							Assert.AreEqual(null, clipboard.Html);
							Assert.AreEqual(null, clipboard.Image);
							Assert.AreEqual(null, clipboard.GetString("eto-woot"));
							Assert.AreEqual(byteData, clipboard.GetData("eto-byte-data"));
							break;
					}
				}

				using (var clipboard = new Clipboard())
				{
					switch (property)
					{
						case "Text":
							Assert.AreEqual("Some Text", clipboard.Text);
							Assert.AreEqual(null, clipboard.Html);
							Assert.AreEqual(null, clipboard.Image);
							Assert.AreEqual(null, clipboard.GetString("eto-woot"));
							Assert.AreEqual(null, clipboard.GetData("eto-byte-data"));
							break;
						case "Html":
							Assert.AreEqual(null, clipboard.Text);
							Assert.AreEqual("<strong>Some Html</strong>", clipboard.Html);
							Assert.AreEqual(null, clipboard.Image);
							Assert.AreEqual(null, clipboard.GetString("eto-woot"));
							Assert.AreEqual(null, clipboard.GetData("eto-byte-data"));
							break;
						case "Image-Icon":
							Assert.AreEqual(null, clipboard.Text);
							Assert.AreEqual(null, clipboard.Html);
							Assert.IsNotNull(clipboard.Image);
							Assert.IsInstanceOf<Icon>(clipboard.Image);
							Assert.AreEqual(null, clipboard.GetString("eto-woot"));
							Assert.AreEqual(null, clipboard.GetData("eto-byte-data"));
							break;
						case "Image-Bitmap":
							Assert.AreEqual(null, clipboard.Text);
							Assert.AreEqual(null, clipboard.Html);
							Assert.IsNotNull(clipboard.Image);
							Assert.IsInstanceOf<Bitmap>(clipboard.Image);
							Assert.AreEqual(null, clipboard.GetString("eto-woot"));
							Assert.AreEqual(null, clipboard.GetData("eto-byte-data"));
							break;
						case "String":
							Assert.AreEqual(null, clipboard.Text);
							Assert.AreEqual(null, clipboard.Html);
							Assert.AreEqual(null, clipboard.Image);
							Assert.AreEqual("Some string value", clipboard.GetString("eto-woot"));
							Assert.AreEqual(null, clipboard.GetData("eto-byte-data"));
							Assert.Contains("eto-woot", clipboard.Types);
							break;
						case "Data":
							Assert.Contains("eto-byte-data", clipboard.Types);
							Assert.AreEqual(null, clipboard.Text);
							Assert.AreEqual(null, clipboard.Html);
							Assert.AreEqual(null, clipboard.Image);
							Assert.AreEqual(null, clipboard.GetString("eto-woot"));
							Assert.AreEqual(byteData, clipboard.GetData("eto-byte-data"));
							break;
					}

				}
			});
		}
	}
}
