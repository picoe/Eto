#if GTK3
using Eto.GtkSharp;
using Eto.Test.UnitTests;
using NUnit.Framework;
using g = Gtk;

namespace Eto.Test.Gtk.UnitTests
{
	[TestFixture]
	public class GroupBoxTests : TestBase
	{
		[Test]
		public void TitleChangesShouldPreserveAssignedStyles()
		{
			Invoke(() =>
			{
				using var font = new Font(FontFamilies.Sans, 17, FontStyle.Bold);
				using var groupBox = new GroupBox { Font = font, TextColor = Colors.Red };
				var frame = (g.Frame)groupBox.ControlObject;
				foreach (var (title, displayed) in new[]
				{
					("&Title && _text", "Title & _text"),
					("Re&named && _text", "Renamed & _text")
				})
				{
					groupBox.Text = title;
					Assert.That(groupBox.Text, Is.EqualTo(title));
					Assert.That(((g.Label)frame.LabelWidget).Text, Is.EqualTo(displayed));
					AssertLabelStyle(groupBox, font, Colors.Red);
				}

				groupBox.Text = null;
				Assert.That(groupBox.Text, Is.Null);
				Assert.That(frame.LabelWidget, Is.Null);
				groupBox.TextColor = default(Color);
				groupBox.Text = "Restored";
				AssertLabelStyle(groupBox, font, default(Color));
			});
		}

		static void AssertLabelStyle(GroupBox groupBox, Font font, Color color)
		{
			var label = ((g.Frame)groupBox.ControlObject).LabelWidget;
			Assert.That(label, Is.Not.Null);
			using var actualFont = label.GetFont();
			var expectedFont = font.ToPango();
			Assert.Multiple(() =>
			{
				Assert.That(actualFont.Size, Is.EqualTo(expectedFont.Size));
				Assert.That(actualFont.Weight, Is.EqualTo(expectedFont.Weight));
				Assert.That(actualFont.Style, Is.EqualTo(expectedFont.Style));
				Assert.That(label.GetForeground().ToArgb(), Is.EqualTo(color.ToArgb()));
			});
		}
	}
}
#endif
