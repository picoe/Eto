using System.CodeDom.Compiler;
using Eto.Serialization.Xaml;
using NUnit.Framework;

namespace Eto.Test.UnitTests.Forms.Xaml
{
	[TestFixture]
	public class CompiledXamlTests : TestBase
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
			var method = typeof(CompiledXamlPanel).GetMethod("InitializeXeto", BindingFlags.Instance | BindingFlags.NonPublic);
			Assert.That(method, Is.Not.Null, "The source generator should have compiled the .xeto");
			Assert.That(method.GetCustomAttribute<GeneratedCodeAttribute>()?.Tool, Is.EqualTo("Portable.Xaml.CodeGen"));
		}

		[Test]
		public void LoadShouldUseCompiledCode()
		{
			Invoke(() =>
			{
				Assert.That(new CompiledXamlPanel().ModeLabel.Text, Is.EqualTo("compiled"));
				Assert.That(new CompiledXamlPanel(parseAtRuntime: true).ModeLabel.Text, Is.EqualTo("runtime"));
			});
		}

		[Test]
		public void BindingToUnsetCommandShouldLoad()
		{
			// the binding gives back the button's null Command, which WPF's CommandConverter can't convert
			Invoke(() => Assert.That(new Sections.Serialization.Xaml.Test().Content, Is.Not.Null));
		}

		[Test]
		public void WatchedFileShouldUseCompiledCodeUntilChanged()
		{
			Invoke(() =>
			{
				// what hot reload does after the first load
				var watched = new CompiledXamlPanel();
				Assert.That(XamlReader.Watch(watched), Is.True, "the .xeto next to this file should be found");
				Assert.That(new CompiledXamlPanel().ModeLabel.Text, Is.EqualTo("compiled"), "watching alone shouldn't stop using compiled code");

				var file = Path.Combine(Path.GetDirectoryName(CallerFile()), "CompiledXamlPanel.xeto");
				var originalTime = File.GetLastWriteTimeUtc(file);
				try
				{
					File.SetLastWriteTimeUtc(file, originalTime.AddSeconds(1));
					Assert.That(new CompiledXamlPanel().ModeLabel.Text, Is.EqualTo("runtime"), "an edited .xeto must be parsed");
				}
				finally
				{
					File.SetLastWriteTimeUtc(file, originalTime);
				}
			});
		}

		[Test]
		public void LoadingAgainIntoTheSameInstanceShouldReplaceResources()
		{
			Invoke(() =>
			{
				// hot reload parses the .xeto into the existing instance
				var panel = new CompiledXamlPanel(parseAtRuntime: true);
				var first = panel.Properties["converter"];
				using var stream = typeof(CompiledXamlPanel).Assembly.GetManifestResourceStream(typeof(CompiledXamlPanel).FullName + ".xeto");
				XamlReader.Load(stream, panel);
				Assert.That(panel.Properties["converter"], Is.Not.SameAs(first));
			});
		}

		static string CallerFile([System.Runtime.CompilerServices.CallerFilePath] string path = null) => path;

		[Test]
		public void CompiledShouldMatchRuntime()
		{
			Invoke(() =>
			{
				var compiled = new CompiledXamlPanel();
				var runtime = new CompiledXamlPanel(parseAtRuntime: true);

				var expected = Describe(runtime).Select(r => r.Replace("Text=runtime", "Text=compiled")).ToList();
				Assert.That(Describe(compiled).ToList(), Is.EqualTo(expected));
				Assert.That(compiled.Padding, Is.EqualTo(runtime.Padding), "On should pick the same platform value");
				Assert.That(compiled.Padding, Is.Not.EqualTo(new Padding(99)), "design-time values are ignored");

				var compiledLayout = compiled.FindChild<TableLayout>("Layout");
				var runtimeLayout = runtime.FindChild<TableLayout>("Layout");
				Assert.That(compiledLayout.Spacing, Is.EqualTo(new Size(5, 5)));
				Assert.That(compiledLayout.Rows.Count, Is.EqualTo(runtimeLayout.Rows.Count));

				var label = compiled.Children.OfType<Label>().Single(r => r.Text == "B");
				var runtimeLabel = runtime.Children.OfType<Label>().Single(r => r.Text == "B");
				Assert.That(label.TextColor, Is.EqualTo(Colors.Red));
				Assert.That(label.BackgroundColor, Is.EqualTo(runtimeLabel.BackgroundColor));
				Assert.That(compiled.Children.OfType<ImageView>().Single().Image, Is.Not.Null);

				Label Find(Panel panel, string text) => panel.Children.OfType<Label>().Single(r => r.Text == text);
				Assert.That(PixelLayout.GetLocation(Find(compiled, "C")), Is.EqualTo(new Point(3, 4)));
				Assert.That(Find(compiled, "C").Size, Is.EqualTo(new Size(40, 20)));
				Assert.That(Find(compiled, "D").Size, Is.EqualTo(new Size(30)));

				var floats = (CompiledXamlFloats)compiled.Properties["floats"];
				var runtimeFloats = (CompiledXamlFloats)runtime.Properties["floats"];
				Assert.That(floats.Size, Is.EqualTo(new SizeF(1.5f, 2)).And.EqualTo(runtimeFloats.Size));
				Assert.That(floats.SingleSize, Is.EqualTo(new SizeF(0.1f)).And.EqualTo(runtimeFloats.SingleSize));
				Assert.That(floats.Point, Is.EqualTo(new PointF(-0.25f, 3)).And.EqualTo(runtimeFloats.Point));
				Assert.That(floats.Rectangle, Is.EqualTo(new RectangleF(1, 2, 3.5f, 100)).And.EqualTo(runtimeFloats.Rectangle));
				Assert.That(floats.Padding, Is.EqualTo(new PaddingF(1.5f, 2)).And.EqualTo(runtimeFloats.Padding));
			});
		}

		[Test]
		public void NamesShouldSetCodeBehindMembers()
		{
			Invoke(() =>
			{
				var panel = new CompiledXamlPanel();
				Assert.That(panel.ModeLabel, Is.Not.Null);
				Assert.That(panel.ModeLabel.ID, Is.EqualTo("ModeLabel"), "x:Name should also set ID");
				Assert.That(panel.CheckBox, Is.Not.Null);
				Assert.That(panel.CheckBox.Checked, Is.True);
				Assert.That(panel.BoundTextBox, Is.Not.Null, "ID should set a field");
				Assert.That(panel.ClickButton, Is.Not.Null);
			});
		}

		[Test]
		public void EventsShouldBeWired()
		{
			Invoke(() =>
			{
				var panel = new CompiledXamlPanel();
				panel.ClickButton.PerformClick();
				Assert.That(panel.Clicks, Is.EqualTo(1));
				panel.BoundTextBox.Text = "changed";
				Assert.That(panel.TextChanges, Is.GreaterThan(0));
			});
		}

		[Test]
		public void BindingsShouldWorkLikeRuntime()
		{
			Invoke(() =>
			{
				foreach (var (mode, panel) in new[] { ("compiled", new CompiledXamlPanel()), ("runtime", new CompiledXamlPanel(parseAtRuntime: true)) })
				{
					var command = new Command();
					var model = new CompiledXamlModel { Name = "a", Count = 3, Command = command };
					panel.DataContext = model;

					var typed = panel.FindChild<TextBox>("TypedTextBox");
					var oneWay = panel.FindChild<TextBox>("OneWayTextBox");
					var stepper = panel.FindChild<NumericStepper>("CountStepper");
					Assert.That(typed.Text, Is.EqualTo("a"), mode);
					Assert.That(oneWay.Text, Is.EqualTo("a"), mode);
					Assert.That(stepper.Value, Is.EqualTo(3), mode);
					Assert.That(panel.FindChild<Button>("CommandButton").Command, Is.SameAs(command), mode);

					typed.Text = "b";
					Assert.That(model.Name, Is.EqualTo("b"), mode + ": two-way should update the model");
					Assert.That(oneWay.Text, Is.EqualTo("b"), mode + ": the model should update the control");
					oneWay.Text = "c";
					Assert.That(model.Name, Is.EqualTo("b"), mode + ": one-way should not update the model");

					stepper.Value = 5;
					Assert.That(model.Count, Is.EqualTo(5), mode + ": the value should be converted back to the model's type");

					var itemText = panel.FindChild<DropDown>("ItemsDropDown").ItemTextBinding;
					Assert.That(itemText.GetValue(new { Title = "x" }), Is.EqualTo("x"), mode);
				}
			});
		}

		[Test]
		public void StaticResourceShouldUseTheSameInstance()
		{
			Invoke(() =>
			{
				var panel = new CompiledXamlPanel();
				Assert.That(panel.Properties["converter"], Is.InstanceOf<CompiledXamlConverter>());
				panel.DataContext = new { Name = "bound" };
				Assert.That(panel.BoundTextBox.Text, Is.EqualTo("bound"));
			});
		}
	}
}
