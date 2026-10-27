#if GTK3
using System.Reflection;
using Eto.GtkSharp.Forms.Controls;
using Eto.Test.UnitTests;
using NUnit.Framework;
using g = Gtk;

namespace Eto.Test.Gtk.UnitTests
{
	[TestFixture]
	public class CheckBoxTests : TestBase
	{
		[Test]
		public void AssigningCheckedShouldPreserveValueAndNotifyOnce()
		{
			Invoke(() =>
			{
				using var checkBox = new CheckBox { ThreeState = true };
				var observed = new List<bool?>();
				checkBox.CheckedChanged += (sender, e) => observed.Add(checkBox.Checked);
				var expected = new bool?[] { true, null, false };

				foreach (var value in expected)
				{
					checkBox.Checked = value;
					Assert.That(checkBox.Checked, Is.EqualTo(value));
					checkBox.Checked = value;
					Assert.That(checkBox.Checked, Is.EqualTo(value));
				}
				Assert.That(observed, Is.EqualTo(expected), "Repeated assignments should not notify");
			});
		}

		[Test]
		public void NativeClicksShouldCycleAndNotifyOnce()
		{
			Invoke(() =>
			{
				using var checkBox = new CheckBox { ThreeState = true };
				var native = (g.CheckButton)checkBox.ControlObject;
				var observed = new List<bool?>();
				checkBox.CheckedChanged += (sender, e) => observed.Add(checkBox.Checked);
				var expected = new bool?[] { null, true, false };

				for (var i = 0; i < expected.Length; i++)
				{
					native.Click();
					Assert.That(checkBox.Checked, Is.EqualTo(expected[i]));
					Assert.That(observed.Count, Is.EqualTo(i + 1));
				}
				Assert.That(observed, Is.EqualTo(expected));
			});
		}

		[Test]
		public void NativeClickCallbackShouldAllowCheckedAssignment()
		{
			Invoke(() =>
			{
				using var checkBox = new CheckBox { ThreeState = true };
				var native = (g.CheckButton)checkBox.ControlObject;
				var observed = new List<bool?>();
				checkBox.CheckedChanged += (sender, e) =>
				{
					observed.Add(checkBox.Checked);
					if (observed.Count == 1)
						checkBox.Checked = false;
				};

				native.Click();
				Assert.That(checkBox.Checked, Is.False);
				Assert.That(observed, Is.EqualTo(new bool?[] { null, false }));
				native.Click();
				native.Click();
				Assert.That(checkBox.Checked, Is.True);
				Assert.That(observed, Is.EqualTo(new bool?[] { null, false, null, true }));
			});
		}

		[Test]
		public void ThrowingToggleCallbackShouldNotSuppressLaterClicks()
		{
			Invoke(() =>
			{
				using var checkBox = new CheckBox { ThreeState = true };
				var native = (g.CheckButton)checkBox.ControlObject;
				var expected = new InvalidOperationException("CheckedChanged failure");
				EventHandler<EventArgs> throwing = (sender, e) => throw expected;
				checkBox.CheckedChanged += throwing;

				// Keep the expected exception on the managed stack, outside GTK's native callback boundary.
				var connector = typeof(CheckBoxHandler).GetProperty("Connector", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly).GetValue(checkBox.Handler);
				var method = connector.GetType().GetMethod("HandleToggled");
				var exception = Assert.Throws<TargetInvocationException>(() => method.Invoke(connector, new object[] { native, EventArgs.Empty }));
				Assert.That(exception.InnerException, Is.SameAs(expected));
				checkBox.CheckedChanged -= throwing;
				var observed = new List<bool?>();
				checkBox.CheckedChanged += (sender, e) => observed.Add(checkBox.Checked);

				native.Click();
				native.Click();
				Assert.That(checkBox.Checked, Is.True);
				Assert.That(observed, Is.EqualTo(new bool?[] { null, true }));
			});
		}
	}
}
#endif
