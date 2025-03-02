using Eto.Mac.Forms.Controls;
using Eto.Test.UnitTests;
using NUnit.Framework;
using System.Runtime.ExceptionServices;
using Eto.Mac;

namespace Eto.Test.Mac.UnitTests
{
	[TestFixture]
	public class ButtonTests : TestBase
	{
		[Test]
		public void ButtonNaturalSizeShouldBeConsistent()
		{
			Exception exception = null;
			Button button = null;
			Panel panel = null;
			Form(form => {
				button = new Button();
				button.Text = "Click Me";
				panel = new Panel { Content = button };
				form.Content = TableLayout.AutoSized(panel);
				form.ClientSize = new Size(200, 200);

				var handler = button?.Handler as ButtonHandler;
				Assert.That(handler, Is.Not.Null, "#1.1");
				
				// big sur changed default height from 21 to 22.
				var defaultButtonHeight = ButtonHandler.DefaultButtonSize.Height;
				
				var b = new EtoButton(NSButtonType.MomentaryPushIn);
				var originalSize = b.GetAlignmentRectForFrame(new CGRect(CGPoint.Empty, b.FittingSize)).Size;
				Assert.That(originalSize.Height, Is.EqualTo((nfloat)defaultButtonHeight), "#2.1");

				var preferred = handler.GetPreferredSize(SizeF.PositiveInfinity);
				Assert.That(preferred.Height, Is.EqualTo(originalSize.Height), "#2.1");
				Assert.That(handler.Control.BezelStyle, Is.EqualTo(NSBezelStyle.Rounded), "#2.2");

				form.Shown += async (sender, e) =>
				{
					try
					{
						// need to use invokes to wait for the layout pass to complete
						panel.Size = new Size(-1, defaultButtonHeight + 1);
						await Task.Delay(1000);
						await Application.Instance.InvokeAsync(() =>
						{
							Assert.That(handler.Control.BezelStyle, Is.EqualTo(NSBezelStyle.RegularSquare), "#3.1");
							Assert.That(handler.Widget.Height, Is.EqualTo(defaultButtonHeight + 1), "#3.2");
						});
						panel.Size = new Size(-1, -1);
						await Application.Instance.InvokeAsync(() =>
						{
							Assert.That(handler.Control.BezelStyle, Is.EqualTo(NSBezelStyle.Rounded), "#4.1");
							Assert.That(handler.Widget.Height, Is.EqualTo(defaultButtonHeight), "#4.2");
						});
						panel.Size = new Size(-1, defaultButtonHeight - 1);
						await Task.Delay(1000);
						await Application.Instance.InvokeAsync(() =>
						{
							Assert.That(handler.Control.BezelStyle, Is.EqualTo(NSBezelStyle.SmallSquare), "#5.1");
							Assert.That(handler.Widget.Height, Is.EqualTo(defaultButtonHeight - 1), "#5.2");
						});
						panel.Size = new Size(-1, -1);
						await Application.Instance.InvokeAsync(() =>
						{
							Assert.That(handler.Control.BezelStyle, Is.EqualTo(NSBezelStyle.Rounded), "#6.1");
							Assert.That(handler.Widget.Height, Is.EqualTo(defaultButtonHeight), "#6.2");
						});

					}
					catch (Exception ex)
					{
						exception = ex;
					}
					finally
					{
						form.Close();
					}
				};



			}, -1);

			if (exception != null)
				ExceptionDispatchInfo.Capture(exception).Throw();
		}
	}
}