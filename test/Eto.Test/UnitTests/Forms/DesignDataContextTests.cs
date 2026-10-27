using Eto.Serialization.Json;
using Eto.Serialization.Xaml;
using NUnit.Framework;
using System.Text;
namespace Eto.Test.UnitTests.Forms
{
	public class TestDesignViewModel
	{
		public static int Created;

		public TestDesignViewModel() => Created++;

		public string Name { get; set; } = "Sample";

		public bool IsOn { get; set; }

		public List<TestDesignViewModel> Items { get; set; }
	}

	public class TestOtherDesignViewModel
	{
	}

	/// <summary>
	/// Loads xaml with its own d:DataContext, like a user control nested in the file being designed
	/// </summary>
	public class TestDesignUserControl : Panel
	{
		public TestDesignUserControl()
		{
			var xaml = $@"<Panel xmlns='{XamlReader.EtoFormsNamespace}' xmlns:d='{XamlReader.DesignNamespace}'
				xmlns:c='clr-namespace:Eto.Test.UnitTests.Forms;assembly=Eto.Test'
				d:DataContext='{{d:DesignInstance c:TestOtherDesignViewModel}}' />";
			XamlReader.Load(new StringReader(xaml), this);
		}
	}

	/// <summary>
	/// Json version of <see cref="TestDesignUserControl"/>, to check nesting across formats
	/// </summary>
	public class TestJsonDesignUserControl : Panel
	{
		public TestJsonDesignUserControl()
		{
			var json = @"{ ""d:DataContext"": ""Eto.Test.UnitTests.Forms.TestOtherDesignViewModel, Eto.Test"" }";
			JsonReader.Load(new MemoryStream(Encoding.UTF8.GetBytes(json)), this);
		}
	}

	[TestFixture]
	public class DesignDataContextTests
	{
		const string Mc = "http://schemas.openxmlformats.org/markup-compatibility/2006";

		static void InDesignMode(bool designMode, Action action)
		{
			TestBase.Invoke(() =>
			{
				var old = Control.IsDesignMode;
				Control.IsDesignMode = designMode;
				TestDesignViewModel.Created = 0;
				try
				{
					action();
				}
				finally
				{
					Control.IsDesignMode = old;
				}
			});
		}

		static string Xaml(bool ignorable, string content) =>
			$@"<Panel xmlns='{XamlReader.EtoFormsNamespace}' xmlns:d='{XamlReader.DesignNamespace}' xmlns:mc='{Mc}'
				{(ignorable ? "mc:Ignorable='d'" : "")}
				xmlns:c='clr-namespace:Eto.Test.UnitTests.Forms;assembly=Eto.Test'
				d:DataContext='{{d:DesignInstance c:TestDesignViewModel}}'>
				{content}
			</Panel>";

		[TestCase(true)]
		[TestCase(false)]
		public void XamlDesignDataContextShouldApplyInDesignMode(bool ignorable)
		{
			InDesignMode(true, () =>
			{
				var panel = XamlReader.Load<Panel>(new StringReader(Xaml(ignorable, "<Label Text='{Binding Name}'/>")), null);

				Assert.That(panel.DataContext, Is.InstanceOf<TestDesignViewModel>());
				Assert.That(((Label)panel.Content).Text, Is.EqualTo("Sample"));
			});
		}

		[TestCase(true)]
		[TestCase(false)]
		public void XamlDesignDataContextShouldBeIgnoredAtRuntime(bool ignorable)
		{
			InDesignMode(false, () =>
			{
				var panel = XamlReader.Load<Panel>(new StringReader(Xaml(ignorable, "<Label Text='{Binding Name}'/>")), null);

				Assert.That(panel.DataContext, Is.Null);
				Assert.That(TestDesignViewModel.Created, Is.EqualTo(0), "view model should not be created at runtime");
			});
		}

		/// <summary>
		/// The designer loads xaml without the class it belongs to, so it can't take the local assembly from it
		/// </summary>
		[Test]
		public void XamlDesignerShouldFindTypesFromNamespaceWithoutAssembly()
		{
			InDesignMode(true, () =>
			{
				var oldDesignMode = XamlReader.DesignMode;
				XamlReader.DesignMode = true;
				try
				{
					var xaml = $@"<Panel xmlns='{XamlReader.EtoFormsNamespace}' xmlns:d='{XamlReader.DesignNamespace}'
						xmlns:c='clr-namespace:Eto.Test.UnitTests.Forms'
						d:DataContext='{{d:DesignInstance c:TestDesignViewModel}}'>
						<c:TestDesignUserControl/>
					</Panel>";
					var panel = XamlReader.Load<Panel>(new StringReader(xaml), null);

					Assert.That(panel.DataContext, Is.InstanceOf<TestDesignViewModel>());
					Assert.That(panel.Content, Is.InstanceOf<TestDesignUserControl>(), "user control should be created instead of a placeholder");
				}
				finally
				{
					XamlReader.DesignMode = oldDesignMode;
				}
			});
		}

		[Test]
		public void XamlDesignDataContextShouldNotApplyToNestedUserControls()
		{
			InDesignMode(true, () =>
			{
				var panel = XamlReader.Load<Panel>(new StringReader(Xaml(true, "<c:TestDesignUserControl/>")), null);

				Assert.That(panel.Content, Is.InstanceOf<TestDesignUserControl>());
				Assert.That(panel.Content.DataContext, Is.SameAs(panel.DataContext), "user control should inherit the parent's data context");
			});
		}

		const string VmType = "Eto.Test.UnitTests.Forms.TestDesignViewModel, Eto.Test";

		static Panel LoadJson(string json) => JsonReader.Load<Panel>(new MemoryStream(Encoding.UTF8.GetBytes(json)));

		[Test]
		public void XamlDesignDataContextShouldNotApplyToNestedJsonUserControls()
		{
			InDesignMode(true, () =>
			{
				var panel = XamlReader.Load<Panel>(new StringReader(Xaml(true, "<c:TestJsonDesignUserControl/>")), null);

				Assert.That(panel.Content, Is.InstanceOf<TestJsonDesignUserControl>());
				Assert.That(panel.Content.DataContext, Is.SameAs(panel.DataContext), "user control should inherit the parent's data context");
			});
		}

		[TestCase("TestDesignUserControl")]
		[TestCase("TestJsonDesignUserControl")]
		public void JsonDesignDataContextShouldNotApplyToNestedUserControls(string userControl)
		{
			InDesignMode(true, () =>
			{
				var panel = LoadJson(@"{
					""d:DataContext"": """ + VmType + @""",
					""Content"": { ""$type"": ""Eto.Test.UnitTests.Forms." + userControl + @", Eto.Test"" }
				}");

				Assert.That(panel.DataContext, Is.InstanceOf<TestDesignViewModel>());
				Assert.That(panel.Content.DataContext, Is.SameAs(panel.DataContext), "user control should inherit the parent's data context");
			});
		}

		[Test]
		public void JsonDesignDataContextShouldApplyInDesignMode()
		{
			InDesignMode(true, () =>
			{
				var panel = LoadJson(@"{
					""d:DataContext"": { ""$type"": """ + VmType + @""", ""Name"": ""From Json"" },
					""Content"": { ""$type"": ""Label"", ""Text"": ""{Binding Name}"" }
				}");

				Assert.That(panel.DataContext, Is.InstanceOf<TestDesignViewModel>());
				Assert.That(((Label)panel.Content).Text, Is.EqualTo("From Json"));
			});
		}

		[Test]
		public void JsonDesignDataContextTypeNameShouldCreateInstance()
		{
			InDesignMode(true, () =>
			{
				var panel = LoadJson(@"{ ""d:DataContext"": """ + VmType + @""" }");

				Assert.That(panel.DataContext, Is.InstanceOf<TestDesignViewModel>());
			});
		}

		[Test]
		public void JsonDesignDataContextShouldBeIgnoredAtRuntime()
		{
			InDesignMode(false, () =>
			{
				var panel = LoadJson(@"{ ""d:DataContext"": { ""$type"": """ + VmType + @""" } }");

				Assert.That(panel.DataContext, Is.Null);
				Assert.That(TestDesignViewModel.Created, Is.EqualTo(0), "view model should not be created at runtime");
			});
		}

		[Test]
		public void JsonBindingsShouldBindToDataContext()
		{
			TestBase.Invoke(() =>
			{
				var panel = LoadJson(@"{
					""Content"": { ""$type"": ""StackLayout"", ""Items"": [
						{ ""$type"": ""TextBox"", ""Text"": ""{Binding Name}"" },
						{ ""$type"": ""CheckBox"", ""Checked"": ""{Binding IsOn, Mode=OneWay}"" },
						{ ""$type"": ""DropDown"", ""DataStore"": ""{Binding Items}"", ""ItemTextBinding"": ""{Binding Name}"" },
						{ ""$type"": ""Label"", ""Text"": ""{}{Binding Name}"" }
					]}
				}");
				var model = new TestDesignViewModel
				{
					Name = "Bound",
					IsOn = true,
					Items = new List<TestDesignViewModel> { new TestDesignViewModel { Name = "First" } }
				};
				panel.DataContext = model;

				var items = ((StackLayout)panel.Content).Items.Select(r => r.Control).ToList();
				var textBox = (TextBox)items[0];
				var checkBox = (CheckBox)items[1];
				var dropDown = (DropDown)items[2];
				Assert.That(textBox.Text, Is.EqualTo("Bound"));
				Assert.That(checkBox.Checked, Is.True);
				Assert.That(dropDown.DataStore, Is.SameAs(model.Items));
				Assert.That(dropDown.ItemTextBinding.GetValue(model.Items[0]), Is.EqualTo("First"));
				Assert.That(((Label)items[3]).Text, Is.EqualTo("{Binding Name}"), "{} should escape a literal value");

				textBox.Text = "Changed";
				Assert.That(model.Name, Is.EqualTo("Changed"), "default mode should be two way");
				checkBox.Checked = false;
				Assert.That(model.IsOn, Is.True, "one way should not update the model");
			});
		}

		[Test]
		public void JsonTestSectionShouldStillLoad()
		{
			TestBase.Invoke(() => Assert.That(new Eto.Test.Sections.Serialization.Json.Test().Content, Is.Not.Null));
		}
	}
}
