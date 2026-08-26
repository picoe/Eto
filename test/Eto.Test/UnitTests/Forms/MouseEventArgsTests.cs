using Eto.Forms.ThemedControls;
using NUnit.Framework;

namespace Eto.Test.UnitTests.Forms
{
	[TestFixture]
	public class MouseEventArgsTests : TestBase
	{
		[Test]
		public void IsDirectionInvertedShouldDefaultToFalse()
		{
			var args = new MouseEventArgs(MouseButtons.None, Keys.None, new PointF(10, 20), new SizeF(0, 1));
			Assert.That(args.IsDirectionInverted, Is.False);
		}

		[TestCase(false)]
		[TestCase(true)]
		public void IsDirectionInvertedShouldComeFromConstructor(bool isDirectionInverted)
		{
			var args = new MouseEventArgs(MouseButtons.Middle, Keys.Shift, new PointF(10, 20), new SizeF(0, -1), 0.5f, isDirectionInverted);

			Assert.Multiple(() =>
			{
				Assert.That(args.IsDirectionInverted, Is.EqualTo(isDirectionInverted));
				// the other values must not shift with the added parameter
				Assert.That(args.Buttons, Is.EqualTo(MouseButtons.Middle));
				Assert.That(args.Modifiers, Is.EqualTo(Keys.Shift));
				Assert.That(args.Location, Is.EqualTo(new PointF(10, 20)));
				Assert.That(args.Delta, Is.EqualTo(new SizeF(0, -1)));
				Assert.That(args.Pressure, Is.EqualTo(0.5f));
			});
		}

		/// <summary>
		/// Controls that forward mouse events from a child control rebuild the event args, so anything added to
		/// <see cref="MouseEventArgs"/> has to be carried over when they do.
		/// </summary>
		[Test]
		public void ThemedTextStepperShouldForwardIsDirectionInverted()
		{
			MouseEventArgs forwarded = null;
			Shown(form =>
			{
				var stepper = new TextStepper();
				stepper.MouseWheel += (sender, e) => forwarded = e;
				form.Content = stepper;
				return stepper;
			},
			stepper =>
			{
				if (!(((IHandlerSource)stepper).Handler is ThemedTextStepperHandler handler))
					Assert.Ignore("TextStepper does not use the themed handler on this platform");
				else
					RaiseMouseWheel(handler.TextBox, isDirectionInverted: true);
			});

			Assert.That(forwarded, Is.Not.Null, "MouseWheel was not forwarded from the child control");
			Assert.That(forwarded.IsDirectionInverted, Is.True);
		}

		static void RaiseMouseWheel(Control control, bool isDirectionInverted)
		{
			var callback = (Control.ICallback)((ICallbackSource)control).Callback;
			var args = new MouseEventArgs(MouseButtons.None, Keys.None, PointF.Empty, new SizeF(0, 1), 1.0f, isDirectionInverted);
			callback.OnMouseWheel(control, args);
		}
	}
}
