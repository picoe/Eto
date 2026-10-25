using Eto.Serialization.Xaml;
using Portable.Xaml.Markup;

namespace Eto.Test.UnitTests.Forms.Xaml
{
	/// <summary>Loaded from compiled code by default, or by parsing the .xeto to compare.</summary>
	public partial class CompiledXamlPanel : Panel
	{
		public Label ModeLabel { get; set; }
		protected CheckBox MyCheckBox;
		public TextBox BoundTextBox;
		public Button ClickButton;
		public int Clicks;
		public int TextChanges;

		public CheckBox CheckBox => MyCheckBox;

		public CompiledXamlPanel()
		{
			XamlReader.Load(this);
		}

		public CompiledXamlPanel(bool parseAtRuntime)
		{
			if (parseAtRuntime)
			{
				using var stream = typeof(CompiledXamlPanel).Assembly.GetManifestResourceStream(typeof(CompiledXamlPanel).FullName + ".xeto");
				XamlReader.Load(stream, this);
			}
			else
				XamlReader.Load(this);
		}

		void HandleClick(object sender, EventArgs e) => Clicks++;

		protected void HandleTextChanged(object sender, EventArgs e) => TextChanges++;
	}

	public class CompiledXamlConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value;

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => value;
	}

	/// <summary>Shows which way the panel was loaded.</summary>
	[XamlCodeGen(nameof(Provide))]
	public class LoadModeExtension : MarkupExtension
	{
		public override object ProvideValue(IServiceProvider serviceProvider) => "runtime";

		public static string Provide() => "compiled";
	}
}
