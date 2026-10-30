using System.CodeDom.Compiler;
using Eto.Serialization.Xaml;
using NUnit.Framework;
using JsonTest = Eto.Test.Sections.Serialization.Json.Test;

namespace Eto.Test.UnitTests.Forms.Xaml
{
	/// <summary>.jeto files loaded and compiled by Eto.Serialization.Xaml.</summary>
	[TestFixture]
	public class CompiledJsonTests : TestBase
	{
		static IEnumerable<string> Describe(Container container)
		{
			foreach (var control in container.Children)
			{
				var text = (control as TextControl)?.Text;
				yield return $"{control.GetType().Name} ID={control.ID} Text={text}";
			}
		}

		[Test]
		public void GeneratedMethodShouldExist()
		{
			var method = typeof(CompiledJsonPanel).GetMethod("InitializeXeto", BindingFlags.Instance | BindingFlags.NonPublic);
			Assert.That(method, Is.Not.Null, "The source generator should have compiled the .jeto");
			Assert.That(method.GetCustomAttribute<GeneratedCodeAttribute>()?.Tool, Is.EqualTo("Portable.Xaml.CodeGen"));
		}

		[Test]
		public void LoadShouldUseCompiledCode()
		{
			Invoke(() =>
			{
				Assert.That(new CompiledJsonPanel().ModeLabel.Text, Is.EqualTo("compiled"));
				Assert.That(new CompiledJsonPanel(parseAtRuntime: true).ModeLabel.Text, Is.EqualTo("runtime"));
			});
		}

		[Test]
		public void CompiledShouldMatchRuntime()
		{
			Invoke(() =>
			{
				var compiled = new CompiledJsonPanel();
				var runtime = new CompiledJsonPanel(parseAtRuntime: true);

				var expected = Describe(runtime).Select(r => r.Replace("Text=runtime", "Text=compiled")).ToList();
				Assert.That(Describe(compiled).ToList(), Is.EqualTo(expected));
				Assert.That(compiled.Padding, Is.EqualTo(new Padding(5)), "design-time values are ignored");
				Assert.That(runtime.Padding, Is.EqualTo(new Padding(5)));

				var layout = compiled.FindChild<TableLayout>("Layout");
				Assert.That(layout.Spacing, Is.EqualTo(new Size(5, 5)));
				Assert.That(layout.Rows.Count, Is.EqualTo(7));
				Assert.That(layout.Rows.Count, Is.EqualTo(runtime.FindChild<TableLayout>("Layout").Rows.Count));

				Assert.That(compiled.Children.OfType<Label>().Select(r => r.Text), Is.SupersetOf(new[] { "Text content", "Nested", "{Escaped}", "A", "B" }));
				Assert.That(compiled.Children.OfType<Label>().Single(r => r.Text == "B").TextColor, Is.EqualTo(Colors.Red));
				Assert.That(compiled.Children.OfType<ImageView>().Single().Image, Is.Not.Null);
				Assert.That(layout.Rows[2].Cells[0].ScaleWidth, Is.True);

				foreach (var panel in new[] { compiled, runtime })
				{
					var items = panel.Choices.Items;
					Assert.That(items.Select(r => r.Text), Is.EqualTo(new[] { "One", "Two" }));
					Assert.That(items[1].Key, Is.EqualTo("2"));
				}
			});
		}

		[Test]
		public void NamesAndEventsShouldBeSet()
		{
			Invoke(() =>
			{
				foreach (var panel in new[] { new CompiledJsonPanel(), new CompiledJsonPanel(parseAtRuntime: true) })
				{
					Assert.That(panel.ModeLabel?.ID, Is.EqualTo("ModeLabel"), "$name should set the member and ID");
					Assert.That(panel.CheckBox?.Checked, Is.True);
					Assert.That(panel.BoundTextBox, Is.Not.Null, "ID should set a field");
					var layoutField = typeof(CompiledJsonPanel).GetField("Layout", BindingFlags.Instance | BindingFlags.NonPublic);
					Assert.That(layoutField?.GetValue(panel), Is.SameAs(panel.Content), "$name without a member should generate a field");

					panel.ClickButton.PerformClick();
					Assert.That(panel.Clicks, Is.EqualTo(1));
					panel.DataContext = new { Name = "bound" };
					Assert.That(panel.BoundTextBox.Text, Is.EqualTo("bound"));
					Assert.That(panel.TextChanges, Is.GreaterThan(0));
				}
			});
		}

		[Test]
		public void JetoShouldLoadLikeJsonReader()
		{
			Invoke(() =>
			{
				var json = new JsonTest();
				var compiled = new JsonTest(t => JetoReader.Load(t));
				var runtime = new JsonTest(t =>
				{
					using var stream = typeof(JsonTest).Assembly.GetManifestResourceStream(typeof(JsonTest).FullName + ".jeto");
					JetoReader.Load(stream, t);
				});

				// unlike JsonReader, $name also sets the ID as x:Name does in .xeto files,
				// and the external LabelSection has random text
				static string GetText(Control control) => control.Parents.OfType<Sections.Controls.LabelSection>().Any() ? null : (control as TextControl)?.Text;
				static List<string> WithoutIDs(Container container) => container.Children.Select(r => $"{r.GetType().Name} Text={GetText(r)}").ToList();
				var expected = WithoutIDs(json);
				Assert.That(WithoutIDs(runtime), Is.EqualTo(expected), "Parsed at runtime");
				Assert.That(WithoutIDs(compiled), Is.EqualTo(expected), "Compiled");
			});
		}
	}
}
