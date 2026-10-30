using Eto.Serialization.Xaml;

namespace Eto.Test.UnitTests.Forms.Xaml
{
	/// <summary>Loaded from compiled code by default, or by parsing the .jeto to compare.</summary>
	public partial class CompiledJsonPanel : Panel
	{
		public Label ModeLabel { get; set; }
		protected CheckBox MyCheckBox;
		public TextBox BoundTextBox;
		public Button ClickButton;
		public DropDown Choices;
		public int Clicks;
		public int TextChanges;

		public CheckBox CheckBox => MyCheckBox;

		public CompiledJsonPanel()
		{
			JetoReader.Load(this);
		}

		public CompiledJsonPanel(bool parseAtRuntime)
		{
			if (parseAtRuntime)
			{
				using var stream = typeof(CompiledJsonPanel).Assembly.GetManifestResourceStream(typeof(CompiledJsonPanel).FullName + ".jeto");
				JetoReader.Load(stream, this);
			}
			else
				JetoReader.Load(this);
		}

		void HandleClick(object sender, EventArgs e) => Clicks++;

		protected void HandleTextChanged(object sender, EventArgs e) => TextChanges++;
	}
}
