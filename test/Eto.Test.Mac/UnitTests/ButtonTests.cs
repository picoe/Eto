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
				Assert.IsNotNull(handler, "#1.1");
				
				// big sur changed default height from 21 to 22.
				var defaultButtonHeight = ButtonHandler.DefaultButtonSize.Height;
				
				var b = new EtoButton(NSButtonType.MomentaryPushIn);
				var originalSize = b.GetAlignmentRectForFrame(new CGRect(CGPoint.Empty, b.FittingSize)).Size;
				Assert.AreEqual(defaultButtonHeight, originalSize.Height, "#2.1");

				var preferred = handler.GetPreferredSize(SizeF.PositiveInfinity);
				Assert.AreEqual(originalSize.Height, preferred.Height, "#2.1");
				Assert.AreEqual(NSBezelStyle.Rounded, handler.Control.BezelStyle, "#2.2");

				form.Shown += async (sender, e) =>
				{
					try
					{
						// need to use invokes to wait for the layout pass to complete
						panel.Size = new Size(-1, defaultButtonHeight + 1);
						await Task.Delay(1000);
						await Application.Instance.InvokeAsync(() =>
						{
							Assert.AreEqual(NSBezelStyle.RegularSquare, handler.Control.BezelStyle, "#3.1");
							Assert.AreEqual(defaultButtonHeight + 1, handler.Widget.Height, "#3.2");
						});
						panel.Size = new Size(-1, -1);
						await Application.Instance.InvokeAsync(() =>
						{
							Assert.AreEqual(NSBezelStyle.Rounded, handler.Control.BezelStyle, "#4.1");
							Assert.AreEqual(defaultButtonHeight, handler.Widget.Height, "#4.2");
						});
						panel.Size = new Size(-1, defaultButtonHeight - 1);
						await Task.Delay(1000);
						await Application.Instance.InvokeAsync(() =>
						{
							Assert.AreEqual(NSBezelStyle.SmallSquare, handler.Control.BezelStyle, "#5.1");
							Assert.AreEqual(defaultButtonHeight - 1, handler.Widget.Height, "#5.2");
						});
						panel.Size = new Size(-1, -1);
						await Application.Instance.InvokeAsync(() =>
						{
							Assert.AreEqual(NSBezelStyle.Rounded, handler.Control.BezelStyle, "#6.1");
							Assert.AreEqual(defaultButtonHeight, handler.Widget.Height, "#6.2");
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