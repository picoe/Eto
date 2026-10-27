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
				foreach (var title in new[] { "Title", "Renamed" })
				{
					groupBox.Text = title;
					AssertLabelStyle(groupBox, font, Colors.Red);
				}

				groupBox.Text = null;
				Assert.That(((g.Frame)groupBox.ControlObject).LabelWidget, Is.Null);
				groupBox.TextColor = default(Color);
				groupBox.Text = "Restored";
				AssertLabelStyle(groupBox, font, default(Color));
			});
		}

		static void AssertLabelStyle(GroupBox groupBox, Font font, Color color)
		{
			var label = ((g.Frame)groupBox.ControlObject).LabelWidget;
			Assert.That(label, Is.Not.Null);
			var actualFont = label.GetFont();
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
